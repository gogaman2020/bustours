<template>
	<div :class="className" ref="Panel">
 		<slot></slot>
		<slot name="custom"></slot>
  	</div>
</template>

<script lang="ts">
import vue from "vue";
import s from "./style.module.scss";

export enum PanelSizes {
	Small = 376,
	Middle = 548,
	Big = 776,
}

export default vue.extend({
	name: "info-panel",

	props: {
		allLeftBold: Boolean,
		size: {
			default: PanelSizes.Small,
			required: false,
			type: Number
		},
		centerNonBoundaryColumns: Boolean,
		cols: {
			type: Number,
			default: 2
		}
	},

	data() {
		return {
			s,
		}
	},

	computed: {
		className() {
			const t = (this as any);

			return [
				t.s.panel,
				[t.s[`size${PanelSizes[t.size]}`]],
				[t.s[`cols${t.cols}`]],
				{
					[t.s.panelDefaultLayout]: t.$slots.default,
					[t.s.cols2]: t.$slots.default,
					[t.s.allLeftBold]: t.allLeftBold,
					[t.s.centerNonBoundaryColumns]: t.centerNonBoundaryColumns,
				}
			]
		}
	},
})
</script>