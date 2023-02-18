<template>
	<div :class="s.layout" :style="inlineStyles">
		<slot></slot>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";

enum Padding {
	pt = "padding-top",
	pr = "padding-right",
	pb = "padding-bottom",
	pl = "padding-left"
}

export default Vue.extend({
	name: "BusLayout",

	props: {
		pt: {
			type: Number,
			default: 0,
		},
		pr: {
			type: Number,
			default: 0,
		},
		pb: {
			type: Number,
			default: 0,
		},
		pl: {
			type: Number,
			default: 0,
		},
	},

	computed: {
		inlineStyles() {
			Object
				.entries(this.$props)
				.filter(([_, value]) => value != 0)
				.reduce((acc: string, [prop, value]) => `${acc}; ${(Padding as any)[prop]}: ${this.px(value)}`, "");
		},
		lang() {
			return this.$i18n.locale;
		},		
	},

	data() {
		return {
			s
		}
	},

	methods: {
		px: (value: Number | undefined): string => `${value}px`,
	},

	mounted() {
		const bodyElement = document.querySelector('body')
		if (bodyElement) {
			bodyElement.classList.remove(...(bodyElement.classList as any));
			bodyElement.classList.add(this.lang);
		}
	}
})
</script>