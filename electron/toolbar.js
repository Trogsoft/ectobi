import { render } from './js/reef/reef.es.js';
import { ectoComponent, ectoCoreComponent } from "./components.js";

export class ectoToolbar extends ectoCoreComponent {

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
        var controlId = e.currentTarget.getAttribute('data-control-id');
        Object.keys(this.controls).forEach(group => {
            Object.keys(this.controls[group]).forEach(control => {
                if (control == controlId) {
                    if (this.controls[group][control].action) {
                        this.controls[group][control].action();
                    } else {
                        throw new Error('No action is configured for this control.');
                    }
                    return;
                }
            });
        });
    };

    render = () => {

        var html = `
            <button class="tb-btn" title="Connect to..."><i class="ri-server-fill ri-lg"></i></button>
            <div class="tb-divider"></div>
            <button title="Create a new blank schema" class="tb-btn new-schema"><i class="ri-database-2-fill ri-lg"></i> New Schema...</button>
        `;

        var renderControl = (id, c) => {
            var enabled = c.enable ? c.enable() : true;
            if (c.type == 'button' || !c.type) {
                return `<button data-control-id="${id}" class="btn" ${enabled ? '' : 'disabled'}>${c.label || 'Button'}</button>`;
            }

            if (c.type == 'divider') {
                return '<div class="tb-divider"></div>';
            }

            if (c.type == 'select') {
                var html = `<select data-control-id="${id}" class="tb-select" ${enabled ? '' : 'disabled'}>`;
                if (c.options) {
                    c.options.forEach(opt => {
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

        el.querySelectorAll('.new-schema').forEach(c=>{
            c.removeEventListener('click', this.ecto.createNewSchema);
            c.addEventListener('click', this.ecto.createNewSchema);
        })
    }
}
