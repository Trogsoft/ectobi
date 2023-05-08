const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');
const wretch = require('wretch');
const { HubConnectionBuilder } = require("@microsoft/signalr");

class realTimeManager {

  ecto;
  tokenManager;
  connection;

  currentTasks = [];

  constructor(ecto, tokenManager) {
    this.ecto = ecto;
    this.tokenManager = tokenManager;
    this.connection = new HubConnectionBuilder()
      .withUrl('http://' + this.ecto.server + '/events')
      .build();

    this.connection.onclose(async () => {
      await this.start();
    });

    this.connection.on("taskBegun", (task) => {
      this.currentTasks.push({
        task: task,
        progress: { total: 0, count: 0 },
        log: [],
        status: '',
        completed: false,
        failed: false
      });
      this.broadcast('taskBegun', task);
    });

    this.connection.on("taskProgressChanged", (task, total, count) => {
      var ct = this.getTask(task);
      ct.progress.total = total;
      ct.progress.count = count;
      this.broadcast('taskProgressChanged', task, total, count);
    });

    this.connection.on("taskStatusChanged", (task, status) => {
      var ct = this.getTask(task);
      ct.status = status;
      this.broadcast('taskStatusChanged', task, status);
    });

    this.connection.on("taskLog", (task, msg) => {
      var ct = this.getTask(task);
      ct.log.push(msg);
      this.broadcast('taskLog', task, msg);
    });

    this.connection.on("taskCompleted", (task) => {
      // setTimeout(() => {
        var taskIndex = this.currentTasks.findIndex(x => x.task.id == task.id);
        this.currentTasks.splice(taskIndex, 1);
      // }, 60000 * 5);
      this.broadcast('taskCompleted', task);
    });

    this.connection.on("taskFailed", (task) => {
      // setTimeout(() => {
        var taskIndex = this.currentTasks.findIndex(x => x.task.id == task.id);
        this.currentTasks.splice(taskIndex, 1);
      // }, 60000 * 5);
      this.broadcast('taskFailed', task);
    });

    // Start the connection.
    this.start();
  }

  getAllTasks = () => this.currentTasks;

  getTask(task) {
    var tasks = this.currentTasks.filter(x => x.task.id == task.id);
    if (!tasks.length) return;
    var t = tasks[0];
    return t;
  }

  broadcast(event, ...args) {
    this.ecto.mainWindow.webContents.send(event, args);
    this.ecto.windows.forEach(win => {
      win.webContents.send(event, args);
    })
  }

  async start() {
    try {
      await this.connection.start();
    } catch (err) {
      setTimeout(this.start, 5000);
    }
  };

}

class tokenManager {

  interval;
  tokenRefreshInterval = 240000; // 4 minutes
  main;
  serverRequiresLogin = true;

  #token = null;

  constructor(main) {
    this.main = main;

    // Request a new login token every refresh interval.
    this.interval = setInterval(this.refreshToken, this.tokenRefreshInterval);

    app.whenReady().then(() => {
      // Handle the login event
      ipcMain.handle('login', this.login);
    });
  }

  /**
   * Refresh the current token.
   */
  refreshToken = () => {

    // Stand down. We don't need a refreshed token when login isn't required.
    if (!this.serverRequiresLogin)
      clearInterval(this.interval);

    // Refresh the token then broadcast the event.
    wretch()
      .post(this.#token, `http://${this.main.server}/api/auth/refresh-token`)
      .json()
      .then(response => {
        this.#token = response.result;
        this.main.tokenChanged('refreshed');
      })
      .catch(err => {
        console.log(err);
      })

  }

  /**
   * Get the current token string
   * @returns The current token or null.
   */
  getCurrentToken = () => this.#token && this.#token.token ? this.#token.token : null;

  /**
   * Check if login is required. This considers whether the 
   * session has expired and also whether the server requires login. 
   * @returns True or false.
   */
  requireLogin = () => this.serverRequiresLogin ? this.#token == null : false;

  /**
   * Log in to the Ectobi server.
   * @param {*} ev The event that triggered this call
   * @param {*} username Username to log in with
   * @param {*} password Password to log in with
   */
  login = (ev, username, password) => {

    wretch()
      .post({
        username: username,
        password: password
      }, `http://${this.main.server}/api/auth`)
      .json()
      .then(response => {
        this.#token = response.result;
        this.main.tokenChanged('loggedIn');
      })
      .catch(err => {
        console.log(err);
      })

  }

}

class ectobiMain {

  server = 'localhost:5247';
  mainWindow;
  loginWindow;
  windows = [];

  tokenManager;
  realTimeManager;

  constructor() {
    this.tokenManager = new tokenManager(this);
    this.realTimeManager = new realTimeManager(this, this.tokenManager);
    this.init();

    app.on('window-all-closed', () => {
      if (process.platform !== 'darwin') {
        app.quit();
      }
    });

  }

  init = () => {

    app.whenReady().then(() => {

      ipcMain.handle('openDialog', (ev, arg) => this.openDialog(arg));
      ipcMain.handle('openFile', (ev, opts) => this.openFile(opts));
      ipcMain.handle('readFileBase64', (ev, opts) => this.readFileBase64(opts));
      ipcMain.handle('alert', (ev, opts) => {
        return dialog.showMessageBox(BrowserWindow.getFocusedWindow(), {
          type: opts.type || 'info',
          message: opts.message || 'No message provided',
          title: opts.title || 'Ectobi'
        });
      });
      ipcMain.handle('getToken', () => this.tokenManager.getCurrentToken());
      ipcMain.handle('getConfirmation', (ev, opts) => this.confirm(opts));
      ipcMain.handle('getBackgroundTasks', () => this.realTimeManager.getAllTasks());

      wretch()
        .get(`http://${this.server}/api/ecto/server`)
        .json()
        .then(response => {

          this.tokenManager.serverRequiresLogin = response.result.requiresLogin;

          if (response.result.requiresLogin) {
            if (this.tokenManager.requireLogin())
              this.createLoginWindow();
            else
              this.createMainWindow();
          } else {
            this.createMainWindow();
          }

        }).catch(x => {

          dialog.showErrorBox('Unable to connect', 'Cannot connect to the specified Ectobi server.');
          app.quit();

        })


    });

  }

  tokenChanged = (reason) => {
    if (reason == 'loggedIn') {
      if (this.loginWindow)
        this.loginWindow.close();

      this.createMainWindow();
    }
    this.mainWindow.webContents.send('tokenRefresh');

    this.windows.forEach(win => {
      win.webContents.send('tokenRefresh');
    });
  }

  confirm = (opts) => {

    return dialog.showMessageBox(BrowserWindow.getFocusedWindow(), {
      type: 'question',
      title: opts.title || 'Confirmation',
      message: opts.message || 'Are you sure?',
      buttons: opts.buttons || [
        'Yes',
        'No'
      ]
    })

  }

  readFileBase64 = (opts) => {

    return new Promise((resolve, reject) => {
      fs.readFile(opts.path, (err, data) => {
        if (err) {
          reject(err);
        }
        resolve(data.toString('base64'));
      })
    })

  }

  openFile = (opts) => {

    return dialog.showOpenDialog(Object.assign(opts, {
      title: 'Please select one or more files',
      properties: ['openFile', 'multiSelections']
    }));

  }

  openWindow = (opts) => {

    var windowOpts = {
      icon: path.join(__dirname, 'img/icon.png'),
      width: opts.width || 500,
      height: opts.height || 700,
      modal: opts.modal || false,
      frame: opts.frame || false,
      parent: opts.parent || null,
      resizable: opts.resizable || true,
    };

    if (opts.preload) {
      windowOpts.webPreferences = {
        preload: opts.preload,
      };
    }

    var window = new BrowserWindow(windowOpts);
    return window;

  }

  createMainWindow = () => {

    this.mainWindow = this.openWindow({
      width: 1366,
      height: 768,
      frame: true,
      preload: path.join(__dirname, 'preload.js'),
    });

    this.mainWindow.loadFile('index.html');

  }

  createLoginWindow = () => {

    this.loginWindow = this.openWindow({
      width: 400,
      height: 600,
      frame: false,
      resizable: false,
      preload: path.join(__dirname, 'preload.js')
    });

    this.loginWindow.loadFile('login.html');

    this.loginWindow.on('closed', x => {
      this.loginWindow = null;
    });

  }

  openDialog = (opts) => {

    var win = this.openWindow({
      width: opts.width || 500,
      height: opts.height || 700,
      modal: opts.modal || false,
      frame: opts.frame || true,
      show: false,
      parent: opts.modal ? this.mainWindow : null,
      resizable: opts.resizable || false,
      preload: path.join(__dirname, 'dialogs/dialog-preload.js'),
    });

    win.setMenu(null);

    var qs = '';
    Object.keys(opts).forEach(k => {
      qs += `${k}=${opts[k]}/`;
    })

    win.loadURL(`file://${__dirname}/dialogs/dialog.html#${qs}`).then(x => {
      if (opts.debug) win.webContents.openDevTools();
    });

    win.on('ready-to-show', l => {
      win.show();
    });

    win.on('closed', x => {
      var idx = this.windows.findIndex(x => x == win);
      if (idx > -1) {
        this.windows.splice(idx, 1);
      }
      win = null;
    });

    //return win;

  }

}

new ectobiMain();