<template>
	<div :class="s.form">

		<control-item :title="$t('booking.guests')" :isRequired="true" :class="s.formGuestsControl">
			<drop-down v-model="guestCount" :items="guestsNumber" :disabled="disabled" />
		</control-item>

		<control-item :title="$t('booking.tableRequirements')" :isRequired="true" :class="s.formTableControl">
			<drop-down v-model="seatingType" :items="tableTypes" :disabled="!editingMode && disabled" />
		</control-item>

		<div :class="s.greedy">
			<control-item :class="[s.formRouteControl]" :title="$t('booking.route')" :isRequired="true">
				<dropdown v-model="routeId" :items="routeItems"  :disabled="disabled"/> 
			</control-item>
		</div>		

		<control-item :title="$t('booking.date')" :isRequired="true" :class="s.formCalendar">
			<calendar :lang="this.lang" v-model="date" :availableDates="tourDates" :noSeatToursDates="noSeatToursDates"/>
		</control-item>

		<control-item :title="$t('booking.time')" :isRequired="true" :class="s.formTimeControl">
			<time-picker v-model="time" :times="tourTimes"/>
		</control-item>

		<div :class="s.greedy">
			<control-item :no-back="true">
				<checkbox 
					v-model="guestsWithDisabilities"
					:label="$t('booking.guestsWithDisabilities')" 
					:text-backward="true"
					:class="s.formCheckboxControl"
					:disabled="disabled"
					variant="wrapSpaces"
				/>
			</control-item>
		</div>
		
		<div v-if="guestsWithDisabilities" :class="s.greedy">
			<control-item  
				:title="$t('booking.guests')" 
				:isRequired="true" 
				:class="s.formGuestsControl"
			>
				<drop-down v-model="disabledGuestCount" :items="disabledGuestsNumber" />
			</control-item>
		</div>

		<control-item :class="s.formTextControl" :title="$t('booking.giftCertificate')" :success="isBadCertificate === false ? $t('booking.giftCertificateCorrect') : ''" :error="isBadCertificate === true ? $t('booking.giftCertificateNotCorrect') : ''">
			<TextInput v-model="certificateNumber" @blur="checkCertificate" />
		</control-item>
		
		<control-item :class="[s.formTextControl,s.formTextControlNarrow]" :title="$t('booking.promoCode')" :success="isBadPromoCode === false ? $t('booking.promocodeCorrect') : ''" :error="isBadPromoCode === true ? $t('booking.promocodeNotCorrect') : ''">
			<TextInput v-model="promoCodeDisplay" @blur="checkPromoCode"/>
		</control-item>
	</div>
</template>

<script lang="ts">
import s from "./style.module.scss";
import Vue from "vue";
import config from "~/config";
import moment from "moment";


import { mapProps } from "~/store/helpers";
import { mapActions, mapMutations, mapGetters, mapState } from "vuex"; 
import { SelectItem, Time } from "~/types/common";

import DropDown from "@/components/controls/dropdown/dropdown.vue";
import ControlItem from "@/components/controlItem/controlItem.vue";
import Checkbox from "@/components/controls/checkbox/checkbox.vue";
import TextInput from "~/components/controls/text-input/text-input.vue";
import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue";
import Calendar from "~/components/controls/calendar/calendarDate.vue";
import TimePicker from "~/components/controls/timepicker/timepicker.vue";
import { SeatingType, Tour, Route, Table } from "~/types/booking";
import { CertificateFilter , GiftCertificate, GiftCertificateStatus} from "@/types/giftCertificate"
import { Promocode, PromoCodeValidateFilter } from "~/types/promocodes/index"

export default Vue.extend({
	components: {
		DropDown,
		ControlItem,
		Checkbox,
		TextInput,
		BbButton,
		Calendar,
		TimePicker,
	},

	props: {
     disabled: {
		type: Boolean,
		required: false,
		default: false,
    	},
  	},

	data() {
		return {
			s,
			ButtonTheme,
			isBadCertificate: null as boolean | null,
			hideCertificateField: false,
			isBadPromoCode: null as boolean | null,
			test: config.supportEmail
		}
	},

	inject: ["editingMode"],

	computed: {
		...mapProps (
			[
				"guestCount",
				"seatingType",
				"date",
				"time",
				"guestsWithDisabilities",
				"disabledGuestCount",
				"promoCode",
				"certificateId",
				"certificateNumber",
				"tourId",
				"routeId"
			], 
			"booking", 
			"order", 
			"setOrder",
		),
	    ...mapState('tour', ['routes']),	
		...mapGetters({
			route: "booking/route",
		}),
		lang(): string {
			return this.$i18n.locale;
		},
		promoCodeDisplay: {
			get() {
				return typeof this.promoCode === 'string' ? this.promoCode : (this.promoCode?.seriesNumber ?? '');
			},
			set(val) {
				this.promoCode = val;
			}			
		},
		guestsNumber(): SelectItem[] {
			return this.getGuestNumbersList(config.maxGuestCount);
		},
		disabledGuestsNumber(): SelectItem[] {
			return this.getGuestNumbersList(config.maxDisabledGuestCount);
		},
 		tableTypes(): SelectItem[] {
			return SelectItem.fromEnum(SeatingType, {
				"SeparateTable": this.$t("booking.separateTable"),
				"BySeats": this.$t("booking.shareTable")
			});
		},
		tours(): Tour[] {
            return this.$store.state.booking.routeInfo.tours;
        },
		noSeatToursDates(): Date[] {
			let notAvailableTours = this.tours.filter(x => !x.isAvailableForBooking);
			
			return notAvailableTours.map(tour => {
				let date = new Date(tour.departure);

				let thisDayTours = this.tours.filter(x => moment(new Date(x.departure)).format('DD.MM.YYYY') == moment(new Date(date)).format('DD.MM.YYYY'));
				let thisDayToursCountNoSeat = thisDayTours.filter(x => x.isAvailableForBooking === false && x.tables.reduce((a , t) => t.seats.length + a ,0) === tour.occupiedSeatsCount).length;
				
				if(thisDayTours.length === thisDayToursCountNoSeat) {
					return date;
				}
			}) as Date[];
        },
		tourDates(): Date[] {
			let availabledTours = this.tours.filter(x => x.isAvailableForBooking);
			return availabledTours.filter(x => x.occupiedSeatsCount === 0 || 
										       x.tables.map((x: Table) => x.seats.length).reduce((a, b) => a + b, 0) >= x.occupiedSeatsCount + Number(this.guestCount))
				.map((x: Tour) => new Date(x.departure));
        },
		tourTimes(): Time[] {
			const date = (this as any).date;
			const time = (this as any).time;

			if (!date) {
				return [];
			} else {
				let times = this.tours.filter(x => x.isAvailableForBooking &&
				(x.occupiedSeatsCount === 0 || x.tables.map((x: Table) => x.seats.length).reduce((a, b) => a + b, 0) >= x.occupiedSeatsCount + Number(this.guestCount)))
					.map((x: Tour) => new Date(x.departure))
					.filter(x => moment(new Date(x)).format('DD.MM.YYYY') == moment(date).format('DD.MM.YYYY'))
					.sort((x,y) => x.getTime() - y.getTime())
					.map(x => Time.fromDate(x));

				let timesStr = times.map((x) => x.toString())      
				timesStr = timesStr.filter((x, i) => timesStr.indexOf(x) === i)    
				times = timesStr.map((x) => Time.fromString(x))     

				if (!time || (times.length && !times.some(x => x.equals(time)))) {
					(this as any).time = times[0];
				}

				return times;
			}
		},
		routeItems(): SelectItem[] {
			return (this.routes ?? []).map((x: Route) => new SelectItem(x.id, x.name[this.lang]));
		}
	},

	methods: {
		...mapActions("booking", [
			"getSelectionModel",
			"getRouteInfo",
			"getEditableBusModel",
			"getBuses"
		]),
		...mapActions('giftCertificate', ['getGiftCertificates']),
		...mapActions('promocodes', ['getPromoCodeWithValidation']),
		...mapMutations("booking", ["setTour","clearSelectedObjects"]),
		
		getDateString(date: Date): string{
			return date.toLocaleDateString('en-us', { year:"numeric", month:"short", day:"numeric"});
		},
		getGuestNumbersList(maxGuestCount: number): SelectItem[] {
			const max = maxGuestCount;
			const min = 1;

			let availableNumbers = Array<number>(max - min).fill(0).map((_, idx) => idx + 1);
			let dataSource: SelectItem[] = availableNumbers.map(item => new SelectItem(item, item.toString()));

			dataSource.push(new SelectItem(max + 1, `${max}+`));

			return dataSource;
		},
		refreshTour() {
			const oldTourId = (this as any).tourId;
			this.setTour();
			const newTourId = (this as any).tourId;
			if (oldTourId != newTourId) {
				this.clearSelectedObjects()
				this.getBusModel()
			}
		},
		async checkCertificate() {
			const certificateNumber = this.certificateNumber as string;
			
			if(!certificateNumber) {
				this.isBadCertificate = null;
				this.$store.commit("booking/resetOrderCertificate");
				return;
			}

			const filter: CertificateFilter = {
				number: certificateNumber,
				statuses: [ GiftCertificateStatus.Active ]
			}
			
			const certificate = (await this.getGiftCertificates(filter))[0];

			if ( !certificate || certificate.status != GiftCertificateStatus.Active || new Date(certificate.dateStart) > new Date()){
				this.isBadCertificate = true;
				this.$store.commit("booking/resetOrderCertificate");
				return
			}

			this.isBadCertificate = false;
			this.$store.commit("booking/setOrderCertificate", certificate);
		},
		async checkPromoCode() {
			const seriesNumber = this.promoCodeDisplay;

			if(!seriesNumber) {
				this.isBadPromoCode = null;
				this.$store.commit("booking/resetOrderPromoCode");
				return;
			}
			
			const filter: PromoCodeValidateFilter = {
				seriesNumber: seriesNumber,
				cityId: this.route.cityId
			}

			const promoCode = await this.getPromoCodeWithValidation(filter) as Promocode;

			if(promoCode === undefined) {
				this.isBadPromoCode = true;
				this.$store.commit("booking/resetOrderPromoCode", {
					isBadPromoCode: true
				});
				return
			} 
			
			this.isBadPromoCode = false;
			promoCode.isBadPromoCode = false;
			this.$store.commit("booking/setOrderPromoCode", promoCode);
		},
		async getBusModel(): Promise<void> {
			if ((<any>this).editingMode) {
				return await this.getEditableBusModel();
			} 
				
			return await this.getSelectionModel();
		},
	},

	created() {
		this.$watch("guestCount", (value: any) => {
			this.clearSelectedObjects()
			this.getBusModel()
		});
		if (this.routes?.length > 0) {
			this.routeId = this.routes[0].id;
		}
	},

	watch: {
		date(value) {
			this.refreshTour()
		},
		time(value) {
			this.refreshTour()
		},
		seatingType(value) {
			this.clearSelectedObjects()
			this.getBusModel()
		},
		certificateNumber(value: string) {
			this.isBadCertificate = null;

			if(value === '') {
				this.$store.commit("booking/resetOrderCertificate");
			}
		},
		promoCode(value: string) {
			this.isBadPromoCode = null;

			if(value === '') {
				this.$store.commit("booking/resetOrderPromoCode");
			}
		},
		async routeId() {
			await this.getRouteInfo({ routeId: this.routeId, ignoreGuestCountReset: true });
		},
		guestsWithDisabilities(value: boolean, oldValue: boolean) {
			if (!value && value != oldValue) {
				this.disabledGuestCount = "0";
			}
			if (value && value != oldValue) {
				this.disabledGuestCount = "1";
			}			
			this.clearSelectedObjects()
			this.getBusModel()
		}
	}
});
</script>