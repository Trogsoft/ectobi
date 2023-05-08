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

    var hash = window.location.hash.replace('#', '');
    var dialogArgs = {};
    hash.split('/').forEach(x => {
        var parts = x.split('=');
        dialogArgs[parts[0]] = parts[1];
    });

    window.ipc.getToken().then(token => {
        if (dialogArgs.dialogType) {
            var dt = dialogs[dialogArgs.dialogType];
            if (dt) {
                dialogInstance = new dt({}, dialogArgs, token);
            } else {
                window.ipc.alert({ message: `Dialog type not found: ${dt}` });
            }
        } else {
            window.ipc.alert(`Dialog type not specified.`);
        }
    })

    window.ipc.dialogConfiguration((event, value) => {
    });

    window.ipc.tokenRefresh((event) => {
        window.ipc.getToken().then(token => {
            dialogInstance.tokenUpdate(token);
        })
    });

})