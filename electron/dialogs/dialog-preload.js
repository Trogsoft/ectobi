const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('ipc', {
  openDialog: (arg) => ipcRenderer.invoke('cunt', arg)
})