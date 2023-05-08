import { render } from './js/reef/reef.es.js';
import { ectoCoreComponent } from './components.js';

export class bgRoot extends ectoCoreComponent {

    tasks = [];

    constructor(ecto, target) {
        super(ecto, target);

        window.ipc.taskBegun((ev, args) => this.taskBegun(...args));
        window.ipc.taskProgressChanged((ev, args) => this.taskProgressChanged(...args));
        window.ipc.taskCompleted((ev, args) => this.taskCompleted(...args));
        window.ipc.taskStatusChanged((ev, args) => this.taskStatusChanged(...args));
        window.ipc.taskFailed((ev, args) => this.taskFailed(...args));
    }

    getTask(id) {
        var t = this.tasks.filter(x => x.task.id == id);
        if (t.length)
            return t[0];
        return {};
    }

    taskBegun = (task) => {
        this.tasks.push({
            completed: false,
            failed: false,
            progress: { total: 0, count: 0 },
            status: null,
            log: [],
            task: task
        });

        window.ipc.getBackgroundTasks().then(x => {
            this.tasks = x;
        });
        this.render();
    }

    taskCompleted = (task) => {
        window.ipc.getBackgroundTasks().then(x=>{
            this.tasks = x;
            var t = this.getTask(task.id);
            t.status = 'Completed';
            t.completed = true;
            this.render();
        })
    }

    taskFailed = (task) => this.taskCompleted(task);

    taskProgressChanged = (task, total, count) => {
        var t = this.getTask(task.id);
        t.progress.total = total;
        t.progress.count = count;
        this.render();
    }

    taskStatusChanged = (task, status) => {
        var t = this.getTask(task.id);
        t.status = status;
        this.render();
    }

    render() {

        var html = '';

        var qTasks = this.tasks.filter(x => !x.completed);
        if (qTasks.length > 0) {
            qTasks.forEach(task => {
                html += '<div class="bg-task">';
                html += '  <div class="icon"><i class="ri-settings-2-fill"></i></div>';
                html += '  <div class="bg-task-info">';
                html += `    <label>${task.task.name}</label>`;
                if (task.progress && task.progress.total > 0) {
                    var pc = task.progress.count / task.progress.total * 100;
                    html += `    <div class="progress"><div class="progress-bar" style="width: ${pc}%;"></div></div>`;
                }
                if (task.status) {
                    html += `   <div class="bg-task-status">${task.status}</div>`;
                }
                html += '  </div>';
                html += '</div>';
            });
        } else {
            html += `<a href="#" class="btn-flex">
                <i class="ri-settings-2-fill"></i>
                <label>Background Tasks</label>
            </a>`
        }

        if (this.notifications > 0) {
            html += `<div class="badge">${this.notifications}</div>`;
        }
        render(this.target, html);
    }
}
