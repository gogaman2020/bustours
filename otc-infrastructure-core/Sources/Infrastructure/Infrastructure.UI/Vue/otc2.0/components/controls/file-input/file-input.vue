<template>
  <div :class="[style.customFileInput, style.customSmartFile] ">
    <div :class="[style.customFileInputFileList]">
        <ul>
            <template v-for="(file, index) in files">
                <li :key="file.Guid">
                    <fileRow :file="file" :canDelete="canDelete" :canDownload="canDownload" @delete="deleteFile($event, index)" />
                </li>
            </template>
        </ul>
    </div>
    <fileRow v-if="canAdd" :file="{Name:'Прикрепить документ'}" :isAttach="true" :canDelete="false" :canDownload="false" @click="()=>$refs.fileInput.click()"/>
    <input :class="[style.customFileInputInput]" :id="'customFileLangHTML__' + (postfixId ? postfixId : '')" ref="fileInput" type="file" multiple @change="processFile($event)">
  </div>
</template>

<script>
    import style from './style.module.scss'
    import axios from 'axios'
    import buttonTransparent from 'OTC2.0/components/controls/buttons/button-transparent'
    import fileRow from './file-row/file-row'

    export default {
        name: 'file-input-2',
        components: {
            fileRow,
            buttonTransparent
        },
        props: {
            postfixId: String,
            files: Array,
            canAdd: {
                type: Boolean,
                defailt: true
            },
            canDelete: {
                type: Boolean,
                defailt: true
            },
            canDownload: {
                type: Boolean,
                defailt: true
            },
            uploadUrl: String
        },
        data() {
            return {
                style
            }
        },
        mounted() {
            if (this.fileName) {
                this.files = [];
                this.files.push({ Name: this.fileName, Content: "" });
            }
        },
        watch: {
            files: function(val){
                if(!val){
                    val = [];
                }
            }
        },
        methods: {
            processFile(event) {
                let component = this;
                let readers = [];
                let files = event.target.files;
                if (this.file_limit && this.file_limit <= component.files.length) {
                    this.deleteFile(event, 0);
                }

                for (let i = 0; i <= files.length; i++) {
                    readers[i] = new FileReader();
                    readers[i].addEventListener("load", function () {
                        if (component.uploadUrl) {
                            var data = new FormData();
                            data.append('file', files[i])
                            axios.post(component.uploadUrl,
                                data,
                                {
                                    contentType: false,
                                    processData: false,
                                    withCredentials: true
                                })
                                .then(data => {
                                    component.files.push({
                                        Name: files[i].name,
                                        Guid: data.data
                                    });
                                })
                                .catch(error => {
                                    console.log(error);
                                });
                        } else {
                            component.files.push({
                                Name: files[i].name,
                                Content: this.result.split(',')[1]
                            });
                        }
                    }, false);

                    if (files[i]) {
                        readers[i].readAsDataURL(files[i]);
                    }
                }
                this.$emit('input', this.files);
            },
            deleteFile(event, index) {
                var input = this.$refs.fileInput;
                input.type = 'text';
                input.type = 'file';
                this.files.splice(index, 1);
                this.$emit('input', this.files);
            },
            clearFiles: function(){
                this.files = [];
                $(this.$refs.fileInput).val('');
                this.$emit('input', this.files);
            }
        }
    }
</script>