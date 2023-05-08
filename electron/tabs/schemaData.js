import { table } from '../table.js';
import { ectoComponent, ectoTabComponent } from '../components.js';
import { AllCommunityModules } from '../js/ag-grid/ag-grid-community.esm.min.js';
import { Grid } from '../js/ag-grid/ag-grid-community.esm.min.js';
import { render } from '../js/reef/reef.es.js';
import { filterType } from '../enums.js';
import { createPopper } from "../js/popper/index.js";

class dataQuery {

    values = {
        page: 1,
        recordsPerPage: 250
    };
    filters;
    needsRefresh = false;

    constructor(filters, values) {
        this.filters = filters;
        this.values = values;
    }

    optionIsSelected(fieldId, optId) {
        var values = this.getQueryValues(fieldId);
        return values.includes(optId.toString());
    }

    getQueryValues(fieldId) {
        if (!this.values.filter[fieldId]) return [];
        return this.values.filter[fieldId];
    }

    getFilter(fieldId) {
        var selection = this.filters.filter(x => x.textId == fieldId);
        if (selection.length)
            return selection[0];
        return null;
    }

    getFilterValues(fieldId) {
        var filter = this.getFilter(fieldId);
        if (!filter) return null;
        return filter.options;
    }

    getFilterOptionLabel(fieldId, value) {
        var values = this.getFilterValues(fieldId);
        var items = values.filter(x => x.id == value);
        if (items.length > 0) return items[0].name;
        return value;
    }

    getUIFilterLabel(fieldId) {
        var label = 'None';
        var availableOptions = this.getFilterValues(fieldId);
        var selectedOptions = this.getQueryValues(fieldId);
        if (selectedOptions && availableOptions) {
            if (selectedOptions.length == availableOptions.length) label = availableOptions.length + ' Items';
            if (selectedOptions.length == 1) label = this.getFilterOptionLabel(fieldId, selectedOptions[0]);
        }
        return label;
    }

    toggleChecked(field, opt) {
        this.values.page = 1;
        if (this.optionIsSelected(field, opt)) {
            var idx = this.values.filter[field].indexOf(opt);
            this.values.filter[field].splice(idx, 1);
        } else {
            this.values.filter[field].push(opt);
        }
        this.needsRefresh = true;
    }

    clearValues(fieldId) {
        this.values.page = 1;
        this.values.filter[fieldId] = [];
        this.needsRefresh = true;
    }

    selectAllValues(fieldId) {
        this.values.page = 1;
        var fv = this.getFilterValues(fieldId);
        this.values.filter[fieldId] = [...fv.map(x=>x.id)];
        this.needsRefresh = true;
    }

}

export class schemaData extends ectoTabComponent {

    table;
    idCode;
    agGrid;
    tblData;
    filters;
    query;

    popper;
    currentFilterPopup;
    container;

    constructor(ecto, target, data) {
        super(ecto, target, data);
        this.idCode = data.path[0] + '-data-' + this.#makeid(8)

        this.table = new table({
            target: '#' + this.IdCode + '-data-table',
        });

        document.addEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        window.addEventListener('click', this.closePopup);
    }

    closePopup = e => {
        if (!this.container) return;

        const popup = e.target.closest('.filter-popup');
        if (popup) return;

        var fp = this.container.querySelector('.filter-popup');
        if (!fp) return;

        fp.classList.remove('show');
        this.container.querySelectorAll('.filter-dropdown').forEach(x => {
            x.classList.remove('open');
        });

        if (this.query.needsRefresh) {
            this.loadData();
        }

    }

    handleInfiniteScroll = e => {
        var scrollY = this.container.scrollHeight - this.container.scrollTop;
        var height = this.container.offsetHeight;
        var offset = height - scrollY;

        if (offset == 0 || offset == 1) {
            this.query.values.page++;
            this.appendPage();
        }
    }

    appendPage() {
        this.client.data.query(this.query.values).then(h => {
            //this.tblData = h.result;
            this.table.appendValueMap(h.result);
            this.ecto.statusBar.addReadout(this.idCode, 'time', 'Time taken', h.result.timeTaken);
            var endValue = this.query.values.recordsPerPage * this.query.values.page;
            if (endValue > h.result.totalRowsForQuery) endValue = h.result.totalRowsForQuery;
            this.ecto.statusBar.addReadout(this.idCode, 'rows', `0-${endValue} of ${h.result.totalRowsForQuery} rows`);
            this.render();
        });
    }

    // From: https://stackoverflow.com/questions/1349404/generate-random-string-characters-in-javascript
    #makeid(length) {
        let result = '';
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const charactersLength = characters.length;
        let counter = 0;
        while (counter < length) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
            counter += 1;
        }
        return result;
    }

    init(soft = false) {

        if (!soft) {

            this.client.data.getFieldFilters(this.data.path[0]).then(h => {
                this.filters = h.result.fieldFilters;
                this.query = new dataQuery(h.result.fieldFilters, h.result.query);

                this.container = document.querySelector('.ct-' + this.idCode);
                this.container.addEventListener('scroll', this.handleInfiniteScroll);

                this.loadData();
            });

        }

    }

    getTitle = () => "Data";

    getIdCode = () => this.idCode;

    loadData() {
        this.client.data.query(this.query.values).then(h => {
            this.tblData = h.result;
            this.table.loadValueMap(h.result);
            this.ecto.statusBar.addReadout(this.idCode, 'time', 'Time taken', h.result.timeTaken);
            var endValue = this.query.values.recordsPerPage * this.query.values.page;
            if (endValue > h.result.totalRowsForQuery)
                endValue = h.result.totalRowsForQuery;
            this.ecto.statusBar.addReadout(this.idCode, 'rows', `0-${endValue} of ${h.result.totalRowsForQuery} rows`);
            this.render();
        });
    }

    setTarget(target) {
        this.target = target;
        this.table.setTarget('#' + this.idCode + '-data-table');
    }

    beforeUnload() {
        document.removeEventListener('ecto:tableStateChanged', this.ecto.toolbar.render);
        this.container.removeEventListener('scroll', this.handleInfiniteScroll);
        window.removeEventListener('click', this.closePopup);
        this.ecto.toolbar.clear(this.idCode);
        this.ecto.statusBar.clear(this.idCode);
    }

    renderGrid() {
        var el = document.querySelector(this.target).querySelector('#agGrid');
        var columns = this.tblData.headings.map(x => ({ 'field': x }));

        var rows = [];
        this.tblData.rows.forEach((row, rowix) => {
            var rowObject = { id: `row-${rowix}` };
            row.forEach((col, colix) => {
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

    renderPopup() {
        var fieldId = this.currentFilterPopup;
        var options = this.query.getFilterValues(fieldId);
        var selected = this.query.getQueryValues(fieldId);

        var filter = this.query.getFilter(fieldId);

        var checked = (opt) => {
            if (this.query.optionIsSelected(fieldId, opt.id))
                return '<i class="ri-check-line"></i>';
            return '';
        }

        var renderItems = () => {
            var html = '';
            options.forEach(opt => {
                html += `<a href="#" class="filter-group-item" data-field="${fieldId}" data-option="${opt.id}">
                    <div class="icon">${checked(opt)}</div>
                    <div class="label">${opt.name}</div>
                </a>`;
            });
            return html;
        }

        var html = `<div class="filter-group">
            <div class="filter-group-header">
                <div class="filter-group-header-title">
                    Options
                </div>
                <div class="options">
                    <button class="btn btn-sm select-all">All</button>
                    <button class="btn btn-sm select-none">None</button>
                </div>
            </div>
            <div class="filter-group-items">
                ${renderItems()}
            </div>
        </div>`;

        render(`.filter-popup-${this.idCode}`, html);
        this.bindPopup();

    }

    bindPopup() {

        var m = this.container.querySelector(`.filter-popup-${this.idCode}`);
        if (!m) return;

        m.querySelectorAll('.filter-group-item').forEach(item => {
            item.removeEventListener('click', this.menuItemSelected);
            item.addEventListener('click', this.menuItemSelected);
        });

        m.querySelectorAll('.select-all').forEach(item => {
            item.removeEventListener('click', this.selectFilterOptions);
            item.addEventListener('click', this.selectFilterOptions);
        })

        m.querySelectorAll('.select-none').forEach(item => {
            item.removeEventListener('click', this.selectFilterOptions);
            item.addEventListener('click', this.selectFilterOptions);
        })

    }

    selectFilterOptions = e => {

        if (!e) return;

        var mode = 'all';
        if (e.currentTarget.classList.contains('select-none')) mode = 'none';

        var fieldId = this.currentFilterPopup;
        if (mode == 'none')
            this.query.clearValues(fieldId);
        else if (mode =='all')
            this.query.selectAllValues(fieldId);


    }

    menuItemSelected = e => {

        var field = e.currentTarget.getAttribute('data-field');
        var opt = e.currentTarget.getAttribute('data-option');

        this.query.toggleChecked(field, opt);
        this.renderPopup();

    }

    showFilterOptions = e => {

        e.preventDefault();
        e.stopPropagation();

        var parent = document.querySelector(this.target);
        var fieldId = e.currentTarget.getAttribute('data-id');
        this.currentFilterPopup = fieldId;

        e.currentTarget.parentElement.querySelectorAll('.filter-dropdown').forEach(x => {
            x.classList.remove('open');
        });

        var filterMenu = parent.querySelector('.filter-popup');
        filterMenu.classList.add('show');
        filterMenu.style['min-width'] = e.currentTarget.offsetWidth + 'px';
        e.currentTarget.classList.add('open');
        this.popper = createPopper(e.currentTarget, filterMenu, {
            placement: 'bottom-start'
        });

        this.renderPopup();

    }

    bind() {
        var parent = document.querySelector(this.target);
        parent.querySelectorAll('.filter-dropdown').forEach(x => {
            x.removeEventListener('click', this.showFilterOptions);
            x.addEventListener('click', this.showFilterOptions);
        });
    }

    renderFilters() {
        var html = '';
        this.filters.forEach(f => {

            var valueLabel = this.query.getUIFilterLabel(f.textId);

            if (f.type == filterType.set) {
                html += `<div class="filter filter-dropdown" data-id="${f.textId}">
                    <div class="label">${f.name}</div>
                    <div class="value">${valueLabel}</div>
                    <div class="ctrl-dropdown"><i class="ri-arrow-down-s-line"></i></div>
                </div>`;
            }

        });
        render(`#${this.idCode}-query`, html);
    }

    render() {
        if (this.readyToRender()) {
            var html = `<div class="data-query"><div class="query" id="${this.idCode}-query"></div><div class="data-tbl" id="${this.idCode}-data-table"></div></div>`;
            html += `<div class="filter-popup popup filter-popup-${this.idCode}"></div>`;
            // var html = `<div id="agGrid" style="width: 100%; height: 100%;" class="ag-theme-alpine"></div>`;
            // render(this.target, html);
            // this.renderGrid();
            render(this.target, html);
            this.renderFilters();
            this.table.render();
            this.bind();
        }
    }

}