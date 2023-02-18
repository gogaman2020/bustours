<template>
	<label :class="[style.check]">
		<input type="checkbox" 
			:value="value" 
			v-model="model" 
			:class="[style.checkInput]" 
			:disabled="disabled"
			v-indeterminate="indeterminate && !model" >
		<span :class="[style.checkBox, indeterminate && !model ? style.checkBoxIndeterminate : '']"></span>
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
			theme: 'white',
			disabled: {
				type: Boolean,
				default: false
			},
			value: {},
			checked: {},
			indeterminate: false
		},
		data() {
			return {
				style: style,
				check: false
			}
		},
		computed: {
			model: {
				get() {
					return this.checked;
				},
				set(val) {
					this.$emit('change', val);
				}
			}
		},
		directives: {
			indeterminate: function(el, binding) {
			  el.indeterminate = Boolean(binding.value)
			}
		}
	}
</script>