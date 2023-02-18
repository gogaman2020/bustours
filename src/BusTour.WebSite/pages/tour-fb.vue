<template>
	<BusLayout>
		<TourFB/>
	</BusLayout>
	
</template>

<script lang="ts">
import Vue from "vue";
import TourFB from "@/components/tour/tour-fb/tour-fb.vue";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Crew },
	},	
  	components: { 
		TourFB
	},
});
</script>