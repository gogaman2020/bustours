<template>
	<div :class="s.booking">
		<h1>{{ step == OrderStep.Confirmation ? $t('bookingSummary.header') : $t("booking.header") }}</h1>

		<Stepper v-model="step" :steps="steps" :class="[s.bookingContainer, s[`bookingContainerStep${step}`]]">
			<Step :steps="[OrderStep.Form]" :class="s.bookingForm">
				<BookingForm :disabled="step != OrderStep.Form" />

				<div v-if="isGuestOverMax" :class="s.bookingFormErrorMessage">
					{{ $t("booking.widgetError.message") }}
					<nuxt-link :to="localePath('/contact-us')" :class="s.bookingFormErrorMessageLink">
						{{ $t("booking.widgetError.link") }}
					</nuxt-link>
				</div>		

				<div v-if="isDisabledGuestOverMax" :class="s.bookingFormErrorMessage">
					<div v-html='$t("booking.isDisabledGuestOverMax.before")' />
					<nuxt-link :to="localePath('/contact-us')" :class="s.bookingFormErrorMessageLink">
						{{ supportEmail }}
					</nuxt-link>					
					<span style="margin-left:0.2em">{{ $t("booking.isDisabledGuestOverMax.after") }}</span>
				</div>					

				<BbButton
					v-if="step == OrderStep.Form"
					:text="$t('buttons.continue')"
					:theme="ButtonTheme.Black" 
					:class="s.bookingFormNextStepButton"
					:disabled="!canProceed"
					@click="$emit('stepper-next')"
				/>
			</Step>

			<Step id="seats" :steps="[OrderStep.Seats]" :class="s.bookingSeats">
				<BookingSeats />
			</Step>
			
			<Step :steps="[OrderStep.Menu]" :class="s.bookingMenu">
				<BookingMenu />
			</Step>

			<Step :steps="[OrderStep.Menu]" :class="s.bookingMenuExtra">
				<BookingMenuExtra />
			</Step>

			<Step :steps="[OrderStep.Confirmation]" :class="[s.bookingSummary, s.bookingSummaryMobile]">
				<BookingSummary :without-title="true" />

				<div :class="s.bookingConfirmationButtons">
					<BackButton :theme="ButtonTheme.White" @click="$emit('stepper-prev')" />
					<BbButton 
						:text="$t('buttons.confirmAndPay')" 
						:theme="ButtonTheme.Black" 
						@click="$emit('stepper-next')" 
					/>
				</div>
			</Step>

			<Step :steps="s => getSummaryStepVisibility(s)" :class="s.bookingSummary">
				<BookingSummary />

				<BbButton 
					v-if="step == OrderStep.Payment" 
					:text="$t('buttons.changeBooking')" 
					:class="s.bookingSummaryButton"
					@click="$emit('stepper-prev')" 
				/>
			</Step>

			<Step :steps="[OrderStep.Payment]" :class="s.bookingPayment">
				<BookingPayment @click="$emit('stepper-prev')" />
			</Step>

			<Step :steps="[OrderStep.Receipt]">
				<OrderView :editing-mode="editingMode" @back-button-clicked="$emit('stepper-prev', 2)" />
			</Step>

			<div v-if="false">
				<div v-for="act in actions" :key="act">
					{{act}}
				</div>
			</div>

			<Step :steps="[OrderStep.Seats, OrderStep.Menu]" :class="[s.bookingButtons, s[`bookingButtonsStep${step}`]]" :breakpoints="['862px']">
				<BbButton :text="$t('buttons.back')" @click="$emit('stepper-prev')" />
				<BbButton :text="!isUpgradeMode ? $t('booking.changeOrUpgradeSeat') : $t('booking.exitUpgradeMode')" 
					v-if="canBeUpgraded && step == OrderStep.Seats"
					:theme="isUpgradeMode ? ButtonTheme.Black : ButtonTheme.White" 
					@click="toggleUpgradeMode()" 
				/>
				<BbButton :text="$t('buttons.confirmAndContinue')" :disabled="!canProceed" :theme="ButtonTheme.Black" @click="confirmAndContinueBtnHandler()" />

				<template #breakpoint862px>
					<BackButton :theme="ButtonTheme.White" @click="$emit('stepper-prev')" />
					<BbButton :text="!isUpgradeMode ? $t('booking.changeOrUpgradeSeat') : $t('booking.exitUpgradeMode')" 
						v-if="canBeUpgraded && step == OrderStep.Seats"
						:theme="isUpgradeMode ? ButtonTheme.Black : ButtonTheme.White"  
						@click="toggleUpgradeMode()" 
					/>
					<BbButton :text="$t('buttons.continue')" @click="$emit('stepper-next')" :disabled="!canProceed" />
				</template>
			</Step>
		</Stepper>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";
import { OrderStep } from '~/types/booking';
import { mapActions, mapGetters, mapMutations, mapState } from "vuex"
import config from "~/config"

import BookingForm from "./booking-form/booking-form.vue";
import BookingSummary from "./booking-summary/booking-summary.vue";
import BookingSeats from "./booking-seats/booking-seats.vue";
import BookingMenu from "./booking-menu/booking-menu.vue";
import BookingMenuExtra from "./booking-menu/menu-extra.vue";
import BookingPayment from "./booking-payment/booking-payment.vue";
import OrderView from "@/components/order/view/order-view.vue";

import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import BackButton from "~/components/controls/back-button/back-button.vue";
import Stepper, { IStep } from "~/components/stepper/stepper.vue";
import Step from "~/components/stepper/step.vue";

import { mapProps } from "@/store/helpers";

export default Vue.extend({
	name: "booking",

  	components: {
		BookingForm,
		BbButton,
		BackButton,
		BookingSummary,
		BookingSeats,
		BookingMenu,
		BookingMenuExtra,
		BookingPayment,
		OrderView,
		Stepper,
		Step
	},

	props: {
		editingMode: {
			type: Boolean,
			default: false,
		},
	},

	provide() {
		return {
			editingMode: this.editingMode,
		}
	},

	data() {
		return {
			s,
			ButtonTheme,
			OrderStep,
			canBeUpgraded: false,
			supportEmail: config.supportEmail ?? '',
			actions: [] as string[]
		}
	},

	created() {
		const params = {} as any;
		if (this.$route?.query?.guestCount) {
			params.guestCount = this.$route.query.guestCount;
			params.seatingType = this.$route.query.seatingType;
			this.$router.replace({query: {}});
		}
		this.resetOrder(params);
		this.resetPayment();
		this.actions = [];
		this.actions.push(`${this.actions.length}. current upgrade mode: ${this.isUpgradeMode}`);	
	},

	watch: {
		async step(val: OrderStep) {
			if (val == OrderStep.Seats) {
				(<any>this).createSchemeTables();
				await (<any>this).createOrUpdateOrderDraft();
				(<any>this).canBeUpgraded = true //await (<any>this).getCanBeUpgraded(this.id);
				await (<any>this).setSchemeTables(this.id);
			}
			if (val == OrderStep.Payment) {
				await (<any>this).createOrUpdateOrderDraft();
				await (<any>this).setForPayment();
			}
		},
		async order() {
			if ((<any>this).step > 0) {
				await (<any>this).getCalculationCostTour();
			}
		},
        '$route.hash'() {
            this.resetOrder();
			this.$emit('stepper-reset');
			this.routeId = this.routes[0]?.id;
        }		
	},
	
	computed: {
		steps(): IStep[] {
			const { Form, Seats, Menu, Confirmation, Payment, Receipt } = OrderStep;
			let steps: IStep[] = [Form, Seats, Menu, Confirmation, Payment, Receipt].map(s => ({ value: s }));

			steps[Confirmation].breakpoint = "862px";

			return steps;
		},
		...mapProps(
			[
				'id',
				"step",
				"routeId",
				"guestCount",
				"tourId",
				"seats",
				"disabledGuestCount"
			], 
			"booking", 
			"order", 
			"setOrder",
		),
		...mapGetters({
			isUpgradeMode: "booking/isUpgradeMode",
			schemeTables: "booking/schemeTables",
		}),	
		...mapState('tour', ['routes']),
		...mapState('booking', ['orderPromocode']),
		isGuestOverMax(): boolean {
			return this.guestCount > config.maxGuestCount;
		},	
		isDisabledGuestOverMax(): boolean {
			return this.disabledGuestCount > 1;
		},		
		notAllOccupied() {
			return this.step >= OrderStep.Seats && this.seats?.length != this.guestCount;
		},
		isBadPromocode(): boolean {
			return this.orderPromocode?.isBadPromoCode === true;
		},
		canProceed(): boolean {
			return !this.isGuestOverMax && this.tourId && !this.notAllOccupied && !this.isBadPromocode && !this.isDisabledGuestOverMax;
		},	
	},

	methods: {
		...mapActions('booking', ['createOrUpdateOrderDraft','setForPayment', 'getCalculationCostTour', 'getCanBeUpgraded', 'setSchemeTables']),
		...mapMutations("booking", ['setUpgradeMode', 'createSchemeTables', 'resetOrder','resetPayment']),
		confirmAndContinueBtnHandler() {
			if (this.editingMode) {
				if (this.step == OrderStep.Menu) {
					this.$emit("stepper-next", 2);
				}
			}

			this.$emit("stepper-next");
		},
		getSummaryStepVisibility(step: OrderStep): boolean {
			const statesWhenHidden = [OrderStep.Confirmation, OrderStep.Receipt];

			if (!this.editingMode) {
				statesWhenHidden.push(OrderStep.Form);
			}

			return !statesWhenHidden.includes(step);
		},
		toggleUpgradeMode() {
			this.actions.push(`${this.actions.length}. set upgrade mode: ${!this.isUpgradeMode}`);			
			(<any>this).setUpgradeMode(!this.isUpgradeMode);
			this.actions.push(`${this.actions.length}. current upgrade mode: ${this.isUpgradeMode}`);
		}
	}
})
</script>