<template>
	<a href=""
	   :class="classNames"
	   @click.prevent="handlerOnClick"
	   :disabled="disabled">
		<slot></slot>
	</a>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: "Button",
		props: {
			size: {
				type: String,
				default: '',
				required: false
			},
			color: {
				type: String,
				default: '',
				required: false
			},
			disabled: {
				type: Boolean,
				default: false,
				required: false
			}
		},
		data() {
			return {
				style: style,
			}
		},
		methods: {
			handlerOnClick(e) {
				if (this.disabled) return;
				this.$emit('click', e);
			}
		},
		computed: {
			classNames() {

				let classNames = [this.style.button];

				switch (this.size) {
					case 'lg':
						classNames.push(this.style.buttonLg);
						break;
					case 'sm':
						classNames.push(this.style.buttonSm);
						break;
					default:
						classNames.push(this.style.buttonMid);
						break;
				}

				if (this.disabled) classNames.push(this.style.buttonDisabled)

				return classNames;
			}
		}
	}
</script>
