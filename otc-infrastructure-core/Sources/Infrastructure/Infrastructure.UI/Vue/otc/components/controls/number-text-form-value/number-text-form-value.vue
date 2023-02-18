<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ label }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="IsDisabled || isDisabled" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{value}}</span>
                </label>
                <input v-else autocomplete="off" class="form-control" :id="elName" :name="elName" type="text"
                    :value="value" 
                    @input="checkQuantity($event)"
                    @blur ="$emit('blur', $event)"
                    @keyup.enter="$emit('keyup-enter', $event)"
                 />
                <div class="suggestion" >
					<slot name="suggestion"></slot>
				</div>
            </div>
           	<div v-if="IsShowError && (!IsDisabled || !isDisabled)" :class="style.errorLabel">
                <span v-if="isEmptyValue">Обязательное поле</span>
                <slot name="error"></slot>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";
    import styles from "EXT/otc/styles/error-style.module.scss";

    export default {
        name: 'number-text-form-value',
        mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
        props: {
            label: String,
            value: {
                type: String,
                default: null
            },
            isDisabled: {
                type: Boolean,
                required: false
            },
        },
        data() {
            return {
                style: styles,
            };
        },
        computed: {
            isEmptyValue() {
                return this.IsRequired && !this.value ? true : false;
            },
        },
        methods: {
            getValidationSettings: function() {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.label + '"',
                    isRequired: true,
                    isDropDown: false
                };
            },
               checkQuantity: function(event) {
            var target = event.target;
            var value = target.value;
            target.value = value.replace(/[^0-9|,]/g, '');  
            if (value.match(/\d+(\,\d{3})/g)) {
                target.value = value.slice(0, -1);
            }
            this.$emit('input', event.target.value)
        },
        }
    }
</script>