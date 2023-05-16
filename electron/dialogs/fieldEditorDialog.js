import { ectoClient } from "../ectoClient.js";
import { fieldType } from "../enums.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class fieldEditorDialog extends dialogBase {

    populators = [];
    lookups = [];
    models = [];

    model = {
        id: 0,
        name: null,
        description: null
    };

    constructor(sender, arg, token) {
        super(sender, arg, token);

        this.client.populator.list().then(pl => {
            this.populators = pl.result;
            return;
        }).then(() => {
            return this.client.lookup.list().then(x => {
                this.lookups = x.result;
            })
        }).then(() => {
            return this.client.model.list().then(x => {
                this.models = x.result;
            });
        }).then(() => {
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

        this.configureTabs();

    }

    save = (e) => {
        if (this.model.id == 0) {
            this.client.field.create(this.args.schema, this.model).then(x => {
                this.close();
            });
        } else {
            this.client.field.update(this.args.schema, this.model).then(x => {
                this.close();
            });
        }
    }

    configureTabs() {

        let modelSelectList = () => {
            var html = '<option>No model</option>';
            this.models.forEach(m => {
                var sel = this.model.modelName == m.textId ? '@selected' : '';
                html += `<option ${sel} value="${m.textId}">${m.name}</option>`;
            });
            return html;
        }

        let fieldTypes = () => {
            var html = '';
            Object.keys(fieldType).sort().forEach(x => {
                if (typeof (fieldType[x]) !== 'function') {
                    var sel = this.model.type == fieldType[x] ? 'selected' : '';
                    html += `<option value="${fieldType[x]}" ${sel}>${x}</option>`;
                }
            });
            return html;
        }

        let modelProperties = () => {
            var html = '';
            if (this.model.modelName) {
                var sModel = this.models.filter(x => x.textId == this.model.modelName);
                if (sModel.length == 0) return '';

                html += `<div class="form-field"><label>Property</label>
                <select class="form-control model-field" name="modelField">`;
                sModel[0].properties.forEach(p => {
                    var sel = p.textId == this.model.modelField ? 'selected' : '';
                    html += `<option ${sel} value="${p.textId}">${p.name}</option>`;
                })
                html += `</select></div>`;
            }
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

                if (this.model.populator) {
                    var populators = this.populators.filter(x => x.textId == this.model.populator);
                    if (populators.length > 0) {
                        var pop = populators[0];

                        pop.options.forEach(opt => {
                            if (opt.type == 2) {
                                html += `<div class="checkbox"><input type="checkbox" name="pop-opt-${opt.id}" /> <label>${opt.name}</label></div>`;
                            }
                            else if (opt.type == 5) {
                                html += `<div class="form-field"><label>${opt.name}</label><input class="form-control" type="number" value="" name="pop-opt-${opt.id}" /></div>`;
                            }
                        })

                    }
                }

            }

            if (this.model.type == fieldType.LookupSet) {
                html += `<div class="form-field"><label>Lookup Set</label>
                <select class="form-control model-field" name="set"><option></option><option value="__create__">(Create new Lookup Set)</option>
                `;
                this.lookups.forEach(l => {
                    html += `<option value="${l.textId}" ${this.model.lookupTid == l.textId ? 'selected' : ''}>${l.name}</option>`;
                })
                html += '</select></div>';
            }

            if (this.model.type == fieldType.Formula) {
                html += `
                    <div class="form-field">
                        <label>Formula</label>
                        <textarea name="formula" class="form-control model-field">${this.model.formula || ''}</textarea>
                    </div>
                `;
            }

            return html;
        }

        this.addTab('details', 'Details', 'edit-box-line', () => {
            return `
            <div class="form-field">
                <label>Field Name</label>
                <input type="text" class="form-control model-field" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control model-field" name="description">${this.model.description || ''}</textarea>
            </div>
            `;
        });

        this.addTab('type', 'Data Type', 'hashtag', () => `
            <div class="form-field">
                <label>Type</label>
                <select name="type" class="form-control model-field">${fieldTypes()}</select>
            </div>

            ${typeSpecificOptions()}
        `);

        this.addTab('model', 'Model', 'user-star-line', () => `
            <div class="form-field">
                <label>Model</label>
                <select name="modelName" class="form-control model-field">${modelSelectList()}</select>
            </div>

            ${modelProperties()}
        `);

    }

    bind() {
        super.bind();

        document.querySelectorAll('.create-field').forEach(x => {
            x.removeEventListener('click', this.save);
            x.addEventListener('click', this.save);
        });
    }

    readyToGo() {
        if (!this.model.name) return false;
        return true;
    }

    render() {

        var html = `

            ${this.renderTabUi()}


            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-field" ${this.readyToGo() ? '' : 'disabled'}>${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
