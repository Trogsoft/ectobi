import { render } from './js/reef/reef.es.js';
import { ectoCoreComponent } from './components.js';
import { schemaFieldEditor, schemaFields } from "./tabs/schemaFields.js";
import { schemaVersions } from "./tabs/schemaVersions.js";
import { schemaRecordEditor, schemaUploads } from './tabs/schemaUploads.js';
import { lookupManager } from './tabs/lookupManager.js';
import { schemaDetail } from './schemaDetail.js';
import { webHookManager } from './tabs/webHookManager.js';
import { schemaData } from './tabs/schemaData.js';

export class ectoTabManager extends ectoCoreComponent {

    componentMap = {
        'schemaFields': schemaFields,
        'schemaVersions': schemaVersions,
        'schemaUploads': schemaUploads,
        'schemaRecordEditor': schemaRecordEditor,
        'schemaFieldEditor': schemaFieldEditor,
        'lookupManager': lookupManager,
        'schemaDetail': schemaDetail,
        'webHookManager': webHookManager,
        'schemaData': schemaData
    };

    currentIndex = -1;
    tabs = [];

    constructor(ecto, target) {
        super(ecto, target);
        document.addEventListener('ecto:tabTitleChanged', x => this.render());
    }

    render() {
        var html = '<div class="tab-row">';
        this.tabs.forEach((tab, i) => {
            html += `<div href="#" class="tab ${this.currentIndex == i ? 'active' : ''}" data-tab-id="${i}">
                <span class="tab-title">${tab.component.getTitle()}</span>
                <a href="#" class="close-tab" data-tab-id="${i}"><i class="ri-close-circle-fill"></i></a>
            </div>`;
        });
        html += '</div>';

        var currentTab = this.tabs[this.currentIndex];
        if (currentTab) {
            html += `<div class="tab-content ct-${currentTab.component.getIdCode()}"></div>`;
        }

        render('.tabs', html);
        if (currentTab) {
            currentTab.component.render();
        }

        this.bind();
    }

    changeTab = e => {

        if (this.currentIndex > -1) {
            var tab = this.tabs[this.currentIndex];
            this.ecto.toolbar.clear(tab.component.getIdCode());
        }

        var el = e.currentTarget;
        var tabId = parseInt(el.getAttribute('data-tab-id'));
        this.switchToTab(tabId);

    };

    switchToTab(tabId) {

        var tab = this.tabs[tabId];
        tab.component.init(true);
        this.currentIndex = tabId;
        this.render();

    }

    openNewTab(path) {
        this.openTab(path);
    }

    closeTab = e => {
        e.stopPropagation();
        var id = parseInt(e.currentTarget.getAttribute('data-tab-id'));
        var tab = this.tabs[id];
        if ((tab.component.canUnload && tab.component.canUnload()) || !tab.component.canUnload) {
            if (tab.component.beforeUnload)
                tab.component.beforeUnload();

            this.ecto.toolbar.clear(tab.component.getIdCode());

            this.tabs.splice(id, 1);
            if (this.tabs.length > 0) {
                if (id - 1 < 0)
                    this.switchToTab(0);
                else
                    this.switchToTab(id - 1);

            } else {
                this.currentIndex = -1;
                this.render();
            }
            tab = null;
        }
    }

    bind() {

        document.querySelector('.tabs').querySelectorAll('.tab').forEach(t => {
            t.removeEventListener('click', this.changeTab);
            t.addEventListener('click', this.changeTab);
        });

        document.querySelector('.tabs').querySelectorAll('.close-tab').forEach(ct => {
            ct.removeEventListener('click', this.closeTab);
            ct.addEventListener('click', this.closeTab);
        });

    }

    openTab = (href) => {

        var parts = href.split('/');
        var componentName = parts[0];
        var path = parts.splice(1, parts.length - 1);
        var data = {
            path: path
        };
        var type = this.componentMap[componentName];
        if (type) {

            var newComponent = new type(this.ecto, null, data);
            var id = newComponent.getIdCode();
            const existingIndex = this.tabs.findIndex(x => x.id == id);

            if (this.currentIndex > -1) {
                var tab = this.tabs[this.currentIndex];
                this.ecto.toolbar.clear(tab.component.getIdCode());
            }

            if (existingIndex > -1) {
                this.currentIndex = existingIndex;
                var tab = this.tabs[this.currentIndex];
                tab.component.init(true);
                newComponent = null;
            } else {
                newComponent.setTarget(`.ct-${id}`);
                newComponent.init(false);
                this.tabs.unshift({ id: id, component: newComponent });
                this.currentIndex = 0;
            }

            this.render();
        }
    };
}
