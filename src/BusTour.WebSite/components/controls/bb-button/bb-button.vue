<template>
	<button 
		:class="className" 
		@click="$emit('click')"
	>
		{{ text }}
		<slot></slot>
	</button>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export enum ButtonTheme {
	Black = "Black",
	White = "White" 
};

export default Vue.extend({
	name: "bbButton",

	props: {
		text: String,
		theme: {
			type: String,
			default: ButtonTheme.White
		},
		disabled: Boolean,
		hoverEffect: {
			type: Boolean,
			default: true
		}
	},

	computed: {
		className(): any {
			return [
				this.s.root, 
				this.s[`rootTheme${this.theme}`], 
				{
					[this.s.disabled]: this.disabled,
					[this.s.rootHover]: this.hoverEffect
				}
			]
		},
	},

	data() {
		return {
			s: style,
		};
	},
});
</script>