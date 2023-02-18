<template>
	<BusLayout>
		<CustomerCharter />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import CustomerCharter from "~/components/static-pages/customer-charter/customer-charter.vue";

export default Vue.extend({
	components: {
		CustomerCharter,
	}
})
</script>