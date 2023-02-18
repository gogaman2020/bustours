<template>
    <div class="chat-tab">
        <chat :bindingObjectId="procedureData.ProcedureInfo.Id" :stepsGuid="stepsGuid" :permissions="permissions"/>
    </div>
</template>

<script>

    import chat from './chat/chat'

    export default {
        name: 'chat-tab',
        components: {
            chat
        },
        props: {
            procedureData: {
				type: Object,
				required: true
            },
            stepsGuid: Object,
            permissions: null
        },
        mounted() {

            
        },
        data() {
            return {
                // style: style
            }
        },
    }
</script>