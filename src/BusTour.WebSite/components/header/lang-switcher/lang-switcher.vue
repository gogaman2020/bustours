<template>
	<div :class="s.langSwitcher" @click="toggleLangMenu()">
		<div :class="s.lang" >
			<div>{{ selectedLocale }}</div>
			<div :class="s.arrow"></div>
		</div>
		<div
			v-show="isLangMenuVisible"
			:class="s.langMenu"
			@mouseleave="changeLangMenuVisibility(false)"
		>
			<nuxt-link
				:class="s.langItem"
				v-for="locale in locales"
				:key="locale.code"
				:to="switchLocalePath(locale.code)"
				@click="changeLangMenuVisibility(false), $emit('lang-selected')"
			>
				{{ locale.code }}
			</nuxt-link>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export default Vue.extend({
	name: "lang-switcher",
	data() {
		return {
			s: style,
			isLangMenuVisible: false,
		};
	},
	computed: {
		selectedLocale() {
			return this.$i18n.locale;
		},
		locales() {
			return this.$i18n.locales;
		},
	},
	methods: {
		toggleLangMenu(): void {
			this.isLangMenuVisible = !this.isLangMenuVisible;
		},
		changeLangMenuVisibility(value: boolean): void {
			this.isLangMenuVisible = value;
		},
	},
});
</script>