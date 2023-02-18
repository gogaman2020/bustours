<template>
    <div id="documentsgrid" class="vue-app">
        <kendo-datasource ref="documentDataSource"
                            :transport-read-url="readUrl"
                            :transport-read-data-type="'jsonp'"
                            :transport-parameter-map="parameterMap"
                            :schema-model-fields="schemaModelFields"
                            :schema="schemaDataConf"
                            :server-sorting="true"
                            :server-filtering="true"
                            :server-grouping="true"
                            :groupable="true"
                            :filter="filterConf"
                            :sort="sortConf"
                            :group="groupsConf"
                            :aggregate="[]">
        </kendo-datasource>

        <kendo-grid ref="documentKendoGrid"
                    :data-source-ref="'documentDataSource'"
                    :max-height="350"
                    :pageable="pagableConf">

            <kendo-grid-column :field="'DictionaryTypeName'"
                                :title="'Тип словаря'"
                                :template="'dictionaryTypeTemplate'"
                                :group-header-template="dictionaryTypeGroupHeaderTepmlate"
                                :width="1">
            </kendo-grid-column>

            <kendo-grid-column :field="'DocumentType'"
                                :title="'Тип документа'">
            </kendo-grid-column>

            <kendo-grid-column :field="'DocumentName'"
                                :title="'Название документа'"
                                :template="fileUrlTemplate">
            </kendo-grid-column>

            <kendo-grid-column :field="'CreateDate'"
                                :title="'Дата добавления'">
            </kendo-grid-column>

            <kendo-grid-column :field="'Description'"
                                :title="'Примечание'">
            </kendo-grid-column>
            <kendo-grid-column :field="'UserCreatorFio'"
                                :title="'Кто прикрепил'">
            </kendo-grid-column>
        </kendo-grid>
    </div>
</template>

<script>
    import axios from 'axios';
    

    export default {
        name: 'DocumentsGrid',
        components: {
        },
        props: {
            objectId: Number,
            objectType: Number,
            nodeKey: String,
            isMrg: Boolean
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                fileServiceUrl: process.env.VUE_APP_FILESERVICE_URL,
                schemaModelFields: {
                    DocumentId: { type: 'number' },
                    DocumentName: { type: 'string' },
                    DocumentType: { type: 'string' },
                    DocumentFileGuid: { type: 'string' },
                    DocumentFileName: { type: 'string' },
                    Description: { type: 'string'},
                    CreateDate: { type: 'string' }
                },
                sortConf: [
                    { field: 'DocumentId', dir: 'asc' }
                ],
                schemaDataConf: {
                    data: "Items",
                    groups: "Groups",
                    total: "Total"
                },
                pagableConf: {
                    refresh: true,
                    numeric: false,
                    pageSizes: false,
                    previousNext: false,
                    info: false,
                    pageSizes: [],
                },
                filterConf: {
                    ObjectId: this.objectId,
                    ObjectType: this.objectType,
                    NodeKey: this.nodeKey,
                    isMrg: this.isMrg
                },
                groupsConf: [
                    {field: "DictionaryTypeName" }
                ]
            }
        },
        created: function() {
        },
        computed: {
            readUrl: function() {
                return this.webApiUrl + "/Document/GetDocuments"
            }
        },
        mounted: function(){
        },
        methods: {
            parameterMap: function(options, operation) {
                return { request: JSON.stringify(options) };
            },
            fileUrlTemplate: function (e) {
                return '<a href="' + this.fileServiceUrl + '?FileGuid=' + e.DocumentFileGuid + '">' + e.DocumentFileName + '</a>';
            },
            dictionaryTypeGroupHeaderTepmlate: function(e) {
                return '<span>'+e.value+'</span>';
            },
            refresh: function(){
                this.$refs.documentDataSource.kendoDataSource.read();
            }
        }
    }

</script>