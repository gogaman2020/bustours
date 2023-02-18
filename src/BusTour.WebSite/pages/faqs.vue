<template>
	<BusLayout>
		<FAQs />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import FAQs from "~/components/static-pages/faqs/faqs.vue";

export default Vue.extend({
	components: {
		FAQs,
	}
})
</script>