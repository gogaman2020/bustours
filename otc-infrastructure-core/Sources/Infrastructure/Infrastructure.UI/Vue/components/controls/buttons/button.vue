<template>
	<button @click="handlerOnClick" :class="style.button" :type="type">
		<slot></slot>
	</button>
</template>

<script>
	import style from './style.module.scss'
	import { buttonMixin } from './mixin'

	export default {
		name: 'Button',
		components: {

		},
		data() {
			return {
				style: style
			}
		},
		mixins: [buttonMixin]
	}
</script>