const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  openDialog: (arg) => ipcRenderer.invoke('openDialog', arg),
  confirm: (arg) => ipcRenderer.invoke('getConfirmation', arg),
  alert: (arg) => ipcRenderer.invoke('alert', arg)
})