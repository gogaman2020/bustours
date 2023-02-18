<template>
	<div v-if="loading" :class="[isAbsolute ? style.spinnerAbsolute : style.spinnerLocal]">
		<pulse-loader :loading="loading" :color="color"/>
	</div>
</template>

<script>
	import style from './style.module.scss'

	import pulseLoader from 'vue-spinner/src/PulseLoader.vue'

	export default {
		name: 'spinner',
		components: {
			pulseLoader
		},
		props: {
			loading: {
				type: Boolean,
				default: true
			},
			isAbsolute: {
				type: Boolean,
				default: true
			}
		},
		data() {
			return {
                style: style,
                color: '#cc181e'
			}
		},
	}
</script>