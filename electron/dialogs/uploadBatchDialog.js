import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class uploadBatchDialog extends dialogBase {

    selectedFiles = [];
    importers = [];
    uploadTasks = [];
    completedTasks = 0;

    constructor(sender, arg, token) {
        super(sender, arg, token);
        this.client.importer.list().then(x => {
            this.importers = x.result;
            this.render();
        });
        this.setTitle('Upload Data');
    }

    selectFiles = (e) => {
        var filters = this.importers.map(x => ({ name: x.name, extensions: x.extensions }));
        filters.unshift({ name: 'All Supported Files', extensions: this.importers.map(x => x.extensions).flat(1) });
        window.ipc.selectFile({
            filters: filters
        }).then(x => {
            this.selectedFiles.push(...x.filePaths);
            this.render();
        });
    };

    removeItem = e => {
        var idx = e.currentTarget.getAttribute('data-index');
        if (!idx)
            return;

        var numericIndex = parseInt(numericIndex);
        this.selectedFiles.splice(numericIndex, 1);
        this.render();
    };

    bind() {
        super.bind();
        var root = document.querySelector('#content');
        var selectFile = root.querySelector('.select-file');
        selectFile.removeEventListener('click', this.selectFiles);
        selectFile.addEventListener('click', this.selectFiles);

        root.querySelectorAll('.upload-files').forEach(x=>{
            x.removeEventListener('click', this.upload);
            x.addEventListener('click', this.upload);
        })

        root.querySelectorAll('.remove-from-list').forEach(x => {
            x.removeEventListener('click', this.removeItem);
            x.addEventListener('click', this.removeItem);
        });
    }

    getFileName = (file) => {
        var slash = file.indexOf('/') > -1 ? '/' : '\\';
        var parts = file.split(slash);
        return parts[parts.length - 1];
    }

    upload = (e) => {

        this.selectedFiles.forEach(file=>{

            window.ipc.getFileContents({ path: file }).then(fc => {

                var model = {
                    schemaTid: this.args.schema,
                    batchName: this.getFileName(file),
                    batchSource: file,
                    binaryFile: {
                        filename: this.getFileName(file),
                        data: fc
                    }
                }
                var task = this.client.batch.upload(model);
                task.then(x=>{
                    this.completedTasks++;
                    if (this.completedTasks == this.uploadTasks.length)
                        window.ipc.closeMe();
                    else 
                        this.render();
                })
                this.uploadTasks.push(task);
                this.render();

            }).catch(err => {
                window.ipc.alert({ message: err.message });
            });

        });

    }

    render() {
        var html = `
            <div class="form-field form-field-oneline">
                <label>File to Upload</label>
                <button class="btn select-file">Select File(s)</button>
            </div>`;

        if (this.selectedFiles.length > 0) {
            html += `
                <div class="form-field form-field-oneline">
                    <label>Selected Files</label>
                    <ul class="form-field-list">
                        ${this.selectedFiles.map((x, i) => '<li><div class="flex-grow-4 me-1">' + x + '</div><a href="#" class="flex-grow-1 remove-from-list" data-index="' + i + '" title="Remove File"><i class="ri-delete-bin-fill"></i></a></li>').join('\r\n')}
                    </ul>
                </div>
            `;
        }

        html += `<div class="form-field form-field-oneline">
                <label>Target schema</label>
                <p class="form-field-text">${this.args.schema}</p>
            </div>`;

        if (this.uploadTasks.length > 0) {
            var pc = ((this.completedTasks / this.uploadTasks.length * 100) / 5) * 5;

            html += `<div class="progress">
                <div class="progress-bar w-${pc}pc">
                    <span class="progress-label">${pc}%</span>
                </div>
            </div>`;

        }

        html += `<div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success upload-files ${this.selectedFiles.length > 0 ? '' : 'disabled'}">Upload</button>
            </div>
        `;
        render('#content', html);
        this.bind();
    }
}
