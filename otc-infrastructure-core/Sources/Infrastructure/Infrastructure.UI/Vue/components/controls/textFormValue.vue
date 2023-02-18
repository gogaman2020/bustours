<template>
    <div v-if="!isHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{value}}</span>
                </label>
                <textarea v-else class="form-control" cols="1" :id="elName" :max-length="elMaxLen" :name="elName" :readonly=elReadonly rows="3" aria-invalid="false" 
                    v-bind:value="value"
                    @input="onInput($event)"></textarea>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'TextFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: String,
            elMaxLen: Number,
            elReadonly: {
                type: Boolean,
                default: false
            }
        },
        data: function() {
            return {
                eventBus: window.eventBus
            }
        },
        computed: {
            value: function () {
                return this.elValue;
            }
        },
        mounted: function () {
            //debugger;
        },
        created: function() {

        },
        methods: {
            onInput: function (e) {
                this.$emit('input', e.target.value);
            },
            getValidationSettings: function() {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.elLabel + '"',
                    isRequired: true,
                    isDropDown: false
                };
            }
        }
    }
</script>