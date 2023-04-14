import { table } from '../table.js';
import { ectoTabComponent } from '../components.js';
import { AllCommunityModules } from '../js/ag-grid/ag-grid-community.esm.min.js';
import { Grid } from '../js/ag-grid/ag-grid-community.esm.min.js';
import { render } from '../js/reef/reef.es.js';

export class schemaData extends ectoTabComponent {

    table;
    idCode;
    agGrid;
    tblData;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-data';

        this.table = new table({
            target: '.main',
        });

    }

    init(soft = false) {

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);

        if (!soft) {
            this.client.data.query({ schemaTid: this.data.path[0] }).then(h => {
                this.tblData = h.result;
                this.table.loadValueMap(h.result);
                this.render();
            });
        }

    }

    getTitle = () => "Data";

    getIdCode = () => this.idCode;

    setTarget(target) {
        this.target = target;
        this.table.setTarget(target);
    }

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.ecto.toolbar.clear(this.idCode);
    }

    renderGrid() {
        var el = document.querySelector(this.target).querySelector('#agGrid');
        var columns = this.tblData.headings.map(x=>({ 'field': x }));

        var rows = [];
        this.tblData.rows.forEach((row, rowix)=>{
            var rowObject = { id: `row-${rowix}` };
            row.forEach((col, colix)=>{
                var head = this.tblData.headings[colix];
                rowObject[head] = col;
            })
            rows.push(rowObject);
        });

        var opts = {
            columnDefs: columns,
            rowData: rows
        }
        this.agGrid = new Grid(el, opts, { modules: AllCommunityModules });
    }

    readyToRender() {
        if (super.readyToRender()) {
            if (!this.tblData) return false;
            return true;
        }
        return false;
    }

    render() {
        if (this.readyToRender()) { 
            // var html = `<div id="agGrid" style="width: 100%; height: 100%;" class="ag-theme-alpine"></div>`;
            // render(this.target, html);
            // this.renderGrid();
            this.table.render();
        }
    }

}