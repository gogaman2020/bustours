<template>
    <div>
        <kendo-upload ref="kupload"
                    name="file"
                    :files="getFiles"
                    :async-auto-upload="true"
                    :async-with-credentials="true"
                    :async-save-url="this.webApiUrl + '/Files/Upload'"
                    :async-remove-url="'ignore'"
                    :validation-allowed-extensions='extentionsFilter'
                    :localization="localizationConf"
                    :multiple="multiple"
                    @success="onSuccess"
                    @remove="onRemove">
        </kendo-upload>

        <modal v-if="message" @close="onCancel">
            <template #header>
                Удаление файла
            </template>
            {{message}}
            <div class="row">
                <div class="col-md-9">
                </div>
                <div class="col-md-3">
                    <button-blue @click="onCancel">Ок</button-blue>
                </div>
            </div>
        </modal>
</div>
</template>

<script>
    import style from './style.module.scss'
    import modal from 'EXT/components/common/modal/modal';
    import buttonBlue from 'EXT/components/controls/buttons/button-blue';
    import {MimeTypeHelper} from 'EXT/utils/mimeTypeHelper.js'

    export default {
        name: 'file-upload',
        components: {
            modal,
            buttonBlue
        },
        props: {
            files: Array,
            possibleExtentions: Array,
            disallowDelete: Boolean,
            disallowDeleteMessage: String,
            multiple: Boolean
        },
        data() {
            return {
                style: style,
                innerFiles: this.files,
                extentionsFilter: this.possibleExtentions,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                fileServiceUrl: process.env.VUE_APP_FILESERVICE_URL,
                mimeTypeHelper: new MimeTypeHelper(),
                message: null,
                localizationConf: {
                    cancel: 'Отменить загрузку',
                    clearSelectedFiles: 'Удалить выбранные файлы',
                    dropFilesHere: 'перетащите сюда файлы для загрузки',
                    headerStatusUploaded: 'Готово',
                    headerStatusUploading: 'Загружается...',
                    invalidFileExtension: 'Недопустимое расширение файла',
                    invalidFiles: 'Ошибка. Пожалуйста, проверьте требования загрузки файла.',
                    invalidMaxFileSize: 'Файл слишком большой ',
                    invalidMinFileSize: 'Файл слишком маленький',
                    remove: 'Удалить',
                    retry: 'Повторить',
                    select: 'Выбрать',
                    statusFailed: 'загрузка прервана',
                    statusUploaded: 'загружен',
                    statusUploading: 'загружается',
                    uploadSelectedFiles: 'Загрузить выбранные файлы'
                }
            }
        },
        mounted() {
            if (!this.innerFiles) {
                this.innerFiles = [];
            }

            var upload = this.$refs.kupload.kendoWidget();
            upload._submitRemove = function (fileNames, eventArgs, onSuccess, onError) {
                onSuccess();
            };
            
            this.fillMimeTypes();
        },
        computed:{
            getFiles: function(){
                return _.filter(this.innerFiles, function(file){
                    return file.IsDeleted == false;
                });
            }
        },
        methods: {
            clearFiles: function(){
                this.innerFiles = [];
                $(this.$refs.fileInput).val('');
                this.$emit('input', this.innerFiles);
            },
            onRemove: function(e) {
                let files = e.files;
                let self = this;
                let loadedExists = false;
                files.forEach(function(file){
                    if (file.Id  && file.Id != 0){
                        loadedExists = true;
                    }
                });

                if (self.disallowDelete && loadedExists) {
                    if (self.disallowDeleteMessage) {
                        self.message = self.disallowDeleteMessage;
                    } else {
                        self.message = 'Удаление загруженных файлов запрещено';
                    }

                    e.preventDefault();
                }
            },
            onSuccess: function(e) {
                var files = e.files;
                var self = this;
                if(!self.innerFiles || self.innerFiles === null){
                    self.innerFiles = [];
                }

                if (e.operation === 'upload') {
                    files.forEach(function(file){
                        $.extend(file, { 
                            Id: 0,
                            Guid: e.response,
                            IsDeleted: false
                        });

                        self.innerFiles.push({
                            Guid: e.response,
                            Name: file.name,
                            IsDeleted: false
                        });
                    });
                } else if (e.operation === 'remove') {
                    files.forEach(function(file){
                        var exists = _.find(self.innerFiles, function(innerFile) {
                            if(innerFile.Guid == file.Guid){
                                return innerFile;
                            }
                        });

                        if (exists) {
                            exists.IsDeleted = true;
                        }
                    });
                }
                
                this.$emit('input', this.innerFiles);
            },

            onCancel: function() {
                this.message = null;
            },

            fillMimeTypes: function() {
                var mimeTypes = this.mimeTypeHelper.getMimeTypes(this.possibleExtentions);
                if (mimeTypes) {
                    $(this.$refs.kupload.$el).attr('accept', mimeTypes);
                }
            }
        }
    }
</script>