<template>
	<modal v-if="show" @close="closeModal()">
		<template #header>{{header}}</template>

		<div class="container">
			<div class="row">
				<div class="col-md-12" id="modalcontent">{{message}}</div>
			</div>

			<div id="duplicatesList" class="row" style="overflow-y:auto; overflow-x:hidden; max-height:60px">

			</div>
		</div>

		<div :class="style.actionButtons">
			<button-blue type="button" href="javascript:void(0)" @click="closeModal()">Закрыть</button-blue>
		</div>
	</modal>
</template>

<script>
    import modal from 'EXT/components/common/modal/modal';
    import buttonBlue from 'EXT/components/controls/buttons/button-blue';
	import style from './style.module.scss'

	export default {
		name: 'message-modal',
		components:{
			buttonBlue,
			modal
		},
		props: {
			size: ''
		},
		data: function(){
			return {
				innerContent: this.content,
				eventBus: window.eventBus,
				header: '',
				content: '',
                options: null,
				callback: function () { },
				style: style,
				show: false,
				message: ''
			}
		},
		created: function(){
			this.eventBus.$on('show-message', this.listener);
		},
        beforeDestroy: function () {
            this.eventBus.$off('show-message', this.listener);
        },
		methods: {
			closeModal: function(){
				this.show = false;
				this.message = '';
                this.callback();
            },
            listener: function (options) {
				this.options = options;
				this.header = options.header;
				if (typeof (options.callback) == "function") {
					this.callback = options.callback;
				} else {
					this.callback = function () { };
				}
				this.message = options.message;
				this.show = true;
            }
		}
	}
</script>