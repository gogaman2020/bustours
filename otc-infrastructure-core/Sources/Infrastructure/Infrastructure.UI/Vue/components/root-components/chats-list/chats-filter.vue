<template>
    <div class="portlet box otcSubBox">
        <div class="portlet-title" onclick="Common.onFormBoxCaptionClick(this)">
            <div class="caption font-white">
                <i class="fa fa-filter font-white"></i>
                <span class="caption-subject bold uppercase">Фильтр</span>
            </div>
            <div class="tools" onclick="Common.onFormBoxToolsClick()">
                <a href="javascript:;" class="expandCollapseBtn collapse" title="" data-original-title=""></a>
            </div>
        </div>
        <div class="portlet-body portlet-collapsed" style="display: block;">
            <div class="row">
                <div class="col-md-5">
                    <div class="row">
                        <div class="col-md-12">
                            <SelectFormValue :labelLen="6" elName="ProcessStatus" :elValue="filterModel.ProcessStatus"
                                             :elItems="processStatuses" elLabel="Статус процесса"
                                             @input="onPropChanged('ProcessStatus', $event)"/>
                        </div>
                    </div>
                </div>
                <div class="col-md-7">
                    <div class="row">
                        <div class="col-md-12">
                            <SelectFormValue :labelLen="6" elName="ProcessStatus" :elValue="filterModel.ObjectStatus"
                                             :elItems="objectStatuses" elLabel="Статус объета согласования"
                                             @input="onPropChanged('ObjectStatus', $event)"/>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <Checkbox v-model="filterModel.HasUnreaded" :labelLen="6" elName="HasUnreaded"
                                      @input="onPropChanged('HasUnreaded', $event)">
                                Есть непрочитанные сообщения
                            </Checkbox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 text-right cl-both">
                    <a class="btn btn-primary btn-filter" href="javascript:void(0)" @click="beginSearch()">Найти</a>
                    <a class="btn btn-default btn-filter" href="javascript:void(0)" @click="clearFilter()">Очистить</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import axios from 'axios';
    import SelectFormValue from './../../../components/controls/selectFormValue';
    import Checkbox from './../../../components/controls/checkbox/checkbox';

    import style from './style.module.scss';

    export default {
        name: 'ChatsFilter',
        components: {
            SelectFormValue,
            Checkbox
        },
        props: {
            stepGuid: String,
            hideStatuses: Boolean,
            baseFilter: { type: Object, required: false }
        },
        data: function () {
            return {
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                siteUrl: process.env.VUE_APP_SITE_URL,
                filterModel: {
                    HasUnreaded: false,
                    ProcessStatus: null,
                    ObjectStatus: null,
                    Templete: this.baseFilter != null ? this.baseFilter.templete : 0,
                    ObjectType: this.baseFilter != null ? this.baseFilter.objectType : '',
                    StepGuids: this.baseFilter != null ? this.baseFilter.stepGuids : [],
                    ChatSubject: this.baseFilter != null ? this.baseFilter.chatSubject : null
                },
                processStatuses: [],
                objectStatuses: [],
                style: style
            }
        },
        created: function () {
            this.loadFilterData();
        },
        methods: {
            beginSearch: function () {
                const filter = this.getFilter();
                this.$emit('onSearch', filter);
            },
            clearFilter: function () {
                this.filterModel = {
                    HasUnreaded: null,
                    ProcessStatus: null,
                    ObjectStatus: null,
                    Templete: this.baseFilter.templete,
                    ObjectType: this.baseFilter.objectType,
                    StepGuids: this.baseFilter.stepGuids
                };
                this.beginSearch();
            },
            getFilter: function () {
                return this.filterModel;
            },
            onPropChanged: function (name, value) {
                this.filterModel[name] = value;
            },
            loadFilterData: function () {
                axios.get(this.webApiUrl + "/Chats/GetFilterInitData", { withCredentials: true })
                    .then((response) => {
                        if (response) {
                            this.processStatuses = response.data.processStatuses;
                            this.objectStatuses = response.data.objectStatuses;
                            this.beginSearch();
                        }
                    });
            }
        }
    }

</script>