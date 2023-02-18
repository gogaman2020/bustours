<template>
	<div :class="'select-label'">
		<span :class="[style.selectLabelText]"><slot></slot></span>
		<tooltip v-if="hint" :message="hint" :class="[style.selectLabelTooltip]" />
		<select-control :list="list" :placeholder="placeholder" :hint="hint" />
	</div>
</template>

<script>
	import style from './select.module.scss'

	import selectControl from './select'
	import tooltip from 'EXT/otc2.0/components/common/tooltip/tooltip'

	export default {
		name: 'select-label',
		components: {
			selectControl,
			tooltip
		},
		props: {
			placeholder: {
				type: String,
				default: ''
			},
			hint: {
				type: String,
				default: ''
			},
			list: false
		},
		data() {
			return {
				style: style
			}
		},
	}
</script>