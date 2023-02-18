<template>
	<div :class="s.menuExtraBlock" v-if="isExtra()">
		<div :class="s.menuBlock" v-if="isExtraBeverages()">
			<div :class="s.blockTitle">
				{{ $t("menuBooking.drinksSelection") }}
			</div>
			<div :class="s.blockTitle">
				{{ $t("menuBooking.extraChargesApply") }}
			</div>
			<div>
				<div :class="s.blockDrinks" >
					<div :class="s.extraItems" v-if="isSoft()">
						<div :class="s.drinkGroupName">
							{{ $t("menuBooking.softDrinks") }}
						</div>
						<menu-extra-items 
							v-for="drink in softDrinks"
							:key="drink.id"
							:item="drink"
							:itemValue="orderBeverages.findOrDefault(x => x.beverageId == drink.id, { amount: 0 }).amount"
							@input="setOrderBeverageAmount($event, drink.id)"
							/>
					</div>
					<div :class="s.extraItems" v-if="isHot()">
						<div :class="s.drinkGroupName">
							{{ $t("menuBooking.hotDrinks") }}
						</div>
						<menu-extra-items 
							v-for="drink in hotDrinks"
							:key="drink.id"
							:item="drink"
							:itemValue="orderBeverages.findOrDefault(x => x.beverageId == drink.id, { amount: 0 }).amount"
							@input="setOrderBeverageAmount($event, drink.id)"/>
					</div>

					<div :class="s.extraItems">
						<div :class="s.itemPrice"></div>
					</div>
				</div>
				<div>
					<menu-alco/>
				</div>
			</div>
			<div :class="s.blockInfo" v-if="isExtraBeverages()">
				{{ $t("menuBooking.info") }}
			</div>
		</div>
		<div :class="s.menuBlock" v-if="isExtraMenu()">
			<div :class="s.blockTitleLarge">
				{{ $t("menuBooking.extra") }}
				<div :class="s.blockTitleNotIncluded">
					{{ $t("menuBooking.notIncluded") }}
				</div>
			</div>
			<div :class="s.blockTitleNote">
				{{ $t("menuBooking.advancedOrder") }}
			</div>
			<menu-extra-items 
							v-for="menu in specialMenus"
							:key="menu.id"
							:item="menu"
							:itemValue="getOrderMenuAmount(menu.id)"
							@input="setOrderMenuAmount($event, menu.id)"/>
		</div>
		<div :class="s.blockTitleFootNote" v-if="isExtra()">
			{{ $t("menuBooking.disServCharge") }}
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import { Func, SelectItem } from "@/types/common"
import menuExtraItems from "./menu-extra-items.vue";
import menuAlco from "./menu-alco.vue";

import {
	Beverage,
	Menu,
	MenuTypeEnum,
	OrderBeverage,
	OrderMenu
} from "@/types/booking";

import {
	TourMenuBeverage,
	TourMenu,
	TourBeverage
} from "@/types/tour";

export default Vue.extend({
	name: "booking-menu",
	components: {
		
	},
	data() {
		return {
			s: style,
			controlItem,
			dropdown,
			menuExtraItems,
			menuAlco
		};
	},
	computed: {
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		menus: {
			get(): Menu[] {
				return this.$store.state.booking.menuInfo.menus;
			},
		},
		beverages: {
			get(): Beverage[] {
				return this.$store.state.booking.menuInfo.beverages;
			},
		},
		tourExtraMenus: {
			get(): TourMenu[] {
				return this.$store.state.tour?.tourMenuBeverage?.tourMenus?.filter((item: TourMenu) => item.isExtra === true) ?? [];
			},
		},
		specialMenus: {
			get(): Menu[] {
				return this.menus
						.filter(item => item.menuType.id !== MenuTypeEnum.Main)
						.filter(item => this.tourExtraMenus.some(x => x.menuId==item.id));
			},
		},
		tourExtraBeverages: {
			get(): TourBeverage[] {
				return this.$store.state.tour?.tourMenuBeverage?.tourBeverages?.filter((item: TourBeverage) => item.isExtra === true)  ?? [];
			},
		},
		extraBeverages: {
			get(): Beverage[] {
				return this.beverages
					.filter((item: Beverage) => this.tourExtraBeverages.some(x => x.beverageId==item.id));
			},
		},
		softDrinks: {
			get(): Beverage[] {
				return this.extraBeverages.filter(
					(item) => !item.alcoholByVolume && !item.isHot
				);
			},
		},
		hotDrinks: {
			get(): Beverage[] {
				return this.extraBeverages.filter(
					(item) => !item.alcoholByVolume && item.isHot
				);
			},
		},
		orderBeverages: {
			get(): OrderBeverage[] {
				return this.$store.state.booking.order.beverages;
			},
		},
		orderMenus: {
			get(): OrderMenu[] {
				return this.$store.state.booking.order.menus;
			},
		},
		beveragesCounts() {
			let items: SelectItem[] = [];
			for(let i = 0; i < 6; i++) {
				items.push(new SelectItem(i));
			}
			return items;
    	},		
		
	},
	methods: {
		isExtraMenu(seatId: number): boolean {
			return this.specialMenus.length>0? true: false;	
		},
		isExtraBeverages(seatId: number): boolean {
			return this.tourExtraBeverages.length>0? true: false;	
		},
		isHot(seatId: number): boolean {
			return this.hotDrinks.length>0? true: false;	
		},
		isSoft(seatId: number): boolean {
			return this.softDrinks.length>0? true: false;	
		},
		isExtra(seatId: number): boolean {
			return (this.tourExtraMenus.length>0 || this.tourExtraBeverages.length>0)? true: false;	
		},
		getOrderBeverageAmount(bevergeId: number): number {
			return this.orderBeverages.filter(
				(item) => item.beverageId === bevergeId
			)[0]?.amount;
		},
		setOrderBeverageAmount(amount: number, bevergeId: number): void {
			this.$store.commit("booking/setOrderBeverageAmount", <OrderBeverage>{
				beverageId: bevergeId,
				amount: amount,
			});
		},
		getOrderMenuAmount(menuId: number): number {
			return this.orderMenus.filter((item) => item.menuId === menuId)[0]?.amount;
		},
		setOrderMenuAmount(amount: number, menuId: number): void {
			this.$store.commit("booking/setOrderMenuAmount", <OrderMenu>{
				menuId: menuId,
				amount: amount,
			});
		},
	},
});
</script>