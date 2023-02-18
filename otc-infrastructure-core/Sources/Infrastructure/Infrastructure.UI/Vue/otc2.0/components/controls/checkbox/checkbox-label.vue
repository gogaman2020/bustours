<template>
	<div :class="[style.checkboxLabel]">
		<checkbox />
		<span :class="[style.checkboxLabelText]">
			<slot></slot>
		</span>
	</div>
</template>

<script>
	import style from './style.module.scss'

	import checkbox from './checkbox'

	export default {
		name: 'checkbox-label',
		components: {
			checkbox
		},
		props: {

		},
		data() {
			return {
				style: style
			}
		},
	}
</script>