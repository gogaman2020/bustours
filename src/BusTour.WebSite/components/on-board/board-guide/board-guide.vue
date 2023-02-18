<template>
	<div>
		<v-tabs vertical v-model="tab" :hide-slider="true" :class="[s.tabs, s.tabsVertical, s.guideTabs]">
			<v-tabs-slider />

			<div :class="s.guideText">{{ $t("onBoard.guideText") }}</div>

			<v-tab 

				v-for="lang in langs"
				:key="lang"
				:class="[s.tab, s.guideTab]" 
				:active-class="[s.tabActive, s.guideTabActive]"
			>
				{{ $t(`onBoard.langs.${lang}`) }}
			</v-tab>

			<v-tabs-items v-model="tab" :class="s.guideImages">
				<v-tab-item v-for="lang in langs" :key="lang">
					<div :class="s.guideImage" :style="`background-image: url(${path.join(baseDir, `${'es'}.${ext}`)})`"></div>
				</v-tab-item>
			</v-tabs-items>
		</v-tabs>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";
import custom from "../vuetify-custom.module.scss";
import path from "path";

export default Vue.extend({
	data() {
		return {
			s: Object.assign(s, custom),
			tab: null,
			baseDir: "/images/on-board/guide/",
			langs: ["en", "de", "es", "ru", "fr"],
			path,
			ext: "jpg"
		}
	}
})
</script>