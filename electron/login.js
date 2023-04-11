import { ectoClient } from "./ectoClient.js";

class login {

    constructor(){
        document.onreadystatechange = (e) =>{
            if (document.readyState == 'complete'){
                this.bind();
            }
        }
    }

    login() {
        var username = document.querySelector('input[name=username]').value;
        var password = document.querySelector('input[name=password]').value;
        window.ipc.login(username, password);
        
    }

    bind() {
        document.querySelectorAll('.btn-login').forEach(x=>{
            x.removeEventListener('click', this.login);
            x.addEventListener('click', this.login);
        });
    }

}

new login();