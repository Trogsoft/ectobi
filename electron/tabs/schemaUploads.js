import { table } from '../table.js';
import { ectoTabComponent } from '../components.js';

export class schemaRecordEditor extends ectoTabComponent {

    idCode;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-' + data.path[1] + '-recordEditor'
    }

    getTitle = () => this.data.path[0].toUpperCase() + " Record Editor";

    getIdCode = () => this.idCode;

}

export class schemaUploads extends ectoTabComponent {

    table;
    idCode;

    constructor(ecto, target, data) {

        super(ecto, target, data);
        this.idCode = data.path[0] + '-uploads';

        this.table = new table({
            target: '.main',
            headers: {
                'name': {
                    label: 'Name'
                },
                created: {
                    label: 'Created',
                    format: x => new Date(x).toLocaleString()
                }
            }
        });

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

    }

    getTitle = () => this.data.path[0].toUpperCase() + " Uploads";

    setTarget(target) {
        this.target = target;
        this.table.setTarget(target);
    }

    uploadData = async (e) => {

        await window.ipc.openDialog({
            dialogType: 'uploadBatchDialog',
            schema: this.data.path[0],
            width: 500,
            height: 500
        }, result => {
            console.log(result);
        });

    };

    newBatch = e => {
    };

    addRecord = e => {
        var record = this.table.getState().selectedItem.id;
        this.ecto.tabManager.openNewTab(`schemaRecordEditor/${this.data.path[0]}/${record}`);
    };

    getIdCode = () => this.idCode;

    deleteBatch = e => {
        window.ipc.confirm({
            message: 'Are you sure you want to delete this batch?'
        }).then(x => {
            if (x.response == 1) return;

            var id = this.table.getState().selectedItem.id;
            this.ecto.client.batch.delete(id).then(x => {
                this.init(false);
            }).catch(err => {
                window.ipc.alert({ message: err.message });
            })
        })
    }

    init(soft = false) {

        if (!soft) {
            this.client.batch.list(this.data.path[0]).then(h => {
                this.table.setData(h.result);
                this.render();
            });
        }

        this.ecto.toolbar.add(this.idCode,
            {
                uploadData: {
                    type: 'button',
                    label: 'Upload Batch',
                    action: this.uploadData
                },
                newBatch: {
                    type: 'button',
                    label: 'New Empty Batch',
                    action: this.newBatch
                },
                addRecord: {
                    type: 'button',
                    label: 'Add Records',
                    action: this.addRecord,
                    enable: x => this.table.getState().selectedItemCount > 0
                },
                deleteBatch: {
                    type: 'button',
                    label: 'Delete',
                    enable: x => this.table.getState().selectedItemCount > 0,
                    action: this.deleteBatch
                }
            }
        );
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
