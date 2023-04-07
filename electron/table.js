import { ectoComponent } from './components.js';
import { render } from './js/reef/reef.es.js';

const KEY_UP = 38;
const KEY_DOWN = 40;
class table extends ectoComponent {

    opts = {
        headers: {}
    };
    #data = [];

    #state = {
        rowCount: 0,
        selectedItemCount: 0,
        selectedItemIndex: -1,
        selectedItem: null,
        selectedItems: []
    };

    constructor(opts) {
        super();
        this.opts = opts;
    }

    getState = () => this.#state;

    setData = (data) => {
        this.#data = data;
        this.#state.rowCount = this.#data.length;
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
            if (!this.#data || this.#data.length == 0)
                return '';
            var html = '';
            this.#data.forEach((r, i) => {
                var rowId = (r.id || r.textId);
                html += `<tr data-id="${rowId}" class="${i == this.#state.selectedItemIndex ? 'hl' : ''}">`;
                Object.keys(this.opts.headers).forEach(k => {
                    var value = r[k];
                    var header = this.opts.headers[k];
                    if (header.format) value = header.format(r[k]);
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
            this.#state.selectedItemCount = 1;
            this.emit('tableStateChanged', this.#state);
            this.render();
        } else if (idx > -1 && this.#state.selectedItemIndex == idx) {
            this.#state.selectedItemIndex = -1;
            this.#state.selectedItem = null;
            this.#state.selectedItemCount = 0;
            this.emit('tableStateChanged', this.#state);
            this.render();
        }

    }

    setTarget(target) {
        this.opts.target = target;
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