<template>
    <div class="portlet box otcSubBox" v-show="visible">
        <div class="portlet-title" onclick="Common.onFormBoxCaptionClick(this)">
            <div class="caption font-white">
                <span class="caption-subject bold uppercase">{{title}}</span>
            </div>
            <div class="tools" onclick="Common.onFormBoxToolsClick()">
                <a href="javascript:;" class="expandCollapseBtn collapse" title="" data-original-title=""></a>
            </div>
        </div>
        <div class="portlet-body portlet-collapsed" style="display: block;">
            <chat :bindingObjectId="bindingObjectId" :stepsGuid="stepsGuid" :permissions="permissions" ref="chat" />
        </div>
    </div>
</template>

<script>

    import chat from './chat/chat'

    export default {
        name: 'chat-box',
        components: {
            chat
        },
        props: {
            stepsGuid: Object,
            bindingObjectId: null,
            title: null,
            permissions: null
        },
        created: function () {
            let self = this;
            this.eventBus.$on('chat-box-visible', (data) => {
                //debugger;
                this.visible = self.$refs.chat.getVisible();
            });
        },
        beforeDestroy: function () {
            this.eventBus.$off('chat-box-visible');
        },
        data() {
            return {
                visible: false,
                eventBus: window.eventBus,
            }
        },
        methods: {
        }
    }
</script>