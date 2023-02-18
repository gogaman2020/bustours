<template>
  <div :class="(valign == 'top' ? s.testTableGroupTop : s.testTableGroupBottom)" :style="{width: tableWidth}">
      <table v-if="seatsCount > 0">
          <tr>
              <td><div :class="classSeat1" @click.prevent="onClick(seat1)">{{seat1}}</div></td>
              <td rowspan="2"><div :class="classTable" @click.prevent="onClick(null)">{{tableNumber}}</div></td>
              <td v-if="type != 'vertical'"><div :class="classSeat2" @click.prevent="onClick(seat2)">{{seat2}}</div></td>
          </tr>
          <tr v-if="seatsCount > 2 || type == 'vertical'">
              <td><div :class="classSeat3" @click.prevent="onClick(seat3)">{{seat3}}</div></td>
              <td v-if="type != 'vertical'"><div :class="classSeat4" @click.prevent="onClick(seat4)">{{seat4}}</div></td>
          </tr>
      </table>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

import style from "./test.module.scss";

import { TestSeatInfo, TestTableInfo, TestSeatPosition } from "@/types/booking";

export default Vue.extend({
    components: {
    },
    props: {
        id: Number,
        seatsCount: Number,
        type: String,
        valign: String,
        width: String,
        info: Object
    },
    computed: {
        tableNumber: {
            get(): string {
                var tableInfo = 
                    this.info && this.info.tables && this.info.tables[this.id] 
                        ? this.info.tables[this.id]
                        : null;

                return tableInfo ? tableInfo.number : null;
            }
        },
        seat1: {
            get(): string {
                return this.seatsCount == 2 
                    ? this.type == 'vertical' 
                        ? this.valign == 'top' ? 'A' : 'B'
                        : 'A'
                    :this.valign == 'top' ? 'A' : 'C';
            }
        },
        seat2: {
            get(): string {
                return this.seatsCount == 2 || this.valign == 'top' ? 'B' : 'D';
            }
        },
        seat3: {
            get(): string {
                return this.seatsCount == 2 
                        ? this.type == 'vertical' 
                            ? this.valign == 'top' ? 'B' : 'A'
                            : 'A'
                        :this.valign == 'top' ? 'C' : 'A';
                }
            },
        seat4: {
            get(): string {
                return this.valign == 'top' ? 'D' : 'B';
            }
        },
        classTable: {
            get(): string[] {
                return this.getTableClass();
            }
        },
        classSeat1: {
            get(): string[] {
                return this.getSeatClass(this.seat1);
            }
        },
        classSeat2: {
            get(): string[] {
                return this.getSeatClass(this.seat2);
            }
        },
        classSeat3: {
            get(): string[] {
                return this.getSeatClass(this.seat3);
            }
        },
        classSeat4: {
            get(): string[] {
                return this.getSeatClass(this.seat4);
            }
        },
        tableWidth: {
            get(): string {
                return this.width 
                        ? this.width
                        : this.seatsCount == 4 || this.type != 'vertical'
                            ? '140px'
                            : '110px';
            }
        },
    },
    data() {
        return {
            s: style,
        };
    },
    methods: {
        onClick(e: number | null) {
            let params = {} as TestSeatPosition;
            params.tableId = this.id;
            params.seatId = e;

            this.$emit('change', params);
        },
        getTableClass() {
            var result = []; 

            var tableInfo = 
                this.info && this.info.tables && this.info.tables[this.id] 
                    ? this.info.tables[this.id]
                    : null;

            if (tableInfo) {
                result.push(this.seatsCount > 2 || this.type == 'vertical' ? this.s.testTable4 : this.s.testTable2);

                if (tableInfo.locked) {
                    result.push(this.s.testSeatLocked);
                } else if (tableInfo.selected) {
                    result.push(this.s.testSeatSelected);
                } else if (tableInfo.isRecommended) {
                    result.push(this.s.testSeatRecommended);
                } else if (!tableInfo.available) {
                    result.push(this.s.testSeatNotAvailable);
                }
            }

            return result;
        },
        getSeatClass(e: string) {
            var result = [ this.s.testSeat ];

            var tableInfo = 
                this.info && this.info.tables && this.info.tables[this.id] 
                    ? this.info.tables[this.id]
                    : null;

            if (tableInfo && tableInfo.seats && tableInfo.seats[e]) {
                let seatInfo = tableInfo.seats[e];

                if (seatInfo.locked) {
                    result.push(this.s.testSeatLocked);
                } else if (seatInfo.selected) {
                    result.push(this.s.testSeatSelected);
                } else if (seatInfo.isRecommended) {
                    result.push(this.s.testSeatRecommended);
                } else if (!seatInfo.available) {
                    result.push(this.s.testSeatNotAvailable);
                }
            }

            return result;
        }
    }
});
</script>