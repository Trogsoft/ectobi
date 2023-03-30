import { ectoComponent, ectoCoreComponent } from './components.js';
import { render } from './js/reef/reef.es.js';

export class ectoStatusBar extends ectoCoreComponent {

    constructor(ecto, target) {
        super(ecto, target);
    }

    render() {
        render(this.target, '<span>Hello</span>');
    }
}
