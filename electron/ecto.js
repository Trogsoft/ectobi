import { ectoClient } from "./ectoClient.js";
import { ectoStatusBar } from "./ectoStatusBar.js";
import { ectoTree } from "./ectoTree.js";
import { render } from './js/reef/reef.es.js';
import { ectoToolbar } from "./toolbar.js";
import { ectoComponent, ectoCoreComponent } from './components.js';
import { ectoTabManager } from "./ectoTabManager.js";

class bgRoot extends ectoCoreComponent {

    notifications = 0;

    constructor(ecto, target) {
        super(ecto,target);
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

class ectobi extends ectoComponent {

    // This is the API client
    client = ectoClient;

    // These are the main UI components
    tree;
    statusBar;
    toolbar;
    tabManager;

    constructor() {
        super();
        this.tree = new ectoTree(this, '.tree');
        this.toolbar = new ectoToolbar(this, '.toolbar');
        this.statusBar = new ectoStatusBar(this, '.status');
        this.tabManager = new ectoTabManager(this, '.tabs');
        this.bgStatus = new bgRoot(this, '#backgroundTaskStatus');

        document.onreadystatechange = e => {
            if (document.readyState === "complete") {
                this.render();
            }
        }
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

}

const ectobiInstance = new ectobi();