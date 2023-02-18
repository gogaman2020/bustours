<template>
	<div v-if="!IsHidden" :class="['row formGroupRow form-group']">
		<label :class="[calcLabelSize, rightText? 'text-right':'text-left', 'formFieldLabel']">
			<span class="originalText">{{ label }}</span>
		</label>
		<div :class="[calcValueSize]">
			<div :class="['col-sm-12 no-space formFieldValueDiv']">
					<checkbox :theme="theme" style="margin-bottom:10px;" 
						  :id="elName"
						  :name="elName"
						  v-model="checked"
						  :disabled="IsDisabled"
						  @input="onInput($event)"
						  />
			</div>
		</div>
	</div>
</template>

<script>
	import style from './style.module.scss';
	import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
	import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

	import checkbox from './checkbox';

	export default {
		name: 'form-label-checkbox',
        mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
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
			rightText:{
                type: Boolean,
                default: true,
                required: false
            },
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
                    message: 'Не заполнено поле: "' + this.label + '"', 
					elValue: this.value,
                    isRequired: true,
                    isDropDown: false
                };
            }
		}
	}
</script>