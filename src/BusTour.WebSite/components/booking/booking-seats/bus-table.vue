<template>
  <div :class="[classTable, ``]"
      :style="{'width':width+'px', 'height':height+'px', 'top':table.y+'px', 'left': table.x+'px'}"
      @click="selectTable()" @mouseover="setHoveredObject()" @mouseleave="resetHoveredObject()" > 

    <p :class="[s.schemeText, s.schemeTextTable, isVip ? s.schemeTextVip : '']">{{ isVip ? "VIP " + table.number : table.number }} </p>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { mapGetters, mapActions } from 'vuex'

import {
  SchemeTableModel,
  SeatingType,
  BusSchemeItem,
  BusObjectTypes,
  BusObject,
} from "@/types/booking";

export default Vue.extend({
  name: "bus-table",
  props: {
    table: {} as () => SchemeTableModel,
    isVip: {
      type: Boolean,
      required: false,
      default: false,
    },
    forDisabled: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      s: style,
    };
  },
  computed: {
    ...mapGetters({
			isUpgradeMode: "booking/isUpgradeMode",
		}),
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
    height: {
      get(): number {
        switch (this.table.ySize) {
          case 1:
            return this.forDisabled ? 40 : 32;
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
    schemeTables: {
      get(): SchemeTableModel[] {
        return this.$store.state.booking.schemeTables;
      },
    },
    isSeparateTable: {
      get(): boolean {
        return this.$store.state.booking.order.seatingType == SeatingType.SeparateTable;
      },
    },
    isUpgradetToTable: {
      get(): boolean {
        return this.table.seats.some(x => x.isSelected == false) === false;
      },
    },
    classTable: {
      get(): string[] {
        let arrClass = [this.s.table, this.table.isRight? this.s.tableRight: this.s.tableLeft];

        if(this.table.y < 120 && this.table.y > 20) {
          arrClass.push(this.s.tableCenter);
        }

        if((this.table.isAvailable && this.isUpgradeMode && this.isSeparateTable) || (this.table.isAvailableForUpgrade && this.isUpgradeMode && !this.isUpgradetToTable)) {
          arrClass.push(this.s.pointer);
        }
        else {
          arrClass.push(this.s.tableLocked);
        }
        if(!this.isUpgradeMode && !this.table.isAvailableForUpgrade && this.isUpgradetToTable) {
          arrClass.push(this.s.defaultCursor);
        }
        
        if(this.table.isSelected){
          arrClass.push(this.s.tableSelected);
        }
        
        if(this.table.isAvailableForUpgrade){
          arrClass.push(this.s.tableUpgrade);
        }

        return arrClass;
      },
    }
  },
  methods: {
    ...mapActions('booking', ['upgradeOrder']),
    async selectTable() {
      if ((this.table.isAvailable && this.isUpgradeMode && this.isSeparateTable) || 
           this.table.isAvailableForUpgrade) {
        // await this.$store.dispatch("booking/selectTable", this.table.id);
        
        const request = {
          id: this.table.id,
          type: BusObjectTypes.Table
        } as BusObject;

        await this.upgradeOrder(request);
      }
    },
    async setHoveredObject() {
      if (this.table.isAvailable || this.table.isAvailableForUpgrade) {
        this.$store.commit("booking/setHoveredObject", <BusSchemeItem>{
          table: this.table,
          seat: undefined,
        });
      }
    },
    async resetHoveredObject() {
      if (this.table.isAvailable || this.table.isAvailableForUpgrade) {
        this.$store.commit("booking/setHoveredObject", <BusSchemeItem>{
          table: undefined,
          seat: undefined,
        });
      }
    },
  },
});
</script>