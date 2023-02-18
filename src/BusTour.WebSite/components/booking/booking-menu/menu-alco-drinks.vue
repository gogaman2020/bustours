<template>
	<div>
		<div :class="s.drinkName">
			{{ drinkSubGroup.name[lang] }}
		</div>
		<div :class="s.drinkSpecContainer">
			<div
				v-for="drink in getDrinkVariants(
					drinkSubGroup.name[lang]
				)"
				:key="drink.id"
				:class="s.drinkSpec"
			>
				<div :class="s.itemName"></div>
				<div :class="s.itemVolume">{{ drink.volume }} {{ drink.unit[lang] }}</div>
				<div :class="s.itemPrice"><currency :value="drink.price" :digits="2"/></div>
				<div :class="s.itemCounts">
					<control-item :text-left="true" :isHiddenTitle="true">
						<dropdown 
							:value="orderBeverages.findOrDefault(x => x.beverageId == drink.id, { amount: 0 }).amount" 
							:items="beveragesCounts"
							@input="setOrderBeverageAmount($event, drink.id)" 
						/>
					</control-item>
				</div>
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
	OrderBeverage
} from "@/types/booking";

import {
	TourBeverage
} from "@/types/tour";

export default Vue.extend({
	name: "booking-menu",
	components: {
		currency
		
	},
	props: {
        drinkSubGroup: {} as () => Beverage,
    },
	data() {
		return {
			s: style,
			controlItem,
			dropdown
		};
	},
	computed: {
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		beverages: {
			get(): Beverage[] {
				return this.$store.state.booking.menuInfo.beverages;
			},
		},
		orderBeverages: {
			get(): OrderBeverage[] {
				return this.$store.state.booking.order.beverages;
			},
		},
		beveragesCounts() {
			let items: SelectItem[] = [];
			for(let i = 0; i < 6; i++) {
				items.push(new SelectItem(i));
			}
			return items;
    	},
		alcoBeverages: {
			get(): Beverage[] {
				return this.beverages
					.filter((item: Beverage) => this.$store.state.tour.tourMenuBeverage.tourBeverages.filter((item: TourBeverage) => item.isExtra === true).some((x:TourBeverage) => x.beverageId==item.id));
			},
		},		
		
	},
	methods: {
		setOrderBeverageAmount(amount: number, bevergeId: number): void {
			this.$store.commit("booking/setOrderBeverageAmount", <OrderBeverage>{
				beverageId: bevergeId,
				amount: amount,
			});
		},
		getDrinkVariants(drinkName: string): Beverage[] {
			return this.alcoBeverages.filter(
				(item) => item.name[this.lang] === drinkName
			);
		},
	},
});
</script>