<template>
    <div v-if="!isHidden" class="row formGroupRow form-group  ">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <label v-if="isView" class="formFieldLabel form-control form-control__disabled">
                    <span class="originalText">{{viewValue}}</span>
                </label>
                <div v-else-if="grouped">
                    <select class="select2 form-control  select2-hidden-accessible" :multiple="multiple" data-live-search="true" data-size="10" :id="elName" :name="elName" tabindex="-1" aria-hidden="true">
                        <template v-for="(item, index) in elItems">
                            <template v-if="item.Children">
                                <optgroup :label="item.Text" :key="index">
                                    <option v-for="(child) in item.Children" :value="child.Value" :key="child.Value">{{ child.Text }}</option>
                                </optgroup>
                            </template>
                            <template v-else>
                                <option :value="item.Value" :key="item.Value">{{ item.Text }}</option>
                            </template>
                        </template>
                        
                    </select>
                </div>
                <div v-else>
                    <select class="select2 form-control  select2-hidden-accessible" :multiple="multiple" data-live-search="true" data-size="10" :id="elName" :name="elName" tabindex="-1" aria-hidden="true">
                        <option v-for="(item) in elItems" :value="item.Value" :key="item.Value">{{ item.Text }}</option>
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
        name: 'SelectFormValue',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            elLabel: String,
            elValue: Number,
            elList: Array,
            elItems: Array,
            placeholder: String,
            multiple: {
                type: Boolean,
                default: false,
                required: false
            },
            grouped: {
                type: Boolean,
                default: false,
                required: false
            },
            allowClear: {
                type: Boolean,
                default: true,
                required: false
            },

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
            elValue: function (value) {
                $(this.$el)
                    .find('select')
      	            .val(value)
      	            .trigger('change')
            },
            elItems: function (options) {
                this.reinit = true;
            },
            viewMode: function(value) {
                this.reinit = true;
            }
        },
        updated: function(){
            if(this.reinit){
                this.initSelect();
                this.reinit = false;
            }
        },
        methods: {
            initSelect: function (){
                var vm = this;

                var placeholder = 
                    vm.placeholder ? 
                    vm.placeholder : 
                    typeof vm.placeholder == "undefined" ? "Выберите" : null;

                if(this.grouped) {
                    $(vm.$el)
                        .find('select')
                        .select2({language: 'ru', allowClear: vm.allowClear, placeholder: placeholder })
                        .val(vm.elList)
                        .trigger('change')
                        .on('change', function (event) {
                            vm.$emit('input', $(vm.$el).find('select').val());
                        });
                } else {
                    $(vm.$el)
                        .find('select')
                        .select2({language: 'ru', allowClear: vm.allowClear, placeholder: placeholder })
                        .val(vm.elValue)
                        .trigger('change')
                        .on('change', function () {
                            var id = parseInt($(vm.$el).find('select').val());
                            if(isNaN(id)){
                                id = null;
                            }
                            vm.$emit('input', id);
                        });
                }
                

                var selected = _.find(vm.elItems, function(item){
                    if(item.Value === vm.elValue){
                        return item;
                    }
                });

                vm.viewValue = selected ? selected.Text : '';
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