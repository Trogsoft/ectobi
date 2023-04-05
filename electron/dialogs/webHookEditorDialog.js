import { ectoClient } from "../ectoClient.js";
import { webHookEventType } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class webHookEditorDialog extends dialogBase {

    client = new ectoClient();

    model = {
        id: 0,
        name: '',
        description: '',
        url: ''
    };

    constructor(sender, arg) {
        super(sender, arg);
        if (arg.id > 0) {
            this.client.webhook.get(arg.id).then(x => {
                this.model = x.result;
                this.setTitle(x.result.name);
                this.render();
            });
        } else {
            this.setTitle('New WebHook');
            this.render();
        }
    }

    bind() {
        super.bind();
    }

    readyToGo() {
        return true;
    }

    getCheckedStatus = (event) => {
        var flagValue = 0;
        Object.keys(webHookEventType).forEach(k => {
            if (webHookEventType[k].textId == event) {
                flagValue = parseInt(k);
                return;
            }
        });

        return (this.model.events & flagValue) > 0 ? 'checked' : '';
    };

    render() {
        var html = `
            <div class="form-field">
                <label>WebHook Name</label>
                <input type="text" class="form-control" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control" name="description">${this.model.description || ''}</textarea>
            </div>
            <div class="form-field">
                <label>URL</label>
                <input type="text" class="form-control" name="url" @value="${this.model.url || ''}" />
            </div>     
            
            <table class="table">
                <thead>
                    <tr>
                        <th></th>
                        <th class="w-15pc">Create</th>
                        <th class="w-15pc">Edit</th>
                        <th class="w-15pc">Delete</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Schema</td>
                        <td><input type="checkbox" name="SchemaCreated" ${this.getCheckedStatus('SchemaCreated')} /></td>
                        <td><input type="checkbox" name="SchemaUpdated" ${this.getCheckedStatus('SchemaUpdated')} /></td>
                        <td><input type="checkbox" name="SchemaDeleted" ${this.getCheckedStatus('SchemaDeleted')} /></td>
                    </tr>
                    <tr>
                        <td>Schema Version</td>
                        <td><input type="checkbox" name="SchemaVersionCreated" ${this.getCheckedStatus('SchemaVersionCreated')} /></td>
                        <td><input type="checkbox" name="SchemaVersionUpdated" ${this.getCheckedStatus('SchemaVersionUpdated')} /></td>
                        <td><input type="checkbox" name="SchemaVersionDeleted" ${this.getCheckedStatus('SchemaVersionDeleted')} /></td>
                    </tr>
                    <tr>
                        <td>Field</td>
                        <td><input type="checkbox" name="FieldCreated" ${this.getCheckedStatus('FieldCreated')} /></td>
                        <td><input type="checkbox" name="FieldUpdated" ${this.getCheckedStatus('FieldUpdated')} /></td>
                        <td><input type="checkbox" name="FieldDeleted" ${this.getCheckedStatus('FieldDeleted')} /></td>
                    </tr>
                    <tr>
                        <td>Batch</td>
                        <td><input type="checkbox" name="BatchCreated" ${this.getCheckedStatus('BatchCreated')} /></td>
                        <td><input type="checkbox" name="BatchUpdated" ${this.getCheckedStatus('BatchUpdated')} /></td>
                        <td><input type="checkbox" name="BatchDeleted" ${this.getCheckedStatus('BatchDeleted')} /></td>
                    </tr>
                    <tr>
                        <td>Record</td>
                        <td><input type="checkbox" name="RecordCreated" ${this.getCheckedStatus('RecordCreated')} /></td>
                        <td><input type="checkbox" name="RecordUpdated" ${this.getCheckedStatus('RecordUpdated')} /></td>
                        <td><input type="checkbox" name="RecordDeleted" ${this.getCheckedStatus('RecordDeleted')} /></td>
                    </tr>
                </tbody>
            </table>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-schema ${this.readyToGo() ? '' : 'disabled'}">${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
