<template>
	<BusLayout>
		<CurrentBookings/>
	</BusLayout>
	
</template>

<script lang="ts">
import Vue from "vue";
import CurrentBookings from "@/components/tour/current-bookings/current-bookings.vue";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Crew },
	},	
  	components: { 
		CurrentBookings
	},
});
</script>