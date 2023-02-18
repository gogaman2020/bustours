<template>
	<BusLayout>
		<Promocodes />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import { AuthorityLevel } from "@/types/private";

import Promocodes from "@/components/promocode/promocodes/promocodes.vue";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Administrator },
	},		
	components: {
		Promocodes,
	},
});
</script>