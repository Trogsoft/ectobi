import { ectoModelPropertyFlags } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class modelEditorDialog extends dialogBase {

    models = [];
    selectedModel = -1;

    model = {
        id: 0,
        name: null,
        description: null
    };

    constructor(sender, arg, token) {
        super(sender, arg, token);
        this.client.model.list().then(x => {
            this.models = x.result;
            this.selectedModel = 0;

            this.setTitle('New Model');
            this.render();

            if (arg.id > 0) {

            } else {

            }
        });

    }

    selectMode = (e) => {
        var idx = parseInt(e.currentTarget.value);
        this.selectedModel = idx;
        this.render();
    }

    bind() {
        super.bind();
        document.querySelectorAll('select').forEach(s=>{
            s.removeEventListener('change', this.selectModel);
            s.addEventListener('change', this.selectModel);
        })
    }

    readyToGo() {
        return true;
    }

    render() {

        var renderModels = () => {
            var html = '';
            this.models.forEach((m,i)=>{
                html += `<option value="${i}">${m.name}</option>`;
            })
            return html;
        }

        var modelOptions = () => {

            var html = '';
            var model = this.models[this.selectedModel];
            model.properties.forEach(prop=>{
                var checked = '';
                if ((prop.flags & ectoModelPropertyFlags.SuggestedDefault) > 0) checked = 'checked';
                html += '<div class="form-check">';
                html += ` <input type="checkbox" id="cbmp-${prop.textId}" name="${prop.textId}" ${checked} /> <label for="cbmp-${prop.textId}">${prop.name}</label>`;
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
                <button class="btn btn-success create-schema ${this.readyToGo() ? '' : 'disabled'}">${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
