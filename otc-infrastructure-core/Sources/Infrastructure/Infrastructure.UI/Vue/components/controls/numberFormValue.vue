<template>
    <div v-if="!isHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
               <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{value}}</span>
                </label>
                <input v-else
                       ref="autonumeric"
                       autocomplete="off" 
                       :class="['form-control', elClass]" 
                       :id="elName" 
                       :name="elName"
                       type="text"
                       v-bind:value="value"
                       @input="onInput($event)"
                       @blur="onInput($event)">
            </div>
        </div>
    </div>
</template>

<script>

/*
    http://www.decorplanit.com/plugin/
*/

    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'NumberFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: Number,
            elClass: String
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                value: this.elValue
            }
        },
        mounted: function () {
            this.autoNumericInitialize();
        },
        created: function() {

        },
        watch: {
            viewMode: function(value) {
                this.reinit = true;
            },
            elValue: function(val) {
                //WARNING!! в недрах autonumeric 1.9.26 вот такой регуляркой "/((-?)[^-\,\d].*?(\d|\,\d)/" сносится точка ВНУТРИ значения,
                //          а не ПЕРЕД, как описано в коде 
                //          autonumeric.autoStrip -> s = s.replace(settings.skipFirstAutoStrip, '$1$2'); /** first replace anything before digits */
                if(val && !isNaN(val)){
                    this.value = val.toString().replace('.', ',');
                }  else {
                    this.value = '';
                }

                if(this.$refs.autonumeric){
                    $(this.$refs.autonumeric).autoNumeric('set', this.value ? this.value : '');
                }
            }
        },
        updated: function(){
            if(this.reinit){
                this.autoNumericInitialize();
                this.reinit = false;
            }

            $(this.$refs.autonumeric).trigger('blur');
        },
        methods: {
            onInput: function(e) {
                var value = $(this.$refs.autonumeric).autoNumeric('get');
                var number = parseFloat(value);
                if(isNaN(number)){
                    number = null;
                }

                this.$emit('input', number);
            },
            getValidationSettings: function() {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.elLabel + '"',
                    isRequired: true,
                    isDropDown: false
                };
            },
            autoNumericInitialize: function () {
                if (!$.fn.autoNumeric || !this.$refs.autonumeric) {
                    return;
                }

                let params = undefined;
                let isUpdate = false;
                
                if($(this.$refs.autonumeric).hasClass('moneyRur') &&
                   $(this.$refs.autonumeric).hasClass('fa-rub')){
                    params = {
                        aSign: ' ₽',
                    };

                    isUpdate = true;
                }
                
                if($(this.$refs.autonumeric).hasClass('moneyMultiCur')){
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00',
                        vMax: '999999999999.99',
                        pSign: 's'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('money')){
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00',
                        vMax: '999999999999.99',
                        pSign: 's'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('percent')){
                    params = { 
                        aSign: ' %', 
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.0000'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('percent2')){
                    params = { 
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.00'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('percent12')){
                    params = { 
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.00000000'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('percent16')){
                    params = { 
                        aSign: ' %', 
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.000000000000'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('decimal')){
                    params = { 
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('decimal4')){
                    params = { 
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.0000'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('decimal5')){
                    params = { 
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00000'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('negdecimal4')){
                    params = { 
                        aSep: ' ',
                        aDec: ',',
                        vMax: '999999999.9999',
                        vMin: '-999999999.9999'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('number')){
                    params = { 
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999999.99',
                        mDec: '0'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('decimal0')){
                    params = { 
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999999',
                        mDec: '0'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('number9')){
                    params = { 
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999.99',
                        mDec: '0'
                    };
                }

                if($(this.$refs.autonumeric).hasClass('integer')){
                    params = { 
                        aSep: '',
                        vMax: '999999999',
                        mDec: '0'
                    };
                }

                if(params) {
                    if(isUpdate){
                        $(this.$refs.autonumeric).autoNumeric('update', params);
                    } else {
                        $(this.$refs.autonumeric).autoNumeric(params);
                    }
                }
            },
        }
    }
</script>