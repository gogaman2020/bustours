<template>
    <div v-if="!isHidden" class="row formGroupRow form-group  ">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <div class="form_date-picker">
                    <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                        <span class="originalText">{{valueFrom}}</span>
                    </label>
                    <input v-else
                           ref="autonumeric_from"
                           autocomplete="off"
                           :class="['form-control', elClass]"
                           :id="elNameFrom()"
                           :name="elNameFrom()"
                           @input="onInput($event)"
                           @blur="onInput($event)"
                           v-model="elValueFrom"/>
                </div>
                <div style="float: left; padding-right: 10px; margin-top: 5px;">-</div>
                <div class="form_date-picker">
                    <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                        <span class="originalText">{{valueTo}}</span>
                    </label>
                    <input v-else
                           ref="autonumeric_to"
                           autocomplete="off"
                           :class="['form-control', elClass]"
                           :id="elNameTo()"
                           :name="elNameTo()"
                           @input="onInput($event)"
                           @blur="onInput($event)"
                           v-model="elValueTo"/>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'NumberRangeFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValueFrom: String,
            elValueTo: String,
            elClass: String
        },
        data: function() {
            return {
                eventBus: window.eventBus
            }
        },
        mounted: function () {
            this.autoNumericInitialize();
        },
        watch: {
            viewMode: function (value) {
                this.reinit = true;
            },
            elValueFrom: function (val) {
                this.value = (val && !isNaN(val)) ? val.toString().replace('.', ',') : '';
                if (this.$refs.autonumeric_from) {
                    $(this.$refs.autonumeric_from).autoNumeric('set', this.value ? this.value : '');
                }
            },
            elValueTo: function (val) {
                this.value = (val && !isNaN(val)) ? val.toString().replace('.', ',') : '';
                if (this.$refs.autonumeric_to) {
                    $(this.$refs.autonumeric_to).autoNumeric('set', this.value ? this.value : '');
                }
            }
        },
        updated: function () {
            if (this.reinit) {
                this.autoNumericInitialize();
                this.reinit = false;
            }

            $(this.$refs.autonumeric_from).trigger('blur');
            $(this.$refs.autonumeric_to).trigger('blur');
        },
        destroyed: function () {
        },
        methods: {
            elNameFrom: function () {
                return this.elName + "_from";
            },
            elNameTo: function () {
                return this.elName + "_to";
            },
            onInput: function (e) {
                if (e.target.value === "") {
                    number = undefined;
                }
                else {
                    var value = $(e.target).autoNumericGet()
                    var number = parseFloat(value);
                    number = number.toString().replace('.', ',');
                    if (isNaN(number)) {
                        number = null;
                    }
                }
                $(e.target.id).value = number;

                var id = e.target.id;
                this.$emit('input' + id.substring(id.indexOf("_")), number,this);
            },
            getValidationSettings: function () {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.elLabel + '"',
                    isRequired: true,
                    isDropDown: false
                };
            },
            autoNumericInitialize: function () {
                if (!$.fn.autoNumeric || !this.$refs.autonumeric_from) {
                    return;
                }

                let params = undefined;
                let isUpdate = false;

                if ($(this.$refs.autonumeric_from).hasClass('moneyRur') &&
                    $(this.$refs.autonumeric_from).hasClass('fa-rub')) {
                    params = {
                        aSign: ' ₽',
                    };

                    isUpdate = true;
                }

                if ($(this.$refs.autonumeric_from).hasClass('moneyMultiCur')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00',
                        vMax: '999999999999.99',
                        pSign: 's'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('money')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00',
                        vMax: '999999999999.99',
                        pSign: 's'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('percent')) {
                    params = {
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.0000'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('percent2')) {
                    params = {
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.00'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('percent12')) {
                    params = {
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.00000000'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('percent16')) {
                    params = {
                        aSign: ' %',
                        pSign: 's',
                        aSep: ' ',
                        aDec: ',',
                        vMax: '100.000000000000'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('decimal')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('decimal4')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.0000'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('decimal5')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMin: '0.00000'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('negdecimal4')) {
                    params = {
                        aSep: ' ',
                        aDec: ',',
                        vMax: '999999999.9999',
                        vMin: '-999999999.9999'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('number')) {
                    params = {
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999999.99',
                        mDec: '0'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('decimal0')) {
                    params = {
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999999',
                        mDec: '0'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('number9')) {
                    params = {
                        aSep: '',
                        aDec: ',',
                        vMax: '999999999.99',
                        mDec: '0'
                    };
                }

                if ($(this.$refs.autonumeric_from).hasClass('integer')) {
                    params = {
                        aSep: '',
                        vMax: '999999999',
                        mDec: '0'
                    };
                }

                if (params) {
                    if (isUpdate) {
                        $(this.$refs.autonumeric_from).autoNumeric('update', params);
                        $(this.$refs.autonumeric_to).autoNumeric('update', params);
                    } else {
                        $(this.$refs.autonumeric_from).autoNumeric(params);
                        $(this.$refs.autonumeric_to).autoNumeric(params);
                    }
                }
            },
        }
    }
</script>