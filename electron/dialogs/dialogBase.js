import { ectoClient } from "../ectoClient.js";

export class dialogBase {

    client;
    title;
    args;
    sender;
    token;

    timers = {};

    constructor(sender, args, token) {
        this.sender = sender;
        this.args = args;
        this.token = token;
        this.client = new ectoClient(token);
    }

    setTitle(title) {
        document.querySelector('.dlg-header').querySelector('h1').innerText = title;
    }

    updateModel = (field, value) => {
        if (Number(value) === parseInt(value))
            value = parseInt(value);

        this.model[field] = value;
        this.render();
    }

    modelFieldInput = (e) => {
        var name = e.currentTarget.getAttribute('name');

        if (e.currentTarget.matches('input[type=text]')) {
            var value = e.currentTarget.value;
            if (this.timers[name])
                clearTimeout(this.timers[name]);

            this.timers[name] = setTimeout(() => this.updateModel(name, value), 500);
        } else if (e.currentTarget.matches('select')) {
            var value = e.currentTarget.value;
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
