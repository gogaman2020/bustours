<template>
    <div :class="[style.fileRow]">
        <div :class="[style.fileRowLabel]">
            <i :class="[getIconClass()]" @click="()=>$emit('click')"></i>
            <span>{{file.Name}}</span>
        </div>
        <div :class="[style.fileRowAction]">
            <i v-if="canDownload" :class="[style.fileRowActionDownload]" @click="onDownload"></i>
            <i v-if="canDelete" :class="[style.fileRowActionDelete]" @click="onDelete"></i>
        </div>
        <a ref="dload" :class="[style.fileRowA]" :href="getUrl()"/>
    </div>
</template>

<script>
    import style from './file-row.module.scss'

    export default {
        name: 'file-row',
        components: {
        },
        props: {
            canDelete: {
                type: Boolean,
                default: true
            },
            canDownload: {
                type: Boolean,
                default: true
            },
            isAttach: {
                type: Boolean,
                default: false
            },
            file: {
                type: Object,
                default: {
                    Name: '',
                    Guid: '',
                    Id: 0
                }
            }
        },
        data() {
            return {
                style,
                
                fileServiceUrl: process.env.VUE_APP_FILESERVICE_URL,
            }
        },
        mounted() {
            if (this.fileName) {
                this.files = [];
                this.files.push({ Name: this.fileName, Content: "" });
            }
        },
        methods: {
            getIconClass: function() {
                if(this.isAttach){
                    return style.fileRowLabelAttach;
                }
                
                return style.fileRowLabelNone;
            },
            onDelete: function() {
                this.$emit('delete', this.file)
            },
            onDownload: function(){
                this.$refs.dload.click();
            },
            getUrl: function(){
                return this.fileServiceUrl + '?FileGuid=' + this.file.Guid;
            }
        }
    }
</script>