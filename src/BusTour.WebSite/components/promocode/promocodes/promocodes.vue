<template>
	<div :class="s.page">
		<h1>Promocodes</h1>

		<div :class="s.pageFilter">
			<ControlItem title="city" :class="s.filterCity">
				<dropdown v-model="cityId" :items="cityList" /> 
			</ControlItem>

			<ControlItem title="Status" :class="s.filterStatuses">
				<Checkbox v-model="isActive" label="Active" :text-backward="true" :class="s.filterCheckbox" />
				<Checkbox v-model="isExpiredOrAmended" label="Expired" :text-backward="true" :class="s.filterCheckbox" />
			</ControlItem>

			<BbButton text="Filter" :theme="ButtonTheme.White" :class="s.filterButton" @click="fetchData" />
		</div>
	
		<BbGrid :data="promoCodesGridData" :class="s.pageGrid" @update="onUpdate" bodyMaxHeight="50vh">
			<BbGridColumn title="Series number" field="seriesNumber" />
			<BbGridColumn title="Amount of discount" field="amountOfDiscount" />
			<BbGridColumn title="Discount percentage" field="discountPercentage" />
			<BbGridColumn title="Creation date" field="creationDate" width="200px" />
			<BbGridColumn title="Date start" field="dateStart" width="200px" />
			<BbGridColumn title="Expiration date" field="expirationDate" width="200px" 
				:editable="true" 
				:editableCondition="(item) => { return item.promoCodeType == PromocodeType.TimeAndQuantity || item.promoCodeType == PromocodeType.Time }"
			/>
			<BbGridColumn title="Issue amount" field="numberOfPromocodes" 
				:editable="true"
				:editableCondition="(item) => { return item.promoCodeType == PromocodeType.TimeAndQuantity || item.promoCodeType == PromocodeType.Quantity }"
			/>
			<BbGridColumn title="Used Issues" field="quantityUsed" />
			<BbGridColumn title="Total discount" field="discountPlanned" />
			<BbGridColumn title="Used discount" field="discountUsed" />
			<BbGridColumn title="Status" field="isActive" :editable="true" />
			<BbGridColumn field="Amend" width="200px" />

			<template #creationDate="{row}">
				<DateTime variant="long" :value="row.creationDate" :type="DisplayType.Date" />
			</template>
			<template #dateStart="{row}">
				<DateTime variant="long" :value="row.dateStart" :type="DisplayType.Date" />
			</template>
			<template #expirationDate="{row}">
				<DateTime variant="long" :value="row.expirationDate" :type="DisplayType.Date" />
			</template>
			<template #amountOfDiscount="{row}">
				<div v-if="row.discountType == DiscountType.Cash">
					<Currency :value="row.amountOfDiscount" :adjustFont="false" />
				</div>
				<div v-else></div>	
			</template>
			<template #discountPercentage="{row}">
				{{ row.discountType == DiscountType.Percent ? row.amountOfDiscount : "" }}
			</template>
			<template #numberOfPromocodes="{row}">
				{{ (row.numberOfPromocodes !== 0 && !row.numberOfPromocodes) ? 'quantity not limited' : row.numberOfPromocodes }}
			</template>
			<template #expirationDate:Editor="{applyAmendment, syncCallback, value, row}">
				<Calendar
					:wrapClass="s.pageGrid"
					:class="[s.pageGridEditor, s.pageDateEditor]" 
					:value="value ? new Date(value) : new Date()" 
					:start-available-date="new Date(row.dateStart)"
					@input="ev => { syncCallback(ev); applyAmendment(); }" 
				/>
				<!-- <calendar :lang="this.lang" v-model="$v.dateEnd.$model" :unavailableDates="unavailableDates" :startAvailableDate="dateStart"/>  -->
			</template>
			<template #numberOfPromocodes:Editor="{syncCallback, value}">
				<ControlItem :is-hidden-title="true" :class="s.pageGridEditor">
					<NumericInput variant="view" :value="parseInt(value)" @input="syncCallback($event)" />
				</ControlItem>
			</template>
			<template #isActive="{row}">
				{{row.isActive == true ? 'Active' : 'Inactive'}}
			</template>
			<template #isActive:Editor="{applyAmendment, syncCallback, value}">
				<!-- <Checkbox :value="value" :text-backward="true" :label="value == true ? 'Active' : 'Inactive'" :class="s.filterCheckbox" @input="syncCallback($event)" /> -->
				<dropdown 
					:value="value ? 1 : 0" 
					:items="promocodeStatusList"  
					@input="ev => { syncCallback(ev); applyAmendment(); }" 
				/> 
			</template>
			<template #Amend="{row}">
				<BbButton @click="editOrUpdateRow(row)">{{ getAmendingControllingButtonText(row) }}</BbButton>
			</template>
		</BbGrid>

		<div :class="s.pageButtonsWrapper">
			<ExcelExport name="PromoCodes.xls" :data="promoCodesGridData" :fields="exportFieldsSettings" >
				<BbButton text="Export to Excel" />
			</ExcelExport>

			
			<nuxt-link v-if="userRole == Roles.Supervisor" :to="localePath('/promocode-creation')">
				<BbButton >
					CREATE PROMOCODE
				</BbButton>
			</nuxt-link>
		</div>	
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import BbGrid from "@/components/bb-grid/bb-grid.vue";
import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue";

import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import TextInput from "@/components/controls/text-input/text-input.vue";
import Calendar from "@/components/controls/calendar/calendarDate.vue";
import ControlItem from "@/components/controlItem/controlItem.vue";
import Checkbox from "@/components/controls/checkbox/checkbox.vue";
import DateTime, { DisplayType } from "@/components/display/dateTime.vue";
import Currency from "@/components/display/currency.vue";
import NumericInput from "@/components/controls/numeric/numeric.vue";
import ExcelExport from "vue-json-excel";

import { mapActions, mapGetters, mapState } from "vuex";
import { mapProps } from "@/store/helpers";
import { GridRowEditingState, IGridElement } from "~/components/bb-grid/bb-grid-types";
import { DiscountType, Promocode, PromocodeType } from "~/types/promocodes";
import { Roles } from "~/types/private";
import { SelectItem } from "@/types/common"
import { City } from "@/types/tour"

export default Vue.extend({
	components: {
		BbGrid,
		BbGridColumn,
		BbButton,
		TextInput,
		Calendar,
		ControlItem,
		Checkbox,
		DateTime,
		Currency,
		NumericInput,
		ExcelExport,
	},

	data() {
		return {
			s,
			ButtonTheme,
			DisplayType,
			Roles,
			DiscountType,
			cityId: null as number | null,
			PromocodeType
		}	
	},

	async created() {
		await this.fetchData()
		if (this.cityList.length) {
			this.cityId = parseInt(this.cityList[0].value.toString());
		}
	},

	computed: {
        lang(): string {
            return this.$i18n.locale
        },		
		...mapProps(
			[
				"city",
				"isActive",
				"isExpiredOrAmended"
			],
			"promocodes",
			"promoCodesGridFilter",
			"setPromoCodesGridFilter"
		),
		...mapGetters(
			"promocodes",
			["promoCodesGridData"]	
		),
		...mapState('tour', ['cities']),
		userRole(): Roles | null {
			return this.$auth?.user?.role;
		},
        cityList(): SelectItem[] {
            return this.cities?.map((x: City) => new SelectItem(x.id, x.name[this.lang])) ?? [];
        },		
		promocodeStatusList(): SelectItem[] {
            return [{
					value: 1,
					text: 'Active',
				},{
					value: 0,
					text: 'Inactive',
				}] as SelectItem[];
        },
		exportFieldsSettings(): any {
			const sourceFields = [
				"seriesNumber",
				"amountOfDiscount",
				"numberOfPromocodes",
				"discountType",
				"creationDate",
				"dateStart", 
				"expirationDate",
				"quantityIssued",
				"quantityUsed",
				"discountPlanned",
				"discountUsed",
				"isActive",
			]

			let fieldsSettings: any = {};

			sourceFields.forEach((field: string) => {
				let key = field
					.replace(/[A-Z]/g, (match, idx) => ` ${idx != 0 ? match.toLowerCase() : match}`);
				
				key = key[0].toUpperCase() + key.slice(1);

				fieldsSettings[key] = field;
			});

			const moneyFormat = (field: string) => ({
				field,
				callback: (value: string) => `£${value}`
			});

			const dateFormat = (field: string) => ({
				field,
				callback: (value: string) => value 
					? new Intl.DateTimeFormat(
						'en-GB', { 
							month: "long", 
							day: "numeric", 
							year: "numeric"
						}
					).format(new Date(value))
					: "",
			});

			fieldsSettings["Amount of discount"] = moneyFormat("amountOfDiscount");
			fieldsSettings["Creation date"] = dateFormat("creationDate");
			fieldsSettings["Date start"] = dateFormat("dateStart");
			fieldsSettings["Expiration date"] = dateFormat("expirationDate");

			return fieldsSettings;
		}
	},

	methods: {
		...mapActions(
			"promocodes",
			[
				"getPromoCodesGridData", 
				"updatePromoCode",
			]
		),
		editOrUpdateRow(row: any): void {
			this.$emit("bb-grid-edit-or-update-row", row);
		},
		getAmendingControllingButtonText(row: any): string {
			return row.editingState == GridRowEditingState.Enabled
				? "save"
				: "edit";
		},
		async fetchData(): Promise<void> {
			this.getPromoCodesGridData();
		},
		onUpdate(updatedRow: IGridElement): void {
			if (!updatedRow.isDirty) {
				return;
			}

			let promoCode = new Promocode();

			promoCode.id = updatedRow.id;
			promoCode.promoCodeType = updatedRow.promoCodeType;
			promoCode.dateStart = updatedRow.dateStart;
			promoCode.dateEnd = updatedRow.expirationDate;
			promoCode.numberOfPromocodes = updatedRow.numberOfPromocodes;
			promoCode.amountOfDiscount = updatedRow.amountOfDiscount;
			promoCode.typeOfDiscount = updatedRow.discountType;
			promoCode.seriesNumber = updatedRow.seriesNumber;
			promoCode.isActive = !!updatedRow.isActive;

			this.updatePromoCode(promoCode);
		},
	},
})
</script>