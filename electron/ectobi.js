import { ectoClient } from './ectoClient.js';
import { render } from './js/reef/reef.es.js';
import { schemaVersions } from './schemaVersions.js';
import { uiElement } from './uiElement.js';

class toolbar extends uiElement {

    controls = {
    };

    constructor() {
        super();
    }

    add(group, controls) {
        this.controls[group] = controls;
        this.render();
    }

    clear(group) {
        delete this.controls[group];
        this.render();
    }

    render() {

        var html = `
            <button class="tb-btn" title="Connect to..."><i class="bi bi-server"></i></button>
            <div class="tb-divider"></div>
            <div class="btn-group">
                <button title="Create a new blank schema" class="tb-btn"><i class="bi bi-table"></i> New Schema</button>
                <button title="Create a new schema from a file" class="tb-btn"><i class="bi bi-cloud-plus-fill"></i> From File...</button>
            </div>
        `;

        var renderControl = (id, c) => {
            if (c.type == 'button' || !c.type) {
                return `<button data-control-id="${id}" class="btn">${c.label || 'Button'}</button>`;
            }
        }

        if (Object.keys(this.controls).length > 0) {

            Object.keys(this.controls).forEach(g => {

                html += '<div class="tb-divider"></div>';

                var controlGroup = this.controls[g];
                Object.keys(controlGroup).forEach(c=>{
                    var control = this.controls[g][c];
                    html += renderControl(c, control);
                });

            });
        }

        render('.toolbar', html);

    }

}

class statusBar extends uiElement {

    render() {
        render('.status', '<span>Hello</span>');
    }

}

class tree extends uiElement {

    client;
    ecto;
    schemas = [];

    constructor(client, ecto) {
        super();
        this.client = client;
        this.ecto = ecto;
        this.client.schema.list().then(h => {
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

class ectobi extends uiElement {

    client = new ectoClient();
    toolbar;
    statusBar;
    tree;
    content;
    treeNode;

    pages = {
        'schemaVersions': schemaVersions
    }

    constructor() {
        super();
        this.toolbar = new toolbar(this.client, this);
        this.statusBar = new statusBar(this.client, this);
        this.tree = new tree(this.client, this);
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
                <div class="main"></div>
            </div>
            <div class="status"></div>
        `;
        render('.app', html);
        this.toolbar.render();
        this.statusBar.render();
        this.tree.render();
        if (this.content) this.content.render();
        this.bind();
    }

    navigate = (e) => {

        e.preventDefault();

        if (this.content && this.content.beforeUnload)
            this.content.beforeUnload();

        var href = e.currentTarget.getAttribute('href');
        var url = href.split('/');

        var page = url[0];
        if (this.pages[page]) {
            var pageRef = this.pages[page];
            this.content = new pageRef(this.client, this, ...url);
            this.treeNode = href;
            this.content.init();
            this.render();
        }

    }

    bind = () => {

        this.tree.bind();

    }

}

new ectobi();

export { ectobi, uiElement };