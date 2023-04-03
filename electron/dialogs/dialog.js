import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js"
import { table } from "../table.js";

export class dialogBase {

    title;
    args;
    sender;

    constructor(sender, args) {
        this.sender = sender;
        this.args = args;
    }

    setTitle(title) {
        document.querySelector('.dlg-header').querySelector('h1').innerText = title;
    }

    bind() {
        document.querySelectorAll('.close-dialog').forEach(cb => {
            cb.removeEventListener('click', this.close);
            cb.addEventListener('click', this.close);
        });
    }

    close = (e) => {
        window.ipc.closeMe();
    }

    render() {
    }

}

class uploadBatchDialog extends dialogBase {

    selectedFiles = [];
    importers = [];
    client = new ectoClient();

    constructor(sender, arg) {
        super(sender, arg);
        this.client.importer.list().then(x=>{
            this.importers = x.result;
            this.render();
        });
        this.setTitle('Upload Data');
    }

    selectFiles = (e) => {
        var filters = this.importers.map(x=>({ name: x.name, extensions: x.extensions }));
        filters.unshift({ name: 'All Supported Files', extensions: this.importers.map(x=>x.extensions).flat(1)  });
        window.ipc.selectFile({
            filters: filters
        }).then(x => {
            this.selectedFiles.push(...x.filePaths);
            this.render();
        })
    }

    removeItem = e =>{
        var idx = e.currentTarget.getAttribute('data-index');
        if (!idx) return;

        var numericIndex = parseInt(numericIndex);
        this.selectedFiles.splice(numericIndex, 1);
        this.render();
    }

    bind() {
        super.bind();
        var root = document.querySelector('#content');
        var selectFile = root.querySelector('.select-file');
        selectFile.removeEventListener('click', this.selectFiles);
        selectFile.addEventListener('click', this.selectFiles);

        root.querySelectorAll('.remove-from-list').forEach(x=>{
            x.removeEventListener('click', this.removeItem);
            x.addEventListener('click', this.removeItem);
        });
    }

    render() {
        var html = `
            <div class="form-field form-field-oneline">
                <label>File to Upload</label>
                <button class="btn select-file">Select File(s)</button>
            </div>`

        if (this.selectedFiles.length > 0) {
            html += `
                <div class="form-field form-field-oneline">
                    <label>Selected Files</label>
                    <ul class="form-field-list">
                        ${this.selectedFiles.map((x, i) => '<li><div class="flex-grow-4 me-1">' + x + '</div><a href="#" class="flex-grow-1 remove-from-list" data-index="' + i + '"><i class="bi bi-trash"></i></a></li>').join('\r\n')}
                    </ul>
                </div>
            `;
        }

        html += `<div class="form-field form-field-oneline">
                <label>Target schema</label>
                <p class="form-field-text">${this.args.schema}</p>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success upload-files ${this.selectedFiles.length > 0 ? '' : 'disabled'}">Upload</button>
            </div>
        `;
        render('#content', html);
        this.bind();
    }

}

class lookupValuesDialog extends dialogBase {

    values;
    client = new ectoClient();
    table;

    constructor(sender, arg) {
        super(sender, arg);
        this.setTitle('Lookup Values');
        
        this.table = new table({
            target: '.form-field-table',
            headers: {
                name: {
                    label: 'Name'
                },
                numericValue: {
                    label: 'Value'
                }
            }
        });

        this.client.lookup.get(arg.id).then(x=>{
            this.values = x.result.values;
            this.setTitle(x.result.name+' Values');
            this.table.setData(this.values);
            this.render();
        });

    }

    bind() {
        super.bind();
        var root = document.querySelector('#content');
    }

    render() {
        var html = `
            <div class="form-field">
                <div class="form-field-table">
                </div>
            </div>
        `;
        render('#content', html);
        this.table.render();
        this.bind();
    }

}

class newSchemaDialog extends dialogBase {

    values;
    client = new ectoClient();

    model = {
        mode: 1,
        file: null,
        name: null,
        description: null
    }

    constructor(sender, arg) {
        super(sender, arg);
        this.setTitle('New Schema');
        this.render();
    }

    radioSelect = e =>{
        var target = e.currentTarget;
        var value = parseInt(target.value);
        this.model.mode = value;
        this.render();
    }

    selectFile = e =>{
        window.ipc.selectFile({
            //filters: filters
        }).then(x => {
            this.model.file = x.filePaths[0];
            this.render();
        })
    }

    updateModel = e => {
        var name = e.currentTarget.getAttribute('name');
        this.model[name] = e.currentTarget.value;
        this.render();
    }

    bind() {
        super.bind();
        var root = document.querySelector('#content');
        root.querySelectorAll('input[type=radio]').forEach(x=>{
            x.removeEventListener('click', this.radioSelect);
            x.addEventListener('click', this.radioSelect);
        });
        root.querySelectorAll('.select-file').forEach(x=>{
            x.removeEventListener('click', this.selectFile);
            x.addEventListener('click', this.selectFile);
        });
        root.querySelectorAll('input[type=text]').forEach(x=>{
            x.removeEventListener('input', this.updateModel);
            x.addEventListener('input', this.updateModel);
        })
    }

    getFileName() {
        if (this.model.file) {
            var slash = this.model.file.indexOf('/') > -1 ? '/' : '\\';
            var parts = this.model.file.split(slash);
            return parts[parts.length-1];
        }
        return '';
    }

    readyToGo() {
        if (this.model.mode == 2 && this.model.file == null) return false;
        if (!this.model.name) return false;
        return true;
    }

    render() {
        var html = `
            <div class="form-field">
                <label>Schema Name</label>
                <input type="text" class="form-control" name="name" />
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
                <button class="btn select-file ${this.model.mode == 2 ? '' : 'disabled'}">Select File...</button>
                <label class="file-label">${this.getFileName()}</label>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-schema ${this.readyToGo() ? '' : 'disabled'}">Create</button>
            </div>
        `;
        render('#content', html);
        this.bind();
    }

}

export const dialogs = {
    'uploadBatchDialog': uploadBatchDialog,
    'lookupValuesDialog': lookupValuesDialog,
    'newSchemaDialog': newSchemaDialog
}

window.addEventListener('DOMContentLoaded', x => {

    window.ipc.dialogConfiguration((event, value) => {

        if (value.dialogType) {
            var dt = dialogs[value.dialogType];
            if (dt) {
                new dt(event.sender, value);
            }
        }

    });

})