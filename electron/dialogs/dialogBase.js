
export class dialogBase {

    title;
    args;
    sender;

    constructor(sender, args) {
        this.sender = sender;
        this.args = args;
    }

    setTitle(title) {
        document.querySelector('.dlg-header').querySelector('h1').innerText = title;
    }

    bind() {
        document.querySelectorAll('.close-dialog').forEach(cb => {
            cb.removeEventListener('click', this.close);
            cb.addEventListener('click', this.close);
        });
    }

    close = (e) => {
        window.ipc.closeMe();
    };

    render() {
    }

}
