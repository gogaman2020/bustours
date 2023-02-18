<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group" style="height: 34px;">
        <label :class="[calcLabelSize, rightText? 'text-right': '', 'formFieldLabel']"><span class="originalText">{{ label }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label class="formFieldLabel form-control form-control__disabled">
                    <a v-show="href" :href="href" :target="target" style="line-height: 20px;">{{ value }}</a>
                </label>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";


    export default {
        name: 'link-form-value',
        mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
        props: {
            value: String,
            label: String,
            href: String,
            target: String,
            rightText: {
                type: Boolean,
                default: true
            }
        }
    }
</script>