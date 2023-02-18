<template>
	<div v-if="data" :class="s.summary">
		<InfoPanel :all-left-bold="true">
			<div>Number</div> 				<div>{{ data.tourNumber ? data.tourNumber : '' }}</div>
			<div>City</div> 				<div>{{ data.city[currentLocale] }}</div>
			<div>Tour type</div> 			<div>{{ data.tourType != undefined ? $t(`enums.TourType.${TourType[data.tourType]}`) : "" }}</div>
			<div>Itinerary</div> 			<div>{{ getItinerary() }}</div>
			<div>Status</div> 				<div :class="[s.blockCellTemplate, s[`blockCellTemplate${getTourStateBackgroundColor(data.tourState)}`]]">{{ data.tourState != undefined? $t(`enums.TourState.${TourState[data.tourState]}`) : "" }}</div>
			<div>Public booking</div>		<div>{{ yesNoTemplate(data.publicBookingBlock) }}</div>
			<div>Date</div> 				<div><DateTime :value="data.departureDateTime" :type="DisplayType.Date" :variant="'long'" /></div>
			<div>Departure time</div> 		<div><DateTime :value="data.departureDateTime" :type="DisplayType.Time" /></div>
			<div>Tour end</div> 			<div><DateTime :value="data.arrivalDateTime" :type="DisplayType.Time" /></div>
			<div>Duration</div> 			<div><Duration :value="data.duration" /></div>
			<div>Conflict</div> 			<div>{{ yesNoTemplate(data.conflict) }}</div>
			<div>Occupied</div>				<div>{{ getOccupied() }}</div>
			<div>Gifts</div> 				<div>{{ numberOrNoStringTemplate(data.usedGiftsCount) }}</div>
			<div>Promo</div> 				<div>{{ numberOrNoStringTemplate(data.userPromoCodesCount) }}</div>
			<div>Extras</div> 				<div>{{ yesNoTemplate(data.extras) }}</div>
			<div>Payment information</div> 	<div>{{ `Paid ${data.tourPaymentInformation}` }}</div>	
		</InfoPanel>

		<InfoPanel>
			<div data-bold>Total</div> 								<div data-bold><Currency :adjust-font="false" :value="getPriceWithVat()" /></div>
			<div data-shift>Including VAT {{ getVatRate() }}%</div> <div><Currency :value="getVatSum()" /></div>
		</InfoPanel>

		<div :class="s.summaryButtons">
			<NuxtLink :to="tourFBLink">
				<BbButton>
					TOUR F&amp;B
				</BbButton>
			</NuxtLink>

			<BbButton v-if="data.tourState != TourState.Canceled && data.tourType == TourType.Regular && (userRole == Roles.Supervisor || userRole == Roles.Administrator)" text="Cancel" @click="cancel" />
		</div>

		<div :class="s.summaryMessages">
			<div v-if="error" :class="s.summaryMessagesError">{{error}}</div>
			<div v-if="success" :class="s.summaryMessagesSuccess">{{success}}</div>			
		</div>	

		<Modal ref="Dialog">
			<template #footer="{returnFalse, returnTrue}">
				<BbButton @click="returnTrue()">Yes</BbButton>
				<BbButton @click="returnFalse()">No</BbButton>
			</template>
		</Modal>		

	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import InfoPanel, { PanelSizes } from "@/components/controls/info-panel/info-panel.vue";
import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import Currency from "@/components/display/currency.vue";
import DateTime, { DisplayType } from "@/components/display/dateTime.vue";
import Duration from "@/components/display/duration.vue";

import { TourType, TourState } from "@/types/tour";
import { Roles } from "~/types/private";
import { mapActions } from 'vuex'
import Modal from "@/components/controls/modal/modal.vue";

enum TourStateBackgroundColors {
	Green = "Green",
	Red = "Red",
    Yellow = "Yellow",
    Gray = "Gray",
    White = "White"
}

export default Vue.extend({
	components: {
		InfoPanel,
		BbButton,
		Currency,
		DateTime,
		Duration,
		Modal
	},

	props: {
		tourId: {
			type: [String, Number],
		},
		data: {
			type: [Object],
		},
		orders: {
			type: [Object, Array]
		}
	},

	data() {
		return {
			s,
			PanelSizes,
			ButtonTheme,
			TourType,
			TourState,
			DisplayType,
			Roles,
			error: '',
			success: ''
		}
	},

	computed: {
		tourFBLink(): string {
			return this.localePath(`/tour-fb?tourId=${this.tourId}`);
		},
		currentLocale(): string {
			return this.$i18n.locale;
		},
		userRole(): Roles {
			return this.$auth.user?.role;
		},
	},

	methods: {
		...mapActions('tour', ['cancelTour']),
		yesNoTemplate(value: boolean): string {			
			return value ? "Yes" : "No";
		},
		numberOrNoStringTemplate(number?: number): string {			
			if (!number) {
				return "No";
			}

			return number.toString();
		},
		getVatRate(): number {
			return parseInt(process.env.NUXT_ENV_VAT!);
		},
		getVatSum(): number {
			return this.data.vatPrice;
		},		
		getPriceWithVat(): number {
			return this.data.totalPriceWithVat; 
		},
		getTourStateBackgroundColor(tourState?: TourState): TourStateBackgroundColors {
			switch (tourState) {
				case TourState.Draft: return TourStateBackgroundColors.Gray;
				case TourState.Active: return TourStateBackgroundColors.Green;
                case TourState.CancelRequest: return TourStateBackgroundColors.Yellow;
				case TourState.Canceled: return TourStateBackgroundColors.Yellow;
				case TourState.Deleted: return TourStateBackgroundColors.Yellow;			
				default: return TourStateBackgroundColors.White;
			}
		},
		async cancel() {
			this.error = ''
			this.success = ''

			const decision = await (<any>this.$refs.Dialog).open("Are you sure?", "Confirmation");
		
			if (!decision) {
				return;
			}

			try {
				await this.cancelTour(this.tourId)
				this.success = this.$t('tours.tourSuccessfullyCancelled').toString()
			} catch(error) {
				const err = error as any
				if (err?.data?.hasPaidOrders){
					this.error = this.$t('tours.hasPaidNotCancelledOrders').toString()
				} else {
					this.error = this.$t('tours.tourCancellationError').toString()
				}
			}
		},
		getItinerary(): string {
			return this.data.itinerary && this.data.tourType != TourType.PrivateHire ? this.data.itinerary[this.currentLocale] : (this.orders[0]?.comment ?? '')
		},
		getOccupied(): string {
			return this.data.tourType==TourType.PrivateHire?"":this.data.occupaid+" of "+this.data.seatsCount;
		}
	},
})
</script>