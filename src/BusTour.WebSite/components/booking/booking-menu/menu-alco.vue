<template>
	<div v-if="isAlco()">
		<div :class="s.blockSubtitle">
			{{ $t("menuBooking.alcoDrinks") }}
		</div>
		<div :class="s.drinkGroupContainer">
			<div v-for="group in alcoholBeveragesGroups" :key="group.id" :class="s.extraItems">
				<menu-alco-group :group="group"/>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import menuAlcoGroup from "./menu-alco-group.vue";

import {
	Beverage,
	BeverageGroup
} from "@/types/booking";

import {
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
			menuAlcoGroup
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
		tourExtraBeverages: {
			get(): TourBeverage[] {
				return this.$store.state.tour.tourMenuBeverage.tourBeverages.filter((item: TourBeverage) => item.isExtra === true);
			},
		},
		extraBeverages: {
			get(): Beverage[] {
				return this.beverages
					.filter((item: Beverage) => this.tourExtraBeverages.some(x => x.beverageId==item.id));
			},
		},
		alcoholBeveragesGroups: {
			get(): BeverageGroup[] {
				return this.extraBeverages
					.filter((item) => item.alcoholByVolume)
					.map((item) => item.group)
					.filter(
						(value, index, self) =>
							self.map((i) => i.id).indexOf(value.id) === index
					);
			},
		},
		
	},
	methods: {
		isAlco(seatId: number): boolean {
			return this.alcoholBeveragesGroups.length>0? true: false;	
		},
	},
});
</script>