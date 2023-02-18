<template>
	<th :class="s.bbGridHeadCell" :data-field="field">
		<slot>{{ title }}</slot>
	</th>
</template>

<script lang="ts">
import Vue from "vue";
import s from "../s.module.scss";

export default Vue.extend({
	props: {
		title: {
			type: String,
			default: "",
		},
		field: {
			type: String,
		},
		width: {
			type: String,
		},
		variants: {
			type: Array,
			default: () => []
		},
		template: {
			type: [Object, Function],
			default: () => (data: string) => `${data}`,
		},
		editable: {
			type: Boolean,
			default: false
		},
		editableCondition: {
			type: Function,
			default: null
		},
		cellClass: {
			type: String,
		},
	},

	data() {
		return {
			s,
		}
	},
})
</script>