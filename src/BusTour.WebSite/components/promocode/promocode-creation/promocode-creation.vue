<template>
	<div>
		<h1>Promocode creation</h1>

		<div :class="s.fieldsContainer">
			<div :class="[s.greedy]">
				<ControlItem 
					title="Series number"
					:error="seriesNumberError"
					:class="s.fieldsContainerControl"
				>
					<TextInput v-model="$v.seriesNumber.$model" />
				</ControlItem>

				<ControlItem 
					title="City" 
					:class="s.fieldsContainerControl"
					:error="$v.cityId.$anyError ? 'This field is required' : ''"
				>
					<Dropdown v-model="$v.cityId.$model" :items="citiesList" />
				</ControlItem>
			</div>

			<div :class="s.greedy">
				<ControlItem title="Promocode type" :class="s.fieldsContainerControl">
					<DropDown v-model="promoCodeType" :items="promocodeTypes" @input="onPromocodeTypeChange" />
				</ControlItem>
			</div>

			<div v-if="dateControlsVisible" :class="s.greedy">
				<ControlItem 
					title="Date start" 
					:class="[s.fieldsContainerControl, s.calendarAutoHeight]"
					:error="$v.dateStart.$anyError && !$v.dateStart.required 
						? 'This field is required' 
						: !$v.dateStart.isValid 
							? 'Start date must be earlier than end date' 
							: ''
					"
				>
					<Calendar v-model="$v.dateStart.$model" />
				</ControlItem>

				<ControlItem 
					title="Date end" 
					:class="s.fieldsContainerControl"
					:error="$v.dateEnd.$anyError && !$v.dateEnd.required ? 'This field is required' : ''"
				>
					<Calendar v-model="dateEnd" />
				</ControlItem>
			</div>

			<div v-if="quantityControlVisible" :class="s.greedy">
				<ControlItem 
					title="Number of uses" 
					:class="[s.fieldsContainerControl, s.numericFieldPadding]"
					:error="$v.numberOfPromocodes.$error ? 'This field is required' : ''"
				>
					<TextInput :mask="integerVueMask" v-model="$v.numberOfPromocodes.$model" />
				</ControlItem>
			</div>

			<ControlItem 
				v-if="DiscountType.Cash == typeOfDiscount" 
				title="Amount of discount" 
				:class="[s.fieldsContainerControl, s.numericFieldPadding]"
				:error="$v.amountOfDiscount.$error ? 'This field is required' : ''"
			>
				<TextInput :mask="currencyVueMask" v-model="$v.amountOfDiscount.$model" />
			</ControlItem>

			<ControlItem 
				v-else 
				title="Amount of discount" 
				:class="s.fieldsContainerControl"
				:error="$v.amountOfDiscount.$error ? 'This field is required' : ''"
			>
				<Dropdown v-model="$v.amountOfDiscount.$model" :items="discountProcentsList" />
			</ControlItem>

			<ControlItem title="Value" :class="s.fieldsContainerControl">
				<Dropdown v-model="typeOfDiscount" :items="discountUnits" @input="clearDiscountAmount" />
			</ControlItem>
		</div>

		<BbButton text="Create" :theme="ButtonTheme.White" :class="s.creationButton" @click="createPromocode" />
		<Modal ref="Modal" />
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";

import { mapActions, mapMutations, mapGetters } from "vuex";

import { mapProps } from "@/store/helpers";
import { DiscountType, Promocode, PromocodeType } from "@/types/promocodes";
import { SelectItem } from "~/types/common";

import ControlItem from "@/components/controlItem/controlItem.vue";
import DropDown from "@/components/controls/dropdown/dropdown.vue";
import Calendar from "@/components/controls/calendar/calendarDate.vue";
import NumericInput from "@/components/controls/numeric/numeric.vue";
import TextInput from "@/components/controls/text-input/text-input.vue";
import Modal from "@/components/controls/modal/modal.vue";
import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import { City } from "~/types/tour";
import { integerVueMask, currencyVueMask } from '@/utils/mask';

export default Vue.extend({
	components: {
		ControlItem,
		DropDown,
		Calendar,
		NumericInput,
		TextInput,
		BbButton,
		Modal,
	},

	data() {
		return {
			s,
			ButtonTheme,
			DiscountType,
			discountProcents: [],
			integerVueMask,
			currencyVueMask
		}	
	},

	async created() {
		this.getCities();
		this.discountProcents = await this.getDiscountAmount();
	},

	beforeDestroy() {
		this.clearPromoCode();
	},

	computed: {
		...mapProps([
				"promoCodeType",
				"dateStart",
				"dateEnd",
				"numberOfPromocodes",
				"amountOfDiscount",
				"typeOfDiscount",
				"seriesNumber",
				"cityId",
			],
			"promocodes",
			"promocode",
			"setPromocode",
		),
		...mapGetters("tour", [
			"cities",
		]),
		promocodeTypes(): SelectItem[] {
			return SelectItem
				.fromEnum(PromocodeType)
				.map((x: SelectItem): SelectItem => ({
					...x, 
					text: x.text.replace(/[A-Z]/g, (match, idx) => ` ${idx != 0 ? match.toLowerCase() : match}`)
				}));
		},
		discountUnits(): SelectItem[] {
			return SelectItem.fromEnum(DiscountType, {
				Percent: "%",
				Cash: "£"
			});
		},
		discountProcentsList(): SelectItem[] {
			return this.discountProcents?.map(x => new SelectItem(x, x)).map(x => ({...x, value: +x.value})) ?? [];
		},
		dateControlsVisible(): boolean {
			return (this as any).promoCodeType != PromocodeType.Quantity;
		},
		quantityControlVisible(): boolean {
			return (this as any).promoCodeType != PromocodeType.Time;
		},
		seriesNumberError(): string {
			let error = "";

			if (!this.$v.seriesNumber.$anyError) {
				return error;
			}

			if (!this.$v.seriesNumber.required) {
				error = "This field is required";
			} else if (!this.$v.seriesNumber.isValid) {
				error = "The field must contain only letters and numbers";
			} else if (!this.$v.seriesNumber.length) {
				error = "The length should be in the range from 4 to 20";
			}

			return error;
		},
		lang(): string {
			return this.$i18n.locale;
		},
		citiesList(): SelectItem[] {
			return this.cities.map((city: City) => new SelectItem(city.id, city.name[this.lang]));
		},
	},

	validations() {
		return {
			numberOfPromocodes: {
				required(value: any, vm: any): boolean {
					if (![PromocodeType.Time, PromocodeType.TimeAndQuantity].includes(+vm.promoCodeType)) {
						return value > 0;
					}

					return true; 
				}
			},
			amountOfDiscount: {
				required(value: any): boolean {
					return value != undefined && value != 0;
				}
			},
			dateStart: {
				isValid(value: any, vm: any): boolean {
					if (vm.promoCodeType != PromocodeType.Quantity && (!!value && !! vm.dateEnd)) {
						return value <= vm.dateEnd;
					}

					return true;
				},
				required(value: any, vm: any): boolean {
					if (vm.promoCodeType != PromocodeType.Quantity) {
						return !!value;
					}

					return true;
				},
			},
			dateEnd: {
				required(value: any, vm: any): boolean {
					if (vm.promoCodeType != PromocodeType.Quantity) {
						return !!value;
					}

					return true;
				},
			},
			seriesNumber: {
				required(value: string): boolean {
					return !!value;
				},
				isValid(value: string): boolean {
					const regExp = /[^a-zA-Z0-9]/g;

					return !regExp.test(value);
				},
				length(value: string): boolean {
					value = value ? value : "";

					return Array.from({ length: 17 }, (_, idx) => idx + 4).includes(value.length);
				}
			},
			cityId: {
				required(value: any): boolean {
					return !!value;
				}
			},
		}
	},

	methods: {
		...mapActions({
			"savePromocode": "promocodes/createPromocode",
			"getDiscountAmount": "promocodes/getAmountOfDiscount",
			"getCities": "tour/getCities",
		}),
		...mapMutations(
			"promocodes", 
			[
				"clearPromoCode",
				"setPromocode",
			]
		),
		clearDiscountAmount(): void {
			(<any>this).amountOfDiscount = undefined;
			this.$v.$reset();
		},
		async createPromocode(): Promise<void> {
			this.$v.$touch();

			if (this.$v.$invalid) {
				return;
			}

			const response = await this.savePromocode();
			let message = "";
			let title = "";

			if (response === true) {
				message = "The promo code was successfully created";
				title = "Information";

				this.clearPromoCode();
				this.$v.$reset();
			} else if (typeof(response) == "string") {
				message = response;
				title = "Error";
			} else {	
				message = "Something went wrong ... ";
				title = "Error";
			}

			await (this as any).$refs.Modal.open(message, title);
		},
		onPromocodeTypeChange(event: PromocodeType): void {			
			this.setPromocode({
				dateStart: undefined,
				dateEnd: undefined,
				numberOfPromocodes: undefined,
			} as Promocode);

			(<any>this).promoCodeType = event;

			this.$v.$reset();
		},
	}
})
</script>