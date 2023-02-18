<template>
	<div :class="s.hint">
		<BbButton :theme="ButtonTheme.Black" :class="s.hintButton">
			<nuxt-link :to="localePath('/booking')">
				{{ $t("menu.booking") }} 
			</nuxt-link>
		</BbButton>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";

import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue";

export default Vue.extend({
	components: {
		BbButton,
	},

	data() {
		return {
			s,
			ButtonTheme
		}
	}
})
</script>