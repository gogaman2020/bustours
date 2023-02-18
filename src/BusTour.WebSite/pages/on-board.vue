<template>	
	<BusLayout>
		<on-board />

		<BookingHint />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import OnBoard from "~/components/on-board/on-board.vue";
import BookingHint from "~/components/booking-hint/booking-hint.vue";

export default Vue.extend({
	components: {
		OnBoard,
		BookingHint,
	},
});
</script>