<template>
  <div :class="[style.customFileInput, style.customSmartFile] ">
    <button-blue :class="style.customFileInputButton">
        <label :for="'customFileLangHTML' + postfixId" :class="[style.customFileInputLabel]">Выбрать</label>
    </button-blue>
    <input :class="[style.customFileInputInput]" :id="'customFileLangHTML' + postfixId" ref="fileInput" type="file" multiple @change="processFile($event)">
    <div :class="[style.customFileInputFileList]">
      <ul>
          <li v-for="file, index in files">
              <div :class="[style.customFileInputFileLabel]">
                  <span>{{file.Name}}</span>
                  <i @click="deleteFile($event, index)"></i>
              </div>
          </li>
      </ul>
    </div>
  </div>
</template>

<script>

  import style from './style.module.scss'


  import buttonBlue from 'EXT/components/controls/buttons/button-blue'

export default {
    name: 'file-input-2',
    components: {
        buttonBlue
    },
    props: {
        label_text: String,
        file_limit: Number,
        postfixId: String,
        fileName: String
    },
    data() {
        return {
            files: [],
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
        fileName(val) {
            this.files = [];
            this.files.push({ Name: val, Content: "" });
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
                    component.files.push({
                        Name: files[i].name,
                        Content: this.result.split(',')[1]
                    });
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