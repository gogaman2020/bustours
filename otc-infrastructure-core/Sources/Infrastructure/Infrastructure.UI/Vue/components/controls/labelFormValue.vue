<template>
    <div v-if="!isHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-bind:class="{'form-control': true, 'form-control__disabled': disabled }"><span class="originalText">{{ elValue }}</span></label>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'labelFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elName: String,
            elLabel: String,
            elValue: String,
            disabled: Boolean
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                value: {}
            }
        },
        mounted: function () {
        },
        created: function() {

        },
        methods: {
            onInput: function(e) {
                this.elValue = e.target.value;
                this.$emit('input', this.elValue);
            },
            getValidationSettings: function() {
                return null;
            }
        }
    }
</script>