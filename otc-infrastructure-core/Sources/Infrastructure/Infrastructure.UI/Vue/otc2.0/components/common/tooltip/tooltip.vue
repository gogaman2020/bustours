<template>
	<div v-tooltip.top-center="message" :class="[style.tooltip]">
		<span>i</span>
	</div>
</template>

<script>
	import style from './style.module.scss'
	import './style.scss'

	export default {
		name: 'tooltip',
		components: {

		},
		props: {
			message: {
				type: String,
				default: ''
			}
		},
		data() {
			return {
				style: style
			}
		},
	}
</script>