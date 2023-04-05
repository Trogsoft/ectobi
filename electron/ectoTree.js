import { render } from './js/reef/reef.es.js';
import { ectoComponent, ectoCoreComponent } from './components.js';

export class ectoTree extends ectoCoreComponent {

    schemas = [];

    constructor(ecto, target) {
        super(ecto, target);
        this.init();
    }

    init() {
        this.ecto.client.schema.list().then(h => {
            this.schemas = h.result;
            this.render();
        });        
    }

    render() {
        var html = '<ul>';

        var link = (url, icon, label, excludeEndTag = false, classes = 'navigate') => {
            var selected = this.ecto.treeNode == url;
            var html = `<li class="${selected ? 'hl' : ''}">
                <i class="ri-md ${icon}"></i> 
                <a href="${url}" class="${classes}">${label}</a>
            `;
            if (!excludeEndTag)
                html += '</li>';
            return html;
        };

        this.schemas.forEach(s => {
            html += `<li>${link('schemaDetail/' + s.textId, 'ri-folder-fill', s.name, true)}<ul>
                ${link('schemaData/' + s.textId, 'ri-database-2-fill', 'Data', true)}
                <ul>
                ${link('lookupManager', 'ri-table-2', 'Lookup Tables')}
                </ul>
                ${link('schemaUploads/' + s.textId, 'ri-upload-2-fill', 'Uploads')}
                ${link('schemaVersions/' + s.textId, 'ri-grid-line', 'Versions')}
                ${link('schemaFields/' + s.textId, 'ri-input-method-fill', 'Fields')}
                ${link('webHookManager', 'ri-plug-fill', 'WebHook Configuration')}
                </ul>
            </li>`;
        });

        html += '</ul>';
        render('.tree', html);
        this.bind();
    }

    bind() {

        document.querySelector(this.target).querySelectorAll('a.navigate').forEach(el => {
            el.removeEventListener('click', this.ecto.navigate);
            el.addEventListener('click', this.ecto.navigate);
        });

    }
}
