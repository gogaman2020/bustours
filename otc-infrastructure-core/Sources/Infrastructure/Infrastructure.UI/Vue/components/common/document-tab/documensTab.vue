<template>
    <div>
        <div id="documentsgrid" class="row">
            <div class="col-md-12">
                <DocumentsGrid ref="documentDataSource" :objectId="objectId" :objectType="objectType" :nodeKey="nodeKey" :isMrg="this.isMrg" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-2">
                <button-blue @click="showAddFileDialog">Добавить</button-blue>
            </div>
        </div>

        <modal v-if="showAddFile" @close="onCancel">
            <template #header>
                Прикрепить документ
            </template>

            <SelectFormValue ref="docTypes" elName="documentTypesSelector" elLabel="Тип документа"
                                v-bind:elList="docTypeHierarchy" @input="onDocumentTypeSelected"
                                :elItems="docTypeHierarchy" :elValue="selectedDocumentType" :grouped="true" :ignoreSettings='true'/>

            <TextFormValue elName="fileMessage" elLabel="Примечание"
                            :elMaxLen="2000" :elValue="fileMessage" :ignoreSettings='true' @input="onMessageChanged"/>

            <loading v-if="processing">
                Пожалуйста, подождите...
            </loading>

            <div v-if="documentInfo != null">
                <fileUpload ref="fileinput" @input="onFileSelected"
                        :possibleExtentions="documentInfo.PossibleExtentions"
                        :files="mapFiles(documentInfo.Files)"
                        :disallowDelete="disallowDelete"
                        :disallowDeleteMessage="disallowDeleteMessage"/>
            </div>

            <div class="row">
                <div class="col-md-6">
                </div>
                <div class="col-md-3">
                    <button-blue @click="onConfirm">Ок</button-blue>
                </div>
                <div class="col-md-3">
                    <button-blue @click="onCancel">Отмена</button-blue>
                </div>
            </div>
        </modal>

        <loading v-if="processing">
            Пожалуйста, подождите...
        </loading>
    </div>
</template>

<script>
    import axios from 'axios';
    import DocumentsGrid from './documentsGrid'
	import SelectFormValue from 'EXT/components/controls/selectFormValue';
	import buttonBlue from 'EXT/components/controls/buttons/button-blue';
    import modal from 'EXT/components/common/modal/modal';
    import TextFormValue from 'EXT/components/controls/textFormValue';
    import loading from 'EXT/components/common/loading/loading';
    import fileUpload from 'EXT/components/controls/file-upload/file-upload';
    

    export default {
        name: 'DocumensTab',
        components: {
            DocumentsGrid,
			SelectFormValue,
			buttonBlue,
            modal,
            TextFormValue,
            loading,
            fileUpload
        },
        props: {
            objectId: Number,
            objectType: Number,
            title: String,
            processId: Number,
            stepGuid: String,
            nodeKey: String,
            extentionsNodeKey: String,
            disallowDelete: Boolean,
            disallowDeleteMessage: String,
            isMrg: Boolean
        },
        data: function() {
            return {
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                fileServiceUrl: process.env.VUE_APP_FILESERVICE_URL,
                tenderUrl: process.env.VUE_APP_TENDER_URL,
                lotPartUrl: process.env.VUE_APP_LOT_PART_URL,

                fileData: {
                    tag: null,
                    type: null
                },
                
                groups: null,
                processing: false,
                docTypeHierarchy: null,
                showAddFile: false,
                files: [],
                selectedDocumentType:null,
                fileMessage: null,

                documentInfo: null
            }
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
            showAddFileDialog: function(){
                var self = this;
                self.clearSelection();
                self.loadHierarchy(function(){
                    self.showAddFile = true;
                });
            },
            onDocumentTypeSelected: function(data){
                var val = parseInt(data);
                if(isNaN(val)){
                    val = null;
                }
                if(this.selectedDocumentType != val) {
                    this.selectedDocumentType = val;
                    this.requestDocumentTypeDescription(this.selectedDocumentType);
                }
            },
            onCancel: function(){
                this.clearSelection();
                this.showAddFile = false;
            },
            clearSelection: function(){
                this.documentInfo = null;
                this.selectedDocumentType = null;
                this.files = [];
                this.fileMessage = null;
            },
            onConfirm:function(){
                if (!this.selectedDocumentType) {
                    this.eventBus.$emit('show-message', { header: 'Ошибка', message: 'Необходимо выбрать тип документа' });
                    return;
                }

                if (this.files.length === 0) {
                    this.eventBus.$emit('show-message', { header: 'Ошибка', message: 'Необходимо выбрать хотябы один файл' });
                    return;
                }

                var self = this;
                var items = this.files.map(function(file){
                    return {
                        Id: file.Id,
                        Name: file.Name,
                        Content: file.Content,
                        Tag: self.selectedDocumentType + 1000,
                        Type: self.selectedDocumentType,
                        Description: self.fileMessage,
                        Guid: file.Guid,
                        IsDeleted: file.IsDeleted,
                        CreatedDate: file.CreatedDate,
                        UserCreatorId: file.UserCreatorId
                    }
                });

                var model = {
                    ObjectId: self.objectId,
                    ObjectType: self.objectType,
                    Files: items
                };

                var self = this;
                $.ajax({
                    method: 'POST',
                    url: this.webApiUrl + '/Document/UploadFiles',
                    data: JSON.stringify(model),
                    contentType: 'application/json',
                    dataType: 'json',
                    xhrFields: {
                        withCredentials: true
                    },
                    success: function () {
                        self.processing = false;
                        self.clearSelection();

                        self.showAddFile = false;
                        self.$refs.documentDataSource.refresh();
                    },
                    error: function(jqXHR, textStatus, errorThrown){
                        self.processing = false;
                        if(jqXHR.responseText){
                            //var error = JSON.parse(jqXHR.responseText);
                            self.eventBus.$emit('show-message', { header: 'Ошибка', message: jqXHR.responseText });
                        } else {
                            self.eventBus.$emit('show-message', { header: 'Ошибка', message: errorThrown });
                        }
                        self.saving = null;
                    }
                });
            }, 
            onFileSelected: function(e){
                this.files = e;
            },
            onMessageChanged: function(e){
                this.fileMessage = e;
            },
            requestDocumentTypeDescription: function(selectedType) {
                var documentType = undefined;
                _.find(this.docTypeHierarchy, function(item){
                    var result = _.find(item.Children, function(subItem){
                        if(subItem.Value == selectedType){
                            return subItem;
                        }
                    });

                    if(result){
                        documentType = result;
                        return result;
                    }
                });

                var self = this;
                self.documentInfo = null;
                if (documentType) {
                    var model = { 
                        ObjectId: self.objectId,
                        ObjectType: self.objectType,
                        DocumentType: documentType.Value,
                        DocumentCode: documentType.Code,
                        NodeKey: self.extentionsNodeKey
                    };

                    $.ajax({
                        method: 'POST',
                        url: this.webApiUrl + '/Document/GetObjectDocuments',
                        data: JSON.stringify(model),
                        contentType: 'application/json',
                        dataType: 'json',
                        xhrFields: {
                            withCredentials: true
                        },
                        success: function (data) {
                            self.documentInfo = data;
                            if(self.documentInfo){
                                self.files = self.documentInfo.Files;
                            }
                        },
                        error: function(jqXHR, textStatus, errorThrown){
                            if(jqXHR.responseText){
                                //var error = JSON.parse(jqXHR.responseText);
                                self.eventBus.$emit('show-message', { header: 'Ошибка', message: jqXHR.responseText });
                            } else {
                                self.eventBus.$emit('show-message', { header: 'Ошибка', message: errorThrown });
                            }
                        }
                    });
                }
            },
            mapFiles: function(files){
                _.each(files, function(file){
                    var match = /\.[a-zA-Z0-9]+$/.exec(file.Name);
                    var ext = {
                        name: file.Name,
                        size: 0,
                        extension: (match && match.length > 0 ? match[0] : '')
                    };

                    $.extend(file, ext);
                });

                return files;
            },
            loadHierarchy: function(callback){
                var self = this;

                var url = self.webApiUrl + '/Document/GetDocumentTypeHierarchy?nodeKey=' + self.nodeKey + '&objectType=' + self.objectType;

                if(self.processId) {
                    url += '&processId=' + self.processId;
                }

                if (self.stepGuid) {
                    url += '&stepGuid=' + self.stepGuid;
                }            

                $.ajax({
                    method: 'GET',
                    url: url,
                    contentType: 'application/json',
                    dataType: 'json',
                    xhrFields: {
                        withCredentials: true
                    },
                    success: function (data) {
                        self.docTypeHierarchy = data;
                        if(callback && typeof(callback) === "function"){
                            callback();
                        }
                    },
                    error: function(jqXHR, textStatus, errorThrown){
                        if(jqXHR.responseText){
                            //var error = JSON.parse(jqXHR.responseText);
                            self.eventBus.$emit('show-message', { header: 'Ошибка', message: jqXHR.responseText });
                        } else {
                            self.eventBus.$emit('show-message', { header: 'Ошибка', message: errorThrown });
                        }
                    }
                });
            }
        }
    }

</script>