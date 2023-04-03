import { table } from './table.js';
import { ectoTabComponent } from './components.js';

export class lookupManager extends ectoTabComponent {

    table;
    idCode;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-lookups';

        this.table = new table({
            target: '.main',
            headers: {
                'name': {
                    label: 'Name'
                },
                description: {
                    label: 'Description'
                }
            }
        });

    }

    newResource = e => {

    }

    editResource = e => {

    }

    deleteResource = e => {

    }

    init(soft = false) {

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

        if (!soft) {
            // this.client.lookup.list().then(h => {
            //     this.table.setData(h.result);
            //     this.render();
            // });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                newResource: {
                    type: 'button',
                    label: 'New',
                    action: this.newResource
                },
                editResource: {
                    type: 'button',
                    label: 'Edit',
                    action: this.editResource,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                deleteResource: {
                    type: 'button',
                    label: 'Delete',
                    action: this.deleteResource,
                    enable: x => this.table.getState().selectedItemCount > 0
                }
            }
        );
    }

    getTitle = () => "";

    getIdCode = () => this.idCode;

    setTarget(target) {
        this.target = target;
        this.table.setTarget(target);
    }

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.ecto.toolbar.clear(this.idCode);
    }

    render() {
        if (this.readyToRender())
            this.table.render();
    }

}