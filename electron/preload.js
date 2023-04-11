const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  openDialog: (arg) => ipcRenderer.invoke('openDialog', arg),
  confirm: (arg) => ipcRenderer.invoke('getConfirmation', arg),
  alert: (arg) => ipcRenderer.invoke('alert', arg),
  getToken: () => ipcRenderer.invoke('getToken'),
  closeMe: () => this.close(),
  tokenRefresh: (callback) => ipcRenderer.on('tokenRefresh', callback),

  // login specific - move them
  login: (username, password) => ipcRenderer.invoke('login', username, password)
})