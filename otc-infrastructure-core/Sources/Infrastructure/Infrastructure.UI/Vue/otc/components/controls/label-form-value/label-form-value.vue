<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize,rightText? 'text-right': '', 'formFieldLabel']"><span class="originalText">{{ label }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label class="formFieldLabel"><span class="originalText">{{ value }}</span></label>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

    export default {
        name: 'label-form-value',
        mixins: [FormValueCalcMixin,ContractFormSettingsMixin],
        props: {
            label: String,
            value: [Number, String],
            rightText: {
                type: Boolean,
                default: true
            }
        }
    }
</script>