<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, rightText ? 'text-right' : 'text-left', 'formFieldLabel']">
            <span class="originalText">{{ label }}</span>
        </label>

        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <div class="form_date-picker">
                    <label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
                        <span class="originalText">{{ someValue }}</span>
                    </label>
                    <div v-else :class="[elClass, 'input-group', 'form_date', 'dtpfrom', isError ? style.error : '']">
                        <input
                            autocomplete="off"
                            class="form-control"
                            :name="elName"
                            :id="elName"
                            type="text"
                            aria-invalid="false"
                            v-model.trim="model"
                            @blur="onInput($event)"
                            @keyup.enter="onInput($event)"
                        />
                        <span class="input-group-btn">
                            <button class="btn default date-set" type="button">
                                <i class="fa fa-calendar"></i>
                            </button>
                        </span>
                    </div>
                </div>
            </div>
            <div v-if="IsShowError && !IsDisabled" :class="style.errorLabel">
                <span v-if="isEmptyValue">Обязательное поле</span>
                <slot name="error"></slot>
            </div>
            <span :class="style.errorLabel" v-if="isError">{{ errorMessage }}</span>
        </div>
    </div>
</template>

<script>
/*
    https://github.com/smalot/bootstrap-datetimepicker
*/

import { FormValueCalcMixin } from "EXT/components/mixins/formValueCalcMixin";
import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";
import styles from "EXT/otc/styles/error-style.module.scss";

export default {
    name: "datetime-form-value",
    mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
    props: {
        label: String,
        value: String,
        elClass: String,
        isError: {
            type: Boolean,
            default: false,
        },
        errorMessage: {
            type: String,
            default: null,
        },
        rightText: {
            type: Boolean,
            default: true,
            required: false,
        },
        mask: {
				type: String,
				default: '99.99.9999',
				required: false
        },
    },
    data() {
        return {
            someValue: null,
            reinit: false,
            style: styles,
        };
    },
    mounted: function () {
        if(this.mask.length > 0) {
            $(this.$el).find('input').inputmask(this.mask)
        }
        this.initSomeValue();
        this.initPickers($(this.$el));
    },
    computed: {
        model: {
            get() {
                return this.value? this.getDateFormat(new Date(this.value)) : this.value;
            },
            set(value) {
                this.$emit('input', this.getDateFormat(new Date(value)));
            }
        },
        isEmptyValue() {
            return this.IsRequired && !this.value ? true : false;
        },
    },
    watch: {
        IsDisabled: function (value) {
            this.reinit = true;
        },
        value: function(value){
            this.initSomeValue();
        }
    },
    updated: function () {
        if (this.reinit) {
            this.initSomeValue();
            this.initPickers($(this.$el));
            this.reinit = false;
        }
    },
    methods: {
        initSomeValue(){
            if (this.value) {
                this.someValue = this.getDateFormat(new Date(this.value));
            } else {
                this.someValue = '';
            }
        },
        onInput: function (e) {
            if (e.target.value) {
                this.$emit("input", this.getDateValueDefaultFormat(e.target.value));
            }
        },
        initPickers: function (parent) {
            var vm = this;
            //copypast Common.js. Нужно событие, чтобы сделать двусторонний биндинг.
            if (jQuery().datetimepicker) {
                //   parent.find(".form_datetime").datetimepicker({
                //       autoclose: true,
                //       isRTL: App.isRTL(),
                //       forceParse: false,
                //       format: "dd.mm.yyyy hh:ii",
                //       keyboardNavigation: false,
                //       language: "ru",
                //       weekStart: 1,
                //       pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left")
                //   });
                //   parent.find(".form_datetime").on("changeDate", function (e) { debugger; vm.$emit('input', vm.value) });
                //if (typeof BootstrapDatetimepickerEventListener === "function") {
                //       var listener = new BootstrapDatetimepickerEventListener({ selector: '.form_datetime' });
                //    listener.init();
                //}
            }
            if (jQuery().datepicker) {
                //parent.find(".form_monthyear").datepicker({
                //    autoclose: true,
                //    forceParse: true,
                //    format: "MM yyyy",
                //    language: "ru",
                //    weekStart: 1,
                //    onSelect: function () {
                //        debugger;
                //        vm.$emit('input', vm.value);
                //    }
                //});

                parent.find(".form_date").each(function () {
                    vm.initSingleDataPicker($(this), vm);
                });

                //parent.find('.form_year').datepicker({
                //    format: "yyyy",
                //    viewMode: "years",
                //    minViewMode: "years",
                //    autoclose: true,
                //    forceParse: true,
                //    language: "ru",
                //    onSelect: function () {
                //        debugger;
                //        vm.$emit('input', vm.value);
                //    }
                //});
            }
        },
        initSingleDataPicker: function (item, vm) {
            var bottom_right = "bottom-right";
            var bottom_left = "bottom-left";

            if (!$.fn.datetimepicker.dates["ru"]) {
                Common.initPickers();
            }

            var pickerPosition = App.isRTL() ? bottom_right : bottom_left;

            if (item.hasClass(bottom_left)) {
                pickerPosition = bottom_left;
            } else if (item.hasClass(bottom_right)) {
                pickerPosition = bottom_right;
            }

            var options = {
                autoclose: true,
                isRTL: App.isRTL(),
                format: "dd.mm.yyyy",
                formatViewType: "time",
                language: "ru",
                minView: "month",
                weekStart: 1,
                pickerPosition: pickerPosition,
            };

            if (item.parents(".mm-date-picker").length) {
                item.datepicker(options);
            } else {
                item.datetimepicker(options);
                item.on("change.dp", function (e) {
                    vm.$emit("input", vm.getDateValueDefaultFormat(e.target ? e.target.value : "", vm));
                });
            }
        },
        getDateValueDefaultFormat: function (value) {
            var val = value;
            if (!val) return;
            var varArray = val.split(".");
            if (varArray.length !== 3) {
                return val;
            }
            return new Date(Date.UTC(parseInt(varArray[2]), parseInt(varArray[1]) - 1, parseInt(varArray[0]))).toISOString();
        },
        getDateFormat: function (date) {
            var mm = date.getMonth() + 1;
            var dd = date.getDate();
            return [(dd > 9 ? "" : "0") + dd, (mm > 9 ? "" : "0") + mm, date.getFullYear()].join(".");
        },
        getValidationSettings: function () {
            return {
                el: this,
                name: this.elName,
                message: 'Не заполнено поле: "' + this.label + '"',
                isRequired: true,
                isDropDown: false,
            };
        },
    },
};
</script>
