const { app, BrowserWindow, ipcMain, dialog } = require('electron')
const path = require('path')
const fs = require('fs');

var mainWindow;

const createWindow = () => {

  mainWindow = new BrowserWindow({
    icon: path.join(__dirname, 'img/icon.png'),
    width: 1366,
    height: 768,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
    },
  })

  // win.webContents.setWindowOpenHandler(({ url, frameName }) => {
  //   var options = {};

  //   if (frameName === 'modal-narrow') {
  //     options = Object.assign(options,{
  //       width: 500,
  //       height: 700
  //     });
  //   }

  //   return {
  //     action: 'allow',
  //     overrideBrowserWindowOptions: Object.assign(options, {
  //       frame: false,
  //       modal: true,
  //       parent: win,
  //       fullscreenable: false
  //     })
  //   }
  // })

  mainWindow.loadFile('index.html')
}

function openDialog(arg) {

  var win = new BrowserWindow({
    icon: path.join(__dirname, 'img/icon.png'),
    width: arg.width || 500,
    height: arg.height || 700,
    modal: true,
    frame: arg.frame || false,
    parent: mainWindow,
    resizable: arg.resizable || false,
    webPreferences: {
      preload: path.join(__dirname, 'dialogs/dialog-preload.js'),
    },
  });

  win.loadURL(`file://${__dirname}/dialogs/dialog.html`).then(x=>{
    win.webContents.send('dialogConfiguration',arg);
  });

  win.on('closed', x => {
    win = null;
  });

}

app.whenReady().then(() => {

  ipcMain.handle('openDialog', async (ev, arg) => {
    openDialog(arg);
  });

  ipcMain.handle('showOpenDialog', (ev, opts) => {
    return dialog.showOpenDialog(Object.assign(opts, {
      title: 'Please select one or more files',
      properties: ['openFile ', 'multiSelections ']
    }));        
  })

  ipcMain.handle('getFileContents', (ev, opts) => {
    return new Promise((resolve,reject)=>{
      fs.readFile(opts.path, (err, data) => {
        if(err){
          reject(err);
        }
        resolve(data.toString('base64'));
      })
    })
  })

  ipcMain.handle('getConfirmation', (ev, opts) =>{
    return dialog.showMessageBox(mainWindow, {
      type: 'question',
      title: opts.title || 'Confirmation',
      message: opts.message || 'Are you sure?',
      buttons: opts.buttons ||  [
        'Yes',
        'No'
      ]
    })
  })

  createWindow();

});


app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});