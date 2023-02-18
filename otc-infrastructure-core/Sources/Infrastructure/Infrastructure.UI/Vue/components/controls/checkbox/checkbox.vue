<template>
	<label :class="[classNames]">
		<input 
			type="checkbox" 
			:value="value" v-model="model" 
			:class="[style.checkInput, theme == 'black' ? style.checkInputBlack : style.checkInputWhite]" 
			:disabled="disabled"
            @input="onInput($event)">
		<span :class="[style.checkBox]"></span>
		<slot></slot>
	</label>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: 'check',
		components: {
		},
		model: {
			prop: 'checked',
			event: 'change'
		},
		props: {
			theme: {
				type: String,
				default: 'white'
			},
			disabled: {
				type: Boolean,
				default: false
			},
			value: {},
			checked: {}
		},
		data() {
			return {
				style: style,
				check: false
			}
		},
		computed: {
			classNames() {
				var classNames = [];

				classNames.push(classNames.push(this.style.check));

				switch (this.theme) {
					case 'black':
						classNames.push(this.style.checkBlack)
						break;
					case 'white':
						classNames.push(this.style.checkWhite)
						break;
				}

				return classNames; 
			},
			model: {
				get() {
					return this.checked;
				},
				set(val) {
					this.$emit('change', val);
				}
			}
        },
        methods: {
            onInput: function (e) {
                this.$emit('input', e.target.checked);
            },
        }
	}
</script>