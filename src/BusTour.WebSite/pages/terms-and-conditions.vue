<template>
	<BusLayout>
		<TermsAndConditions />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import TermsAndConditions from "~/components/static-pages/terms-and-conditions/terms-and-conditions.vue";

export default Vue.extend({
	components: {
		TermsAndConditions,
	}
})
</script>