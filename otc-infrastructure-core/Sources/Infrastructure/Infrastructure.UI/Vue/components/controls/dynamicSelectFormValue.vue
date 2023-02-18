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
        name: 'DynamicSelectFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            dataId: Number,
            elValue: Number,
            dataSelectedValue: String,
            url: String
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                viewValue: null,
                reinit: false
            }
        },
        created: function() {

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
                        placeholder: "Выбрать",
                        ajax:{
                            delay: 500,
                            url: vm.url,
                            xhrFields:{
                                withCredentials: true
                            },
                            dataType: 'json',
                            crossDomain: true,
                            data: function (params) {
                                return {
                                    search: params.term,
                                    id: vm.dataId
                                };
                            },
                            processResults: function (data) {
                                return {
                                    results: data.map(function(item) {
                                        return {
                                            id: item.Id,
                                            text: item.Value
                                        }
                                    })
                                };
                            }
                        }
                    })
                    .val(vm.elValue)
                    .trigger('change')
                    .on('change', function () {                        
                        var params  = {
                            id: null,
                            text: ''
                        };

                        var data = $(vm.$el).find('select').select2('data');
                        if(data !== undefined && data !== null && data.length > 0){
                            var id = parseInt(data[0].id);
                            params.id = (isNaN(id) ? null : id);
                            params.text = data[0].text;
                        }
                        
                        vm.$emit('input', params);
                    });

                if(vm.elValue &&  vm.elValue > 0){
                    
                    var option = new Option(vm.dataSelectedValue, vm.elValue, true, true);
                    $(vm.$el).find('select').append(option).trigger('change');
                    // $(vm.$el).find('select').trigger({
                    //     type: 'select2:select',
                    //     params: {
                    //         data: {text:vm.dataSelectedValue, value:vm.elValue}
                    //     }
                    // });

                    vm.viewValue = vm.dataSelectedValue;
                }
            },
            getValidationSettings: function() {
                return {
                    el: this,
                    name: this.elName,
                    message: 'Не назначен ответственный: "' + this.elLabel + '"',
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