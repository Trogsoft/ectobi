import { table } from '../table.js';
import { ectoTabComponent } from '../components.js';

export class webHookManager extends ectoTabComponent {

    table;
    idCode;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = 'webhooks';

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
        window.ipc.openDialog({
            dialogType: 'webHookEditorDialog',
            schema: 0,
            width: 700,
            height: 600
        });
    }

    editResource = e => {
        window.ipc.openDialog({
            dialogType: 'webHookEditorDialog',
            id: this.table.getState().selectedItem.id,
            width: 700,
            height: 600
        });
    }

    deleteResource = e => {

    }

    init(soft = false) {

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

        if (!soft) {
            this.client.webhook.list().then(h => {
                this.table.setData(h.result);
                this.render();
            });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                newResource: {
                    type: 'button',
                    label: 'New WebHook',
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

    getTitle = () => "WebHooks";

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