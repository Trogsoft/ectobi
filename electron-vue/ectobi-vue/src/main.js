import { createApp } from 'vue'
import { createStore } from 'vuex'
import App from './App.vue'

const toolbar = {
    state: () => ({
        controls: []
    }),
    mutations: {
        addControl(control) {
            state.controls.push(control);
        }
    },
    actions: {
        addControl(context,control) {
            context.commit('addControl', control);
        }
    },
    getters: {

    }
}

const store = createStore({
    modules: {
        toolbar: toolbar
    }
});

const app = createApp(App).mount('#app')
app.use(store);