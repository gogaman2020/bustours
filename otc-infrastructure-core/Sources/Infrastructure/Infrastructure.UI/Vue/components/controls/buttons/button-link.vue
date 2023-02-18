<template>
	<a href="href" :class="style.button">
		<slot></slot>	
	</a>
</template>

<script>
	import { buttonMixin } from './mixin'

	export default {
		name: 'button-link',
		components: {

		},
		props: {
			href: {
				type: String,
				required: true
			}
		},
		data() {
			return {
				// style: style
			}
		},
		mixins: [buttonMixin]
	}
</script>