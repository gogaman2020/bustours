<template>
  <div :class="s.schemeContainer">
   <div :class="[s.busFloor2]">
     <div :class="s.busFloorPosition">
        <bus-table
          v-for="(table) in tables"
          :key="`floor-2-table-${table.id}`"
          :table="table"
          :isVip="getIsVip(table)"
        />

        <bus-seat
          v-for="(seat) in seats"
          :key="`floor-2-seat-${seat.id}`"
          :seat="seat"
          :table="getTableForSeat(seat)"
        />
      </div>
      <!-- <pulse-loader :class="s.loader" :color="color" :loading="loading" :size="size"></pulse-loader> -->
    </div>
    <bus-tooltip v-show="showTooltip"/>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import BusTable from "./bus-table.vue";
import BusSeat from "./bus-seat.vue";
import BusTooltip from "./bus-tooltip.vue";
// import PulseLoader from '@/components/controls/spinner/pulse-loader.vue'

import { TableCategoryEnum, SchemeSeatModel, SchemeTableModel } from "@/types/booking";
import { mapGetters } from 'vuex'


export default Vue.extend({
  name: "bus-floor-2",
  components: {
    BusTable,
    BusSeat,
    BusTooltip
  },
  data() {
    return {
      s: style,
      color: '#202020',
      size: '10px'
    };
  },
  computed: {
     ...mapGetters({
      schemeTables: 'booking/schemeTables'
    }),
    tables: {
      get(): SchemeTableModel[] {
        return this.schemeTables.filter(
          (item: SchemeTableModel) => item.floor === 2
        );
      },
    },
    seats: {
      get(): SchemeSeatModel[] {
        return this.tables
          .map((item) => item.seats)
          .reduce((x: SchemeSeatModel[], y: SchemeSeatModel[]) => x.concat(y), []);
      },
    },
    showTooltip: {
      get(): boolean {
        return this.$store.state.booking.hoveredSchemeItem.table
          ? this.$store.state.booking.hoveredSchemeItem.table.floor === 2
          : false;
      },
    },
  },
  
  methods: {
    getIsVip(table: SchemeTableModel) {
      return table.category.id === TableCategoryEnum.Vip;
    },
    getTableForSeat(seat: SchemeSeatModel): SchemeTableModel {
      return this.tables.filter(
        (t) => t.seats.filter((s) => s.id === seat.id).length > 0
      )[0];
    },
  },
});
</script>