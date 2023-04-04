import { ectoClient } from "../ectoClient.js";
import { render } from "../js/reef/reef.es.js";
import { table } from "../table.js";
import { dialogBase } from "./dialogBase.js";

export class lookupValuesDialog extends dialogBase {

    values;
    client = new ectoClient();
    table;

    constructor(sender, arg) {
        super(sender, arg);
        this.setTitle('Lookup Values');

        this.table = new table({
            target: '.form-field-table',
            headers: {
                name: {
                    label: 'Name'
                },
                numericValue: {
                    label: 'Value'
                }
            }
        });

        this.client.lookup.get(arg.id).then(x => {
            this.values = x.result.values;
            this.setTitle(x.result.name + ' Values');
            this.table.setData(this.values);
            this.render();
        });

    }

    bind() {
        super.bind();
        var root = document.querySelector('#content');
    }

    render() {
        var html = `
            <div class="form-field">
                <div class="form-field-table">
                </div>
            </div>
        `;
        render('#content', html);
        this.table.render();
        this.bind();
    }
}
