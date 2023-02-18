<template>
	<BusLayout>
		<ToursManagement />
	</BusLayout>
</template>

<script lang="ts">
	import Vue from "vue";
	import { AuthorityLevel } from "@/types/private";

	export default Vue.extend({
		middleware: "authy",
		meta: {
			auth: { authority: AuthorityLevel.Supervisor },
		},			
	});
</script>