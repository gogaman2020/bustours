<template>
	<div :class="s.pager">
		<div 
			v-for="page in visiblePages" 
			:key="page.page" 
			:class="[s.pagerPage, {[s.pagerPageActive]: page.isActive}]"
			@click="changeActivePage(page.page)"
		>
			{{ page.page }}
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import { IPagerPage, IPaging, IRange } from "../bb-grid-types";

export default Vue.extend({
	props: {
		total: {
			type: Number,
			default: 0,
		},
		pageSize: {
			type: Number,
			default: 10
		},
		visiblePageCount: {
			type: Number,
			default: 10
		}
	},

	data() {
		return {
			s,
			activePage: 1,
			visibleInterval: {
				min: 0,
				max: this.visiblePageCount,
			} as IRange
		}
	},

	computed: {
		pages(): IPagerPage[] {
			const pagesCount = Math.ceil(this.total / this.pageSize);
			let pages = Array(pagesCount)
				.fill(0)
				.map((_, idx) => ({ page: ++idx, isActive: false }));

			pages[this.activePage - 1].isActive = true;

			this.fillPagingData();
			
			return pages;
		},
		visiblePages(): IPagerPage[] {
			return this.pages.slice(this.visibleInterval.min, this.visibleInterval.max);
		}
	},

	watch: {
		activePage(value: number): void {
			if (value == this.visibleInterval.max && this.visibleInterval.max != Math.max(...this.pages.map(p => p.page))) {
				this.visibleInterval.max = this.visibleInterval.max + 1;
				this.visibleInterval.min = this.visibleInterval.min + 1;
			} else if (value == this.visibleInterval.min + 1 && this.visibleInterval.min != 0) {
				this.visibleInterval.max = this.visibleInterval.max - 1;
				this.visibleInterval.min = this.visibleInterval.min - 1;
			}
		},
		total(_: number): void {
			this.changeActivePage(1);
		}
	},

	methods: {
		changeActivePage(page: number): void {
			this.activePage = page;
		},
		fillPagingData(): void {
			const paging: IPaging = {
				skip: (this.activePage - 1) * this.pageSize,
				take: this.activePage * this.pageSize
			}
			
			this.$emit("page", paging);
		}
	}
})
</script>