<template>
	<div :class="[s.masonry, s[parentClass]]">
		<div :class="s.masonryItem" v-for="item in elements" :key="item.id">
			<img :src="item.url" />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";
import path from "path";

class MasonryItem {
	constructor(public id: number, public url: string) {}
}

export default Vue.extend({
	props: {
		directory: String,
		elementsCount: Number,
		parentClass: String,
		naming: Function,
	},

	data() {
		return {
			s,
			baseDirectory: "/images/on-board/",
			extention: "jpg"
		}
	},

	computed: {
		elements(): MasonryItem[] {
			let config = Array(this.elementsCount)
				.fill(0)
				.map((_, idx) => new MasonryItem(
					idx, 
					path.join(this.baseDirectory, this.directory, `${this.naming ? this.naming(++idx) : ++idx}.${this.extention}`))
				);

			return config;
		}
	},
})
</script>