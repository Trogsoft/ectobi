import { render } from './js/reef/reef.es.js';
import { ectoComponent } from "./components.js";

export class ectoToolbar extends ectoComponent {

    controls = {};

    constructor(ecto) {
        super(ecto);
    }

    add(group, controls) {
        this.controls[group] = controls;
        this.render();
    }

    clear(group) {
        delete this.controls[group];
        this.render();
    }

    handleControlClick = e => {
        var id = e.currentTarget.getAttribute('data-control-id');
        Object.keys(this.controls).forEach(g => {
            Object.keys(this.controls[g]).forEach(c => {
                if (c == id)
                    this.controls[g][c].action();
            });
        });
    };

    render = () => {

        var html = `
            <button class="tb-btn" title="Connect to..."><i class="bi bi-server"></i></button>
            <div class="tb-divider"></div>
            <div class="btn-group">
                <button title="Create a new blank schema" class="tb-btn"><i class="bi bi-table"></i> New Schema</button>
                <button title="Create a new schema from a file" class="tb-btn"><i class="bi bi-cloud-plus-fill"></i> From File...</button>
            </div>
        `;

        var renderControl = (id, c) => {
            var enabled = c.enable ? c.enable() : true;
            if (c.type == 'button' || !c.type) {
                return `<button data-control-id="${id}" class="btn ${enabled ? '' : 'disabled'}">${c.label || 'Button'}</button>`;
            }

            if (c.type == 'divider') {
                return '<div class="tb-divider"></div>';
            }

            if (c.type == 'select') {
                var html = `<select data-control-id="${id}" class="tb-select ${enabled ? '' : 'disabled'}">`;
                if (c.options) {
                    c.options.forEach(opt=>{
                        html += `<option value="${opt.textId}">${opt.name}</option>`;
                    });
                }
                html += `</select>`;
                return html;
            }

        };

        if (Object.keys(this.controls).length > 0) {

            Object.keys(this.controls).forEach(g => {

                html += '<div class="tb-divider"></div>';

                var controlGroup = this.controls[g];
                Object.keys(controlGroup).forEach(c => {
                    var control = this.controls[g][c];
                    html += renderControl(c, control);
                });

            });
        }

        render('.toolbar', html);

        this.bind();

    };


    bind() {
        var el = document.querySelector('.toolbar');
        el.querySelectorAll('button[data-control-id]').forEach(c => {
            c.removeEventListener('click', this.handleControlClick);
            c.addEventListener('click', this.handleControlClick);
        });
    }
}
