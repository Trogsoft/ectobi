export class ectoComponent {

    constructor() {        
    }

    emit (type, detail, elem = document) {

        let event = new CustomEvent(`ecto:${type}`, {
            bubbles: true,
            cancelable: true,
            detail: detail
        });
    
        return elem.dispatchEvent(event);
    
    }

}

export class ectoCoreComponent extends ectoComponent {

    ecto;
    client;
    target;

    constructor(ecto, target) {
        super();
        this.ecto = ecto;
        this.target = target;
        this.client = ecto.client;
    }

    render() {        
    }

    setTarget(newTarget) {
        this.target = newTarget;
    }
    
}

export class ectoTabComponent extends ectoCoreComponent {

    data;

    constructor(ecto, target, data) {
        super(ecto,target);
        this.data = data;
    }

    render() {        
    }

    canUnload = () => true;

    readyToRender() {
        if (!this.target) return;
        if (!document.querySelector(this.target)) return false;
        return true;
    }

    beforeUnload() {        
    }

    init(soft = false) {        
    }

    getTitle = () => "Component";

    getIdCode = () => Math.random().toString();

}