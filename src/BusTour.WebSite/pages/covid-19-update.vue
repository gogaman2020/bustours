<template>
	<BusLayout>
		<Covid19update />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import Covid19update from "~/components/static-pages/covid-19-update/covid-19-update.vue";

export default Vue.extend({
	components: {
		Covid19update,
	}
})
</script>