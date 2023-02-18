<template>
	<div :class="[style.modal, modalSizeClass]" @click="clickOutSide">
		<div :class="[style.modalWrapper]">
		  	<div :class="[style.modalContent]">
		  		<div :class="style.modalHeader" ref="wrapper">
		  			<h4><slot name="header"></slot></h4>
		    		<span @click="$emit('close')" :class="[style.modalClose]">&times;</span>
		  		</div>
		  		<slot></slot>
		  	</div>
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
			size: ''
		},
		data() {
			return {
				style: style
			}
		},
		created() {
			// this.monted
			// document.addEventListener('mousedown', this.clickOutSide);
		},
		methods: {
			clickOutSide(e) {
				let el = e.target;

				if (el === this.$el) {
					console.log('close')
					this.$emit('close')
                }
                else return
			}
		},
		computed: {
			modalSizeClass() {
				switch (this.size) {
					case 'sm':
						return this.style.modalSm
					case 'l':
						return this.style.modalL
					case 'xl':
						return this.style.modalXl
					case 'mid2':
						return this.style.modalMid2
					default:
						return this.style.modalMid
				}
			}
		}
	}
</script>