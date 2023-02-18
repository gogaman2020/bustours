<template>
	<div :class="s.seats">
		<h2 :class="s.seatsTitle">{{ $t("booking.seatChoice") }}</h2>

		<div :class="s.bus">
			<p :class="s.deckName">{{ $t("booking.upperDeck") }}</p>
			<BusFloor2 />
		</div>
		
		<div :class="s.bus">
			<p :class="s.deckName">{{ $t("booking.lowerDeck") }}</p>
			<BusFloor1 />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./style.module.scss";

import BusFloor1 from "./bus-floor-1.vue";
import BusFloor2 from "./bus-floor-2.vue";

export default Vue.extend({
	name: "booking-seats",
	components: {
		BusFloor1,
		BusFloor2,
	},

	data() {
		return {
			s,
		};
	},
});
</script>