<template>
    <div v-if="!IsHidden" class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']">
            <span class="originalText">{{ label }}</span>
        </label>

        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="IsDisabled" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{ viewValue }}</span>
                </label>
                <div v-else>
                    <select class="select2 form-control select2-hidden-accessible" data-live-search="true" data-size="10" :id="elName" :name="elName" tabindex="-1" aria-hidden="true"></select>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { FormValueCalcMixin } from "EXT/components/mixins/formValueCalcMixin";
import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

export default {
    name: "ajax-select-form-value",
    mixins: [FormValueCalcMixin, ContractFormSettingsMixin],
    props: {
        label: String,
        value: [Number, String],
        textSelectedValue: String,
        url: String,
        params: {
            type: Object,
            default: () => ({}),
        },
        typeValueString: {
            type: Boolean,
            default: false,
        },
    },
    data() {
        return {
            viewValue: null,
            reinit: false,
        };
    },
    mounted() {
        this.initSelect();

        if (this.value) {
            this.viewValue = this.textSelectedValue;
            var option = new Option(this.textSelectedValue, this.value, true, true);
            $(this.$el).find("select").append(option).trigger("change");
        }
    },
    watch: {
        value: function (value) {
            $(this.$el).find("select").val(value).trigger("change");
        },
        IsDisabled: function () {
            this.reinit = true;
        },
    },
    updated: function () {
        if (this.reinit) {
            this.initSelect();
            this.reinit = false;
        }
    },
    methods: {
        // https://select2.org/
        initSelect: function () {
            var vm = this;
            $(vm.$el)
                .find("select")
                .select2({
                    language: "ru",
                    allowClear: true,
                    placeholder: "Не выбрано",
                    ajax: {
                        delay: 500,
                        url: vm.url,
                        data: function (params) {
                            var query = {
                                phrase: params.term,
                                ...vm.params,
                            };
                            return query;
                        },
                        xhrFields: {
                            withCredentials: true,
                        },
                        dataType: "json",
                        crossDomain: true,
                        processResults: function (data) {
                            return {
                                results: data.map(function (item) {
                                    return {
                                        id: item.Value,
                                        text: item.Text,
                                    };
                                }),
                            };
                        },
                    },
                })
                .val(vm.value)
                .trigger("change")
                .on("change", function () {
                    let value = null;
                    const data = $(vm.$el).find("select").select2("data");

                    if (data.length > 0) {
                        vm.viewValue = data[0].text;
                        value = data[0].id;

                        if (vm.typeValueString) {
                            vm.$emit("input", value);
                        } else {
                            vm.$emit("input", +value);
                        }
                    } else {
                        vm.viewValue = null;
                        vm.$emit("input", null);
                    }
                });
        },
    },
    destroyed: function () {
        // $(this.$el).find('select').select2('destroy');
    },
};
</script>
