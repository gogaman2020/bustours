<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, rightText ? 'text-right' : 'text-left', 'formFieldLabel']">
            <span class="originalText">{{ label }}</span>
        </label>

        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{viewValue}}</span>
                </label>
                <div v-else>
                    <select class="select2 form-control select2-hidden-accessible" :multiple="multiple" data-live-search="true" data-size="10" :id="elName" :name="elName" tabindex="-1" aria-hidden="true">
                        <option v-for="item in this.getItemsToSelect(elItems)" 
                                :value="item.Value" 
                                :key="item.Value" 
                                :selected="item.Value == value"
                                :disabled="item.CanNotBeSelected">{{ item.Text }}</option>
                    </select>
                </div>
            </div>
            <div v-if="IsShowError && !IsDisabled" :class="style.errorLabel">
                <span v-if="isEmptyValue">Обязательное поле</span>
                <slot name="error"></slot>
            </div>
        </div>
    </div>
</template>

<script>
import { FormValueCalcMixin } from "EXT/components/mixins/formValueCalcMixin";
import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";
import styles from "EXT/otc/styles/error-style.module.scss";

export default {
    name: "select-form-value",
    mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
    props: {
        label: String,
        value: [Number, String],
        elList: Array,
        elItems: Array,
        placeholder: String,
        multiple: {
            type: Boolean,
            default: false,
            required: false,
        },
        grouped: {
            type: Boolean,
            default: false,
            required: false,
        },
        allowClear: {
            type: Boolean,
            default: true,
            required: false,
        },
        rightText: {
            type: Boolean,
            default: true,
            required: false,
        },
        typeValueString:  {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            viewValue: null,
            reinit: false,
            style: styles,
        };
    },
    mounted: function () {
        this.initSelect();
    },
    watch: {
        value: function (val) {
            if (this.IsDisabled) {
                this.initSelect();
            } else {
                $(this.$el).find("select").val(val).trigger("change");
            }
        },
        elItems: function (options) {
            if (this.IsDisabled) {
                this.initSelect();
            } else {
                this.reinit = true;
            }
        },
        IsDisabled: function (options) {
            if (this.IsDisabled) {
                this.initSelect();
            } else {
                this.reinit = true;
            }
        },
    },
    updated: function () {
        if (this.reinit) {
            this.initSelect();
            this.reinit = false;
        }
    },
    destroyed: function () {
        //$(this.$el).find('select').off().select2('destroy');
    },
    methods: {
        initSelect: function () {
            var vm = this;

            var placeholder = vm.placeholder ? vm.placeholder : typeof vm.placeholder == "undefined" ? "Не выбрано" : null;
            
            if (!this.IsDisabled) {
                $(vm.$el)
                    .find("select")
                    .select2({ language: "ru", allowClear: vm.allowClear, placeholder: placeholder })
                    .val(vm.value)
                    .trigger("change.select2")
                    .on("change.select2", function (e) {
                        let id = $(vm.$el).find("select").val();

                        if (id) {
                            if (vm.typeValueString) {
                                vm.$emit("input", id);
                            } else {
                                vm.$emit("input", +id);
                            }
                        } else {
                            vm.$emit("input", null);
                        }
                    });
            }

            var selected = _.find(vm.elItems, function (item) {
                if (item.Value == vm.value) {
                    return item;
                }
            });

            vm.viewValue = selected ? selected.Text : "";
        },
        getItemsToSelect(items) {
            return items;
            // if (items) {
            //     return items.filter(function canBeSelected(value) {
            //         return !value.CanNotBeSelected;
            //     });
            // }
        },
    },
    computed: {
        isEmptyValue() {
            return this.IsRequired && !this.value ? true : false;
        },
    },
};
</script>
