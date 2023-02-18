<template>
	<BusLayout>
		<OrderCard :hash="$route.params.hash" />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import OrderCard from "~/components/order/orderCard/orderCard.vue";

export default Vue.extend({
	components: { 
		OrderCard
	}
});
</script>