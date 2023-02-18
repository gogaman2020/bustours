<template>
	<div class="order">
		<h1 class="noPrint" v-if="!editingMode">{{ $t("order.view.header") }}</h1>

		<div class="order__summary">
			<div class="order__summary-title ">{{ $t("order.view.summaryTitle") }}</div>
			<BookingSummary :without-title="true" />

			<BbButton 
				class="order__summary-button noPrint" 
				:text="$t('order.view.print')" 
				:theme="ButtonTheme.Black" 
				@click="print"
			/>
		</div>

		<div class="order__email-inscription noPrint">{{ $t("order.view.emailText") }}</div>
		<TextInput 
			v-model="email"
			class="order__email-field noPrint"
			mode="view"
			variant="uppercase"
			:has-mode-toggler="true"
		/>

		<div class="order__email-inscription noPrint">{{ $t("order.view.serviceEmail") }}</div>
		<TextInput 
			value="test@mail.ru" 
			class="order__email-field noPrint"
			mode="view" 
			variant="uppercase"
			:has-mode-toggler="false" 
		/>

		<div v-if="successMessage" class="order__success-message noPrint">
			{{successMessage}}
		</div>

		<div v-if="order.id && isAdminOrSupervisor()" class="order__buttons noPrint">
			<BbButton v-if="editingMode" @click="$emit('back-button-clicked')">Back</BbButton>
			<BbButton v-if="editingMode" @click="saveOrder">Save changes</BbButton>
			<BbButton v-if="orderStateIs(OrderState.Paid)" @click="sendConfirmation">Send confirmation</BbButton>
			<BbButton v-if="orderStateIs(OrderState.WaitingForPayment)" @click="sendForPayment">Send for payment</BbButton>
			<a :href="buildOrderEditingPath()" target="_blank"><BbButton v-if="!editingMode && !orderStateIs(OrderState.Canceled)">Change</BbButton></a>
			<BbButton v-if="!orderStateIs(OrderState.Canceled)" @click="tryCancelOrder">Cancel</BbButton>
		</div>

		<Modal ref="Dialog">
			<template #footer="{returnFalse, returnTrue}">
				<BbButton @click="returnTrue()">Yes</BbButton>
				<BbButton @click="returnFalse()">No</BbButton>
			</template>
		</Modal>

		<Modal ref="Alert"></Modal>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapMutations, mapGetters } from "vuex";

import BookingSummary from "@/components/booking/booking-summary/booking-summary.vue";
import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import TextInput from "@/components/controls/text-input/text-input.vue";
import Modal from "@/components/controls/modal/modal.vue";

import { Roles } from "~/types/private";
import { Order, OrderState, OrderType } from "~/types/booking";

import config from "@/config/index";

export default Vue.extend({
	components: {
		BookingSummary,
		BbButton,
		TextInput,
		Modal,
	},

	props: {
		editingMode: {
			type: Boolean,
			default: false,
		},
	},

	data() {
		return {
			ButtonTheme,
			OrderState,
			successMessage: null as string | null
		}
	},

	computed: {
		...mapGetters("booking", [
			"order",
		]),
		email: {
			get(): string {
				return this.order?.client?.email ?? "";
			},
			set(value: string): void {
				this.setOrderClient({ email: value });
			}
		},
		currentUserRole(): Roles {
			return this?.$auth?.user?.role;
		},
		orderState(): OrderState {
			return (<Order>this.order).orderState;
		},
        lang(): string {
			return this.$i18n.locale
		},		
	},

	methods: {
		...mapMutations("booking", [
			"setOrder",
			"setOrderClient",
		]),
		...mapActions("booking", [
			"cancelOrder",
			"getOrder",
			"createOrUpdateOrderDraft",
			"createPrivateHireOrder",
			"addNotification"
		]),
		...mapActions("tour", [
			"cancelTour",
		]),
		isAdminOrSupervisor(): boolean {
			return [Roles.Administrator, Roles.Supervisor].includes(this.currentUserRole);
		},
		orderStateIs(orderState: OrderState): boolean {
			return this.orderState == orderState;
		},
		async sendConfirmation() {
			await this.addNotification({
				email: this.email, 
				lang: this.lang
			});

			this.successMessage = this.$t("order.view.successConfirmationSent").toString();
		},
		sendForPayment() {
			// TODO
		},
		saveOrder() {
			let operationResult;
			
			if (this.order.type == OrderType.PrivateHire) {
				operationResult = this.createPrivateHireOrder();
			} else {
				operationResult = this.createOrUpdateOrderDraft();
			}

			let message = "The order was successfully saved";
			let title = "Alert";

			operationResult
				.then(result => {
					if (!result) {
						message = "Something went wrong...";
					}

					(<any>this.$refs.Alert).open(message, title);
				});
		},
		async tryCancelOrder() {
			const decision = await (<any>this.$refs.Dialog).open("Are you sure?", "Confirmation");
		
			if (!decision) {
				return;
			}

			const orderId = (<Order>this.order).id;
			const result = await this.cancelOrder(orderId);
			let message = `The order №${orderId} was succesfully canceled`;
			let title = "Alert";

			if (!result) {
				message = "Something went wrong...";
			}

			
			const tourId = (<Order>this.order).tourId;
			if((<Order>this.order).type==OrderType.PrivateHire)
				await this.cancelTour(tourId);

			this.getOrder(orderId)
			
			await (<any>this.$refs.Alert).open(message, title);
		},
		buildOrderEditingPath(): string {
			return `${config.bustourSiteUrl}/order/edit/${this.order.hash}`;
		},
		print() {
			window.print();	
		},
	},
})
</script>

<style lang="scss" scoped>
	@import "/styles/mixins.scss";

	.order {
		&__summary {
			&-title {
				font-family: "Helvetica Neue";
				font-size: rem(18);
				line-height: em(27, 18);
				text-transform: uppercase;
				font-weight: 500;
				letter-spacing: em(1, 27);
			}

			&-button {
				margin-top: 40px;
				width: 100%;
			}

			margin-top: 25px;
			width: min-content;
		}

		&__email-inscription {
			font-family: "Helvetica Neue";
			font-size: rem(16);
			line-height: em(21, 16);
			letter-spacing: em(1, 16);
			text-transform: uppercase;
			margin-top: 40px;
		}

		&__buttons {
			display: flex;
			flex-wrap: wrap;
			column-gap: 20px;
			row-gap: 20px;
			margin-block-start: 40px; 
			writing-mode: horizontal-tb;
		}

		&__email-field {
			max-width: 400px;
			min-width: 100px;
		}

		&__success-message {
			color: $success-color;
			margin: 1em 0em;
		}
	}
</style>