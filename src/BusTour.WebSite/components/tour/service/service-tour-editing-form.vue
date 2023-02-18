<template>
	<div class="form">
		<h1>Service tour editing</h1>

		<div class="form__body">
			<ControlItem title="Date start" :is-required="true">
				<Calendar v-if="getActionsAvailability()" v-model="startDate" />
				<DateTime v-else :value="startDate" variant="long" />
			</ControlItem>

			<ControlItem title="Time start" :is-required="true">
				<TimePicker v-if="getActionsAvailability()" v-model="startTime" />
				<DateTime v-else :value="startTime.date" :type="DisplayType.Time" :variant="'short'" />
			</ControlItem>

			<ControlItem title="Duration" :is-required="true">
				<TimePicker v-if="getActionsAvailability()" v-model="duration" />
				<Duration v-else :value="duration.toString()" />
			</ControlItem>
		</div>

		<div v-if="getActionsAvailability()" class="form__buttons">
			<BbButton text="Save changes" @click="saveChanges" />
			<BbButton text="Delete" @click="tryDeleteTour" />
		</div>

		<Modal ref="Alert" />
		<Modal ref="Dialog">
			<template #footer="{returnFalse, returnTrue}">
				<BbButton @click="returnTrue()">Yes</BbButton>
				<BbButton @click="returnFalse()">No</BbButton>
			</template>
		</Modal>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapMutations, mapActions } from "vuex";

import ControlItem from "@/components/controlItem/controlItem.vue";
import DropDown from "@/components/controls/dropdown/dropdown.vue"
import BbButton from "@/components/controls/bb-button/bb-button.vue";
import TimePicker from "@/components/controls/timepicker/timepicker.vue";
import Calendar from "@/components/controls/calendar/calendarDate.vue";
import Modal from "@/components/controls/modal/modal.vue";
import DateTime, { DisplayType } from "@/components/display/dateTime.vue";
import Duration from "@/components/display/duration.vue";

import { Time } from "~/types/common";
import { BaseResponse } from "~/types/booking";
import { TourState } from "~/types/tour";

export default Vue.extend({
	name: "ServiceTourEditingForm",

	components: {
		ControlItem,
		DropDown,
		BbButton,
		TimePicker,
		Calendar,
		Modal,
		DateTime,
		Duration,
	},

	props: {
		orderHash: {
			type: String,
			required: true,
		},
	},

	data() {
		return {
			DisplayType,
		}
	},

	computed: {
		...mapGetters("booking", [
			"order",
		]),
		duration: {
			get(): Time {
				return Time.fromString(this.order.tour.serviceMaintenance.duration);
			},
			set(value: Time): void {
				let serviceMaintenance = {...this.order.tour.serviceMaintenance};
				this.setServiceTour({ serviceMaintenance: { ...serviceMaintenance, duration: value.toString() } });
			},
		},
		startDate: {
			get(): Date {
				return new Date(this.order.tour.departure);
			},
			set(value: Date) {
				let date = new Date(this.order.tour.departure);
				let time = Time.fromDate(date);

				date = value;
				date.setUTCHours(time.hours);
				date.setUTCMinutes(time.minutes);
				date.setUTCSeconds(time.seconds);

				this.setServiceTour({ departure: date });
			},
		},
		startTime: {
			get(): Time {
				return Time.fromDate(new Date(this.order.tour.departure));
			},
			set(value: Time) {
				let date = new Date(this.order.tour.departure);
				let time = Time.fromPlain(value).date;

				time.setUTCFullYear(date.getFullYear());
				time.setUTCMonth(date.getMonth());
				time.setUTCDate(date.getDate());

				this.setServiceTour({ departure: time });
			},
		},
	},

	methods: {
		...mapMutations("booking", [
			"setServiceTour",
			"setOrder",
		]),
		...mapActions("tour", [
			"updateServiceTour",
			"deleteTour",
		]),
		...mapActions("booking", [
			"getOrderByHash",
		]),
		async saveChanges() {
			const result: BaseResponse = await this.updateServiceTour(this.order.tour);
			let title = "Alert";
			let message = "Successfully saved";

			if (!result?.isSuccess) {
				message = result.message!;
			}

			await (<any>this.$refs.Alert).open(message, title);

			this.reloadOrder();
		},
		async tryDeleteTour() {
			const decision = await (<any>this.$refs.Dialog).open("Are you sure you want to delete this tour?", "Warning");

			if (!decision) {
				return;
			}

			const result: BaseResponse = await this.deleteTour(this.order.tour.id);
			let title = "Alert";
			let message = "Tour and nested order successfully deleted";

			if (!result?.isSuccess) {
				message = result.message!;
			}

			await (<any>this.$refs.Alert).open(message, title);

			this.reloadOrder();
		},
		getActionsAvailability(): boolean {
			const tourState = this.order?.tour?.tourState;

			return tourState && ![TourState.Deleted, TourState.Canceled].includes(tourState);
		},
		reloadOrder() {
			this.getOrderByHash(this.orderHash)
				.then(order => this.setOrder(order));
		},
	},
})
</script>

<style lang="scss" scoped>
	.form {
		&__body {
			font-size: 18px;
			margin-top: 25px;
			display: grid;
			grid-template: 1fr 1fr / 1fr;
			gap: 30px;
			max-width: 250px;
		}

		&__buttons {
			display: flex;
			flex-wrap: wrap;
			column-gap: 20px;
			row-gap: 20px;
			margin-block-start: 40px; 
			writing-mode: horizontal-tb;
		}
	}
</style>