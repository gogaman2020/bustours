<template>
	<BusLayout>
		<Booking />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import Booking from "~/components/booking/booking.vue";

export default Vue.extend({
	components: { 
		Booking
	},
});
</script>