import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js";
import { dialogBase } from "./dialogBase.js";

export class templateDialog extends dialogBase {

    client = new ectoClient();

    model = {
        id: 0,
        name: null,
        description: null
    };

    constructor(sender, arg) {
        super(sender, arg);
        if (arg.id > 0) {
            // this.client.webhook.get(arg.id).then(x => {
            //     this.model = x.result;
            //     this.setTitle(x.result.name);
            //     this.render();
            // });
        } else {
            // this.setTitle('New WebHook');
            // this.render();
        }
    }

    bind() {
        super.bind();
    }

    readyToGo() {
        return true;
    }

    render() {
        var html = `
            <div class="form-field">
                <label>WebHook Name</label>
                <input type="text" class="form-control" name="name" @value="${this.model.name || ''}" />
            </div>
            <div class="form-field">
                <label>Description</label>
                <textarea class="form-control" name="description">${this.model.description || ''}</textarea>
            </div>

            <div class="dlg-button-row">
                <button class="btn btn-danger close-dialog">Cancel</button>
                <button class="btn btn-success create-schema ${this.readyToGo() ? '' : 'disabled'}">${this.model.id == 0 ? 'Create' : 'Save'}</button>
            </div>

        `;
        render('#content', html);
        this.bind();
    }
}
