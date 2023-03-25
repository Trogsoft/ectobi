import { table } from './table.js';
import { uiElement } from './uiElement.js';

export class schemaVersions extends uiElement {

    client;
    ecto;
    data;

    versions;
    table;

    constructor(client, ecto, ...data) {
        super();
        this.client = client;
        this.ecto = ecto;
        this.data = data;
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

        this.ecto.toolbar.add('schemaVersions',
            {
                newSchemaVersion: {
                    type: 'button',
                    label: 'New Version',
                    action: () => { }
                },
            }
        );
    }

    init() {
        this.client.schema.getVersions(this.data[1]).then(h => {
            this.table.setData(h.result);
        });
    }

    beforeUnload() {
        this.ecto.toolbar.clear('schemaVersions');
    }

    render() {
    }
}
