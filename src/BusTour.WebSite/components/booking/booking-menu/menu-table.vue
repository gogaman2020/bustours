<template>
<div :class="s.partBusPosition"
    :style="{'width': widthPosition+'px', 'height': wrapHeight+'px'}">
     <div :class="[s.table, table.isRight? s.tableRight: table.isLeft ? s.tableLeft : s.tableCenter, s.tableSelected, s.defaultCursor]"
      :style="{'width': width+'px', 'height': height+'px', 'top': orderInfoTable.y+'px', 'left': orderInfoTable.x+'px'}" > 
    <p :class="[s.schemeText, isVip ? s.schemeTextVip : '']">{{ isVip ? "VIP " + table.number : table.number }} </p>
  </div>
  <menu-seat v-for="seat in orderInfoTable.seats"
            :key="seat.id"
            :orderInfoSeat="seat"
            :tableId="orderInfoTable.id"
            :hasShowAlergy="hasShowAlergy"/>
  </div>
</template>
<script lang="ts">
import Vue from 'vue'
import style from "./style.module.scss";
import menuSeat from "./menu-seat.vue";
import {
  OrderInfoTable,
  OrderTable,
  SeatingType,
  ResponseTable,
  BusSchemeItem,
  Table,
  TableCategoryEnum
} from "@/types/booking";
import { mapGetters } from 'vuex'

export default Vue.extend({
    name: "menu-table",
    components:{
        menuSeat
    },
    props: {
        orderInfoTable: {} as () => OrderInfoTable,
        hasShowAlergy: Boolean,
    },
    data() {
        return {
            s: style,
            widthPosition: 127
        };
    },
    computed: {
        ...mapGetters({
            bookingTables: 'booking/tables',
            order: 'booking/order'
        }),
        table: {
            get(): Table {
                return this.bookingTables.filter((item: Table) => item.id == this.orderInfoTable.id)[0];
            },
        },
        isVip(): boolean {
            return this.table.category.id === TableCategoryEnum.Vip;
        },
        width: {
            get(): number {
                switch (this.table.xSize) {
                case 1:
                    return 25;
                case 2:
                    return 49;
                default:
                    return 10;
                }
            },
        },
        wrapHeight: {
            get(): number {
                switch (this.table.ySize) {
                case 1:
                    return 47;
                case 2:
                    return 56;
                case 3:
                    return 70;
                case 4:
                    return 166;
                default:
                    return 30;
                }
            }
        },
        height: {
             get(): number {
                switch (this.table.ySize) {
                case 1:
                    return 32;
                case 2:
                    return 36;
                case 3:
                    return 54;
                case 4:
                    return 72;
                default:
                    return 10;
                }
            },
        },
        
    },
    async created() {
		await this.$store.dispatch("booking/getOrder", this.order.id);
    	await this.$store.dispatch("booking/getOrderExtras", this.order.id);
		// this.$store.commit("booking/setOrderSeatsForOrderInfo");
        //await this.$store.dispatch("booking/getSelectionModel", {clickedObject: { type: 2, id: 0 }});
    },
})
</script>