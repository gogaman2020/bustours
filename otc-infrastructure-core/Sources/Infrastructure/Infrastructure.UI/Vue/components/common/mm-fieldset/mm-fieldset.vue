<template>
	<fieldset :class="style.fieldset">
		<legend :class="style.legend">{{header}}</legend>
		<slot></slot>
	</fieldset>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: 'mm-fieldset',
		components: {

		},
		props: {
            header: null
		},
		data() {
			return {
				style: style
			}
		}
	}
</script>