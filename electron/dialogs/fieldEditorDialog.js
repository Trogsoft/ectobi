import { ectoClient } from "../ectoClient.js";
import { fieldType } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class fieldEditorDialog extends dialogBase {

    client = ectoClient;
    populators = [];
    lookups = [];

    model = {
        id: 0,
        name: null,
        description: null
    };

    constructor(sender, arg) {
        super(sender, arg);

        this.client.populator.list().then(pl=>{
            this.populators = pl.result;
            return;
        }).then(()=>{
            return this.client.lookup.list().then(x=>{
                this.lookups = x.result;
            })
        }).then(()=>{
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
        });

    }

    save = (e) => {
        if (this.model.id == 0) {
            this.client.field.create(this.args.schema, this.model).then(x=>{
                if (x.succeeded) {
                    this.close();
                } else {
                    console.log(x);
                }
            });
        }
    }

    bind() {
        super.bind();

        document.querySelectorAll('.create-field').forEach(x=>{
            x.removeEventListener('click', this.save);
            x.addEventListener('click', this.save);
        });
    }

    readyToGo() {
        if (!this.model.name) return false;
        return true;
    }

    render() {

        let fieldTypes = () => {
            var html = '';
            Object.keys(fieldType).forEach(x => {
                var sel = this.model.type == fieldType[x] ? 'selected' : '';
                html += `<option value="${fieldType[x]}" ${sel}>${x}</option>`;
            });
            return html;
        }

        let typeSpecificOptions = () => {
            var html = '';
            if (this.model.type == fieldType.Populator) {
                html += `<div class="form-field"><label>Populator</label>
                <select class="form-control model-field" name="populator"><option></option>`;
                this.populators.forEach(pop => {
                    html += `<option value="${pop.textId}" ${this.model.populator == pop.textId ? 'selected' : ''}>${pop.name}</option>`;
                })
                html += `</select></div>`;
            }

            if (this.model.type == fieldType.LookupSet) {
                html += `<div class="form-field"><label>Lookup Set</label>
                <select class="form-control model-field" name="set"><option></option><option value="-1">(Create new Lookup Set)</option>
                `;
                this.lookups.forEach(l=>{
                    html += `<option value="${l.textId}" ${this.model.lookupTid == l.textId ? 'selected' : ''}>${l.name}</option>`;
                })
                html += '</select></div>';
            }
            return html;
        }

        var html = `
            <div class="form-field">
                <label>Field Name</label>
                <input type="text" class="form-control model-field" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control model-field" name="description">${this.model.description || ''}</textarea>
            </div>

            <div class="row">
                <div class="col-2">
                    <div class="form-field">
                        <label>Type</label>
                        <select name="type" class="form-control model-field">${fieldTypes()}</select>
                    </div>
                </div>

                <div class="col-2">
                    ${typeSpecificOptions()}
                </div>
            </div>

            <div class="form-field">
                <input type="checkbox" disabled name="newVersion" />
                <label>Create new schema version</label>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-field" ${this.readyToGo() ? '' : 'disabled'}>${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
