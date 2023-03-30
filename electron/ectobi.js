import { ectoClient } from './ectoClient.js';
import { render } from './js/reef/reef.es.js';
import { schemaVersions } from './schemaVersions.js';
import { uiComponent, tabComponent, coreComponent } from './uiElement.js';
import { schemaUploads } from './schemaUploads.js';
import { table } from './table.js';
import { fieldType, fieldFlags } from "./enums.js";
import { ectoToolbar } from './toolbar.js';

class statusBar extends coreComponent {

    render() {
        render('.status', '<span>Hello</span>');
    }

}

class tree extends coreComponent {

    schemas = [];

    constructor(ecto) {
        super(ecto);
        ecto.client.schema.list().then(h => {
            this.schemas = h.result;
            this.render();
        });
    }

    render() {
        var html = '<ul>';

        var link = (url, icon, label, classes = 'navigate') => {
            var selected = this.ecto.treeNode == url;
            return `<li class="${selected ? 'hl' : ''}">
                <i class="bi ${icon}"></i> 
                <a href="${url}" class="${classes}">${label}</a>
            </li>`;
        }

        this.schemas.forEach(s => {
            html += `<li><i class="bi bi-folder"></i> ${s.name}<ul>
                ${link('schemaData/' + s.textId, 'bi-file-plus-fill', 'Data')}
                ${link('schemaUploads/' + s.textId, 'bi-card-list', 'Uploads')}
                ${link('schemaVersions/' + s.textId, 'bi-card-list', 'Versions')}
                ${link('schemaFields/' + s.textId, 'bi-card-checklist', 'Fields')}
            </ul>
            </li>`;
        })
        html += '</ul>';
        render('.tree', html);
        this.bind();
    }

    bind() {

        document.querySelectorAll('a.navigate').forEach(el => {
            el.removeEventListener('click', this.ecto.navigate);
            el.addEventListener('click', this.ecto.navigate);
        });

    }

}

class schemaFields extends tabComponent {

    table;
    populators;
    idCode;

    constructor(ecto, data) {
        super(ecto, data);

        this.idCode = data.path[1] + "-fields";

        this.table = new table({
            target: '.main',
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
                            if ((intx & flag) > 0) items.push(fieldFlags[flag]);
                        })
                        return items.join(', ');
                    }
                }
            }
        });
    }

    getTitle = () => "Fields";

    allowMultiple = () => false;

    getIdCode = () => this.idCode;

    editField = e => {
        var fieldId = this.table.getState().selectedItem.textId;
        this.ecto.navigateTo('schemaFieldEditor', [this.data[1], fieldId])
    }

    init() {

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

        this.client.field.list(this.data.path[1]).then(x => {
            this.table.setData(x.result);
        });

        this.client.populator.list().then(x => {
            this.populators = x.result;
            this.ecto.toolbar.render();
        });

        this.ecto.toolbar.add('schemaFields',
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
            }
        );

        this.table.render();

    }

    render() {
        this.table.render();
    }

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.ecto.toolbar.clear('schemaFields');
    }

}

class schemaFieldEditor extends uiComponent {

    field;
    title = "Field Editor";

    constructor(client, ecto, args) {
        super(client, ecto, args);
    }

    getTitle = () => this.title;

    init() {
        this.client.field.getLatest(this.data[0], this.data[1]).then(x => {
            this.field = x.result;
            this.title = x.result.name;
            this.render();
        });
    }

    render() {
        let field = this.field;
        if (!field) return;
        var html = `
            <div class="editor">
                <div class="row mb">
                    <div class="col-2">
                        <div class="editor-field">
                            <label>Name</label>
                            <input type="text" class="editor-control" name="name" value="${field.name}" />
                        </div>
                    </div>
                    <div class="col-2">
                        <div class="editor-field">
                            <label>Name</label>
                            <input type="text" class="editor-control" name="name" value="${field.name}" />
                        </div>
                    </div>
                </div>
            </div>
        `;
        render(this.target, html);
    }

}

class tabController extends coreComponent {

    tabs = [];
    currentIndex = -1;

    constructor(client, ecto) {
        super(client, ecto);
    }

    render() {
        var html = '<div class="tab-row">';
        this.tabs.forEach((tab, i) => {
            html += `<a href="#" class="tab" data-tab-id="${i}">${tab.getTitle()}</a>`;
        })
        html += '</div>';
        html += '<div class="main"></div>'
        render('.tabs', html);

        if (this.currentIndex > -1)
            this.tabs[this.currentIndex].render();
    }

    openTab = (instance) => {
        if (instance.init) instance.init();
        this.tabs.unshift(instance);
        this.currentIndex = this.tabs.length - 1;
        this.render();
    }

}

class ectobi {

    client = new ectoClient();
    toolbar;
    statusBar;
    tree;
    tabControl;

    treeNode;

    components = {
        'schemaVersions': schemaVersions,
        'schemaUploads': schemaUploads,
        'schemaFields': schemaFields,
        'schemaFieldEditor': schemaFieldEditor
    }

    constructor() {
        this.client = new ectoClient(this);
        this.toolbar = new ectoToolbar(this);
        this.statusBar = new statusBar(this);
        this.tree = new tree(this);
        this.tabControl = new tabController(this);

        document.onreadystatechange = e => {
            if (document.readyState === "complete") {
                this.render();
            }
        }
    }

    render = () => {

        var html = `
            <div class="toolbar"></div>
            <div class="main-panel">
                <div class="tree"></div>
                <div class="tabs">
                </div>
            </div>
            <div class="status"></div>
        `;

        render('.app', html);
        this.toolbar.render();
        this.statusBar.render();
        this.tree.render();
        this.tabControl.render();
        this.bind();

    }

    openTab = (component, url, href) => {
        var data = {
            path: url
        }
        var componentRef = this.components[component];
        var instance = new componentRef(this, data);

        this.tabControl.openTab(instance);
    }

    navigate = (e) => {

        e.preventDefault();

        var href = e.currentTarget.getAttribute('href');
        var url = href.split('/');

        var component = url[0];
        this.openTab(component, url, href);

    }

    bind = () => {

        this.tree.bind();

    }

}

new ectobi();
