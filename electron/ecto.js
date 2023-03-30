import { ectoClient } from "./ectoClient.js";
import { ectoStatusBar } from "./ectoStatusBar.js";
import { ectoTree } from "./ectoTree.js";
import { render } from './js/reef/reef.es.js';
import { ectoToolbar } from "./toolbar.js";
import { ectoComponent } from './components.js';
import { ectoTabManager } from "./ectoTabManager.js";

class ectobi extends ectoComponent {

    // This is the API client
    client = new ectoClient();

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
                <div class="tree"></div>
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

        this.bind();
    }

    bind() {
        this.tree.bind();
    }

    navigate = (e) => {
        e.preventDefault();
        var el = e.currentTarget;
        var href = el.getAttribute('href');
        this.tabManager.openTab(href);
    }

}

new ectobi();