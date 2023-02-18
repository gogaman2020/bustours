<template>
	<BusLayout>
		<ModernSlaveryStatement />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import ModernSlaveryStatement from "~/components/static-pages/modern-slavery-statement/modern-slavery-statement.vue";

export default Vue.extend({
	components: {
		ModernSlaveryStatement,
	}
})
</script>