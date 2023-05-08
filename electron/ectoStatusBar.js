import { ectoComponent, ectoCoreComponent } from './components.js';
import { render } from './js/reef/reef.es.js';

export class ectoStatusBar extends ectoCoreComponent {

    serverInfo;
    fields = {
        sys: {}
    };

    constructor(ecto, target) {
        super(ecto, target);
        document.addEventListener('ecto:serverInfo', this.serverInfoReceived);
    }

    serverInfoReceived = (si) => {
        this.serverInfo = si.detail;
        this.render();
    }

    addSystemReadout(id, title, value) {
        this.addReadout('sys', id, title, value);
    }

    addReadout(section, id, title, value = null) {
        if (!this.fields[section])
            this.fields[section] = {};

        this.fields[section][id] = { title: title, value: value };
        this.render();
    }

    clear(section) {
        if (this.fields[section])
            delete this.fields[section];

        this.render();
    }

    render() {

        var connectedString = 'Waiting...';
        if (this.serverInfo) {
            connectedString = `Connected to ${this.serverInfo.name}`;
            if (this.serverInfo.userCapabilities && this.serverInfo.userCapabilities.displayNam) connectedString += ` as ${this.serverInfo.userCapabilities.displayName}`;
            if (this.serverInfo.userCapabilities && this.serverInfo.userCapabilities.username) connectedString += ` as ${this.serverInfo.userCapabilities.username}`;
        }

        var panels = () => {
            var html = '';
            Object.keys(this.fields).forEach(key => {
                html += `<div class="sb-panel" id="sb-section-${key}">`;
                Object.keys(this.fields[key]).forEach(id => {
                    var item = this.fields[key][id];
                    if (item.value) {
                        html += `<div class="sb-panel-item"><label>${item.title}</label> <span class="sb-value">${item.value}</span></div>`;
                    } else {
                        html += `<div class="sb-panel-item"><span class="sb-value">${item.title}</span></div>`;
                    }
                });
                html += `</div>`;
            });
            return html;
        }

        var html = `
            <div class="sb-panel">
                <div class="sb-icon"><i class=""></i></div>
                <div class="label">${connectedString}</div>
            </div>
            ${panels()}
        `;
        render(this.target, html);
    }
}
