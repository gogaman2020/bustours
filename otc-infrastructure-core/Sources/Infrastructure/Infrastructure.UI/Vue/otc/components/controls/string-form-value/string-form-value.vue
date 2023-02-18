<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, rightText ? 'text-right' : 'text-left', 'formFieldLabel']">
            <span class="originalText">{{ label }}</span>
        </label>

        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{ value }}</span>
                </label>
                <input
                    v-else
                    autocomplete="off"
                    class="form-control"
                    :id="elName"
                    :name="elName"
                    type="text"
                    :value="value"
                    @input="$emit('input', $event.target.value)"
                />
               <div v-if="IsShowError && !IsDisabled" :class="style.errorLabel">
                    <span v-if="isEmptyValue">Обязательное поле</span>
                    <slot name="error"></slot>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { FormValueCalcMixin } from "EXT/components/mixins/formValueCalcMixin";
import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";
import styles from "EXT/otc/styles/error-style.module.scss";

export default {
    name: "string-form-value",
    mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
    props: {
        label: String,
        value: {
            type: String,
            default: null,
        },
        rightText: {
            type: Boolean,
            default: true,
            required: false,
        },
    },
    data() {
        return {
            style: styles,
        };
    },
    methods: {},
    computed: {
        isEmptyValue() {
            return this.IsRequired && !this.value ? true : false;
        },
    },
};
</script>
