<template>
    <div v-if="!isHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
               <label v-if="isView && isReadOnly" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{value}}</span>
                </label>
                <input v-else autocomplete="off" class="form-control" :id="elName" :name="elName" type="text"
                       v-bind:value="value"
                       @input="onInput($event)">
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'StringFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: String,
            isReadOnly:Boolean
        },
        data: function() {
            return {
                eventBus: window.eventBus,
            }
        },
        computed: {
            value: function () {
                return this.elValue;
            }
        },
        mounted: function () {
        },
        created: function() {

        },
        methods: {
            onInput: function(e) {
                //this.elValue = e.target.value;
                //this.value = e.target.value;
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