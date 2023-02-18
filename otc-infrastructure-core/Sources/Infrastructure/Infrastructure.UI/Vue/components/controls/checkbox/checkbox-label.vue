<template>
	<div v-if="!IsHidden" :class="[style.checkboxLabel]">
		<checkbox :theme="theme" style="margin-bottom:16px;" 
						  :id="elName"
						  :name="elName"
						  v-model="checked"
						  :disabled="IsDisabled"
						  @input="onInput($event)"/>
		<label :class="[style.checkboxLabelText, 'text-left formFieldLabel']">
			<span class="originalText"><slot></slot></span>
		</label>
	</div>
</template>

<script>
	import style from './style.module.scss'
	import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

	import checkbox from './checkbox'

	export default {
		name: 'checkbox-label',
		mixins: [ContractFormSettingsMixin],
		components: {
			checkbox
		},
		props: {
			theme: {
				type: String,
				default: 'white'
			},
			value:{
				type: Boolean,
				default: false
			},
			label: String,
		},
		data() {
			return {
				style: style,
				checked: this.value
			}
		},
		watch: {
			value: function(val) {
				this.checked = val;
			}
		},
		methods: {
			onInput: function (e) {
                this.$emit('input', e);
			},
            getValidationSettings: function () {
				return {
					el: this,
					name: this.elName,
                    message: '�� ��������� ����: "' + this.label + '"',
					elValue: this.value,
                    isRequired: true,
                    isDropDown: false
                };
            }
		}
	}
</script>