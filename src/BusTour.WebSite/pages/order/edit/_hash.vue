<template>
	<BusLayout>
		<OrderEdit :order-hash="$route.params.hash" />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import OrderEdit from "@/components/order/edit/order-edit.vue";

import { AuthorityLevel } from "~/types/private";

export default Vue.extend({
	middleware: "authy",

	meta: {
		auth: { authority: AuthorityLevel.Administrator },
	},

	components: {
		OrderEdit,
	},

	validate({ params: { hash } }): boolean {
		return !!hash;
	},
})
</script>