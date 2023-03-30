import { ectoTabComponent } from './components.js';
import { table } from './table.js';

export class schemaVersions extends ectoTabComponent {

    versions;
    table;
    idCode;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-versions';

        this.table = new table({
            target: '.main',
            headers: {
                'name': {
                    label: 'Name'
                },
                'version': {
                    label: 'Version'
                },
                'created': {
                    label: 'Created'
                }
            }
        });

    }

    setTarget(target) {
        this.target = target;
        this.table.setTarget(target);
    }

    init(soft = false) {

        if (!soft) {
            this.ecto.client.schema.getVersions(this.data.path[0]).then(h => {
                this.table.setData(h.result);
                this.render();
            });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                newSchemaVersion: {
                    type: 'button',
                    label: 'New Version',
                    action: () => { }
                },
            }
        );
    }

    getTitle = () => this.data.path[0].toUpperCase() + " Versions";

    beforeUnload() {
        this.ecto.toolbar.clear(this.idCode);
    }

    getIdCode = () => this.idCode;

    render() {
        if (this.readyToRender())
            this.table.render();
    }
}
