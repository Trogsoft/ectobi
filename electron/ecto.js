import { ectoClient } from "./ectoClient.js";
import { ectoStatusBar } from "./ectoStatusBar.js";
import { ectoTree } from "./ectoTree.js";
import { render } from './js/reef/reef.es.js';
import { ectoToolbar } from "./toolbar.js";
import { ectoComponent } from './components.js';
import { ectoTabManager } from "./ectoTabManager.js";
import { bgRoot } from "./bgRoot.js";

class ectobi extends ectoComponent {

    // This is the API client
    client;

    // These are the main UI components
    tree;
    statusBar;
    toolbar;
    tabManager;
    token;

    serverInfo;

    constructor(token) {
        super();
        this.token = token;

        this.client = new ectoClient(token);
        this.tree = new ectoTree(this, '.tree');
        this.toolbar = new ectoToolbar(this, '.toolbar');
        this.statusBar = new ectoStatusBar(this, '.status');
        this.tabManager = new ectoTabManager(this, '.tabs');
        this.bgStatus = new bgRoot(this, '#backgroundTaskStatus');

        this.client.getServerInfo().then(si=>{
            this.emit('serverInfo', si.result);
            this.serverInfo = si.result
        });

        this.render();
    }

    render = () => {
        var html = `
            <div class="toolbar"></div>
            <div class="main-panel">
                <div class="tree-container">
                    <div class="tree"></div>
                    <div class="roots">
                        <a href="#" class="root" id="backgroundTaskStatus">
                        </a>
                    </div>
                </div>
                <div class="tabs">
                </div>
            </div>
            <div class="status"></div>
        `;

        render('.app', html);

        this.tree.render();
        this.toolbar.render();
        this.statusBar.render();
        this.tabManager.render();
        this.bgStatus.render();

        this.bind();
    }

    bind() {
        this.tree.bind();
    }

    createNewSchema = (e) => {
        window.ipc.openDialog({
            dialogType: 'newSchemaDialog',
            width: 600,
            height: 450
        }, result => {
            console.log(result);
        });
    }

    navigate = (e) => {
        e.preventDefault();
        var el = e.currentTarget;
        var href = el.getAttribute('href');
        this.tabManager.openTab(href);
    }

    tokenUpdate = (token) => {
        this.token = token;
        this.client.updateToken(this.token);
        console.log('Token updated: '+this.token);
        this.emit('tokenRefresh', token);
    }

}

let ectobiInstance;
document.onreadystatechange = (e) => {
    if (document.readyState == 'complete') {
        if (window.ipc && window.ipc.getToken) {
            window.ipc.getToken().then(token=>{
                ectobiInstance = new ectobi(token);
            });
        }   
    }
}

window.addEventListener('DOMContentLoaded', x => {
    window.ipc.tokenRefresh((event) => {
        window.ipc.getToken().then(token=>{
            ectobiInstance.tokenUpdate(token);
        })
    });
});      