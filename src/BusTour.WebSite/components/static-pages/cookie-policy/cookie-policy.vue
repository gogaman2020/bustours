<template>
	<div :class="s.page">
		<h1 data-center>{{ $t("staticPages.cookiePolicy.header") }}</h1>
		<h2 data-center :class="s.pageSubtitle">{{ $t("staticPages.cookiePolicy.whatAreCookies.header") }}?</h2>

		<div :class="s.pageParagraphContainer">
			<p v-for="p in $t('staticPages.cookiePolicy.whatAreCookies.paragraphs')" :key="p" :class="s.pageParagraph">{{ p }}</p>
		</div>

		<h2 data-center :class="s.pageSubtitle">{{ $t("staticPages.cookiePolicy.cookiesUsing.header") }}</h2>

		<div :class="s.pageParagraphContainer">
			<p :class="s.pageParagraph">{{ $t("staticPages.cookiePolicy.cookiesUsing.paragraph") }}</p>
		</div>

		<ul :class="s.pageCookiesList">
			<li v-for="type in $t('staticPages.cookiePolicy.cookiesUsing.cookiesType')" :key="type">
				<p :class="s.pageParagraph">{{ type }}</p>
			</li>
		</ul>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

export default Vue.extend({
	data() {
		return {
			s,
		}
	}
})
</script>