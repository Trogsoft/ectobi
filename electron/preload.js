const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  openDialog: (arg) => ipcRenderer.invoke('openDialog', arg),
  confirm: (arg) => ipcRenderer.invoke('getConfirmation', arg),
  alert: (arg) => ipcRenderer.invoke('alert', arg),
  getToken: () => ipcRenderer.invoke('getToken'),
  closeMe: () => this.close(),
  tokenRefresh: (callback) => ipcRenderer.on('tokenRefresh', callback),

  // background
  getBackgroundTasks: () => ipcRenderer.invoke('getBackgroundTasks'),
  taskBegun: (callback) => ipcRenderer.on('taskBegun', callback),
  taskProgressChanged: (callback) => ipcRenderer.on('taskProgressChanged', callback),
  taskCompleted: (callback) => ipcRenderer.on('taskCompleted', callback),
  taskFailed: (callback) => ipcRenderer.on('taskFailed', callback),
  taskStatusChanged: (callback) => ipcRenderer.on('taskStatusChanged', callback),

  // login specific - move them
  login: (username, password) => ipcRenderer.invoke('login', username, password)
})