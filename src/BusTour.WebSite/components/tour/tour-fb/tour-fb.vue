<template>
	<div :class="s.page">
		<h1 :class="s.title">{{ $t("tourFB.header") }}</h1>
		<div :class="s.infoRow" v-if="isId">
			<div  >
				<div :class="s.label">{{ $t("tourFB.city") }}</div> 
				<input
					type="text"
					readonly="true"
					:value="city"
					:class="s.text"
				/>
			</div>
			<div  >
				<div :class="s.label">{{ $t("tourFB.dateFrom") }}</div> 
				<input
					type="text"
					readonly="true"
					:value="tourDate"
					:class="s.text"
				/>
			</div>
			<div  >
				<div :class="s.label">{{ $t("tourFB.number") }}</div> 
				<input
					type="text"
					readonly="true"
					:value="tourNumber"
					:class="s.text"
				/>
			</div>			
		</div>
		<div :class="s.infoRow" v-if="!isId">
			<div :class="s.select">
				<control-item :text-left="true" :title="$t('tourFB.city')">
					<dropdown v-model="cityId" :items="cityItem" @input="setDateOrCity()"/> 
				</control-item>
			</div>
			<div :class="s.select">
				<control-item :title="$t('tourFB.dateFrom')" :class="s.formCalendar">
					<calendar :lang="this.lang" v-model="date" @input="setDateOrCity()" :startAvailableDate="new Date()" :endAvailableDate="endAvailableDate"/>
				</control-item>
			</div>
		</div>

		<div :class="s.panel">
			<div row>
				<div data-name data-bold>{{ $t("tourFB.departureTime") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{ time(tour.id)}}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.tourEnd") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{tourEnd(tour.id)}}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.tourType") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{ tour.tourType != undefined? $t(`enums.TourType.${TourType[tour.tourType]}`) : "-" }}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.guestsNumber") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{guestsNumber(tour.id)}}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.itinerary") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{ itinerary(tour.id)}}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.status") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id"><div :class="[s.blockCellTemplate, s[`blockCellTemplate${getTourStateBackgroundColor(tour.status)}`]]">{{ tour.status != undefined? $t(`enums.TourState.${TourState[tour.status]}`) : "-"}}</div></div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.duration") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{ duration(tour.id) }}</div>
				<div data></div>
			</div>
			<div row>
				<div data-name data-bold></div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">
					<control-item>
                    	<checkbox :value="checkedIds.some(x => x == tour.id)" @input="onTourChecked(checkedIds, tour.id, $event)" :class="s.box"/>  
                	</control-item>  
				</div>
				<div data-name data-bold></div>
			</div>		
		</div>

		<div :class="s.panel">
			<div row>
				<div data-name data-bold>{{ $t("tourFB.menu") }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id"></div>
				<div data-name data-bold>{{ $t("tourFB.total") }}</div>
			</div>
			<div row v-for="menu in menus" :key="menu.id+'-m'">
				<div data-shift data-name>{{ menu.name[lang] }}</div>
				<div data-name data-center></div>
				<div data v-for="tour in tours" :key="tour.id">{{ menuAmount(menu.id,tour.id) }}</div>
				<div data data-shift>{{ sumMenu(menu.id) }}</div>
			</div>
			<div row>
				<div data-name data-bold>{{ $t("tourFB.beverage") }}</div>
				<div data-name data-center></div>
				<div data></div>
				<div data></div>
			</div>
			<div row v-for="beverage in beverages" :key="beverage.id+'-b'">
				<div data-shift data-name>{{ beverage.name[lang] }}</div>
				<div data-name data-center>{{ beverage.volume }} {{ beverage.unit[lang] }}</div>
				<div data v-for="tour in tours" :key="tour.id">{{ beverageAmount(beverage.id,tour.id) }}</div>
				<div data data-shift>{{ sumBeverage(beverage.id) }}</div>
			</div>
			<div row>
				<div data-name data-bold v-if="isSoft()">{{ $t("tourFB.softDrinks") }}</div>
				<div data-name data-center></div>
				<div data></div>
				<div data></div>
			</div>
			<div row v-for="beverage in softDrinks" :key="beverage.id+'-sd'">
				<div data-shift data-name>{{ beverage.name[lang] }}</div>
				<div data-name data-center>{{ beverage.volume }} {{ beverage.unit[lang] }}</div>
				<div data v-for="tour in tours" :key="tour.id">{{ extraBeverageAmount(beverage.id,tour.id) }}</div>
				<div data data-shift>{{ sumExtraBeverage(beverage.id) }}</div>
			</div>
			<div row>
				<div data-name data-bold v-if="isHot()">{{ $t("tourFB.hotDrinks") }}</div>
				<div data-name data-center></div>
				<div data></div>
				<div data></div>
			</div>
			<div row v-for="beverage in hotDrinks" :key="beverage.id+'-hd'">
				<div data-shift data-name>{{ beverage.name[lang] }}</div>
				<div data-name data-center>{{ beverage.volume }} {{ beverage.unit[lang] }}</div>
				<div data v-for="tour in tours" :key="tour.id">{{ extraBeverageAmount(beverage.id,tour.id) }}</div>
				<div data data-shift>{{ sumExtraBeverage(beverage.id) }}</div>
			</div>
			<div row>
				<div data-name data-bold v-if="isAlco()">{{ $t("tourFB.alcoDrinks") }}</div>
				<div data-name data-center></div>
				<div data></div>
				<div data></div>
			</div>
			<div row v-for="beverage in alcoDrinks" :key="beverage.id+'-ad'">
				<div data-shift data-name>{{ beverage.name[lang] }}</div>
				<div data-name data-center>{{ beverage.volume }} {{ beverage.unit[lang] }}</div>
				<div data v-for="tour in tours" :key="tour.id">{{ extraBeverageAmount(beverage.id,tour.id) }}</div>
				<div data data-shift>{{ sumExtraBeverage(beverage.id) }}</div>
			</div>
			<div row>
				<div data-name data-bold v-if="isExtra()">{{ $t("tourFB.extras") }}</div>
				<div data-name data-center></div>
				<div data></div>
				<div data></div>
			</div>
			<div row v-for="menu in extraMenus" :key="menu.id+'-em'">
				<div data-shift data-name>{{ menu.name[lang] }}</div>
				<div data-name data-center>{{ menu.volume }} {{ menu.unit[lang] }}</div>
				<div data v-for="tour in tours" :key="tour.id">{{ extraMenuAmount(menu.id,tour.id) }}</div>
				<div data data-shift> {{ sumExtraMenu(menu.id) }}</div>
			</div>
		</div>

		<div :class="s.panel" v-if="isSpecialRequest()">
			<div title>{{ $t("tourFB.specialRequests") }}</div>
			<div white-row v-for="specialRequest in specialRequestsInfo" :key="specialRequest.id">
				<div info>{{ time(specialRequest.id)}}</div>
				<div info>{{ tourEnd(specialRequest.id)}}</div>
				<div info>{{ specialRequests(specialRequest.id)}}</div>
			</div>
		</div>

		<div :class="s.panel" v-if="isComment()">
			<div title>{{ $t("tourFB.comments") }}</div>
			<div white-row v-for="comment in commentsInfo" :key="comment.id">
				<div info>{{ time(comment.id)}}</div>
				<div></div>
				<div info>{{ comments(comment.id)}}</div>
			</div>
		</div>

		<div :class="s.button" v-if="this.tourId">
			<NuxtLink :to="localePath(`/tour-information/${this.tourId}`)"><BbButton>{{$t('buttons.back')}}</BbButton></NuxtLink>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters } from "vuex";
import s from "./style.module.scss"
import moment from 'moment'
import Calendar from "~/components/controls/calendar/calendarDate.vue";
import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"
import { SelectItem, Time } from "@/types/common"
import { DisplayFormatter } from "~/types/common";
import Currency from "~/components/display/currency.vue";
import DateTime, { DisplayType } from "~/components/display/dateTime.vue";
import {
  	TourFB,
	OrderTourMenu,
	OrderTourBeverage,
	City,
	TourFilter,
 	TourInfo,
	PrivateTourInfo,
	TourType, 
	TourState,
} from "@/types/tour";
import {
	Beverage,
	Menu,
  MenuTypeEnum
} from "@/types/booking";
import { Roles } from "~/types/private";

enum TourStateBackgroundColors {
	Green = "Green",
	Red = "Red",
    Yellow = "Yellow",
    Gray = "Gray",
    White = "White"
}

export default Vue.extend({
	name: "tour-fb",
  	components: { 
		Currency,
		DateTime,
		Calendar,
		controlItem,
		dropdown,
		checkbox
	},

	props: {
		withoutTitle: Boolean,
	},

	data() {
		return {
			s,
			DisplayFormatter,
			DisplayType,
			controlItem,
			dropdown,
			isId: null as Boolean| null,
			cityId: null as number | null,
			date: null as Date | null,
			tourId: null as number | null,
			TourType,
			TourState,
			checkedIds: [] as number[]
		}
	},

	computed: {
		lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
		...mapGetters({
			order: "booking/order",
			route: "booking/route",
		}),
		tour: {
			get(): TourFB {
				return this.$store.state.tour.tourFB;
			},
		},
		tours: {
			get(): TourInfo[] {
				return this.$store.state.tour.tourFB.toursInfo?.filter((x:TourInfo)=>x.status!=TourState.Draft);
			},
		},
		activeTours: {
			get(): TourInfo[] {
				return this.$store.state.tour.tourFB.toursInfo?.filter((x:TourInfo)=>x.status==TourState.Active);
			},
		},
		cityItem(): SelectItem[] {
			let cities = this.$store.state.tour.cities;
			return cities.length>0?
					cities.map((x:City) => new SelectItem(x.id, x.name?x.name[this.lang].toString():"")):
					[];
        }, 
		city: {
			get(): string {
				if(this.tourId==undefined) {
					return "";
				}
					
				let city = this.tours?.find(c=>c.id==this.tourId)?.city;
				return city?city[this.lang].toString():"";
			},
		},
		tourDate: {
			get(): string {
				if(this.tours==undefined) {
					return "";
				}
				let date = this.tours?.filter(c=>c.id==this.tourId)[0]?.departure;
				if(date==undefined){
					return "";
				}

				return this.lang=='ru'?new Intl.DateTimeFormat('ru-RU', { month: "long", day: "numeric", year: "numeric"}).format(new Date(date)).toLowerCase()
						:new Intl.DateTimeFormat('en-GB', { month: "long", day: "numeric", year: "numeric"}).format(new Date(date)).toLowerCase();
			},
		},
		tourNumber(): string {
			return this.tours?.filter(c=>c.id==this.tourId)[0]?.number ?? ''
		},		
		menus: {
			get(): Menu[] {
				if(this.tour?.menus==undefined) {
					return [];
				}

				let menus=this.$store.state.booking.menuInfo.menus
							.filter((item:Menu) => (this.tour.menus.some((x:OrderTourMenu) => x.menuId==item.id)));
				return menus;
			},
		},
		beverages: {
			get(): Beverage[] {
				if(this.tour?.beverages==undefined) {
					return [];
				}

				let beverages=this.$store.state.booking.menuInfo.beverages
								.filter((item:Beverage) => (this.tour.beverages.some((x:OrderTourBeverage) => x.beverageId==item.id)));
				return beverages;
			},
		},
		softDrinks: {
			get(): Beverage[] {
				if(this.tour?.beverages==undefined) {
					return [];
				}

				let beverages=this.$store.state.booking.menuInfo.beverages
								.filter((item:Beverage) => !item.alcoholByVolume && !item.isHot 
								&& (this.tour.extraBeverages.some((x:OrderTourBeverage) => x.beverageId==item.id)));
				return beverages;
			},
		},
		hotDrinks: {
			get(): OrderTourBeverage[] {
				if(this.tour?.beverages==undefined) {
					return [];
				}

				let beverages=this.$store.state.booking.menuInfo.beverages
								.filter((item:Beverage) => !item.alcoholByVolume && item.isHot 
								&& (this.tour.extraBeverages.some((x:OrderTourBeverage) => x.beverageId==item.id)));
				return beverages;
			},
		},
		alcoDrinks: {
			get(): OrderTourBeverage[] {
				if(this.tour?.beverages==undefined) {
					return [];
				}

				let beverages=this.$store.state.booking.menuInfo.beverages
								.filter((item:Beverage) => item.alcoholByVolume && (this.tour.extraBeverages.some((x:OrderTourBeverage) => x.beverageId==item.id)));
				return beverages;
			},
		},
		extraMenus: {
			get(): Menu[] {
				if(this.tour?.menus==undefined) {
					return [];
				}
				let menus=this.$store.state.booking.menuInfo.menus
							.filter((item:Menu) => (this.tour.extraMenus.some((x:OrderTourMenu) => x.menuId==item.id) && item.menuType.id!=MenuTypeEnum.Main));
				return menus;
			},
		},
		specialRequestsInfo: {
			get(): PrivateTourInfo[] {
				if(this.tour?.privateTourInfo==undefined) {
					return [];
				}
				let privateTourInfo=this.tour?.privateTourInfo
							.filter((item:PrivateTourInfo) => item.specialRequests!="");
				return privateTourInfo.length>0?privateTourInfo:[];
			},
		},
		commentsInfo: {
			get(): PrivateTourInfo[] {
				if(this.tour?.privateTourInfo==undefined) {
					return [];
				}
				let privateTourInfo=this.tour?.privateTourInfo
							.filter((item:PrivateTourInfo) => item.comment!="");
				return privateTourInfo.length>0?privateTourInfo:[];
			},
		},
		currentUserRole(): Roles {
			return this?.$auth?.user?.role;
		},   		
        endAvailableDate(): Date | null {
            if (this.currentUserRole == Roles.Crew) {
                var date = new Date()
                date.setDate(date.getDate() + 2);
                return date;
            } else {
                return null;
            }
        }
	},
	methods: {
		time(id: number): string {
			if(this.tours==undefined) {
				return "";
			}

			let tour=this.tours.find((x:TourInfo)=>x.id==id);
			return tour?.departure?moment(tour?.departure).format('HH:mm'):"";
		},
		tourEnd(id: number): string {
			if(this.time(id)=="") {
				return "";
			}

			let startTime = this.time(id)!=""?Time.fromString(this.time(id)):new Time(0,0,0);
			startTime.seconds=0;
			let duration = this.tours.find(x=>x.id==id)?.duration;
			let durationTime=duration!=undefined?Time.fromString(duration): new Time(0,0,0) ;
			let endTime=startTime.addTime(durationTime);
			return `0${endTime.hours}`.slice(-2) + ':' + `0${endTime.minutes}`.slice(-2);
		},
		tourType(id: number): string {
			let type= this.tours.find(x=>x.id==id)?.tourType;
			if(type==undefined) {
				return "-";
			}

			return this.lang=='ru'?
					(type==0?"Регулярный":"Частный"):
					(type==0?"Regular":"Private");
		},
		guestsNumber(id: number): string {
			let guestsNumber = this.tours.find(x=>x.id==id)?.guestsNumber;
			return guestsNumber?guestsNumber.toString():"-";
		},
		itinerary(id: number): string {
			let itinerary = this.tours.find(x=>x.id==id)?.itinerary;
			return itinerary?itinerary[this.lang]?.toString():"-";
		},
		duration(id: number): string {
			let duration = this.tours.find(x=>x.id==id)?.duration;
			if(duration==undefined) {
				return "-";
			}
			
			let time=duration!=undefined?Time.fromString(duration):new Time(0,0,0);
			return this.lang=='ru'?
					(time.minutes>0?
						time.hours.toString()+"ч "+time.minutes.toString()+"м":
						time.hours.toString()+"ч"):
					(time.minutes>0?
						time.hours.toString()+"h "+time.minutes.toString()+"m":
						time.hours.toString()+"h");
		},
		onTourChecked(items: any[], itemId: number, checked: boolean) {
            if (checked) {
                items.push(<Number>itemId);
            } else {
                const index = items.map(x => x.toString()).indexOf(itemId.toString());
                if (index > -1) {
                    items.splice(index, 1);
                }            
            }			
        },
		toursMenu(id: number): OrderTourMenu[]{
			let m=this.tour.menus.filter(x=>x.menuId==id);
				return this.tour.menus.filter(x=>x.menuId==id);
		},
		isActiveTour(id: number): Boolean{
			return this.$store.state.tour.tourFB.toursInfo?.find((x:TourInfo)=>x.id==id)?.status==TourState.Active?true:false;
		},
		menuAmount(menuId: number, tourId: number): string {
			let menu = this.tour.menus.find(x=>x.menuId==menuId && x.tourId==tourId);
			return (this.isActiveTour(tourId) && menu)?menu.amount.toString():"-";
		},
		sumMenu(menuId: number): string {
			let sum= 0;
			let menus = this.tour.menus.filter(item=>item.menuId==menuId && this.checkedIds.some(x=>x==item.tourId && this.isActiveTour(item.tourId)));
			menus.forEach(item=>{
				sum=sum+item.amount
			})
			return sum.toString();
		},
		beverageAmount(beverageId: number, tourId: number): string {
			let beverage = this.tour.beverages.find(x=>x.beverageId==beverageId && x.tourId==tourId);
			return this.isActiveTour(tourId) && beverage?beverage.amount.toString():"-";
		},
		sumBeverage(beverageId: number): string {
			let sum= 0;
			let beverages = this.tour.beverages.filter(item=>item.beverageId==beverageId && this.checkedIds.some(x=>x==item.tourId && this.isActiveTour(item.tourId)));
			beverages.forEach(item=>{
				sum=sum+item.amount
			})
			return sum.toString();
		},
		extraBeverageAmount(beverageId: number, tourId: number): string {
			let beverage = this.tour.extraBeverages.find(x=>x.beverageId==beverageId && x.tourId==tourId);
			return this.isActiveTour(tourId) && beverage?beverage.amount.toString():"-";
		},
		sumExtraBeverage(beverageId: number): string {
			let sum= 0;
			let beverages = this.tour.extraBeverages.filter(item=>item.beverageId==beverageId && this.checkedIds.some(x=>x==item.tourId && this.isActiveTour(item.tourId)));
			beverages.forEach(item=>{
				sum=sum+item.amount
			})
			return sum.toString();
		},
		extraMenuAmount(menuId: number, tourId: number): string {
			let menu = this.tour.extraMenus.find(x=>x.menuId==menuId && x.tourId==tourId);
			return this.isActiveTour(tourId) && menu?menu.amount.toString():"-";
		},
		sumExtraMenu(menuId: number): string {
			let sum= 0;
			let menus = this.tour.extraMenus.filter(item=>item.menuId==menuId && this.checkedIds.some(x=>x==item.tourId) && this.isActiveTour(item.tourId));
			menus.forEach(item=>{
				sum=sum+item.amount
			})
			return sum.toString();
		},
		specialRequests(tourId:number): string {
			let specialRequests = this.specialRequestsInfo.find(item => item.id==tourId)?.specialRequests;
			return specialRequests??"";
		},
		comments(tourId:number): string {
			let comment = this.commentsInfo.find(item => item.id==tourId)?.comment;
			return comment??"";
		},
		isHot(): boolean {
			return this.hotDrinks.length>0? true: false;	
		},
		isSoft(): boolean {
			return this.softDrinks.length>0? true: false;	
		},
		isAlco(): boolean {
			return this.alcoDrinks.length>0? true: false;	
		},
		isExtra(): boolean {
			return this.extraMenus.length>0? true: false;	
		},
		isSpecialRequest(): boolean {
			return this.specialRequestsInfo.length>0? true: false;	
		},
		isComment(): boolean {
			return (this.$route.query.tourId!=undefined && this.commentsInfo.length>0)? true: false;	
		},
		setDateOrCity(): void{
			this.getTour();
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
		async getCities(){
        	await this.$store.dispatch("tour/getCities");
    	},
		async getTour(){
			const filter: TourFilter = {
                cityId: this.cityId ?? undefined,
                tourTypes: undefined,
				date: this.date?moment( this.date).format('YYYY-MM-DD'):undefined,
                departureDateTo: undefined,
                withoutOrders: undefined,
				hasOrders: true,
                ids: undefined,
				states: [TourState.Active]
            }
        	await this.$store.dispatch("tour/getTourFB",filter);
			if(this.checkedIds.length==0)
				this.tours?.forEach((item:TourInfo)=>{
					this.checkedIds.push(item.id)
				});
    	},
	},
	created() {
        this.getCities();
		this.tourId = Number(this.$route.query.tourId)?? null;
		if(this.$route.query.tourId!=undefined) {
			this.isId = true;
		}
		else {
			this.cityId = 1;
			this.date = new Date();
			this.isId = false;
		}	

		const filter: TourFilter = {
                cityId: this.cityId?this.cityId:undefined,
                tourTypes: undefined,
                date: this.date?moment( this.date).format('YYYY-MM-DD'):undefined,
                departureDateTo: undefined,
                withoutOrders: undefined,
				hasOrders: true,
                ids: this.tourId?[this.tourId]:undefined
            }
		this.$store.dispatch("tour/getTourFB",filter);
		if(this.checkedIds.length==0)
			this.tours?.forEach((item:TourInfo)=>{
				this.checkedIds.push(item.id)
			});
		
	},
});
</script>