import { render } from './js/reef/reef.es.js';
import { ectoCoreComponent } from './components.js';

export class bgRoot extends ectoCoreComponent {

    notifications = 0;

    constructor(ecto, target) {
        super(ecto, target);
    }

    render() {
        var html = `
            <div class="icon"><i class="ri-settings-5-fill"></i></div>
            <div class="label">Background Tasks</div>
        `;

        if (this.notifications > 0) {
            html += `<div class="badge">${this.notifications}</div>`;
        }
        render(this.target, html);
    }
}
