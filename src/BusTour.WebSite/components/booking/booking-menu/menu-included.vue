<template>
	<div>
		<div
			v-for="guest in guests"
			:key="guest.seatId"
			:class="s.menuType"
		>
			<div :class="s.guest" v-if="mainMenuNames.length>0 || mainBeverageNames.length>0">
				{{ $t("menuBooking.seat") }}&ensp;{{ seatFullName(guest.seatId) }} *
			</div>
			<div :class="s.menuSelect">
				<div :class="s.menu" v-if="mainMenuNames.length>0">
						<control-item :text-left="true">
							<dropdown :value="getMainMenuForGuest(guest.seatId)" @input="setMainMenuForGuest($event, guest.seatId)" :items="mainMenuNames" /> 
						</control-item>
				</div>
				<div :class="s.menu" v-if="mainBeverageNames.length>0">
						<control-item :text-left="true">
							<dropdown :value="getMainBeverageForGuest(guest.seatId)" @input="setMainBeverageForGuest($event, guest.seatId)" :items="mainBeverageNames" :longname="true"/> 
						</control-item>
				</div>
			</div>
			<div :class="s.allergy" v-if="mainMenuNames.length>0 || mainBeverageNames.length>0">
				<div :class="s.allergyRow">
					<div :class="s.box">
						<control-item :noBack="true" :class="s.controlItem">
							<checkbox :value="getAllergyForGuest(guest.seatId)" @input="setAllergyForGuest($event, guest.seatId)" :label="$t('menuBooking.allergy')" :text-backward="true"/> 
						</control-item>									
					</div>	
					
					<div :class="s.allergyName">
						<control-item :text-left="true" v-if="getAllergyForGuest(guest.seatId)">
							<dropdown :value="getAllergyNameForGuest(guest.seatId)" @input="setAllergyNameForGuest($event, guest.seatId)" :items="allergyNames" /> 
						</control-item>
					</div>
				</div>
				<div :class="s.otherAllergy">
					<control-item :class="s.allergyInput" v-if="getAllergyNameForGuest(guest.seatId)==5 && getAllergyForGuest(guest.seatId)">
						<textInput :value="getOtherAllergyForGuest(guest.seatId)" @input="setOtherAllergyForGuest($event, guest.seatId)" />
					</control-item>
				</div>
			</div>	
			<div :class="s.imageContainer" v-if="isSelectedMenu(guest.seatId) && mainMenuNames.length>0">
				<img :class="s.imageMobile" :src="getImgSrc(guest.seatId)" />
				<div :class="s.imageDesc">
					<p>
						4 dishes menu : 2 starters, fish dish as main course and a dessert
					</p>
					<p>{{ $t("menuBooking.noDrinksIncluded") }}</p>
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
import textInput from "@/components/controls/text-input/text-input.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"

import {
	Beverage,
	Menu,
	MenuTypeEnum,
	OrderBeverage,
	OrderMenu,
	OrderSeat,
  	Allergy
} from "@/types/booking";

import {
	TourMenu,
	TourBeverage
} from "@/types/tour";

import { mapGetters } from 'vuex'

export default Vue.extend({
	name: "booking-menu",
	components: {
		textInput,
		checkbox
	},
	data() {
		return {
			s: style,
			lastMenuImgPath: "",
			controlItem,
			dropdown
		};
	},
	computed: {
		...mapGetters({
			seatFullName: "booking/seatFullName",
		}),		
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		guests: {
			get(): OrderSeat[] {
				return this.$store.state.booking.order.seats;
			},
		},
		allergies: {
			get(): Allergy[] {
				return this.$store.state.booking.menuInfo.allergies;
			},
		},
		allergyNames(): SelectItem[] {
            return this.allergies
					.map(x => new SelectItem(x.id, x.name[this.lang].toString()));
        }, 
		menus: {
			get(): Menu[] {
				return this.$store.state.booking.menuInfo.menus;
			},
		},
		mainMenus: {
			get(): Menu[] {
				return this.menus.filter(
					(item: Menu) => item.menuType.id === MenuTypeEnum.Main
				);
			},
		},
		tourTicketMenus: {
			get(): TourMenu[] {
				return this.$store.state.tour.tourMenuBeverage.tourMenus.filter((item: TourMenu) => item.isTicket === true);
			},
		},
		mainMenuNames(): SelectItem[] {
			const list: SelectItem[] = (this.menus
					.filter((item) => item.menuType.id === MenuTypeEnum.Main && this.tourTicketMenus.some(x => x.menuId==item.id)) as Menu[])
					.map(x => new SelectItem(x.id, x.name[this.lang].toString()));
			list.unshift(new SelectItem(-1, this.$t("menuBooking.none").toString()));
            return list;
        }, 
		mainBeverage: {
			get(): Beverage[] {
				return this.beverages;
			},
		},
		tourTicketBeverages: {
			get(): TourBeverage[] {
				return this.$store.state.tour.tourMenuBeverage.tourBeverages.filter((item: TourBeverage) => item.isTicket === true);
			},
		},
		mainBeverageNames(): SelectItem[] {
			let list =(this.beverages
					.filter((item: Beverage) => this.tourTicketBeverages.some(x => x.beverageId==item.id) ) as Beverage[])
					.map(x => new SelectItem(x.id, x.name[this.lang].toString()+" "+x.volume+" "+x.unit[this.lang].toString()));
			list.unshift(new SelectItem(-1, this.$t("menuBooking.none").toString()));
			return list;
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
		orderMenus: {
			get(): OrderMenu[] {
				return this.$store.state.booking.order.menus;
			},
		},
		imgSrc: {
			get(): string {
				return this.lastMenuImgPath
					? this.lastMenuImgPath
					: this.menus && this.menus.length > 0
					? "/images/" + this.menus[0].imgPath
					: "";
			},
		},	
		
	},
	methods: {
		async getTourMenuBeverage(){
        	await this.$store.dispatch("tour/getTourMenuBeverage",this.$store.state.booking.order.tourId);
    	},
		isSelectedMenu(seatId: number): boolean {
			let menuId = this.guests.filter((item) => item.seatId === seatId)[0].menuId;
			return menuId!=undefined? true: false;
			
		},
		getImgSrc(seatId: number): string {
			let menuId=this.guests.filter((item) => item.seatId === seatId)!=undefined? this.guests.filter((item) => item.seatId === seatId)[0].menuId:undefined;
			let imgPath=(menuId!=undefined && menuId!=-1)? this.menus.filter(
					(item: Menu) => item.id=== menuId
				)[0].imgPath:"";
			return imgPath!=""?"/images/" + imgPath:"";
		},
		getMainMenuForGuest(seatId: number): number | undefined {
			const mainMenu = this.guests.filter((item) => item.seatId === seatId)[0];
			return mainMenu.menuId ?? -1;
		},
		setMainMenuForGuest(menuId: string, seatId: number): void {
			this.$store.commit("booking/setMainMenuForGuest", <OrderSeat>{
				menuId: parseInt(menuId),
				seatId: seatId,
			});
			if (this.menus && this.menus.length > 0 && menuId!="-1") {
				this.lastMenuImgPath =
					"/images/" +
					this.menus.filter((item) => item.id === parseInt(menuId))[0].imgPath;
			}
			//this.isSelectedMenu=true;
			this.$emit('path', (menuId!="" && menuId!="-1")?"/images/" +this.menus.filter((item: Menu) => item.id=== parseInt(menuId))[0].imgPath:"")
		},
		getMainBeverageForGuest(seatId: number): number | undefined {
			const beverage = this.guests.filter((item) => item.seatId === seatId)[0];
			return beverage.beverageId ?? -1; 
		},
		setMainBeverageForGuest(beverageId: string, seatId: number): void {
			this.$store.commit("booking/setMainBeverageForGuest", <OrderSeat> {
				beverageId: parseInt(beverageId),
				seatId: seatId
			});
		},
		getAllergyForGuest(seatId: number): boolean{
			const guest = this.guests.filter((item) => item.seatId === seatId)[0];
			return guest.isAllergy;
		},
		getAllergyNameForGuest(seatId: number): number | undefined {
			const guest = this.guests.filter((item) => item.seatId === seatId)[0];
			return guest.allergyId;
		},
		getOtherAllergyForGuest(seatId: number): string | undefined {
			const guest = this.guests.filter((item) => item.seatId === seatId)[0];
			return guest.otherAllergy;
		},
		setAllergyForGuest(isAllergy: boolean, seatId: number): void {
			this.$store.commit("booking/setAllergyForGuest", <OrderSeat> {
				isAllergy: isAllergy,
				seatId: seatId
			});
		},
		setAllergyNameForGuest(allergyId: number, seatId: number): void {
			this.$store.commit("booking/setAllergyNameForGuest", <OrderSeat> {
				allergyId: allergyId,
				seatId: seatId
			});
		},
		setOtherAllergyForGuest(otherAllergy: string, seatId: number): void {
			this.$store.commit("booking/setOtherAllergyForGuest", <OrderSeat> {
				otherAllergy: otherAllergy,
				seatId: seatId
			});
		},		
	},
	created() {
		let orderMainMenu=this.guests.filter((item) => item.menuId != undefined)[0];
		if (orderMainMenu!=undefined)
		{
			if (this.menus && this.menus.length > 0) {
				this.lastMenuImgPath =
					"/images/" +
					this.menus.filter((item) => item.id === orderMainMenu.menuId)[0].imgPath;
			}
			this.$emit('path', "/images/" +this.menus.filter((item: Menu) => item.id=== orderMainMenu.menuId)[0].imgPath)
		} 
		this.getTourMenuBeverage();
	},
});
</script>