<template>
	<div :class="s.orderList">
		<h3 :class="s.orderListTitle">Booking information</h3>

		<BbGrid column-spacing="30px" :data="data" :pager="false" background="#F5F5F5">
			<BbGridColumn field="clientName" :cellClass="s.bbGridColumnRelative" title="Name"/>
			<BbGridColumn field="clientPhone" :cellClass="s.bbGridColumnRelative" title="Phone"/>	
			<BbGridColumn field="clientEmail" :cellClass="s.bbGridColumnRelative" title="Email"/>			
			<BbGridColumn field="orderState" :cellClass="s.bbGridColumnRelative" title="Status"/>
			<BbGridColumn field="conflict" :cellClass="s.bbGridColumnCustom" title="Conflict"/>
			<BbGridColumn field="came" :cellClass="s.bbGridColumnCustom" title="Came"/>
			<BbGridColumn field="Buttons" :cellClass="s.bbGridColumnCustom" />

			<template #clientName="{row}">
				<input :class="[s.blockCellTemplate, s.blockCellTemplateWhite]" type="text" :value="row.client ? row.client.fullName : '' " readonly>
			</template>

			<template #clientPhone="{row}">
				<input :class="[s.blockCellTemplate, s.blockCellTemplateWhite]" type="text" :value="row.client ? row.client.phoneNumber : '' " readonly>
			</template>		

			<template #clientEmail="{row}">
				<input :class="[s.blockCellTemplate, s.blockCellTemplateWhite]" type="text" :value="row.client ? row.client.email : '' " readonly>
			</template>	

			<template #orderState="{row}">
				<div :class="[s.blockCellTemplate, s[`blockCellTemplate${getOrderStateBackgroundColor(row.orderState)}`]]">
					{{ row.orderState ? $t(`enums.OrderState.${OrderState[row.orderState]}`) : "" }}
				</div>
			</template>

			<template #conflict="{row}">
				{{ row.conflict ? "yes" : "no" }}
			</template>

			<template #came="{row}">
				{{ getGuestsCount(row.orderId) }}
			</template>

			<template #Buttons="{row}">
				<div :class="s.gridRowButtonsWrapper">
					<NuxtLink :to="localePath(`/order/view/${row.hash}`)"><BbButton :class="s.orderInfoButton">ORDER INFO</BbButton></NuxtLink>
					<NuxtLink :to="localePath(`/order-information/${row.orderId}`)"><BbButton>F&amp;B</BbButton></NuxtLink>
				</div>
			</template>
		</BbGrid>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import BbGrid from "@/components/bb-grid/bb-grid.vue";
import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue";
import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";
import { OrderSeat } from "@/types/booking";

import { OrderState } from "@/types/booking";

enum OrderStateBackgroundColors {
	Green = "Green",
	Red = "Red",
}

export default Vue.extend({
	components: {
		BbGrid,
		BbGridColumn,
		BbButton,
	},

	props: {
		tourId: {
			type: [String, Number],
		},
		data: {
			type: [Array],
		},
	},

	data() {
		return {
			s,
			ButtonTheme,
			OrderState,
		}
	},
	
	methods: {
		getOrderStateBackgroundColor(orderState?: OrderState): OrderStateBackgroundColors {
			switch (orderState) {
				case OrderState.Draft:;
				case OrderState.Paid: return OrderStateBackgroundColors.Green;
				case OrderState.Canceled:;
				case OrderState.NotPaid:;
				case OrderState.WaitingForPayment: return OrderStateBackgroundColors.Red;
			
				default: return OrderStateBackgroundColors.Red;
			}
		},
		getGuestsCount(id: number): string {
			let oredr = (this.data as any).find((x: any) => x.orderId === id);
			let totalGuests = oredr?.seats?.length ?? 0;
			let guestHasCome = oredr?.seats?.filter((x: OrderSeat) => x.guestHasCome).length ?? 0;

			return `${guestHasCome} of ${totalGuests}`;
		}
	}
})
</script>