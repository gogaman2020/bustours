import Vue from "vue";
import BusLayout from "~/components/layout/layout.vue";

const components = { BusLayout };

Object
	.entries(components)
	.forEach(([name, component]) => Vue.component(name, component));