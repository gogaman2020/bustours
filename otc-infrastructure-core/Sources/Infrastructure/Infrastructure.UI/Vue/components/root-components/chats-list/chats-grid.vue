<template>
    <div id="chatsgrid" class="vue-app">
        <kendo-grid-columns-save :kendoGridRef="'procedureKendoGrid'" :skipColumnCount="1" :nodeKey="154" />
        <kendo-datasource ref="procedureDataSource"
                          :transport-read-url="readUrl"
                          :transport-read-data-type="'jsonp'"
                          :transport-parameter-map="parameterMap"
                          :schema-model-fields="schemaModelFields"
                          :schema="schemaDataConf"
                          :page-size='10'
                          :server-paging="true"
                          :server-sorting="true"
                          :server-filtering="true"
                          :filter="filterConf"
                          :sort="sortConf">
        </kendo-datasource>

        <kendo-grid ref="procedureKendoGrid"
                    :data-source-ref="'procedureDataSource'"
                    :pageable="pagableConf"
                    :reorderable="true"
                    :resizable="true"
                    :excel-file-name="'Процедуры закупок.xlsx'"
                    :excel-proxy-URL="'https://demos.telerik.com/kendo-ui/service/export'"
                    :max-height="350"                                                            
                    :column-menu="true"
                    v-on:change="onChange">

            <kendo-grid-column :field="'ObjectType'"
                               :title="'Тип объекта'"
                               :width="100">
            </kendo-grid-column>

            <kendo-grid-column :field="'ProcessId'"
                               :title="'Маршрут согласования'"
                               :template="routeUrlTemplate"
                               :width="140">
            </kendo-grid-column>

            <kendo-grid-column :field="'ChatType'"
                               :title="'Тип чата'"
                               :template="chatNameUrlTemplate"
                               :width="100">
            </kendo-grid-column>

            <kendo-grid-column :field="'ObjectName'"
                               :title="'Номер объекта'"
                               :template="numberUrlTemplate"
                               :width="120">
            </kendo-grid-column>

            <kendo-grid-column :field="'ObjectStatus'"
                               :title="'Статус объекта'"
                               :width="100">
            </kendo-grid-column>

            <kendo-grid-column :field="'ProcessStatus'"
                               :title="'Статус процесса согласования объекта'"
                               :width="180">
            </kendo-grid-column>

            <kendo-grid-column :field="'MessagesCount'"
                               :title="'Число сообщений'"
                               :width="150">
            </kendo-grid-column>

            <kendo-grid-column :field="'UnreadedMessagesCount'"
                               :title="'Число непрочитанных сообщений'"
                               :width="150">
            </kendo-grid-column>
        </kendo-grid>
        
        <br />

        <a class="btn btn-default btn-filter" href="javascript:void(0)" @click="onExcelExport">Экспорт в Excel</a>
        <a class="btn btn-default btn-filter" href="javascript:void(0)" @click="navigator">Назад</a>
        <message-modal />
    </div>
</template>

<script>
    import axios from 'axios';
    import messageModal from './../../../components/common/message-modal/message-modal';
    import KendoGridColumnsSave from './../../../components/common/kendo-grid-columns-save/kendo-grid-columns-save';

    import style from './style.module.scss'

    export default {
        name: 'ChatsGrid',
        components: {
            messageModal,
            KendoGridColumnsSave
        },
        props: {
            permissions: Object
        },
        data: function () {
            return {
                style: style,
                grid: null,
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                siteUrl: process.env.VUE_APP_SITE_URL,
                reconcileUrl: process.env.VUE_APP_RECONCILE_URL,
                reconcileRoutePartUrl: process.env.VUE_APP_RECONCILE_PART_ROUTE_URL,
                tenderUrl: process.env.VUE_APP_TENDER_URL,
                schemaModelFields: {
                    ProcedureId: { type: 'number' },
                    Number: { type: 'number' },
                    ProcedureNumber: { type: 'number' },
                    ProcedureName: { type: 'string' }
                },
                sortConf: [
                    { field: 'Number', dir: 'asc' }
                ],
                schemaDataConf: {
                    data: "Items",
                    total: "Count"
                },
                pagableConf: {
                    refresh: true,
                    pageSizes: [10, 20, 50, 100, 'All'],
                },
                filterConf: {
                },
                selectedIds: [],
                draftStatusId: 1,
                isSendToCoordinationButtonVisible: false,
                isChangeResponsibleButtonVisible: false,
                withdrawStatusIds: [undefined,2,3,4,5],
                isWithdrawButtonVisible: false,
                repeatStatusIds: [9],
                isRepeatButtonVisible: false,
                acceptDialogConf: {
                    visible: false,
                    title: "Заполните комментарий (необязательно)"
                },
                rejectDialogConf: {
                    visible: false,
                    title: "Заполните комментарий"
                },
                withdrawDialogConf: {
                    visible: false,
                    title: "Заполните комментарий (необязательно)"
                },
                changeResponsiblesDialogConf: {
                    visible: false,
                    title: 'Изменить ответственного'
                },
                showDigitalSign: false,
                coordinationInfo: null,
                contentForSign: '',
                signedData: null,
                currentDesicion: '',
                currentCertificateThumbprint: '',
                useDigitalSignature: false,
                validatonResults: [],
                errors: []
            }
        },
        created: function () {
            this.eventBus = window.eventBus;
        },
        computed: {
            readUrl() {
                return this.webApiUrl + "/Chats/Get"
            }
        },
        mounted() {           
            this.getCurrentCertificateThumbprint();
        },
        methods: {
            search: function (filter) {
                this.$refs.procedureDataSource.kendoDataSource.filter(filter);
            },
            parameterMap: function (options, operation) {
                return { request: JSON.stringify(options) };
            },
            numberUrlTemplate: function (e) {
                var url = window.location.href.replace(/#.*/g, '#/edit/' + e.ObjectId + '?chatId=' + e.ChatId +'&showChatTab=true');
                return '<a href="' + url + '">' + e.ObjectId + '</a>';
            },
            chatNameUrlTemplate: function (e) {
                var url = window.location.href.replace(/#.*/g, '#/edit/' + e.ObjectId + '?chatId=' + e.ChatId +'&showChatTab=true');
                return '<a href="' + url + '">' + e.ChatType + '</a>';
            },
            routeUrlTemplate: function (e) {
                if(e.ProcessId != null) {
                    return '<a href="' + this.reconcileUrl + this.reconcileRoutePartUrl + e.ProcessId + '">' + e.ProcessName + '</a>';
                }
                return "";
            },
            onExcelExport: function (e) {
                const grid = this.$refs.procedureKendoGrid.kendoWidget();
                grid.saveAsExcel();
            },
            onChange: function (e) {
                return true;
            },
            navigator: function () {
                window.location.replace(this.tenderUrl + '/coordination/navigator/index');
            },
        }
    }

</script>