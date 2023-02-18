<template>
	<BusLayout>
		<Careers />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import Careers from "~/components/static-pages/careers/careers.vue";

export default Vue.extend({
	components: {
		Careers,
	}
})
</script>