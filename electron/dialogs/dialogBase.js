import { ectoClient } from "../ectoClient.js";

export class dialogBase {

    client;
    title;
    args;
    sender;
    token;

    timers = {};

    currentTab = null;
    tabs = {        
    }

    constructor(sender, args, token) {
        this.sender = sender;
        this.args = args;
        this.token = token;
        this.client = new ectoClient(token);
    }

    addTab(tabId, tabName, icon, renderer) {
        if (Object.keys(this.tabs).length == 0) this.currentTab = tabId;
        this.tabs[tabId] = { name: tabName, icon: icon, render: renderer }
        if (this.args.debug) console.log(`Added tab ${tabId}; name=${tabName}`);
    }

    getTabContent() {
        var tab = this.tabs[this.currentTab];
        if (tab) {
            var html = `<div id="dlgc-${this.currentTab}">`;
            html += tab.render();
            html += '</div>';
            if (this.args.debug) console.log('tab content: '+html)
            return html;
        }
    }

    removeTab(tabId) {

    }

    renderTabUi() {
        var html = `<div class="dialog-tabs" id="dlg-tags"><div class="dialog-tabs-row">`;
        Object.keys(this.tabs).forEach(tab=>{
            html += `<a href="#${tab}" class="dialog-tab ${this.currentTab == tab ? 'active' : ''}"><i class="icon ri-${this.tabs[tab].icon}"></i><span class="label">${this.tabs[tab].name}</span></a>`
        });
        html += `
        </div></div>
        <div class="dialog-tab-content" id="dlg-tab-content">
            ${this.getTabContent()}
        </div>`;
        return html;
    }

    switchTab = (e) => {
        e.preventDefault();
        var tabId = e.currentTarget.getAttribute('href').replace('#', '');
        this.currentTab = tabId;
        this.render();
    }

    setTitle(title) {
        document.title = title;
        var dlgHead = document.querySelector('.dlg-header');
        if (dlgHead)
            dlgHead.querySelector('h1').innerText = title;
    }

    updateModel = (field, value) => {
        if (Number(value) === parseInt(value))
            value = parseInt(value);

        this.model[field] = value;
        this.render();
        if (this.args.debug) console.log(`model.${field} updated to '${value}'`);
    }

    modelFieldInput = (e) => {
        var name = e.currentTarget.getAttribute('name');

        if (e.currentTarget.matches('input[type=text]')) {
            var value = e.currentTarget.value;
            if (this.timers[name])
                clearTimeout(this.timers[name]);

            this.timers[name] = setTimeout(() => this.updateModel(name, value), 500);
        } else if (e.currentTarget.matches('select')) {
            var value = e.currentTarget.selectedOptions[0].value;
            this.updateModel(name, value);
        }
    }

    bind() {
        document.querySelectorAll('.close-dialog').forEach(cb => {
            cb.removeEventListener('click', this.close);
            cb.addEventListener('click', this.close);
        });

        document.querySelectorAll('.model-field').forEach(cb => {
            cb.removeEventListener('input', this.modelFieldInput);
            cb.addEventListener('input', this.modelFieldInput);
        })

        document.querySelectorAll('a.dialog-tab').forEach(t=>{
            t.removeEventListener('click', this.switchTab);
            t.addEventListener('click', this.switchTab);
        });
    }

    tokenUpdate = (token) =>{
        this.client.updateToken(token);
    }

    close = (e) => {
        window.ipc.closeMe();
    };

    render() {
    }

}
