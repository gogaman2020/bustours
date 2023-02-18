<template>
	<div :class="s.page">
		<div>
			<div :class="s.blockTitle">
				{{ $t("menuBooking.preferences") }}
			</div>
			<div :class="s.blockTitle">
				{{ $t("menuBooking.noExtraCharge") }}
			</div>
			<div :class="s.partBus">
				<menu-table v-for="tab in orderInfoTables"
					:key="tab.id"
					:orderInfoTable="tab"
					:style="{'left': -getPartLeft(tab) + 'px', 'top': -getPartTop(tab) + 'px'}"
				/>
			</div>
			<div :class="s.blockMenu">
				<menu-included @path="imgChange($event)"/>
			</div>	
		</div>
		<div :class="s.mainImage" v-if="isMenu() && imgSrc!=''">
			<img :class="s.image" :src="imgSrc" />
			<div :class="s.imageDesc">
				<p style="font-size: 18px;">
					4 dishes menu : 2 starters, fish dish as main course and a dessert
				</p>
				<p style="font-size: 18px;">{{ $t("menuBooking.noDrinksIncluded") }}</p>
			</div>
		</div>

		<div v-if="showSpecialRequests" :class="s.specialRequests">
			<control-item :class="s.specialRequestsControl" :title="$t('booking.specialRequests')" :autoHeight="true">
				<textarea v-model="specialRequests" />
			</control-item>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import menuIncluded from "./menu-included.vue";
import menuTable from "./menu-table.vue";
import menuExtra from "./menu-extra.vue";
import controlItem from "@/components/controlItem/controlItem.vue"
import { mapProps } from "@/store/helpers"
import { TourMenu } from "@/types/tour"
import { mapGetters } from "vuex"

import {
	OrderInfoTable, OrderType, Menu, MenuTypeEnum
} from "@/types/booking";

export default Vue.extend({
	name: "booking-menu",
	components: {
		menuTable,
		menuIncluded,
		menuExtra,
		controlItem
	},
	data() {
		return {
			s: style,
			openDrinkGroups: [] as number[],
			imgSrc: "" as String,
			path: "" as String,
			isSelectedMenu: false as Boolean
		};
	},
	computed: {
		...mapProps(
			[	
				"specialRequests",
				"type"

			], 
			"booking", 
			"order", 
			"setOrder"
		),  	
		...mapGetters (
			"booking",
			[
				"orderInfo"
			], 
		),
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		orderInfoTables(): OrderInfoTable{
			return this.orderInfo.tables;
		},
		showSpecialRequests(): boolean {
			return this.type == OrderType.RegularGroup;
		},
		
	},
	methods: {
		imgChange(event: string): void {
			this.imgSrc=event;
		},
		isMenu(): boolean {
			let tourMenu=this.$store.state.tour.tourMenuBeverage.tourMenus.filter((item: TourMenu) => item.isTicket === true);
			let menu=this.$store.state.booking.menuInfo.menus.filter((item:Menu) => item.menuType.id === MenuTypeEnum.Main && tourMenu.some((x:TourMenu) => x.menuId==item.id)) as Menu[];
			return menu.length>0?true: false;
		},	
		getPartLeft(table: OrderInfoTable) {
			const lefts = table.seats.map(x => x.x)
			lefts.push(table.x)
			return Math.min(...lefts)
		},
		getPartTop(table: OrderInfoTable) {
			const tops = table.seats.map(x => x.y)
			tops.push(table.y)
			return Math.min(...tops)
		}		
	},
	
});
</script>