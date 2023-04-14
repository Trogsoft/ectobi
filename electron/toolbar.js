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

    findControl = (controlId, controlCollection) => {
        var result = null;
        if (!controlCollection) {
            Object.keys(this.controls).map(tlg => {
                var r = this.findControl(controlId, this.controls[tlg]);
                if (r) result = r;
            });
        } else {
            Object.keys(controlCollection).map(key => {
                var control = controlCollection[key];
                if (key == controlId) result = control;
                if (control.type && control.type == 'group') {
                    var r = this.findControl(controlId, control.controls);
                    if (r) result = r;
                }
            });
        }
        return result;
    }

    handleControlClick = e => {
        var controlId = e.currentTarget.getAttribute('data-control-id');
        var control = this.findControl(controlId);
        if (control != null) {
            control.action();
        }
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
                var tooltip = '';
                if (c.tooltip) tooltip = `title="${c.tooltip}"`;
                if (c.icon) {
                    return `<button data-control-id="${id}" ${tooltip} class="btn" ${enabled ? '' : 'disabled'}><i class="${c.icon} ri-lg"></i> ${c.label || ''}</button>`;
                }
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
                    if (control.type == 'group') {
                        html += '<div class="btn-group">';
                        Object.keys(control.controls).forEach(subc => {
                            var subcontrol = control.controls[subc];
                            html += renderControl(subc, subcontrol);
                        });
                        html += '</div>';
                    } else {
                        html += renderControl(c, control);
                    }
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

        el.querySelectorAll('.new-schema').forEach(c => {
            c.removeEventListener('click', this.ecto.createNewSchema);
            c.addEventListener('click', this.ecto.createNewSchema);
        })
    }
}
