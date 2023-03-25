import { render } from './js/reef/reef.es.js';
import { uiElement } from "./uiElement.js";

class table extends uiElement {

    opts = {
        headers: {}
    };
    #data = [];

    #state = {
        rowCount: 0,
        selectedItemIndex: -1,
        selectedItem: null
    };

    constructor(opts) {
        super();
        this.opts = opts;
    }



    getState = () => this.#state;

    setData(data) {
        this.#data = data;
        this.#state.rowCount = this.#data.length;
        this.render();
    }

    render() {

        var headers = () => {
            if (!this.opts.headers)
                return '';
            var o = '<tr>';
            Object.keys(this.opts.headers).forEach(k => {
                var header = this.opts.headers[k];
                o += `<th>${header.label || k}</th>`;
            });
            o += '</tr>';
            return o;
        };

        var rows = () => {
            if (!this.#data)
                return '';
            var html = '';
            this.#data.forEach((r, i) => {
                var rowId = (r.id || r.textId);
                html += `<tr data-id="${rowId}" class="${i == this.#state.selectedItemIndex ? 'hl' : ''}">`;
                Object.keys(this.opts.headers).forEach(k => {
                    var value = r[k];
                    html += `<td>${value}</td>`;
                });
                html += ' </tr>';
            });
            return html;
        };

        var html = `
            <table class="tbl">
                <thead>
                    ${headers()}
                </thead>
                <tbody>
                    ${rows()}
                </tbody>
            </table>
        `;

        render(this.opts.target, html);
        this.bind();

    }

    selectRow = e => {

        var el = e.currentTarget;
        var id = el.getAttribute('data-id');
        if (!id) return;

        var idx = this.#data.findIndex(x => (x.id || x.textId) == id);
        if (idx > -1 && this.#state.selectedItemIndex != idx) {
            this.#state.selectedItemIndex = idx;
            this.#state.selectedItem = this.#data[idx];
            this.render();
        } else if (idx > -1 && this.#state.selectedItemIndex == idx) {
            this.#state.selectedItemIndex = -1;
            this.#state.selectedItem = null;
            this.render();
        }

    }

    bind() {

        var target = document.querySelector(this.opts.target);
        target.querySelectorAll('tr[data-id]').forEach(el => {
            el.removeEventListener('click', this.selectRow);
            el.addEventListener('click', this.selectRow);
        });

    }

}

export { table }