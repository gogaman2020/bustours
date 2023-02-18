<template>
    <modal v-if="visible" @close="onClose">
        <template #header>
            {{titleMessage}}
        </template>
        <div class="border-light p-5">            
            <ResponsiblePanel v-if="thereAreWorkGroups()" :panelModel="panelModel" :url="webApiUrl + '/WorkGroups/GetUsersByWorkGroup'" />   
            <div v-if="!thereAreWorkGroups()" :class="[style.noGroups]">
                <span>Нет доступных рабочих групп</span>             
            </div>
            <div :class="style.formActions">
                <button-blue v-if="thereAreWorkGroups()" type="submit" @click="onConfirm">Изменить</button-blue>
                <button-green  v-if="!thereAreWorkGroups()" type="button" @click="onClose">Отмена</button-green>
            </div>
        </div>
    </modal>
</template>

<script>
    import modal from 'EXT/components/common/modal/modal';
    import buttonBlue from 'EXT/components/controls/buttons/button-blue';
    import buttonGreen from 'EXT/components/controls/buttons/button-green'
    import style from './style.module.scss';
    import axios from 'axios';
    import ResponsiblePanel from 'EXT/components/controls/responsiblePanel';

	export default {
		name: 'change-responsibles-modal',
		components: {
            modal,
            ResponsiblePanel,
            buttonBlue,
            buttonGreen
		},
        props: {            
            visible: false,
            title: '',
            close: null,
            ids: null,
		},
        data() {
			return {
                style: style,
                titleMessage: this.title,
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL
			}
        },
        created() {
            this.loadResponsiblesModel();
		},
		methods: {
            onConfirm(e) {
                var result = {};
                result.success = true;
                this.$emit('close');                
                this.onChange();
                this.onClose(result);
            },
            onClose: function (result) {
                this.close(result);
                this.loadResponsiblesModel();
            },
            onChange() {
                var body = { 
                    procedureIds: this.ids,
                    responsibleUsers: this.panelModel.ResponsibleUsers 
                };
                var config = { withCredentials: true };
                axios.put(this.webApiUrl + '/responsibles', body, config)
                .then(result => {
                    self.eventBus.$emit('show-message', { header: 'Внимание', message: 'Ответственные успешно изменены' });
                })
                .catch(message => {
                    self.eventBus.$emit('show-message', { header: 'Ошибка', message: 'Не удалось изменить отвественных у выбранных процедур' });
                });
            },
            thereAreWorkGroups(){
                return this.panelModel && this.panelModel.ResponsibleUsers && this.panelModel.ResponsibleUsers.length != 0;
            },
            loadResponsiblesModel() {
                var self = this;
                var config = { withCredentials: true };
                axios.get(this.webApiUrl + '/responsibles/GetUsersAvailableWorkGroups', config)
                    .then(result => {
                        this.panelModel = {
                            ResponsibleUsers: result.data
                        };
                    })
                    .catch(message => {
                        self.eventBus.$emit('show-message', { header: 'Ошибка', message: 'Не удалось загрузить рабочие группы пользователя' });
                    });
            }
		},
	}
</script>