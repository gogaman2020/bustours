<template>
    <div v-if="!isHidden">
        <DynamicSelectFormValue v-for="(item) in panelModel.ResponsibleUsers"
            :key="item.Id"
            :dataId="item.WorkGroupId"
            :elValue="item.UserId"
            :dataSelectedValue="item.UserName"
            :elLabel="item.WorkGroupName"
            :elName="item.FormSettingsName"
            :additionalTester="onTestSettings" 
            :url="url" 
            @input="onPropChanged(item.FormSettingsName, $event)"/>
    </div>
</template>

<script>
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';
    import DynamicSelectFormValue from 'EXT/components/controls/dynamicSelectFormValue';

    export default {
        name: 'ResponsiblePanel',
        mixins: [FormValueSettingsMixin],
        components: {
            DynamicSelectFormValue
        },
        props: {
            panelModel: Object,
            url: String
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                name: 'ResponsibleUserPanel',
                testRegex: /^ResponsibleUserPanel(-(\d+))+$/
            }
        },
        methods: {
            getValidationSettings: function() {
                return null;
            },
            onTestSettings: function(el, settings) {
                if (this.testRegex.test(el.elName)) {
                    var searchId = el.elName.replaceAll('ResponsibleUserPanel-', '');
                    for(var key in settings) {
                        if (this.testRegex.test(key)) {
                            var ids = key.replaceAll('ResponsibleUserPanel-', '').split('-');
                            var id = _.find(ids, function(item){
                                if(item == searchId){
                                    return item;
                                }
                            });

                            if(id !== undefined){
                                return settings[key];
                            }
                        }
                    }
                } else {
                    return null;
                }
            },
            onPropChanged: function(name, params){
                var item = _.find(this.panelModel.ResponsibleUsers, function(item){
                    if(item.FormSettingsName === name){
                        return item;
                    }
                });

                if(item) {
                    item.UserId = params.id;
                    item.UserName = params.text;
                }
            }
        }
    }
</script>