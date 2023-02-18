import Vue from 'vue'

export default () => {
	if(!window.eventBus) {
		window.eventBus = new Vue();
	}
}