<template>
	<div>
		<div :class="s.drinkBlock">
			<div :class="s.drinkGroupName">{{ group.name[lang] }}</div>
		</div>
		<div
			v-for="drinkSubGroup in getAlcoDrinks(group.id)"
			:key="drinkSubGroup.name[lang]"
			:class="s.drinkItem"
		>
			<menu-alco-drinks :drinkSubGroup="drinkSubGroup"/>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import menuAlcoDrinks from "./menu-alco-drinks.vue";

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
	props: {
        group: {} as () => BeverageGroup,
    },
	data() {
		return {
			s: style,
			controlItem,
			dropdown,
			menuAlcoDrinks
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
		alcoBeverages: {
			get(): Beverage[] {
				return this.beverages
					.filter((item: Beverage) => this.$store.state.tour.tourMenuBeverage.tourBeverages.filter((item: TourBeverage) => item.isExtra === true).some((x:TourBeverage) => x.beverageId==item.id));
			},
		},	
		
	},
	methods: {
		getAlcoDrinks(groupId: number): Beverage[] {
			return this.alcoBeverages
				.filter((item) => item.alcoholByVolume && item.group.id === groupId)
				.filter(
					(value, index, self) =>
						self
							.map((i) => i.name[this.lang])
							.indexOf(value.name[this.lang]) === index
				);
		}
	},
});
</script>