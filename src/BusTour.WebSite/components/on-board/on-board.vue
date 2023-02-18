<template>
	<div>
		<h1 data-center>{{ $t("onBoard.title") }}</h1>

		<div :class="s.boardContainer">
			<v-tabs v-model="tab" :class="s.tabs" :hide-slider="true">
				<v-tabs-slider></v-tabs-slider>

				<v-tab :class="s.tab" :active-class="s.tabActive">{{ $t("onBoard.bus") }}</v-tab>
				<v-tab :class="s.tab" :active-class="s.tabActive">{{ $t("onBoard.menu") }}</v-tab>
				<v-tab :class="s.tab" :active-class="s.tabActive">{{ $t("onBoard.guide") }}</v-tab>
			</v-tabs>

			<v-tabs-items v-model="tab" :class="s.tabsContent">
				<v-tab-item>
					<board-masonry directory="bus" :elements-count="3" parentClass="bus" />
				</v-tab-item>

				<v-tab-item>
					<board-masonry directory="menu" :naming="(x) => x + ''" :elements-count="12" parentClass="menu" />
				</v-tab-item>

				<v-tab-item>
					<board-guide />
				</v-tab-item>
			</v-tabs-items>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";
import custom from "./vuetify-custom.module.scss";

import BoardMasonry from "./board-masonry/board-masonry.vue";
import BoardGuide from "./board-guide/board-guide.vue";

export default Vue.extend({
	components: {
		BoardMasonry,
		BoardGuide,
	},

	data() {
		return {
			s: Object.assign(s, custom),
			tab: null,
		}
	}
})
</script>