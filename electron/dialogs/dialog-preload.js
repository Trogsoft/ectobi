const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  dialogConfiguration: (callback) => ipcRenderer.on('dialogConfiguration', callback),
  closeMe: () => this.close(),
  selectFile: (opts, callback) => ipcRenderer.invoke('openFile', opts, callback),
  getFileContents: (opts) => ipcRenderer.invoke('readFileBase64', opts),
  alert: (opts) => ipcRenderer.invoke('alert', opts),
  getToken: () => ipcRenderer.invoke('getToken'),
  tokenRefresh: (callback) => ipcRenderer.on('tokenRefresh', callback)
})
