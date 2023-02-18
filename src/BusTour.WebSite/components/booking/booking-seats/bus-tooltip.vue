<template>
  <div
    v-if="hoveredItem.table || (isBySeats && hoveredItem.seat)"
    :class="s.tooltip"
    :style="customStyle"
  >
    <div :class="[s.tooltipRow, s.tooltipRowSplit, s.tooltipRowHeader]">
      <template v-if="isBySeats && hoveredItem.seat && hoveredItem.table">
        <div :class="s.tooltipName">
          {{ "Seat " + hoveredItem.table.number + hoveredItem.seat.name }}
        </div>
        <div><currency :value="hoveredItem.seat.price" :digits="2" :marginTop="-1" /></div>
      </template>
      <template v-else-if="hoveredItem.table">
        <div :class="s.tooltipName">
          {{ "Table " + (isVip ? "VIP " : "") + hoveredItem.table.number }}
        </div>
        <div><currency :value="hoveredItem.table.price" :digits="2" :marginTop="-1" /></div>
      </template>
    </div>
    <template v-if="isBySeats && hoveredItem.table">
      <div :class="s.tooltipRow">or</div>
      <div :class="[s.tooltipRow, s.tooltipRowSplit]">
        <div>whole table</div>
        <div><currency :value="hoveredItem.table.price" :digits="2" :marginTop="-1" /></div>
      </div>
    </template>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import { TableCategoryEnum, SeatingType, BusSchemeItem } from "@/types/booking";

import currency from "@/components/display/currency.vue"

export default Vue.extend({
  name: "bus-tooltip",
  data() {
    return {
      s: style,
    };
  },
  components: {
    currency
  },
  computed: {
    customStyle: {
      get(): Object {
        if (this.isBySeats && this.hoveredItem.seat) {
          return {
            left: this.hoveredItem.seat.x - 60 + "px",
            top: this.hoveredItem.seat.y - 86 + "px",
          };
        } else if(this.isBySeats && this.hoveredItem.table){
          return {
            left: this.hoveredItem.table.x - 60 + "px",
            top: this.hoveredItem.table.y - 80 + "px",
          };
        } else if (this.hoveredItem.table) {
          return {
            left: this.hoveredItem.table.x - 60 + "px",
            top: this.hoveredItem.table.y - 40 + "px",
          };
        } else {
          return {
            left: "0px",
            top: "0px",
          };
        }
      },
    },
    isBySeats: {
      get(): boolean {
        return this.$store.state.booking.order.seatingType == SeatingType.BySeats;
      },
    },
    hoveredItem: {
      get(): BusSchemeItem {
        return this.$store.state.booking.hoveredSchemeItem;
      },
    },
    isVip: {
      get(): boolean {
        return this.hoveredItem.table
          ? this.hoveredItem.table.category.id == TableCategoryEnum.Vip
          : false;
      },
    },
  },
});
</script>