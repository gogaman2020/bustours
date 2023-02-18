<template>
	<BusLayout>
		<PromocodeCreation />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import config from "@/config";
import PromocodeCreation from "~/components/promocode/promocode-creation/promocode-creation.vue";

import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
  	meta: {
    	auth: { 
			authority: AuthorityLevel.Supervisor
		},
  	},

	components: {
		PromocodeCreation,
	}
})
</script>