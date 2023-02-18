<template>
	<button :class="[s.backButton, s[`backButtonTheme${theme}`]]" @click="$emit('click')">
		<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50" :class="s.backButtonSvg">
			<path d="m5.414 24 6.293-6.293-1.414-1.414L1.586 25l8.707 8.707 1.414-1.414L5.414 26H49v-2z"/>
		</svg>
	</button>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

export { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue"; 

export default Vue.extend({
	props: {
		theme: String,
	},	

	data() {
		return {
			s,
		}
	}
})
</script>