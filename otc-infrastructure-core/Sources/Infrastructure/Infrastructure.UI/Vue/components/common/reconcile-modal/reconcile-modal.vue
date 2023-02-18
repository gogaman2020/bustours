<template>
    <div>
        <modal v-if="visible" @close="onClose">
            <template #header>
                {{titleMessage}}
            </template>
            <div class="border-light p-5 reconcile-modal">
                <textarea type="text" id="textInput" v-model="comment" :class="[style.formControl, style.mb4]" placeholder=""></textarea>
                <div :class="[style.inputGroup]">
                    <div :class="[style.customFile, style.mb4 ]">
                        <label for="fileUpload" :class="[style.customFileUpload]">
                            <i class="fa fa-cloud-upload"></i> Прикрепить файл
                        </label>
                        <input type="file" id="fileUpload" ref="filesInput" :class="style.file" @change="handleFileUpload">
                    </div>
                    <div :class="[style.correction]">
                        <checkbox v-if="visibleCorrection" v-model="correction" :labelLen="6" elName="needCorrectionPg"
                                  @input="onPropChanged('correction', $event)">
                            Требуется корректировка ПГ
                        </checkbox>
                    </div>
                    <ul class="list-group" style="margin-bottom:0!important">
                        <li class="list-group-item d-flex justify-content-between align-items-center" v-for="(file, key) in files" :key="key">
                            {{file.name}}
                            <span @click="removeFile(key)" :class="[style.modalClose]">&times;</span>
                        </li>
                    </ul>
                </div>
                <div :class="style.formActions">
                    <button-blue type="submit" @click="onConfirm">Ок</button-blue>
                </div>
            </div>
        </modal>
        <modal v-if="errorVisible" @close="onErrorClose">
            <template #header>
                Ошибка
            </template>
            <p>Необходимо оставить комментарий</p>
            <div class="modal-footer">
                 <a href="javascript:void(0)" class="btn btn-success" @click="onErrorClose">Oк</a>
            </div>
        </modal>
    </div>
</template>

<script>
    import modal from 'EXT/components/common/modal/modal';
    import buttonBlue from 'EXT/components/controls/buttons/button-blue';
    import style from './style.module.scss';
    import axios from 'axios';
    import checkbox from "EXT/components/controls/checkbox/checkbox";

	export default {
		name: 'reconcile-modal',
		components: {
            checkbox,
            modal,
            buttonBlue
		},
        props: {            
            visible: false,
            visibleCorrection: false,
            requiredComment: false,
            title: '',
            close: null,
		},
        data() {
			return {
                style: style,
                errorVisible: false,
                titleMessage: this.title,
                commentRequired: this.requiredComment,
                comment: '',
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                files: [],
                correction: false,
			}
		},
        created() {

		},
		methods: {
                onConfirm(e) {
                var result = {};
                result.success = true;
                result.comment = this.comment;
                if(this.visibleCorrection){
                    result.correction = this.correction;
                }
                if (this.commentRequired===false) {
                    result.files = this.files.map(function (item) {
                        return item.guid;
                    });
                    this.$emit('close');
                    this.onClose(result);
                }
                else {
                    if (this.comment.length < 1) {
                        this.errorVisible = true;
                        return;
                    }
                    else {
                        result.files = this.files.map(function (item) {
                            return item.guid;
                        });
                        this.$emit('close');
                        this.onClose(result);
                    }
                }
            },
            handleFileUpload(e) {
                var files = e.target.files || e.dataTransfer.files;
                var formData = new FormData();
                var file = files[0];
                formData.append('file', files[0]);
                axios.post(
                    this.webApiUrl + '/Files/Create',
                    formData,
                    { headers: { 'Content-Type': 'multipart/form-data' }, withCredentials: true }).then(result => {
                        this.files.push({ name: file.name, guid: result.data });
                        this.$refs.filesInput.value = '';
                    });
            },
            removeFile: function (key) {
                this.files.splice(key, 1);
            },
            onClose: function (result) {
                this.close(result);
                this.comment = '';
                this.files = [];
            },
            onErrorClose: function (result) {
                this.errorVisible = false;
            },
            onPropChanged: function (name, value) {
                this[name] = value;
            },
		},
	}
</script>