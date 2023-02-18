<template>
  <div :class="s.schemeContainer">
    <div :class="[s.busFloor1]">
      <div>
        <bus-table
          v-for="table in tables"
          :key="`floor-1-table-${table.id}`"
          :table="table"
          :forDisabled="getIsForDisabled(table)"
        />

        <bus-seat
          v-for="seat in seats"
          :key="`floor-1-seat-${seat.id}`"
          :seat="seat"
          :table="getTableForSeat(seat)"
        />
        </div>
        <!-- <pulse-loader :loading="loading" :class="s.loader" :color="color" :size="size"/> -->
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
import { mapGetters } from 'vuex'
// import PulseLoader from '@/components/controls/spinner/pulse-loader.vue'

import { SchemeSeatModel, SchemeTableModel, TableCategoryEnum } from "@/types/booking";

export default Vue.extend({
  name: "bus-floor-1",
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
          (item: SchemeTableModel) => item.floor === 1
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
          ? this.$store.state.booking.hoveredSchemeItem.table.floor === 1
          : false;
      },
    },
  },

  methods: {
    getIsForDisabled(table: SchemeTableModel) {
      return table.category.id === TableCategoryEnum.Wheelchair;
    },
    getTableForSeat(seat: SchemeSeatModel): SchemeTableModel {
      return this.tables.filter(
        (t) => t.seats.filter((s) => s.id === seat.id).length > 0
      )[0];
    },
  },
});
</script>