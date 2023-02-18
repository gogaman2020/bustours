<template>
	<BusLayout>
		<OrderView />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";

import { mapActions, mapMutations, mapGetters } from "vuex";

import OrderView from "@/components/order/view/order-view.vue";

import { AuthorityLevel } from "~/types/private";

export default Vue.extend({
	middleware: "authy",

	meta: {
		auth: { authority: [ AuthorityLevel.Administrator, AuthorityLevel.Crew] },
	},

	components: {
		OrderView,
	},

	validate({ params: { hash } }): boolean {
		return !!hash;
	},

	async created() {
		const order = await this.getOrderByHash(this.$route.params.hash)
		this.setOrder(order)
		if (order.promoCode?.id) {
			this.$store.commit("booking/setOrderPromoCode", order.promoCode);
		}
		if (order.giftCertificate?.id) {
			this.$store.commit("booking/setOrderCertificate", order.giftCertificate);
		}
		await this.$store.dispatch("booking/getCalculationCostTour");		
	},

	methods: {
		...mapActions("booking", [
			"getOrderByHash",
		]),
		...mapMutations("booking", [
			"setOrder",
		]),
	},
});
</script>