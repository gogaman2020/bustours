<template>
	<div :class="s.guestWrap">
		<div  v-for="(seat, index) in seats" :key="seat.seatId" >
			<div :class="s.guestNumber">Guest {{index + 1}}</div>
			<info-panel :class="s.guestPanel" :size="PanelSizes.Middle" :cols="4" :center-non-boundary-columns="true">
				<template #custom>
					<div>
					<div data-bold>Seat</div>
					<div>{{ seatFullName(seat.seatId) }}</div>
					<div></div>
					<div @click="guestHasCome(seat)">
						<checkbox :class="s.checkbox" :value="seat.guestHasCome" />
					</div>
					</div>
				</template>
			</info-panel>
			<info-panel v-if="seat.menuId" :class="s.guestPanel" :size="PanelSizes.Middle" :cols="4" :center-non-boundary-columns="true">
				<template #custom>
					<div>
						<div data-bold>Menu</div>
						<div>{{ getMenuName(seat.menuId) }}</div>
						<div>1</div>
						<div @click="hasMenuIssued(seat)">
							<checkbox :class="s.checkbox" :value="seat.hasMenuIssued" />
						</div>
					</div>
				</template>
			</info-panel>
			<info-panel v-if="seat.beverageId" :class="s.guestPanel" :size="PanelSizes.Middle" :cols="4" :center-non-boundary-columns="true">
				<template #custom>
					<div>
						<div data-bold>Beverage</div>
						<div></div>
						<div></div>
						<div></div>
					</div>
					<div>
						<div data-shift>{{ getBeverageName(seat.beverageId) }}</div>
						<div>{{ getBeverageVolume(seat.beverageId) }}</div>
						<div>1</div>
						<div @click="hasBeverageIssued(seat)">
							<checkbox :class="s.checkbox" :value="seat.hasBeverageIssued" />
						</div>
					</div>
				</template>
			</info-panel>

			<info-panel v-if="seat.otherAllergy || seat.allergyId" :class="[s.guestPanel,s.allergyPanel]" :size="PanelSizes.Middle" :cols="4" :center-non-boundary-columns="true">
				<template #custom>
					<div>
						<div data-bold>Allergy</div>
						<div>{{ seat.allergyId ? getAllergyName(seat.allergyId) : seat.otherAllergy }}</div>
						<div></div>
						<div></div>
					</div>
				</template>
			</info-panel>

		</div>

		<info-panel v-if="seats.length % 2 !== 0" style="background-color: rgb(0 0 0 / 0%)"></info-panel>

		<div v-if="getExtrasVisibility()"> 
			<div :class="s.guestNumber">Extras</div>
			<info-panel :class="s.guestPanel" :size="PanelSizes.Middle" :cols="4" :center-non-boundary-columns="true">
				<template #custom>
					<template v-if="getMenuExtrasVisibility()">
						<div>
							<div data-bold>Menu</div>
							<div></div>
							<div></div>
						</div>
						<div v-for="menu in orderExtras.menus" :key="menu.menuId + 'memu'">
							<div data-shift>{{ getMenuName(menu.menuId) }}</div>
							<div>{{ getMenuVolume(menu.menuId) }}</div>
							<div>{{ menu.amount }}</div>
							<div @click="hasExtraMenuIssued(menu)">
								<checkbox :class="s.checkbox" :value="menu.issued" />
							</div>
						</div>
					</template>
					<template v-if="getBeverageExtrasVisibility()">
						<div>
							<div data-bold>Beverages</div>
							<div></div>
							<div></div>
						</div>
						<div v-for="beverage in orderExtras.beverages" :key="beverage.beverageId + 'beverage'">
							<div data-shift>{{ getBeverageName(beverage.beverageId) }}</div>
							<div>{{ getBeverageVolume(beverage.beverageId) }}</div>
							<div>{{ beverage.amount }}</div>
							<div @click="hasExtraBeverageIssued(beverage)">
								<checkbox :class="s.checkbox" :value="beverage.issued" />
							</div>
						</div> 
					</template> 
				</template>
			</info-panel>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { OrderExtras, Beverage, MenuInfo, Order, Menu, Allergy, OrderSeat, OrderMenu, OrderBeverage} from "@/types/booking";
import infoPanel, { PanelSizes } from "@/components/controls/info-panel/info-panel.vue";
import { mapGetters } from "vuex";

export default Vue.extend({
	components: { infoPanel },
	data() {
		return {
			PanelSizes: PanelSizes,
			s: style,
		};
	},
	computed: {
		...mapGetters({
			seatFullName: "booking/seatFullName",
		}),		
		menusInfo(): MenuInfo {
			return this.$store.state.booking.menuInfo;
		},
		order(): Order {
			return this.$store.state.booking.order;
		},
		orderExtras(): OrderExtras | undefined {
			return this.$store.state.booking.orderExtras;
		},
		seats(): OrderSeat[] {
			let seats = this.order.seats.filter(x => x.orderId === this.order.id) || [];
			return seats;
		}
	},
	methods: {
		getAllergyName(id: number): string | undefined {
			return this.menusInfo.allergies.find(x => x.id === id)?.name.en;
		},
		getBeverageName(id: number): String | undefined {
			return this.menusInfo.beverages.find(x => x.id === id)?.name.en;
		},
		getBeverageVolume(id: number): String | undefined {
			let beverage = this.menusInfo.beverages.find(x => x.id === id) as Beverage;

			if (!beverage) {
				return "";
			}

			return `${beverage.volume} ${beverage.unit.en}`;
		},
		getMenuName(id: number): String | undefined {
			return this.menusInfo.menus.find((x: Menu) => x.id === id)?.name.en;
		},
		getMenuVolume(id: number): String | undefined {
			let menu = this.menusInfo.menus.find(x => x.id === id) as Menu;
			if(menu.volume === 0 || menu.unit.en === null) {
				return '';
			}
			return `${menu.volume} ${menu.unit.en}`;
		},
		getExtrasVisibility(): boolean {
			return this.getMenuExtrasVisibility() || this.getBeverageExtrasVisibility();
		},
		getMenuExtrasVisibility(): boolean {
			return !!this.orderExtras?.menus?.length;
		},
		getBeverageExtrasVisibility(): boolean {
			return !!this.orderExtras?.beverages?.length;
		},
		async guestHasCome(seat: OrderSeat) {
			let orderSeat = this.getChangedCloneSeat(seat,'guestHasCome', !seat.guestHasCome)

			await this.$store.dispatch("booking/updateOrderSeat", orderSeat);
		},
		async hasMenuIssued(seat: OrderSeat) {
			let orderSeat = this.getChangedCloneSeat(seat,'hasMenuIssued', !seat.hasMenuIssued)

			await this.$store.dispatch("booking/updateOrderSeat", orderSeat);
		},
		async hasBeverageIssued(seat: OrderSeat) {
			let orderSeat = this.getChangedCloneSeat(seat,'hasBeverageIssued', !seat.hasBeverageIssued)

			await this.$store.dispatch("booking/updateOrderSeat", orderSeat);
		},
		async hasExtraMenuIssued(menu: OrderMenu) {
			let extraMenu = {
				id: menu.id,
				orderId: menu.orderId,
				menuId: menu.menuId,
				amount: menu.amount,
				issued: !menu.issued
			} as OrderMenu;

			await this.$store.dispatch("booking/updateOrderMenu", extraMenu);
		},
		async hasExtraBeverageIssued(beverage: OrderBeverage) {
			let extraBeverage = {
				id: beverage.id,
				orderId: beverage.orderId,
				beverageId: beverage.beverageId,
				amount: beverage.amount,
				issued: !beverage.issued
			} as OrderBeverage;

			await this.$store.dispatch("booking/updateOrderBeverage", extraBeverage);
		},
		getChangedCloneSeat(seat: OrderSeat, field: string, value: any) : OrderSeat {
			let orderSeat = {
				id: seat.id,
				orderId: seat.orderId,
				seatId: seat.seatId,
				menuId: seat.menuId,
				beverageId: seat.beverageId,
				isAllergy: seat.isAllergy,
				allergyId: seat.allergyId,
				seatFullName: seat.seatFullName,
				otherAllergy: seat.otherAllergy,
				guestHasCome: seat.guestHasCome,
				hasMenuIssued: seat.hasMenuIssued,
				hasBeverageIssued: seat.hasBeverageIssued,
			} as OrderSeat;

			switch ( field ) {
				case 'guestHasCome':
					orderSeat.guestHasCome = value;
					break;
				case 'hasMenuIssued':
					orderSeat.hasMenuIssued = value;
					break;
				case 'hasBeverageIssued':
					orderSeat.hasBeverageIssued = value;
					break;
			}	

			return orderSeat;
		}
	}
});
</script>