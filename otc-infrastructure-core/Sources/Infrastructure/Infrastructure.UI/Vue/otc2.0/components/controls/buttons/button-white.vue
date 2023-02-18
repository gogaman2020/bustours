<template>
	<Button :class="[themeClass]" :size="size" :disabled="disabled" @click="handlerOnClick">
		<slot></slot>	
	</Button>
</template>

<script>
	import Button from './button';
	import { click } from 'EXT/otc2.0/components/mixins/button-mixin'

	export default {
		name: 'button-white',
		components: {
			Button,
			click
		},
		props: {
			theme: {
				type: String,
				default: 'black',
				required: false
			}
		},
		computed: {
			themeClass() {
				return this.theme == 'black' ? this.style.buttonWhite : this.style.buttonTransparent
			}
		},
		mixins: [click]
	}
</script>