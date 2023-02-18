<template>	
	<BusLayout>
        <order-information :orderId="$route.params.id" :tour-id="$route.query.tourId" />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import orderInformation from "~/components/order-information/order-information.vue";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Crew },
	},		
	components: { orderInformation },
	validate({params}) {
    	return /^\d+$/.test(params.id)
  	},
});
</script>