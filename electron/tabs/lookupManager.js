import { table } from '../table.js';
import { ectoTabComponent } from '../components.js';

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
                'description': {
                    label: 'Description'
                },
                'values': {
                     label: 'Values',
                     format: x => x.values ? x.values.length : 0
                }
            }
        });

    }

    editValues = e => {
        window.ipc.openDialog({
            dialogType: 'lookupValuesDialog',
            id: this.table.getState().selectedItem.textId,
            width: 600,
            height: 700
        });
    }

    editLookup = e => {

    }

    deleteRecord = e => {

    }

    newLookup = e => {

    }

    init(soft = false) {

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

        if (!soft) {
            this.client.lookup.list().then(h => {
                this.table.setData(h.result);
                this.render();
            });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                editValues: {
                    type: 'button',
                    label: 'Values',
                    action: this.editValues,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                div1: {
                    type: 'divider'
                },
                newLookup: {
                    type: 'button',
                    label: 'New Lookup',
                    action: this.newLookup
                },
                editLookup: {
                    type: 'button',
                    label: 'Edit Lookup',
                    action: this.editLookup,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                deleteLookup: {
                    type: 'button',
                    label: 'Delete',
                    action: this.deleteRecord,
                    enable: x => this.table.getState().selectedItemCount > 0
                }
            }
        );
    }

    getTitle = () => "Lookup Tables";

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