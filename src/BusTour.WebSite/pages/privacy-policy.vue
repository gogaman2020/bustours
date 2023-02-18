<template>
	<BusLayout>
		<PrivacyPolicy />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import PrivacyPolicy from "~/components/static-pages/privacy-policy/privacy-policy.vue";

export default Vue.extend({
	components: {
		PrivacyPolicy,
	}
})
</script>