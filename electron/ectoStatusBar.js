import { ectoComponent, ectoCoreComponent } from './components.js';
import { render } from './js/reef/reef.es.js';

export class ectoStatusBar extends ectoCoreComponent {

    serverInfo;

    constructor(ecto, target) {
        super(ecto, target);
        document.addEventListener('ecto:serverInfo', this.serverInfoReceived);
    }

    serverInfoReceived = (si) =>{
        this.serverInfo = si.detail;
        this.render();
    }

    render() {

        var connectedString = 'Waiting...';
        if (this.serverInfo) {
            connectedString = `Connected to ${this.serverInfo.name}`;
            if (this.serverInfo.userCapabilities && this.serverInfo.userCapabilities.displayNam) connectedString += ` as ${this.serverInfo.userCapabilities.displayName}`;
            if (this.serverInfo.userCapabilities && this.serverInfo.userCapabilities.username) connectedString += ` as ${this.serverInfo.userCapabilities.username}`;
        }

        var html = `
            <div class="sb-panel">
                <div class="sb-icon"><i class=""></i></div>
                <div class="label">${connectedString}</div>
            </div>
        `;
        render(this.target, html);
    }
}
