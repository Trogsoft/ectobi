import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class uploadBatchDialog extends dialogBase {

    selectedFiles = [];
    importers = [];
    client = new ectoClient();

    constructor(sender, arg) {
        super(sender, arg);
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

        root.querySelectorAll('.remove-from-list').forEach(x => {
            x.removeEventListener('click', this.removeItem);
            x.addEventListener('click', this.removeItem);
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
