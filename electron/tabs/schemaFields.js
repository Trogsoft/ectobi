import { ectoTabComponent } from '../components.js';
import { table } from "../table.js";
import { fieldType, fieldFlags } from "../enums.js";
import { render } from '../js/reef/reef.es.js';

export class schemaFieldEditor extends ectoTabComponent {

    idCode;
    field = null;
    tabTitle = "Field Editor";
    populators = [];

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = (data.path[0] + '-' + data.path[1] + '-editor').replace('.', '-');
    }

    getIdCode = () => this.idCode;

    getTitle = () => this.tabTitle;

    init() {
        this.client.field.getLatest(this.data.path[0], this.data.path[1]).then(x => {
            this.field = x.result;
            this.tabTitle = `${x.result.name} Editor`;
            this.render();
            this.emit('tabTitleChanged', this.idCode);
        });

        this.client.populator.list().then(x => {
            this.populators = x.result;
        });
    }

    fieldTypeDropdown(current) {
        var html = '<select name="type" class="editor-control">';
        Object.keys(fieldType).map(x => ({ id: x, label: fieldType[x] })).forEach(x => {
            html += `<option value="${x.id}" ${current == x.id ? 'selected' : ''}>${x.label}</option>`;
        });
        html += '</select>';
        return html;
    }

    fieldFlags(current) {
        var html = '';
        Object.keys(fieldFlags).forEach(f => {
            html += '<input type="checkbox"> ' + fieldFlags[f] + '<br />';
        });
        return html;
    }

    render() {
        if (!this.readyToRender()) return;
        if (!this.field) return;

        var html = `
            <div class="editor">
                <div class="row mb">
                    <div class="col-2">
                        <div class="editor-field">
                            <label>Name</label>
                            <input type="text" name="name" class="editor-control" value="${this.field.name}" />
                        </div>
                    </div>
                    <div class="col-2">
                        <div class="editor-field">
                            <label>Type</label>
                            ${this.fieldTypeDropdown(this.field.type)}
                        </div>
                    </div>
                </div>
                <div class="row mb">
                    <div class="col-2">
                        <div class="editor-field">
                            <label>Name</label>
                            ${this.fieldFlags(this.field.flags)}
                        </div>
                    </div>
                    <div class="col-2">
                        <div class="editor-field">
                        </div>
                    </div>
                </div>
            </div>
        `;

        render(this.target, html);

    }

}

export class schemaFields extends ectoTabComponent {

    idCode;
    table;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-fields';

        this.table = new table({
            target: this.target,
            headers: {
                'name': {
                    label: 'Name'
                },
                type: {
                    label: 'Type',
                    format: x => fieldType[x] ?? x
                },
                flags: {
                    label: 'Flags',
                    format: x => {
                        var items = [];
                        var intx = parseInt(x);
                        Object.keys(fieldFlags).forEach(f => {
                            var flag = parseInt(f);
                            if ((intx & flag) > 0)
                                items.push(fieldFlags[flag]);
                        });
                        return items.join(', ');
                    }
                }
            }
        });

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

    }

    setTarget(target) {
        this.target = target;
        this.table.setTarget(target);
    }

    newField = e => {
        this.openDialog('fieldEditorDialog', {
            height: 700,
            width: 700,
            schema: this.data.path[0],
            field: null
        });
    }

    editField = e => {
        var item = this.table.getState().selectedItem;
        this.openDialog('fieldEditorDialog', {
            height: 700,
            width: 700,
            schema: this.data.path[0],
            field: item.textId
        });
    }

    init(soft = false) {
        if (!soft) {
            this.client.field.list(this.data.path[0]).then(x => {
                this.table.setData(x.result);
                this.render();
            });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                addField: {
                    type: 'button',
                    label: 'Add Field',
                    action: this.newField
                },
                editField: {
                    type: 'button',
                    label: 'Edit',
                    action: this.editField,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                deleteField: {
                    type: 'button',
                    label: 'Delete',
                    action: this.deleteField,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                divider: {
                    type: 'divider'
                }
            });
    }

    render() {
        if (this.readyToRender())
            this.table.render();
    }

    getIdCode = () => this.idCode;

    getTitle = () => this.data.path[0].toUpperCase() + " Fields";

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.ecto.toolbar.clear(this.idCode);
    }
}
