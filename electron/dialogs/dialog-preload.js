const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  dialogConfiguration: (callback) => ipcRenderer.on('dialogConfiguration', callback),
  closeMe: () => this.close(),
  selectFile: (opts, callback) => ipcRenderer.invoke('showOpenDialog', opts, callback),
  getFileContents: (opts) => ipcRenderer.invoke('getFileContents', opts)
})
