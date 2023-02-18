<template>
	<div>
		<div v-if="order.id !== 0">
			<h1>{{ $t("orderInformation.title") }} №{{ order.id }}</h1>
			<div :class="s.container">
				<div :class="s.listRow">
					<control-item :title="$t('booking.date')" :class="s.listItem">
						<DateTime :value="departure" variant="long" v-show="show"/>
					</control-item>
					<control-item :title="$t('booking.time')" :class="s.listItem">
						<DateTime :value="departure" :type="DisplayType.Time" variant="short" v-show="show"/>
					</control-item>
					<control-item :title="$t('booking.guests')" :class="s.listItem">
						<div :class="s.listInput" v-show="show">{{ guestCount }}</div>
					</control-item>
					<bbButton :disabled="isСameAll" :class="s.allGuestHasCome" @click="allGuestHasCome()">
							{{$t('booking.cameAll')}}
					</bbButton>
					
				</div>
				<div>
					<div :class="s.partBus">
						<menu-table
							v-for="tab in orderInfoTables"
							:key="tab.id"
							:orderInfoTable="tab"
							:hasShowAlergy="true" 
						/>
					</div>
				</div>

				<order-information-info-panel />

				<div  v-if="order.specialRequests">
					<div :class="s.guestNumber">Special request</div>
					<info-panel :class="s.guestPanel" :size="PanelSizes.Middle" :cols="1" :center-non-boundary-columns="true" style="margin-bottom: 40px">
						<template #custom>
							<div>
								<div data-shift>{{order.specialRequests }}</div>
							</div>
						</template>
					</info-panel>
				</div>
				

				<nuxt-link :to="`/tour-information/${order.tourId}`">
					<bbButton>
						{{ $t('orderInformation.back') }}
					</bbButton>
				</nuxt-link>
			</div>
		</div>
		<div v-else-if="show">
			Order not found
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import menuTable from "~/components/booking/booking-menu/menu-table.vue";
import { OrderInfoTable, OrderSeat, MenuInfo, Order, Tour, Route, Seat } from "@/types/booking";
import orderInformationInfoPanel from "./order-information-info-panel.vue";
import bbButton from "../controls/bb-button/bb-button.vue";
import DateTime, { DisplayType } from "~/components/display/dateTime.vue";
import { mapGetters } from "vuex";
import infoPanel, { PanelSizes } from "@/components/controls/info-panel/info-panel.vue";

export default Vue.extend({
	components: {
		bbButton,
		menuTable,
		orderInformationInfoPanel,
		DateTime
	},
	props: {
        orderId: String,
		tourId: String,
    },
	data() {
		return {
			DisplayType,
			PanelSizes: PanelSizes,
			s: style,
			show: false,
		};
	},
	computed: {
		...mapGetters({
			order: "booking/order",
		}),
		orderInfoTables(): OrderInfoTable {
			return this.$store.state.booking.selectionModel.orderInfo.tables;
		},
		orderSeats(): OrderSeat[] {
			return this.$store.state.booking.orderSeats as OrderSeat[];
		},
		departure(): Date {
			return new Date(this.order.tour ? this.order.tour?.departure : "");
		},
		menu(): MenuInfo {
			return this.$store.state.booking.menuInfo;
		},
		isСameAll() : boolean {
			return this.order.seats.find((x: OrderSeat) => !x.guestHasCome) === undefined;
		},
		guestCount(): number {
			return this.order?.tour?.privateHire?.guestCount ?? this.order.guestCount
		}
    },
    async created() {
		await this.$store.dispatch("booking/getOrder", this.orderId);
    	await this.$store.dispatch("booking/getOrderExtras", this.orderId);

		// this.$store.commit("booking/setOrderSeatsForOrderInfo");

        //await this.$store.dispatch("booking/getSelectionModel", {clickedObject: { type: 2, id: 0 }});
        this.show = true;
    },
	methods:{
		async allGuestHasCome() {
			await this.$store.dispatch("booking/allGuestHasCome", this.orderId);
		}
	}
});
</script>