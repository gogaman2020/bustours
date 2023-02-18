<template>
    <div class="chat chat-container">
        <div class="padding-top-10">
            <div class="chat-body" ref="chatBody">
                <ul class="chats" data-x="chat/0/5030/messages" v-html="allMessages" v-show="allMessages"></ul>
            </div>
            <form data-x="chat/5030/message-form" v-show="showChat">
                <div class="chat-new-message-back">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <textarea v-model="message" data-x="chat/0/5030/message-text" class="chat-new-message form-control" placeholder="Введите текст..." rows="3"></textarea>
                        </div>
                    </div>
                </div>
                
                <div class="chat-control-panel">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="row">
                                <div class="col-md-6">
                                    
                                </div>
                                <div class="col-md-6">
                                    <button class="btn btn-primary pull-right" type="button" @click="sendMessage">Отправить</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>

<script>

    import { 
        getCurrentChats,
        getAllMessages,
        sendMessage,
        chatMicroserviceUrl,
        setChatMessageReaded
    } from './api';

    import './styles.css'

    export default {
        name: 'chat',
        components: {

        },
        props: {
            bindingObjectId: null,
            stepsGuid: Object,
            permissions: null
        },
        mounted() {
            this.getCurrentChats();
        },
        data() {
            return {
                eventBus: window.eventBus,
                AccessLevel: '',
                ChatId: '',
                BindingObjectId: this.bindingObjectId,
                StepId: this.stepsGuid != null && this.stepsGuid.StepGuid != null
                    ? this.stepsGuid.StepGuid
                    : this.stepsGuid.InProgressStepsGuid != null && this.stepsGuid.InProgressStepsGuid.length > 0
                        ? this.stepsGuid.InProgressStepsGuid[0]
                        : null,
                showChat: false,
                chatMicroserviceUrl: chatMicroserviceUrl,
                allMessages: '',
                message: '',
                //см. микросервис MM_Chat AccessLevel
                writeAccessLevel: 2,
                Permissions: this.permissions,
                readedTimer: '',
                viewChatId: this.$route.query.chatId
            }
        },
        methods: {
            getCurrentChats() {
                if (this.viewChatId != null) {
                    this.showChat = false;
                    this.AccessLevel = 1;
                    this.ChatId = Number(this.viewChatId);

                    window._MMCHATID = this.viewChatId;

                    this.eventBus.$emit('chat-box-visible');
                    this.getAllMessages();
                }
                else
                {
                    let self = this;
                    const params = {
                        BindingObjectId: this.BindingObjectId,//1
                        StepId: this.StepId,//2
                        Permissions: this.Permissions
                    };
                    getCurrentChats(params)
                    .then(data => {
                        if(!data.IsSuccess || data.Chats.length == 0) {
                            console.log('произошла ошибка -', data.errorMessage, 'или чаты не найдены')
                            return false
                        }                    
                    
                        self.showChat = data.Chats[0].AccessLevel == this.writeAccessLevel;

                        self.AccessLevel = data.Chats[0].AccessLevel;
                        self.ChatId = data.Chats[0].ChatId;

                        window._MMCHATID = this.ChatId;

                        this.eventBus.$emit('chat-box-visible');

                        self.getAllMessages();
                    })
                    .catch(error => {
                        console.log(error)
                    })
                }
            }, 
            getVisible() {
                return this.AccessLevel >= 1;
            },
            getAllMessages() {
                debugger;
                let form_data = new FormData();

                form_data.append('ChatId', this.ChatId);

                getAllMessages(form_data)
                
                .then(data => {
                    if(!data.IsSuccess) {
                        console.log('произошла ошибка')
                        return false
                    }

                    this.allMessages = data.Data
                    this.scrollDown();

                    this.timer = setInterval(this.setChatMessagesReaded, 5000)

                })
                .catch(error => {
                    console.log(error)
                })
            },
            sendMessage() {
                let form_data = new FormData();

                form_data.append('Text', this.message);
                form_data.append('ChatId', this.ChatId);

                sendMessage(form_data)
                
                .then(data => {
                    if(!data.IsSuccess) {
                        console.log('произошла ошибка')
                        return false
                    }

                    this.message = '';
                    this.allMessages = this.allMessages + data.Data;

                    this.scrollDown();
                })
                .catch(error => {
                    console.log(error)
                })
            },
            scrollDown() {
                this.$nextTick(function(){
                    const chatBody = this.$refs.chatBody;
                    let height = chatBody.scrollHeight;

                    if (!height) {
                        height = 10000;
                    }

                    chatBody.scroll({
                        top: height,
                        left: 0
                    })
                });
            },
            setChatMessagesReaded() {
                setChatMessageReaded(this.ChatId)
                    .then(data => {
                        if (!data.IsSuccess) {
                            return false;
                        }
                        
                        clearInterval(this.timer)

                        return true;
                    })
                    .catch(error => {
                        console.log(error)
                    });
            }
        },
        beforeDestroy () {
            clearInterval(this.timer)
        }
    }
</script>