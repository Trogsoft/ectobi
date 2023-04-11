import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class newSchemaDialog extends dialogBase {

    values;

    fileContent;

    model = {
        mode: 1,
        file: null,
        name: null,
        description: null
    };

    constructor(sender, arg, token) {
        super(sender, arg, token);
        this.setTitle('New Schema');
        this.render();
    }

    radioSelect = e => {
        var target = e.currentTarget;
        var value = parseInt(target.value);
        this.model.mode = value;
        this.render();
    };

    selectFile = e => {
        window.ipc.selectFile({
            //filters: filters
        }).then(x => {
            this.model.file = x.filePaths[0];
            if (!this.model.name)
                this.model.name = this.getFileName(this.model.file).split('.')[0];
            window.ipc.getFileContents({ path: x.filePaths[0] }).then(fc => {
                this.fileContent = fc;
                this.render();
            }).catch(err => {
                window.ipc.alert(err.message);
            });
            this.render();
        });
    };

    updateModel = e => {
        var name = e.currentTarget.getAttribute('name');
        this.model[name] = e.currentTarget.value;
        this.render();
    };

    createSchema = e => {
        var model = {
            name: this.model.name,
            description: this.model.description,
            file: {}
        };

        if (this.model.mode == 2) {
            model.file = {
                filename: this.getFileName(this.model.file),
                data: this.fileContent
            };
            this.client.schema.create(model).then(x => {

                var upload = document.querySelector('#schema-upload-data');
                if (upload && upload.checked) {
                    var batchModel = {
                        schemaTid: x.result.textId,
                        batchName: model.file.filename,
                        batchSource: this.model.file,
                        binaryFile: model.file
                    };
                    this.client.batch.upload(batchModel).then(x=>{
                        window.ipc.closeMe();
                    }).catch(x=>{
                        window.ipc.alert({ message: x.statusMessage });
                    })
                } 

            }).catch(x=>{

                window.ipc.alert({ message: x.statusMessage });

            });
        }

    };

    bind() {
        super.bind();
        var root = document.querySelector('#content');
        root.querySelectorAll('input[type=radio]').forEach(x => {
            x.removeEventListener('click', this.radioSelect);
            x.addEventListener('click', this.radioSelect);
        });
        root.querySelectorAll('.select-file').forEach(x => {
            x.removeEventListener('click', this.selectFile);
            x.addEventListener('click', this.selectFile);
        });
        root.querySelectorAll('input[type=text]').forEach(x => {
            x.removeEventListener('input', this.updateModel);
            x.addEventListener('input', this.updateModel);
        });
        root.querySelectorAll('.create-schema').forEach(x => {
            x.removeEventListener('click', this.createSchema);
            x.addEventListener('click', this.createSchema);
        });
    }

    getFileName() {
        if (this.model.file) {
            var slash = this.model.file.indexOf('/') > -1 ? '/' : '\\';
            var parts = this.model.file.split(slash);
            return parts[parts.length - 1];
        }
        return '';
    }

    readyToGo() {
        if (this.model.mode == 2 && this.model.file == null)
            return false;
        if (!this.model.name)
            return false;
        return true;
    }

    render() {
        var html = `
            <div class="form-field">
                <label>Schema Name</label>
                <input type="text" class="form-control" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control" name="description"></textarea>
            </div>
            <div class="form-radio">
                <input type="radio" name="blank" value="1" checked id="new-schema-blank" />
                <label for="new-schema-blank">Create a blank schema</label>
            </div>
            <div class="form-radio">
                <input type="radio" name="blank" value="2" id="new-schema-file" />
                <label for="new-schema-file">Create a schema from a file</label>
            </div>
            <div class="form-field ps-3 mt-1">
                <button class="btn select-file" ${this.model.mode == 2 ? '' : 'disabled'}>Select File...</button>
                <label class="file-label">${this.getFileName()}</label>
                <div class="form-check mt-1">
                    <input type="checkbox" name="upload-data" value="1" ${this.model.mode == 2 ? '' : 'disabled'} checked id="schema-upload-data" />
                    <label for="schema-upload-data">Upload data as well as schema</label>
                </div>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-schema" ${this.readyToGo() ? '' : 'disabled'}>Create</button>
            </div>
        `;
        render('#content', html);
        this.bind();
    }
}
