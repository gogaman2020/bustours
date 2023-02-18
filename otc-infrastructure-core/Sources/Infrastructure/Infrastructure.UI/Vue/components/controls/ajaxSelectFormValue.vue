<template>
    <div v-if="!isHidden" class="row formGroupRow form-group  ">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{viewValue}}</span>
                </label>
                <div v-else>
                    <select class="select2 form-control  select2-hidden-accessible" data-live-search="true" data-size="10" :id="elName" :name="elName" tabindex="-1" aria-hidden="true">
                        <!-- <option v-for="(item) in values" :value="item.Value" :key="item.Value" selected>{{ item.Text }}</option> -->
                    </select>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';

    export default {
        name: 'AjaxSelectFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: Number,
            dataSelectedValue: String,
            callback: Function,
            url: String
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                viewValue: null,
                reinit: false
            }
        },
        mounted: function () {
            this.initSelect();
        },
        watch: {
            dataId: function (value) {
                $(this.$el)
                    .find('select')
      	            .val(value)
      	            .trigger('change')
            },
            viewMode: function(value) {
                this.reinit = true;
            }
        },
        updated: function() {
            if (this.reinit) {
                this.initSelect();
                this.reinit = false;
            }

            if(this.elValue &&  this.elValue > 0){
                $(this.$el).find('select').trigger('change');
            }
        },
        methods: {
            initSelect: function() {
                var vm = this;                
                $(vm.$el).find('select')
                    .select2({
                        language: 'ru',
                        allowClear: true,
                        placeholder: "Выберите",
                        ajax:{
                            delay: 500,
                            url: vm.url,
                            xhrFields:{
                                withCredentials: true
                            },
                            dataType: 'json',
                            crossDomain: true,
                            processResults: function (data) {
                                return {
                                    results: data.map(function(item) {
                                        return {
                                            id: item.Value,
                                            text: item.Text
                                        }
                                    })
                                };
                            }
                        }
                    })
                    .val(vm.elValue)
                    .trigger('change')
                    .on('change', function () {                        
                        var value = null;

                        var data = $(vm.$el).find('select').select2('data');
                        if(data !== undefined && data !== null && data.length > 0) {
                            value = data[0].id;
                        }
                        
                        vm.$emit('input', value);
                    });

                if(vm.elValue && vm.elValue > 0){
                    
                    var option = new Option(vm.dataSelectedValue, vm.elValue, true, true);
                    $(vm.$el).find('select').append(option).trigger('change');

                    vm.viewValue = vm.dataSelectedValue;
                }
            },
            getValidationSettings: function() {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не заполнено поле: "' + this.elLabel + '"',
                    isRequired: false,
                    isDropDown: true
                };
            }
        },
        destroyed: function () {
            //$(this.$el).find('select').off().select2('destroy');
        }
    }
</script>