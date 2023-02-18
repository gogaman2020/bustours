<template>
	<BusLayout>
		<TourInformation :tour-id="$route.params.id" />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import TourInformation from "@/components/tour/tour-information/tour-information.vue";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Crew },
	},	
		
	components: {
		TourInformation,
	},

	validate({ params: { id } }): boolean {
		return /^[1-9]\d{0,}$/.test(id);
	},
})
</script>