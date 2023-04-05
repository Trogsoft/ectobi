import { ectoClient } from "../ectoClient.js";
import { fieldType } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class fieldEditorDialog extends dialogBase {

    client = new ectoClient();

    model = {
        id: 0,
        name: null,
        description: null
    };

    constructor(sender, arg) {
        super(sender, arg);
        if (arg.field) {
            this.client.field.getLatest(arg.schema, arg.field).then(x => {
                this.model = x.result;
                this.setTitle(x.result.name);
                this.render();
            });
        } else {
            this.setTitle('New Field');
            this.render();
        }
    }

    bind() {
        super.bind();
    }

    readyToGo() {
        return true;
    }

    render() {

        let fieldTypes = () => {
            var html = '';
            Object.keys(fieldType).forEach(x=>{
                var sel = this.model.type == x ? 'selected' : '';
                html += `<option value="${x}" ${sel}>${fieldType[x]}</option>`;
            });
            return html;
        }

        var html = `
            <div class="form-field">
                <label>Field Name</label>
                <input type="text" class="form-control" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control" name="description">${this.model.description || ''}</textarea>
            </div>

            <div class="row">
                <div class="col-2">
                    <div class="form-field">
                        <label>Type</label>
                        <select name="type" class="form-control">${fieldTypes()}</select>
                    </div>
                </div>

                <div class="col-2">
                </div>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-schema ${this.readyToGo() ? '' : 'disabled'}">${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
