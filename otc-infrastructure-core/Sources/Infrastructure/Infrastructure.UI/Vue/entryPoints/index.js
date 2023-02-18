import Vue from 'vue'
import App from './../components/root-components/app/App.vue'
import main from './main'

Vue.config.productionTip = false;

new Vue({
  render: h => h(App),
}).$mount('#app');
