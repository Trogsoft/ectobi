import { ectoModelPropertyFlags } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class modelEditorDialog extends dialogBase {

    models = [];
    selectedModel = -1;
    fields = [];

    model;

    constructor(sender, arg, token) {
        super(sender, arg, token);
        this.client.field.listBySchemaVersion(arg.schema, '0').then(x => {

            this.fields = x.result;

        }).then(x => {
            this.client.model.list().then(x => {

                this.models = x.result;
                this.selectedModel = 0;

                var defaultProperties = this.models[this.selectedModel].properties
                    // This selects the current model's properties based on whether it's a suggested default or whether the current schema already contains it. 
                    .filter(x => (x.flags & ectoModelPropertyFlags.SuggestedDefault) > 0 || this.fields.filter(y=>y.modelField == x.textId).length > 0)
                    .map(x => x.textId);

                this.model = {
                    schemaTid: this.args.schema,
                    modelName: this.models[this.selectedModel].textId,
                    properties: defaultProperties
                }

                this.setTitle('New Model');
                this.render();

                if (arg.id > 0) {

                } else {

                }
            });
        });
    }

    selectModel = (e) => {
        var idx = parseInt(e.currentTarget.value);
        this.selectedModel = idx;
        this.model.modelTid = this.mdoels[idx].textId;
        this.render();
    }

    fieldCheck = (e) => {
        var propName = e.currentTarget.getAttribute('name');
        var propIdx = this.model.properties.indexOf(propName);
        if (propIdx > -1) {
            this.model.properties.splice(propIdx, 1);
        } else {
            this.model.properties.push(propName);
        }
        this.render();
    }

    configureModel = (e) => {

        this.client.model.configure(this.model).then(x => {
            window.ipc.closeMe();
        }).catch(err => {

        })

    }

    bind() {
        super.bind();
        document.querySelectorAll('select').forEach(s => {
            s.removeEventListener('change', this.selectModel);
            s.addEventListener('change', this.selectModel);
        })
        document.querySelectorAll('.model-field').forEach(x => {
            x.removeEventListener('click', this.fieldCheck);
            x.addEventListener('click', this.fieldCheck);
        })
        document.querySelectorAll('.configure-model').forEach(x => {
            x.removeEventListener('click', this.configreModel);
            x.addEventListener('click', this.configureModel);
        })
    }

    readyToGo() {
        return true;
    }

    render() {

        var renderModels = () => {
            var html = '';
            this.models.forEach((m, i) => {
                html += `<option value="${i}">${m.name}</option>`;
            })
            return html;
        }

        var modelOptions = () => {

            var html = '';
            var model = this.models[this.selectedModel];
            model.properties.forEach(prop => {

                // Checked if default, or part of the currently selected collection
                var checked = '';
                if (this.model.properties.indexOf(prop.textId) > -1) checked = 'checked';

                // Disable if it's already part of the schema 
                var dis = '';
                if (this.fields.filter(x => x.modelName == model.textId && x.modelField == prop.textId).length > 0) { dis = 'disabled'; checked = 'checked' };

                html += '<div class="form-check">';
                html += ` <input type="checkbox" class="model-field" id="cbmp-${prop.textId}" name="${prop.textId}" ${checked} ${dis} /> <label for="cbmp-${prop.textId}">${prop.name} <small>${prop.description || ''}</small></label>`;
                html += '</div>';
            });

            return html;

        }

        var html = `
            <div class="form-field">
                <label>Choose Model</label>
                <select name="model" class="form-control">
                    ${renderModels()}
                </select>
            </div>

            ${modelOptions()}

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success configure-model ${this.readyToGo() ? '' : 'disabled'}">${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
