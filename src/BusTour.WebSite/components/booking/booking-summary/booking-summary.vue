<template>
	<div :class="s.bookingSummary">
		<div v-if="order.type == OrderType.PrivateHire">
			<BookingSummaryPrivateHire :withoutTitle="withoutTitle" />
		</div>

		<div v-else :class="s.bookingSummaryPanelsWrapper">
			<h2 v-if="!withoutTitle" :class="s.bookingSummaryTitle">{{ $t("bookingSummary.header") }}</h2>

			<InfoPanel :all-left-bold="true">
				<div v-if="order.hash">{{ $t("bookingSummary.number") }}</div>
				<div v-if="order.hash"><a :href="'/order/' + order.hash" target="__blank">{{order.id}}</a></div>				
				<div>{{ $t("bookingSummary.city") }}</div>
				<div>{{ route.cityName[lang] }}</div>
				<div>{{ $t("bookingSummary.date") }}</div>
				<DateTime :value="orderDate" variant="long" />
				<div>{{ $t("bookingSummary.itinerary") }}</div>
				<div>{{ route.name[lang] }}</div>
				<div>{{ $t("bookingSummary.departureTime") }}</div>
				<DateTime :value="departureTime" :type="DisplayType.Time" variant="short"  />
				<div>{{ $t("bookingSummary.guests") }}</div>
				<div>{{ order.guestCount }}</div>
				<div v-if="order.guestsWithDisabilities">{{ $t("bookingSummary.disabledGuests") }}</div>
				<div v-if="order.guestsWithDisabilities">{{ order.disabledGuestCount }}</div>
				<div>{{ $t("bookingSummary.table") }}</div>
				<div>{{ seatingTypeName }}</div>
			</InfoPanel>

			<InfoPanel :all-left-bold="true">
				<div data-bold>{{ order.seatingType == 2 ? $t("bookingSummary.seats") : "" }}</div>
				<div></div>
				<template v-for="seat in order.seats">
					<div v-if="order.seatingType == 2" data-shift :key="`${seat.seatId}-seat-name`">{{ seatFullName(seat.seatId) }}</div>
					<div v-if="order.seatingType == 2" :key="`${seat.seatId}-price`"><Currency :value="seatPrice(seat.seatId)" /></div>
				</template>
				<div data-bold>{{ $t("bookingSummary.tourPrice") }}</div>
				<div data-bold><Currency :adjust-font="false" :value="seatTotalPrice" /></div>
			</InfoPanel>

			<InfoPanel :cols="3" :centerNonBoundaryColumns=true>
				<template #custom>
					<div>
						<div data-bold>{{ $t("bookingSummary.extras") }}</div>
						<div></div>
						<div></div>
					</div>
					<div v-for="item in extras" :key="item.name">	
						<div data-shift >{{ item.name }}</div>
						<div >{{ item.amount }}</div>
						<div><Currency :value="item.price" :digits="2"/></div>
					</div>
				</template>
			</InfoPanel>

			<InfoPanel v-if="order.promoCodeId">
				<div data-bold>{{ $t("bookingSummary.promoCode") }}</div>
				<div>{{ order.promoCodeId ? $t("yes") : $t("no") }}</div>
				<div></div>
				<div>
					<div v-if="promoCodeIsPercent">{{orderPromoCode.amountOfDiscount}}%</div>
					<Currency v-else :value="orderPromoCode.amountOfDiscount"/>
				</div>
			</InfoPanel>

			<InfoPanel v-if="order.certificateId">
				<div data-bold>{{ $t("bookingSummary.certificate") }}</div>
				<div>{{ order.certificateId ? $t("yes") : $t("no") }}</div>
				<div></div>
				<div><Currency :value="orderCertificate.amountVariant.amount" /></div>
			</InfoPanel>
				
			<InfoPanel>
				<div data-bold>{{ $t("bookingSummary.total") }}</div>
				<div data-bold><Currency :adjust-font="false" :value="calculationCostTour.totalPrice + calculationCostTour.vat" /></div>
				<div data-shift>{{ $t("bookingSummary.vat") }} {{vat}}%</div>
				<div><Currency :value="calculationCostTour.vat" /></div>
			</InfoPanel>

			<InfoPanel v-if="giftBalance !== 0">
				<div data-bold>{{ $t("bookingSummary.certificate") }}</div>
				<div>{{ $t("bookingSummary.balance") }}</div>
				<div></div>
				<div><Currency :value="giftBalance" /></div>
			</InfoPanel>

			<InfoPanel v-if="order.certificateNumber" :class="s.bookingSummaryCertificateWarning">
				{{ $t("booking.certificateWarning") }}
			</InfoPanel>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue"
import { mapGetters, mapState } from "vuex"
import s from "./style.module.scss"

import { DisplayFormatter } from "~/types/common"
import { GiftCertificate } from "~/types/giftCertificate"
import { Promocode, DiscountType } from "~/types/promocodes"
import InfoPanel, { PanelSizes } from "~/components/controls/info-panel/info-panel.vue"
import Currency from "~/components/display/currency.vue"
import DateTime, { DisplayType } from "~/components/display/dateTime.vue"
import { Seat, Extras, Menu, OrderMenu, OrderBeverage, Beverage, OrderType, Order, SeatingType } from "~/types/booking"
import BookingSummaryPrivateHire from "@/components/booking/booking-summary/booking-summary-private-hire.vue"
import Config from "~/config/index"

export default Vue.extend({
  	components: { 
		InfoPanel,
		Currency,
		DateTime,
		BookingSummaryPrivateHire
	},

	props: {
		withoutTitle: Boolean
	},

	data() {
		return {
			s,
			PanelSizes,
			DisplayFormatter,
			DisplayType,
			OrderType,
		}
	},

	computed: {
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		seatingTypeName(): string {
			return this.$t(`enums.SeatingType.${SeatingType[this.order.seatingType]}`).toString()
		},
		...mapGetters({
			order: "booking/order",
			route: "booking/route",
			seatPrice: "booking/seatPrice",
			totalPrice: "booking/totalPrice",
			seatFullName: "booking/seatFullName",
			seatTotalPrice: "booking/seatTotalPrice",
			extrasTotalPrice: "booking/extrasTotalPrice",
			calculationCostTour: "booking/calculationCostTour"
		}),
		orderCertificate(): GiftCertificate {
			return this.$store.state.booking.orderCertificate;
		},
		orderPromoCode(): Promocode {
			return this.$store.state.booking.orderPromocode;
		},
		...mapState('tour', ['routes']),
		orderDate(): any {
			return this.order.tour?.departure ?? this.order.date 
		},		
		departureTime(): any {
			return this.order.time?.date ?? this.order.tour?.departure
		},
		extras(): Extras[] {
			let extras = [] as Extras[];
            this.$store.state.booking.order.menus
				.filter((item: OrderMenu) => item.amount > 0)
				.forEach((x: OrderMenu) => {
					let menu = this.$store.state.booking.menuInfo.menus
						.find((item: Menu) => item.id === x.menuId);
					extras.push(<Extras>{name: menu.name[this.lang].toString(), amount: x.amount, price: menu.price})
			});
			this.$store.state.booking.order.beverages
				.filter((item: OrderBeverage) => item.amount > 0)
				.forEach((x: OrderBeverage) => {
					let beverage = this.$store.state.booking.menuInfo.beverages
						.find((item: Beverage) => item.id === x.beverageId);
					extras.push(<Extras>{name: beverage.name[this.lang].toString(), amount: x.amount, price: beverage.price})
			});
			return extras;
        },
		promoCodeIsPercent(): boolean {
			return this.orderPromoCode.typeOfDiscount === DiscountType.Percent;
		},
		vat(): number {
			return Config.vat;
		},
		giftBalance(): number {
			return this.total < 0 ? this.total * -1 : 0;
		},
		total(): number {
			let promo = (this.orderPromoCode as any).amountOfDiscount | 0;
			let certificate = (this.orderCertificate as any).amountVariant?.amount | 0

			let witPromo = this.promoCodeIsPercent
				? this.seatTotalPrice - (this.seatTotalPrice / 100 * promo)
				: this.seatTotalPrice - promo;
			
			let result = (witPromo + this.extrasTotalPrice) - certificate + this.includingVAT;

			return result;
		},
		includingVAT(): number {
			let promo = (this.orderPromoCode as any).amountOfDiscount | 0;
			let certificate = (this.orderCertificate as any).amountVariant?.amount | 0

			let witPromo = this.promoCodeIsPercent
				? this.seatTotalPrice - (this.seatTotalPrice / 100 * promo)
				: this.seatTotalPrice - promo;
			
			let result = ((witPromo + this.extrasTotalPrice) - certificate) * this.vat / 100;

			return result > 0 ? result : 0;
		},
	},
});
</script>