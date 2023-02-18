<template>
	<div :class="s.bookingSummary">
		<h2 v-if="!withoutTitle" :class="s.bookingSummaryTitle">{{ $t("bookingSummary.headerPrivateHire") }}</h2>

		<InfoPanel :all-left-bold="true" :class="s.panel">
			<div>{{ $t("bookingSummary.number") }}</div>
			<div><a :href="'/order/' + order.hash" target="__blank">{{order.id}}</a></div>		
			<div>{{ $t("bookingSummary.name") }}</div>
			<div>{{ order.client.fullName }}</div>	
			<div>{{ $t("bookingSummary.phoneNumber") }}</div>
			<div>{{ order.client.phoneNumber }}</div>
		</InfoPanel>

		<InfoPanel :all-left-bold="true" :class="s.panel">
			<div>{{ $t("bookingSummary.city") }}</div>
			<div>{{ cityName }}</div>		
			<div>{{ $t("bookingSummary.date") }}</div>
			<DateTime :value="privateHire.date" variant="long" />
			<div>{{ $t("bookingSummary.itinerary") }}</div>
			<div>{{ routeName }}</div>
			<div>{{ $t("bookingSummary.departureTime") }}</div>
			<DateTime :value="dateTimeFrom" :type="DisplayType.Time" variant="short"  />
			<div>{{ $t("booking.departurePoint") }}</div>
			<div>{{ privateHire.departurePoint }}</div>
			<div>{{ $t("booking.arrivalPoint") }}</div>
			<div>{{ privateHire.arrivalPoint }}</div>
			<div>{{ $t("booking.stops") }}</div>
			<div>
				<template v-for="(stop, index) in privateHire.stops">
					<div :key="index">{{ stop }}</div>
				</template>	
			</div>
		</InfoPanel>

		<InfoPanel :cols="3" v-if="regularMenus.length" :centerNonBoundaryColumns=true :class="s.panel">
			<template #custom>
				<div>
					<div data-bold>{{ $t("bookingSummary.menus") }}</div>
					<div></div>
					<div></div>
				</div>
				<div v-for="item in regularMenus" :key="item.menuId">	
					<div data-shift >{{ getMenu(item.menuId).name[lang] }}</div>
					<div>x{{ item.amount }}</div>
					<div><Currency :value="getMenu(item.menuId).price" /></div>
				</div>
			</template>
		</InfoPanel>

		<InfoPanel :cols="3" v-if="orderBeverages.length" :centerNonBoundaryColumns=true :class="s.panel">
			<template #custom>
				<div>
					<div data-bold>{{ $t("bookingSummary.beverages") }}</div>
					<div></div>
					<div></div>
				</div>
				<div v-for="item in orderBeverages" :key="item.beverageId">	
					<div data-shift >{{ getBeverage(item.beverageId).name[lang] }}</div>
					<div>x{{ item.amount }}</div>
					<div><Currency :value="getBeverage(item.beverageId).price" /></div>
				</div>
			</template>
		</InfoPanel>	

		<InfoPanel :cols="3" v-if="extras.length" :centerNonBoundaryColumns=true :class="s.panel">
			<template #custom>
				<div>
					<div data-bold>{{ $t("bookingSummary.extras") }}</div>
					<div></div>
					<div></div>
				</div>
				<div v-for="extra in extras" :key="extra.name">	
					<div data-shift >{{ extra.name }}</div>
					<div>x{{ extra.amount }}</div>
					<div><Currency :value="extra.price" /></div>
				</div>
			</template>
		</InfoPanel>		


		<InfoPanel v-if="order.specialRequests" :class="s.panel">
			<template #custom>
				<div>
					<div data-bold>{{ $t("booking.specialRequests") }}</div>
				</div>
				<div data-shift>	
					<div>{{ order.specialRequests }}</div>
				</div>
			</template>			
		</InfoPanel>	

		<InfoPanel v-if="order.comment" :class="s.panel">
			<template #custom>
				<div>
					<div data-bold>{{ $t("booking.comment") }}</div>
				</div>
				<div data-shift>	
					<div v-html="formatComment(order.comment)"></div>
				</div>
			</template>			
		</InfoPanel>		

		<InfoPanel :class="s.panel">
			<div data-bold>{{ $t("bookingSummary.total") }}</div>
			<div data-bold><Currency :adjust-font="false" :value="totalPrice" /></div>
			<div data-shift>{{ $t("bookingSummary.vat") }} 20%</div>
			<div><Currency :value="totalVat" /></div>
		</InfoPanel>				

	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapState, mapActions } from "vuex";
import s from "./style.module.scss";

import { Seat } from '@/types/booking'

import { DisplayFormatter, Time } from "~/types/common";
import InfoPanel, { PanelSizes } from "~/components/controls/info-panel/info-panel.vue";
import Currency from "~/components/display/currency.vue";
import DateTime, { DisplayType } from "~/components/display/dateTime.vue";
import { Extras, Menu, OrderMenu, OrderBeverage, Beverage, OrderType, Route, OrderPrivateHire, MenuTypeEnum } from "~/types/booking";
import { City } from "~/types/tour";


export default Vue.extend({
  	components: { 
		InfoPanel,
		Currency,
		DateTime,
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
			OrderType
		}
	},

	computed: {
		lang(): string {
			return  this.$i18n.locale
		},
		...mapGetters({
			order: "booking/order",
			route: "booking/route",
			seatPrice: "booking/seatPrice",
			totalPrice: "booking/totalPrice",
			totalVat: "booking/totalVat",
			seatFullName: "booking/seatFullName"
		}),
		...mapState('tour', ['routes', 'cities']),
		...mapState('booking', ['menuInfo']),
		privateHire(): OrderPrivateHire {
			return this.order.privateHire;
		},
		routeById(): Route | undefined {
			return this.routes.find((x: Route) => x.id == this.order?.privateHire?.routeId)
		},
		routeName(): string {
			return this.routeById ? this.routeById.name[this.lang] : this.$t('bookingSummary.otherRoute').toString()
		},
		cityName(): City {
			return this.routeById ? this.cities.find((x: City) => x.id == this.routeById?.cityId)?.name[this.lang] : this.cities[0]?.name[this.lang]
		},
		dateTimeFrom():Date {
			return Time.toDate(this.privateHire.timeFrom ?? new Time(0,0,0));
		},
		orderMenus(): OrderMenu[] {
			return this.order.menus.filter((x: OrderMenu) => x.menuId && x.amount)
		},
		orderBeverages(): OrderBeverage[] {
			return this.order.beverages.filter((x: OrderBeverage) => x.beverageId && x.amount)
		},		
		extras(): Extras[] {
			let extras: (Extras | null) [] = []
			extras = extras.concat(this.orderMenus
				.map(x => {
					const menu = this.menuInfo.menus.find((m: Menu) => m.id == x.menuId)
					return menu.menuType.id != MenuTypeEnum.Main ? <Extras>{id: x.menuId, name: menu.name[this.lang].toString(), amount: x.amount, price: menu.price} : null
			}))
			// .concat(this.orderBeverages
			// 	.map(x => {
			// 		const beverage = this.menuInfo.beverages.find((m: Beverage) => m.id == x.beverageId)
			// 		return <Extras>{id: x.beverageId, name: beverage.name[this.lang].toString(), amount: x.amount, price: beverage.price}
			// }))			
			return extras.filter(x => !!x) as Extras[];
        },
		regularMenus(): OrderMenu[] {
			return this.orderMenus.filter(x => !this.extras.some(e => e.id == x.menuId))
		}
	},
	methods: {
		getMenu(menuId: number): Menu {
			return this.menuInfo.menus.find((x: any) => x.id == menuId)
		},	
		getBeverage(beverageId: number): Beverage {
			return this.menuInfo.beverages.find((x: any) => x.id == beverageId)
		},
		formatComment(comment: string) {
			return comment.replace(/(?:\r\n|\r|\n)/g, '<br>');
		}	
	},
});
</script>