<template>
	<BusLayout>
		<ContactUs />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import ContactUs from "~/components/static-pages/contact-us/contact-us.vue";

export default Vue.extend({
	components: {
		ContactUs,
	}
})
</script>