import { render } from './js/reef/reef.es.js';
import { ectoComponent, ectoCoreComponent } from './components.js';

export class ectoTree extends ectoCoreComponent {

    schemas = [];

    constructor(ecto, target) {
        super(ecto, target);
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
        };

        this.schemas.forEach(s => {
            html += `<li><i class="bi bi-folder"></i> ${s.name}<ul>
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
