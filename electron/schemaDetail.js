import { table } from './table.js';
import { ectoTabComponent } from './components.js';
import { render } from './js/reef/reef.es.js';

export class schemaDetail extends ectoTabComponent {

    idCode;
    schema;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-detail';
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
            this.client.schema.get(this.data.path[0]).then(h => {
                this.schema = h.result;
                this.render();
            });
        }

    }

    getTitle = () => this.schema ? this.schema.name : this.data.path[0].toUpperCase();

    getIdCode = () => this.idCode;

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.ecto.toolbar.clear(this.idCode);
    }

    deleteSchema = e => {
        window.ipc.confirm({
            message: 'Deleting this schema will remove all associated data. Are you sure?'
        }).then(x=>{
            if (x.response == 0) {
                this.client.schema.delete(this.data.path[0]).then(y=>{
                    this.ecto.tree.init();
                })
            }
        });
    }

    render() {
        if (!this.readyToRender() || this.schema == null)
            return;

        var html = `
        <div class="editor">
            <div class="row">
                <div class="col-2 me-3">
                    <div class="form-field">
                        <label>Schema Name</label>
                        <input type="text" class="form-control" value="${this.schema.name}" />
                    </div>
                </div>
                <div class="col-2">
                    <div class="form-field">
                        <label>Description</label>
                        <textarea class="form-control">${this.schema.description || ''}</textarea>
                    </div>
                </div>
            </div>
            <div class="editor-buttons">
                <button class="btn btn-danger delete-schema">Delete Schema</button>
                <button class="btn btn-success save-changes">Save Changes</button>
            </div>
        </div>
        `;

        render(this.target, html);
        this.bind();
    }

    bind() {
        var root = document.querySelector(this.target);
        if (root) {
            root.querySelectorAll('.delete-schema').forEach(x=>{
                x.removeEventListener('click', this.deleteSchema);
                x.addEventListener('click', this.deleteSchema);
            })
        }
    }

}