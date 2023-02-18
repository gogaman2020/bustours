<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, rightText? 'text-right': '', 'formFieldLabel']"><span class="originalText">{{ label }}</span></label>
        <div :class="calcValueSize">
            <div :class="['col-sm-12', 'no-space', 'formFieldValueDiv', isError ? style.error : '']">
                <label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{valueView}}</span>
                </label>
                <input v-else
                       ref="autonumeric"
                       autocomplete="off"
                       :class="['form-control', elClass]"
                       :id="elName"
                       :name="elName"
                       type="text"
                       v-bind:value="elValue"
                       @input="onInput($event)"
                       @blur="onInput($event)">
            </div>
            <div v-if="IsShowError && !IsDisabled" :class="style.errorLabel">
                <span v-if="isEmptyValue">Обязательное поле</span>
                <slot name="error"></slot>
            </div>
            <span :class="style.errorLabel" v-if="isError">{{errorMessage}}</span>
        </div>
    </div>
</template>

<script>

    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";
    import styles from 'EXT/otc/styles/error-style.module.scss'

    export default {
        name: 'number-form-value',
        mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
        props: {
            label: String,
            value: Number,
            elClass: String,
            isError: {
                type: Boolean,
                default: false
            },
            errorMessage: {
                type: String,
                default: null
            },
            rightText: {
                type: Boolean,
                default: true
            }
        },
        data: function() {
            return {
                elValue: this.value,
                style: styles,
                valueView: this.value
            }
        },
        mounted: function () {
            this.parsevalue(this.elValue);
            this.autoNumericInitialize();
        },
        created: function() {

        },
        watch: {
            isDisabled: function(val) {
                this.reinit = true;
            },
            value: function(val) {
                this.parsevalue(val);
                if(this.$refs.autonumeric){
                    $(this.$refs.autonumeric).autoNumeric('set', this.elValue ? this.elValue : '');
                }
                this.valueView = new Intl.NumberFormat('ru-RU').format(val);
            },
        },
        updated: function(){
            if(this.reinit){
                this.autoNumericInitialize();
                this.reinit = false;
            }

            $(this.$refs.autonumeric).trigger('blur');
        },
        computed: {
            isEmptyValue() {
                return this.IsRequired && !this.value ? true : false;
            },
        },
        methods: {
            parsevalue: function(val){
                //WARNING!! в недрах autonumeric 1.9.26 вот такой регуляркой "/((-?)[^-\,\d].*?(\d|\,\d)/" сносится точка ВНУТРИ значения,
                //          а не ПЕРЕД, как описано в коде 
                //          autonumeric.autoStrip -> s = s.replace(settings.skipFirstAutoStrip, '$1$2'); /** first replace anything before digits */
                if(val !== undefined && val !==null && !isNaN(val)){
                    this.elValue = val.toString().replace('.', ',');
                }  else {
                    this.elValue = '';
                }
            },
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
                    message: 'Не заполнено поле: "' + this.label + '"',
                    isRequired: true,
                    isDropDown: false
                };
            },
            autoNumericInitialize: function () {
                this.valueView = this.valueViewCalc(this.value);

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
                
                if($(this.$refs.autonumeric).hasClass('percentOutIcon')){
                    params = { 
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
            valueViewCalc: function(value){
                if(this.$options.filters && this.$options.filters.currency && this.$options.filters.number){
                    if (this.value === undefined || this.value === null || isNaN(this.value)){
                        return '';
                    }

                    if (this.elClass == 'moneyRur' ||
                        this.elClass == 'moneyMultiCur' ||
                        this.elClass == 'money') {
                        return this.$options.filters.currency(this.value);
                    }

                    if(this.elClass == 'percent'){
                        return this.$options.filters.number(this.value, '0')+' %';
                    }

                    if(this.elClass == 'percent2'){
                        return this.$options.filters.number(this.value, '0.00')+' %';
                    }

                    if(this.elClass == 'percent12'){
                        return this.$options.filters.number(this.value, '0.00000000')+' %';
                    }

                    if(this.elClass == 'percent16'){
                        return this.$options.filters.number(this.value, '0.000000000000')+' %';
                    }

                    if(this.elClass == 'decimal'){
                        return this.$options.filters.currency(this.value);
                    }

                    if(this.elClass == 'decimal4'){
                        return this.$options.filters.number(this.value, '0.0000');
                    }

                    if(this.elClass == 'decimal5'){
                        return this.$options.filters.number(this.value, '0.00000');
                    }

                    if(this.elClass == 'negdecimal4'){
                        return this.$options.filters.number(this.value, '0.0000');
                    }

                    if(this.elClass == 'number'){
                        return this.$options.filters.number(this.value, '0');
                    }

                    if(this.elClass == 'decimal0'){
                        return this.$options.filters.number(this.value, '0');
                    }

                    if(this.elClass == 'number9'){
                        return this.$options.filters.number(this.value, '0.00');
                    }

                    if(this.elClass == 'integer'){
                        return this.$options.filters.number(this.value, '0');
                    }
                }

                return this.value;
            },
        }
    }
</script>