import { lookupValuesDialog } from "./lookupValuesDialog.js";
import { newSchemaDialog } from "./newSchemaDialog.js";
import { uploadBatchDialog } from "./uploadBatchDialog.js";
import { webHookEditorDialog } from "./webHookEditorDialog.js";

export const dialogs = {
    'uploadBatchDialog': uploadBatchDialog,
    'lookupValuesDialog': lookupValuesDialog,
    'newSchemaDialog': newSchemaDialog,
    'webHookEditorDialog': webHookEditorDialog
}

window.addEventListener('DOMContentLoaded', x => {

    window.ipc.dialogConfiguration((event, value) => {

        if (value.dialogType) {
            var dt = dialogs[value.dialogType];
            if (dt) {
                new dt(event.sender, value);
            }
        }

    });

})