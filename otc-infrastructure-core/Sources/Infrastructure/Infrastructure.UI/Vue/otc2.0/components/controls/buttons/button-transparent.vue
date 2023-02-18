<template>
	<Button :class="[style.buttonTransparent]" :disabled="disabled" :size="size" @click="handlerOnClick">
		<slot></slot>	
	</Button>
</template>

<script>
	import Button from './button';
	import { click } from 'EXT/otc2.0/components/mixins/button-mixin'

	export default {
		name: 'button-transparent',
		components: {
			Button,
			click
		},
		mixins: [click]
	}
</script>