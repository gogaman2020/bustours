<template>
	<BusLayout>
		<CookiePolicy />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import CookiePolicy from "~/components/static-pages/cookie-policy/cookie-policy.vue";

export default Vue.extend({
	components: {
		CookiePolicy,
	}
})
</script>