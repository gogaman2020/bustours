<template>
	<div :class="[style.modal, modalSizeClass]">
		<div :class="[style.modalWrapper, allowOverflow ? style.modalOverflow : '']">
		  	<div class="modal_content">
		  		<slot></slot>	
		  	</div>
		    <span @click="$emit('close')" :class="[style.modalClose]">&times;</span>
		</div>
	</div>
</template>

<!-- to do: -->
<!-- Скрытие при клике вне контента окна -->

<script>
	import style from './style.module.scss'

	export default {
		name: 'modal',
		components: {

		},
		props: {
			size: '',
			allowOverflow: {
				type: Boolean,
				default: true
			}
		},
		data() {
			return {
				style: style
			}
		},
		computed: {
			modalSizeClass() {
				switch (this.size) {
					case 'sm':
						return this.style.modalSm
					case 'xl':
						return this.style.modalXl
					default:
						return this.style.modalMid
				}
			}
		}
	}
</script>