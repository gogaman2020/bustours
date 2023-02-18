<template>
  <div :class="s.test">
    <h1 :class="s.header">SEAT SELECTION TEST</h1>

    <div :class="[s.settingsContainer, s.marginTop20]">
      <div :class="s.settingsItem">
        <div :class="s.settingName">{{ $t("booking.guests") }}</div>
        <select
          v-model="selectionFilter.guestCount"
          :class="[s.settingInput, s.smoky]"
        >
          <option v-for="i in 8" :key="i" :value="i">
            {{ i }}
          </option>
        </select>
      </div>
      <div :class="s.settingsItem">
        <div :class="s.settingName">{{ $t("booking.tableRequirements") }}</div>
        <select
          v-model="selectionFilter.seatingType"
          :class="[s.settingInput, s.smoky]"
        >
          <option :value="seatingTypeEnum.SeparateTable">
            {{ $t("booking.separateTable") }}
          </option>
          <option :value="seatingTypeEnum.BySeats">
            {{ $t("booking.shareTable") }}
          </option>
        </select>
      </div>
      <div :class="s.settingsItem">
        <input type="checkbox" id="lockMode" v-model="lockMode" />
        <label for="lockMode">Режим блокировки мест</label>
      </div>
    </div>

    <div
      :class="[s.settingsContainer]"
      v-if="info && info.selectionResult && info.selectionResult.isAutoSelect"
    >
      <div :class="s.settingsItem">
        <button
          :class="[s.settingButton]"
          type="button"
          @click="onChangeSeatOrUpgrade"
        >
          Change seat or upgrade
        </button>
      </div>
    </div>

    <div :class="[s.subTitle, s.marginTop20]">
      {{ $t("booking.secondFloor") }}
    </div>
    <div :class="s.testFloor">
      <div :class="s.testTableRow">
        <testTable
          @change="onChange"
          :id="10"
          :info="info"
          :seatsCount="4"
          :valign="'top'"
        />
        <testTable :seatsCount="0" :width="'280px'" />
        <testTable
          @change="onChange"
          :id="8"
          :info="info"
          :seatsCount="4"
          :valign="'top'"
        />
        <testTable
          @change="onChange"
          :id="6"
          :info="info"
          :seatsCount="4"
          :valign="'top'"
        />
        <testTable
          @change="onChange"
          :id="4"
          :info="info"
          :seatsCount="4"
          :valign="'top'"
        />
        <testTable
          @change="onChange"
          :id="2"
          :info="info"
          :seatsCount="2"
          :valign="'top'"
          :type="'vertical'"
        />
      </div>
      <div :class="[s.testTableRow, s.marginTop20]">
        <testTable
          @change="onChange"
          :id="12"
          :info="info"
          :seatsCount="4"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="11"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="9"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="7"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="5"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="3"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
        <testTable
          @change="onChange"
          :id="1"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
          :type="'vertical'"
        />
      </div>
    </div>

    <div :class="[s.subTitle, s.marginTop20]">
      {{ $t("booking.firstFloor") }}
    </div>
    <div :class="s.testFloor">
      <div :class="s.testTableRow">
        <testTable :seatsCount="0" :width="'100px'" />
        <testTable
          @change="onChange"
          :id="19"
          :info="info"
          :seatsCount="2"
          :valign="'top'"
          :type="'vertical'"
        />
        <testTable :seatsCount="0" :width="'280px'" />
        <testTable
          @change="onChange"
          :id="15"
          :info="info"
          :seatsCount="4"
          :valign="'top'"
        />
        <testTable
          @change="onChange"
          :id="14"
          :info="info"
          :seatsCount="2"
          :valign="'top'"
          :type="'vertical'"
        />
        <testTable :seatsCount="0" :width="'210px'" />
      </div>
      <div :class="[s.testTableRow, s.marginTop20]">
        <testTable :seatsCount="0" :width="'100px'" />
        <testTable
          @change="onChange"
          :id="18"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
          :type="'vertical'"
        />
        <testTable
          @change="onChange"
          :id="17"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
          :type="'vertical'"
        />
        <testTable
          @change="onChange"
          :id="16"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
          :type="'vertical'"
        />
        <testTable :seatsCount="0" :width="'140px'" />
        <testTable
          @change="onChange"
          :id="13"
          :info="info"
          :seatsCount="2"
          :valign="'bottom'"
        />
      </div>
    </div>

    <div
      v-if="
        info &&
        info.selectionResult &&
        info.selectionResult.path &&
        info.selectionResult.path.length > 0
      "
      :class="[s.marginTop20, s.stepsCollection]"
    >
      <div
        v-for="step in info.selectionResult.path"
        :key="step.position"
        :class="[s.step]"
      >
        <div>
          <b>{{ step.position }}</b>
        </div>
        <div v-if="step.checkTwo">
          {{ step.checkTwo }}? <b>{{ step.checkResultTwo }}</b>
        </div>
        <div v-if="step.checkFour">
          {{ step.checkFour }}? <b>{{ step.checkResultFour }}</b>
        </div>
        <div>
          <i>{{ step.action }}</i>
        </div>
      </div>
    </div>

    <div
      v-if="
        debugInfo &&
        debugInfo.table &&
        debugInfo.table.rows &&
        debugInfo.table.rows.length > 0
      "
      :class="[s.marginTop20, s.table]"
    >
      <table>
        <tr v-for="row in debugInfo.table.rows" :key="row.code">
          <td
            v-for="cell in row.cells"
            :key="cell.code"
            :colspan="cell.colSpan > 1 ? cell.colSpan : 1"
            :rowspan="cell.rowSpan > 1 ? cell.rowSpan : 1"
            :style="cell.style"
          >
            {{ cell.text }}
          </td>
        </tr>
      </table>
    </div>

    <div
      v-if="
        debugInfo &&
        debugInfo.legend &&
        debugInfo.legend.rows &&
        debugInfo.legend.rows.length > 0
      "
      :class="[s.marginTop20, s.table]"
    >
      <table>
        <tr v-for="row in debugInfo.legend.rows" :key="row.code">
          <td
            v-for="cell in row.cells"
            :key="cell.code"
            :colspan="cell.colSpan > 1 ? cell.colSpan : 1"
            :rowspan="cell.rowSpan > 1 ? cell.rowSpan : 1"
            :style="cell.style"
          >
            {{ cell.text }}
          </td>
        </tr>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import testTable from "@/components/test/test-table.vue";

import style from "./test.module.scss";

import config from "@/config";
import {
  SeatingType,
  TestSeatInfo,
  TestTableInfo,
  TestInfo,
  TestSeatPosition,
  TestSelectionResultInfo,
  DebugInfo,
  BusObject,
  BusObjectTypes,
} from "@/types/booking";

import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
  middleware: "authy",
  meta: {
    auth: { disabled: !config.showComingSoon },
  },
  components: {
    testTable,
  },
  data() {
    return {
      s: style,
      config: config,
      seatingTypeEnum: SeatingType,
      selectionFilter: {
        tourId: 1,
        guestCount: 1,
        seatingType: SeatingType.SeparateTable,
        selectedObjects: [] as BusObject[],
        clickedObject: null as any,
        manualSelectionMode: false,
      },
      lockMode: false,
    };
  },
  computed: {
    info: {
      get(): TestInfo {
        return this.$store.state.booking.testInfo;
      },
    },
    debugInfo: {
      get(): DebugInfo {
        return this.$store.state.booking.debugInfo;
      },
    },
  },
  watch: {
    "selectionFilter.guestCount": async function (newValue, oldValue) {
      await this.load();
      await this.loadDebugInfo();
    },
    "selectionFilter.seatingType": async function (newValue, oldValue) {
      await this.load();
      await this.loadDebugInfo();
    },
  },
  methods: {
    async onChange(e: TestSeatPosition) {
      let table = this.info.tables[e.tableId];
      let seat = e.seatId ? table.seats[e.seatId] : null;

      if (seat) {
        if (!this.lockMode && !seat.available && !seat.selected) {
          return;
        }
      } else {
        if (!this.lockMode && !table.available && !table.selected) {
          return;
        }
      }

      this.selectionFilter.clickedObject = {
        type: e.seatId ? BusObjectTypes.Seat : BusObjectTypes.Table,
        id: e.seatId ? seat?.id : e.tableId,
      } as BusObject;

      this.selectionFilter.selectedObjects =
        this.info.selection.selectedObjects;
      this.selectionFilter.manualSelectionMode = false;

      await this.$store.dispatch(
        this.lockMode ? "booking/changeLock" : "booking/getBusModel",
        this.selectionFilter
      );
      await this.loadDebugInfo();
    },
    async onChangeSeatOrUpgrade() {
      this.selectionFilter.clickedObject = null;
      this.selectionFilter.manualSelectionMode = true;

      await this.$store.dispatch("booking/getBusModel", this.selectionFilter);
      await this.loadDebugInfo();
    },
    async load() {
      this.selectionFilter.clickedObject = null;
      this.selectionFilter.manualSelectionMode = false;

      await this.$store.dispatch("booking/getBusModel", this.selectionFilter);
    },
    async loadDebugInfo() {
      this.selectionFilter.selectedObjects =
        this.info.selection.selectedObjects;

      await this.$store.dispatch("booking/loadDebugInfo", this.selectionFilter);
    },
  },
  async created() {
    await this.load();
    await this.loadDebugInfo();
  },
});
</script>