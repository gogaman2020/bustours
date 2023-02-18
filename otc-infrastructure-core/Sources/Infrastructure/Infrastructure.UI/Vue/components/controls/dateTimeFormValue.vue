<template>
    <div v-if="!isHidden" class="row formGroupRow form-group  ">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <div class="form_date-picker">
                    <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                        <span class="originalText">{{someValue}}</span>
                    </label>
                    <div v-else :class="[elClass, 'input-group', 'form_date', 'dtpfrom']">
                        <input autocomplete="off" class="form-control" :name="elName" :id="elName" type="text" aria-invalid="false"
                               v-bind:value="someValue"
                               @input="onInput($event)">
                        <span class="input-group-btn">
                            <button class="btn  default date-set" type="button">
                                <i class="fa fa-calendar"></i>
                            </button>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

/*
    https://github.com/smalot/bootstrap-datetimepicker
*/

    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'DateTimeFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: String,
            elClass: String
        },
        data() {
            return {
                eventBus: window.eventBus,
                someValue: null,
                reinit: false
            }
        },
        mounted: function () {
            if (this.elValue) {
                this.someValue = this.getDateFormat(new Date(this.elValue));
            }
            this.initPickers($(this.$el));
        },
        watch: {
            elValue: function (value) {
                if (value) {
                    this.someValue = this.getDateFormat(new Date(value));
                }
            },
            viewMode: function(value) {
                this.reinit = true;
            }
        },
        updated: function(){
            if(this.reinit){
                this.initPickers($(this.$el));
                this.reinit = false;
            }
        },
        destroyed: function () {
        },
        methods: {
            onInput: function(e) {
                this.$emit('input', this.getDateValueDefaultFormat(e.target.value));
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

                 //   parent.find(".form_datetime").on("changeDate", function (e) { debugger; vm.$emit('input', vm.elValue) });

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
                    //        vm.$emit('input', vm.elValue);
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
                    //        vm.$emit('input', vm.elValue);
                    //    }
                    //});
                }
            },
            initSingleDataPicker: function(item, vm){
                var bottom_right = "bottom-right";
                var bottom_left = "bottom-left";


                if(!$.fn.datetimepicker.dates['ru']) {
                    Common.initPickers();
                }

                var pickerPosition = (App.isRTL() ? bottom_right : bottom_left);

                if (item.hasClass(bottom_left)) {
                    pickerPosition = bottom_left;
                }
                else if (item.hasClass(bottom_right)) {
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
                    pickerPosition: pickerPosition
                };

                if (item.parents('.mm-date-picker').length) {
                    item.datepicker(options);
                }
                else {
                    item.datetimepicker(options);
                    item.on('change.dp', function (e) {
                        vm.$emit('input', vm.getDateValueDefaultFormat(e.target ? e.target.value : "", vm));
                    });
                }
            },
            getDateValueDefaultFormat: function (value) {
                var val = value;
                if (!val) return;
                var varArray = val.split('.');
                if (varArray.length !== 3) {
                    return val;
                }
                return new Date(Date.UTC(parseInt(varArray[2]), parseInt(varArray[1]) - 1, parseInt(varArray[0]))).toISOString();
            },
            getDateFormat: function (date){
                var mm = date.getMonth() + 1;
                var dd = date.getDate();
                return [(dd>9 ? '' : '0') + dd,(mm>9 ? '' : '0') + mm,date.getFullYear()].join('.');
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