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
                <i class="bi ${icon}"></i> 
                <a href="${url}" class="${classes}">${label}</a>
            `;
            if (!excludeEndTag)
                html += '</li>';
            return html;
        };

        html += `<li>${link('lookupManager', 'bi-table', 'Lookup Tables')}</li>`;

        this.schemas.forEach(s => {
            html += `<li>${link('schemaDetail/' + s.textId, 'bi-folder', s.name, true)}<ul>
                ${link('schemaData/' + s.textId, 'bi-file-plus-fill', 'Data')}
                ${link('schemaUploads/' + s.textId, 'bi-card-list', 'Uploads')}
                ${link('schemaVersions/' + s.textId, 'bi-card-list', 'Versions')}
                ${link('schemaFields/' + s.textId, 'bi-card-checklist', 'Fields')}
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
