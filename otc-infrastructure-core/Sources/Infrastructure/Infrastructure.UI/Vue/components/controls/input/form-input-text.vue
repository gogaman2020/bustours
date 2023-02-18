<template>
	<div v-if="!IsHidden" class="row formGroupRow form-group">
		<label :class="[style.input, calcLabelSize, style.inputLabel, 'formFieldLabel text-right']">
			<span class="originalText">
				{{ label }}
			</span>
		</label>
		<div :class="[calcValueSize, style.inputField]">
			<div class="col-sm-12 no-space formFieldValueDiv">
				<label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
					<span class="originalText">{{value}}</span>
				</label>
				<input v-else autocomplete="off" :class="[!valid ? style.inputError : '', 'form-control']" :id="elName" :name="elName" type="text"
						v-bind:value="value"
						:disabled="disabled"
						:placeholder="placeholder"
						@keyup="$emit('input', $event.target.value)"
						@blur="$emit('blur', $event)">
				<div :class="style.inputSuggestion">
					<slot name="suggestion"></slot>
				</div>
			</div>
			<div :class="style.inputErrors">
				<slot name="error"></slot>
			</div>
		</div>
	</div>
	
</template>

<script>

	import style from './style.module.scss'
	import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

	export default {
		name: 'form-input-text',
        mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
		components: {

		},
		props: {
			placeholder: {
				type: String,
				default: '',
				required: false
			},
			value: {
				type: String,
				default: '',
				required: false
			},
			disabled: {
				type: Boolean,
				default: false,
				required: false
			},
			valid: {
				type: Boolean,
				default: true,
				required: false
			},
			mask: {
				type: String,
				default: '',
				required: false
			},
			label: String,
		},
		
		data() {
			return {
				style: style,
			}
		},
		mounted() {
			if(this.mask.length > 0) {
				$(this.$el).find('input').inputmask(this.mask)
			}
		},
		computed: {
			elValue() {
				return this.value;
            }
        },
        methods: {
            getValidationSettings: function () {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.label + '"',
                    isRequired: true,
                    isDropDown: false
                };
            }
        }

	}
</script>