export class baseComponent {

    constructor() {        
    }

    emit (type, detail, elem = document) {

        // Create a new event
        let event = new CustomEvent(`ecto:${type}`, {
            bubbles: true,
            cancelable: true,
            detail: detail
        });
    
        // Dispatch the event
        return elem.dispatchEvent(event);
    
    }

}

export class coreComponent extends baseComponent {

    ecto;

    constructor(ecto) {
        super();
        this.ecto = ecto;
    }

    render() {
    }

}

export class uiComponent extends coreComponent {

    data;
    client;

    constructor(ecto, data) {
        super(ecto);
        this.data = data;
        this.client = ecto.client;
    }


    
    init() {
    }

    render() {
    }

    canUnload = () => true;
    
    beforeUnload() {
    }

}


export class tabComponent extends uiComponent {

    constructor(ecto, data) {
        super(ecto, data)
    }

    allowMultiple() {
        return true;
    }

    getTitle() {
        return "Tab";
    }

    getIdCode() {
        return Math.random().toString();
    }

}