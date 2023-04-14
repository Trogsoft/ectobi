import { fieldEditorDialog } from "./fieldEditorDialog.js";
import { lookupValuesDialog } from "./lookupValuesDialog.js";
import { modelEditorDialog } from "./modelEditorDialog.js";
import { newSchemaDialog } from "./newSchemaDialog.js";
import { uploadBatchDialog } from "./uploadBatchDialog.js";
import { webHookEditorDialog } from "./webHookEditorDialog.js";

export const dialogs = {
    'uploadBatchDialog': uploadBatchDialog,
    'lookupValuesDialog': lookupValuesDialog,
    'newSchemaDialog': newSchemaDialog,
    'webHookEditorDialog': webHookEditorDialog,
    'fieldEditorDialog': fieldEditorDialog,
    'modelEditorDialog': modelEditorDialog
}

var dialogInstance;

window.addEventListener('DOMContentLoaded', x => {

    window.ipc.dialogConfiguration((event, value) => {
        window.ipc.getToken().then(token=>{
            if (value.dialogType) {
                var dt = dialogs[value.dialogType];
                if (dt) {
                    dialogInstance = new dt(event.sender, value, token);
                } else {
                    window.ipc.alert({ message: `Dialog type not found: ${dt}` });
                }
            }    
        })
    });

    window.ipc.tokenRefresh((event) => {
        window.ipc.getToken().then(token=>{
            dialogInstance.tokenUpdate(token);
        })
    });

})