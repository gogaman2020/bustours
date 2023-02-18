<template>
	<div>
		<h1>Tour information</h1>

		<div :class="s.tourInfo">
			<div v-if="tourInformation.tourSummary" :class="s.tourInfoSummary" >
				<TourSummary :data="tourInformation.tourSummary" :orders="tourInformation.tourOrders" :tourId="tourId" />
			</div>

			<div v-if="tourInformation.tourOrders" :class="s.tourInfoOrderList">
				<TourOrderList :data="tourInformation.tourOrders" :tour-id="tourId" />
			</div>

			<div v-if="tourInformation.tourConflicts && tourInformation.tourConflicts.length" :class="s.tourInfoOrderConflicts">
				<p :class="s.tourInfoConflictsWarning">Check conflicts with existing booking</p>
				<BookingConflicts :hideBlocking="true" :conflicts-reponse="tourInformation.tourConflicts" :class="s.tourInfoOrderConflictsGrid" />
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import { mapActions, mapGetters } from "vuex";

import TourSummary from "./tour-summary/tour-summary.vue";
import TourOrderList from "./tour-order-list/tour-order-list.vue";
import BookingConflicts from "@/components/booking/booking-conflicts/booking-conflicts.vue";

export default Vue.extend({
	components: {
		TourSummary,
		TourOrderList,
		BookingConflicts,
	},

	props: {
		tourId: {
			type: [Number, String],
		},
	},

	data() {
		return {
			s,
		}
	},

	computed: {
		...mapGetters("tour", [
			"tourInformation",
		]),
	},

	created() {
		this.getCities()
			.then(() => this.getTourInformation(this.tourId));
	},

	methods: {
		...mapActions("tour", [
			"getTourInformation",
			"getCities",
		]),
	},
})
</script>