<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, rightText ? 'text-right' : 'text-left', 'formFieldLabel']">
            <span class="originalText">{{ label }}</span>
        </label>

        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="IsDisabled" class="form-control form-control__disabled">
                    <span class="originalText" v-html="value"></span>
                </label>
                <textarea
                    v-else
                    class="form-control textarea"
                    cols="1"
                    :id="elName"
                    :max-length="elMaxLen"
                    :name="elName"
                    rows="3"
                    :readonly="isReadOnly"
                    aria-invalid="false"
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
    name: "text-form-value",
    mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
    props: {
        label: String,
        value: String,
        elMaxLen: Number,
        rightText: {
            type: Boolean,
            default: true,
            required: false,
        },
        isReadOnly: Boolean,
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

<style lang="scss" scoped>
.textarea {
    resize: vertical;
}
</style>
