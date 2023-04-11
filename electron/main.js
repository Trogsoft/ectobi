const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');
const wretch = require('wretch');

class tokenManager {

  interval;
  tokenRefreshInterval = 240000; // 4 minutes
  main;

  #token = null;

  constructor(main) {
    this.main = main;
    this.interval = setInterval(this.refreshToken, this.tokenRefreshInterval);

    app.whenReady().then(() => {

      ipcMain.handle('login', this.login);

    });
  }

  refreshToken = () => {

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

  getCurrentToken = () => this.#token && this.#token.token ? this.#token.token : null;

  requireLogin = () => this.#token == null;

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
        console.log('token: ' + this.#token.token);
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

  constructor() {
    this.tokenManager = new tokenManager(this);
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
      ipcMain.handle('alert', (ev, opts) => dialog.showMessageBox(ev.sender, opts));
      ipcMain.handle('getToken', () => this.tokenManager.getCurrentToken());
      ipcMain.handle('getConfirmation', (ev, opts) => this.confirm(opts));

      if (this.tokenManager.requireLogin())
        this.createLoginWindow();
      else
        this.createMainWindow();

    });

  }

  tokenChanged = (reason) => {
    if (reason == 'loggedIn') {
      if (this.loginWindow)
        this.loginWindow.close();

      this.createMainWindow();
    }
    this.mainWindow.webContents.send('tokenRefresh');

    this.windows.forEach(win=>{ 
      win.webContents.send('tokenRefresh');
    });
  }

  confirm = (opts) => {

    return dialog.showMessageBox(mainWindow, {
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
      modal: true,
      frame: opts.frame || false,
      parent: this.mainWindow,
      resizable: opts.resizable || false,
      preload: path.join(__dirname, 'dialogs/dialog-preload.js'),
    });

    win.loadURL(`file://${__dirname}/dialogs/dialog.html`).then(x => {
      win.webContents.send('dialogConfiguration', opts);
    });

    win.on('closed', x => {
      var idx = this.windows.findIndex(x => x == win);
      if (idx > -1) {
        this.windows.splice(idx, 1);
      }
      win = null;
    });

    return win;

  }

}

new ectobiMain();