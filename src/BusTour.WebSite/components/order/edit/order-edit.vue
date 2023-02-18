<template>
	<div>
		<component :is="componentName" :editingMode="true" :order-hash="orderHash" />
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapMutations } from "vuex";
import { Order, OrderType } from "~/types/booking";

import Booking from "@/components/booking/booking.vue";
import BookingPrivateHire from "@/components/booking-admin/booking-private-hire/booking-private-hire.vue";
import BookingGroup from "@/components/booking-admin/booking-group/booking-group.vue";
import ServiceTourEditingForm from "@/components/tour/service/service-tour-editing-form.vue";

export default Vue.extend({
	components: {
		Booking,
		BookingPrivateHire,
		BookingGroup,
		ServiceTourEditingForm,
	},

	props: {
		orderHash: String,	
	},

	data() {
		return {
			componentName: "",
		}
	},

	created() {
		this.getOrderByHash(this.orderHash)
			.then((order: Order) => { this.setOrder(order); return order })
			.then((order: Order) => this.defineComponent(order));
	},

	methods: {
		...mapActions("booking", [
			"getOrderByHash",
			"getEditableBusModel",
			"setPrivateHireOrderState",
		]),
		...mapMutations("booking", [
			"setOrder",
			"setRegularTourEditingOrderState",
		]),
		defineComponent(order: Order): void {
			if (!order?.id) {
				return;
			}

			const orderType = order.type;
			
			switch (orderType) {
				case OrderType.Regular: 
					this.componentName = (<any>Booking).extendOptions.name;
					this.setRegularTourEditingOrderState(order);
					this.getEditableBusModel();
					break;
				case OrderType.PrivateHire:
					this.componentName = (<any>BookingPrivateHire).extendOptions.name;
					this.setPrivateHireOrderState(order);
					break;
				case OrderType.RegularGroup: 
					this.componentName = (<any>BookingGroup).extendOptions.name;
					this.setRegularTourEditingOrderState(order);
					this.getEditableBusModel();
					break;
				case OrderType.Service:
					this.componentName = (<any>ServiceTourEditingForm).extendOptions.name;
					break;
				default: throw new Error("Unknown order type");
			}
		},
	},
});
</script>