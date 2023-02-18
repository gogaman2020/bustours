<template>
	<div :class="[style.selectTextInput]">
		<input type="text" :placeholder="placeholder">
	</div>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: 'select-text-input',
		components: {

		},
		props: {
			placeholder: {
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