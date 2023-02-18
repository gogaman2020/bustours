<template>
		<div>
			<div :class="[s.itemName,s.itemMobileName]">{{ item.name[lang] }}</div>

			<div :class="s.item">
				<div :class="s.itemName">{{ item.name[lang] }}</div>
				<div :class="s.itemVolume">{{ item.volume }} {{ item.unit[lang] }}</div>
				<div :class="s.itemPrice"><currency :value="item.price" :digits="2"/></div>
				<div :class="s.itemCounts">
					<control-item :text-left="true" :isHiddenTitle="true">
						<dropdown :value="itemValue" @input="selectedAmount($event)" :items="beveragesCounts" /> 
					</control-item>
				</div>
			</div>	
		</div>		
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import { SelectItem } from "@/types/common"
import currency from "@/components/display/currency.vue"

import {
	Beverage,
	BeverageGroup,
	Menu,
	MenuTypeEnum,
	OrderBeverage,
	OrderMenu,
	OrderInfoTable
} from "@/types/booking";
import { numeric } from "vuelidate/lib/validators";

export default Vue.extend({
	name: "booking-menu",
	components: {
		currency
	},
	props: {
        item: {} as () => Beverage,
		itemValue: Number
    },
	data() {
		return {
			s: style,
			openDrinkGroups: [] as number[],
			controlItem,
			dropdown,

		};
	},
	computed: {
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		orderInfoTables(): OrderInfoTable{
			return this.$store.state.booking.selectionModel.orderInfo.tables;
		},
		menus: {
			get(): Menu[] {
				return this.$store.state.booking.menuInfo.menus;
			},
		},
		specialMenus: {
			get(): Menu[] {
				return this.menus.filter(
					(item) => item.menuType.id === MenuTypeEnum.Special
				);
			},
		},
		beverages: {
			get(): Beverage[] {
				return this.$store.state.booking.menuInfo.beverages;
			},
		},
		softDrinks: {
			get(): Beverage[] {
				return this.beverages.filter(
					(item) => !item.alcoholByVolume && !item.isHot
				);
			},
		},
		hotDrinks: {
			get(): Beverage[] {
				return this.beverages.filter(
					(item) => !item.alcoholByVolume && item.isHot
				);
			},
		},
		alcoholBeveragesGroups: {
			get(): BeverageGroup[] {
				return this.beverages
					.filter((item) => item.alcoholByVolume)
					.map((item) => item.group)
					.filter(
						(value, index, self) =>
							self.map((i) => i.id).indexOf(value.id) === index
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
		getAlcoDrinks(groupId: number): Beverage[] {
			return this.beverages
				.filter((item) => item.alcoholByVolume && item.group.id === groupId)
				.filter(
					(value, index, self) =>
						self
							.map((i) => i.name[this.lang])
							.indexOf(value.name[this.lang]) === index
				);
		},
		getDrinkVariants(drinkName: string): Beverage[] {
			return this.beverages.filter(
				(item) => item.name[this.lang] === drinkName
			);
		},
		setOrderBeverage(drinkId: number, event: InputEvent): void {
			console.log(drinkId,event);
		},
		selectedAmount(event: InputEvent){
			this.$emit('input', event != undefined ? Number(event):0)
		}
	},
});
</script>