<template>
    <div class="row">
        <div class="col-md-12">
            <ChatsFilter ref="chatsFilter" @onSearch="onSearch" :baseFilter="baseFilter"></ChatsFilter>
            <ChatsGrid ref="chatsGrid"></ChatsGrid>
        </div>
    </div>
</template>

<script>
    import ChatsFilter from './chats-filter';
    import ChatsGrid from './chats-grid';

    export default {
        name: 'ChatList',
        components: {
            ChatsFilter,
            ChatsGrid,
        },
        props: {
            templete: { type: String, required: false },
            objectType:{ type: Number, required: false }, 
            stepGuids: { type: Array, required: false }
        },
        data: function () {
            return {
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                siteUrl: process.env.VUE_APP_SITE_URL,
                baseFilter: {
                    templete: this.$route.query.templete,
                    objectType: this.$route.query.objectType,
                    stepGuids: this.$route.query.stepGuids,
                    chatSubject: this.$route.query.chat
                }
            }
        },
        methods: {
            onSearch: function(filter) {
                this.$refs.chatsGrid.search(filter);
            }
        }
    }
</script>