<template>
<div :class="classSeat" 
    :style="{'top':seat.y+'px', 'left': seat.x+'px', 'transform': 'rotate(' + seat.rotate * 90 + 'deg)', 'transform': 'scale(' + seat.scaleX + ',' + seat.scaleY + ')' }" 
    @click="selectSeat()" @mouseover="setHoveredObject()" @mouseleave="resetHoveredObject()"> 
  <p :class="s.schemeTextSeat" :style="{'transform': `scale(${this.seat.scaleX},${this.seat.scaleY})` }">
    <span :class="s.schemeTextDesktop" :style="{'transform': `${transformText}`, 'backcground': '#000' }">
      {{ !seat.isSelected? seat.name: '' }}
      </span>
    <span :class="s.schemeTextLg" :style="{'transform': `${transformTextMobile}` }">
      {{ !seat.isSelected? seat.name: '' }}
      </span>  
  </p>
</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { OrderType, BusObject, BusObjectTypes, SchemeSeatModel, SchemeTableModel, SeatType } from "@/types/booking"
import { mapActions, mapGetters, mapState } from 'vuex'

import {
  BusSchemeItem,
  SeatingType,
} from "@/types/booking";

export default Vue.extend({
  name: "bus-seat",
  props: {
    seat: {} as () => SchemeSeatModel,
    table: {} as () => SchemeTableModel,
  },
  
  inject: ["editingMode"],

  data() {
    return {
      s: style,
    };
  },

  computed: {
    // ...mapGetters({
		// 	isUpgradeMode: "booking/isUpgradeMode",
		// }),
    ...mapState('booking', ['order']),
    isBlockingConflicted(): boolean {
      const conflictsResponse = this.order?.conflictsResponse;
      if (conflictsResponse && conflictsResponse.conflicts?.length) {
        return conflictsResponse.conflicts.some((x: any) => x.isBlocking && x.seatIds.some((z: number) => z == this.seat.id));
      } else {
        return false;
      }
    },
    isUpgradeMode(): boolean {
      return this.$store.getters['booking/isUpgradeMode'] || this.orderType == OrderType.RegularGroup;
    },
    orderType(): OrderType {
      return this.$store.state.booking.order.type;
    },
    isBySeats: {
      get(): boolean {
        return this.$store.state.booking.order.seatingType == SeatingType.BySeats || this.orderType == OrderType.RegularGroup;
      },
    },
    schemeTables: {
      get(): SchemeTableModel[] {
        return this.$store.state.booking.schemeTables;
      },
    },
    schemeSeats: {
       get(): SchemeSeatModel[] {
        return this.schemeTables
          .map((item) => item.seats)
          .reduce((x, y) => x.concat(y), []);
      },
    },
    transformText: {
      get(): string {
        return  this.seat.type === SeatType.Default ? `translateX(${4 * this.seat.scaleX}px)` :
                this.seat.type === SeatType.Double ? `translate(${3 * this.seat.scaleX}px, ${1 * this.seat.scaleY}px)` :
                this.seat.type === SeatType.Side ? `translateY(${3 * this.seat.scaleY}px)` :
                this.seat.type === SeatType.Long ? `translate(${5 * this.seat.scaleX}px, ${3 * this.seat.scaleY}px)` :
                `translateX(0)`
      }
    },
    transformTextMobile: {
      get(): string {
        return  this.seat.type === SeatType.Default ? `translateX(${4 * this.seat.scaleX}px)  rotate(${90}deg)` :
                this.seat.type === SeatType.Double ? `translate(${3 * this.seat.scaleX}px, ${1 * this.seat.scaleY}px)  rotate(${90}deg)` :
                this.seat.type === SeatType.Side ? `translateY(${3 * this.seat.scaleY}px)  rotate(${90}deg)` :
                this.seat.type === SeatType.Long ? `translate(${5 * this.seat.scaleX}px, ${3 * this.seat.scaleY}px)  rotate(${90}deg)` :
                `translateX(0)`
      }
    },
    classSeat: {
      get(): string[] {
        let arrClass = [this.s.seat, this.seat.isAvailable && this.isUpgradeMode && this.isBySeats ? this.s.pointer: this.s.defaultCursor];

        if (this.isBlockingConflicted) {
          arrClass.push(this.s.seatBlockingConflicted);
        }

        if(this.seat.type === SeatType.Default) {
            arrClass.push(this.s.seatDefault);
        }
        else if (this.seat.type === SeatType.Side) {
          arrClass.push(this.s.seatSide);
        }
        // else if (this.seat.type === SeatType.Сorner) {
        //   arrClass.push(this.s.seatCorner);
        // }

        if(this.seat.isSelected === true) {
          if(this.seat.type === SeatType.Default) {
            arrClass.push(this.s.seatSelectedDefault);
          }
          else if (this.seat.type === SeatType.Side) {
            arrClass.push(this.s.seatSelectedSide);
          }
          else if (this.seat.type === SeatType.Double) {
            arrClass.push(this.s.seatSelectedDouble);
          }
          else if (this.seat.type === SeatType.Long) {
            arrClass.push(this.s.seatSelectedLong);
          }
          if(this.seat.type === SeatType.Disabled) {
            arrClass.push(this.s.seatSelectedDisabled);
          }          
        }

        // else if (this.seat.isAvailable && this.orderType == OrderType.RegularGroup) {
        //   // arrClass.push(this.s.seatOrdered);
        // }
        
        else if(this.seat.isAvailable && this.isBySeats) {
          //arrClass.push(this.seat.isBackward? this.s.seatLeftWithoutPeople : this.s.seatRightWithoutPeople);
          if(this.seat.type === SeatType.Default) {
            arrClass.push(this.s.seatWithoutPeopleDefault);
          }
          else if (this.seat.type === SeatType.Side) {
            arrClass.push(this.s.seatWithoutPeopleSide);
          }
          else if (this.seat.type === SeatType.Double) {
            arrClass.push(this.s.seatWithoutPeopleDouble);
          }
          else if (this.seat.type === SeatType.Long) {
            arrClass.push(this.s.seatWithoutPeopleLong);
          }
        }
        
        else {
          //arrClass.push(this.seat.isBackward? this.s.seatLeftLocked : this.s.seatRightLocked);
          if(this.seat.type === SeatType.Default) {
            arrClass.push(this.s.seatLockedDefault);
          }
          else if (this.seat.type === SeatType.Side) {
            arrClass.push(this.s.seatLockedSide);
          }
          else if (this.seat.type === SeatType.Double) {
            arrClass.push(this.s.seatLockedDouble);
          }
          else if (this.seat.type === SeatType.Long) {
            arrClass.push(this.s.seatLockedLong);
          }
          if(this.seat.type === SeatType.Disabled) {
            arrClass.push(this.s.seatLockedDisabled);
          }          
        }

        return arrClass;
      },
    }
  },

  methods: {
    ...mapActions('booking', ['upgradeOrder']),
    async selectSeat() {
      if (this.seat.isAvailable && this.isUpgradeMode && this.isBySeats) {
      //  await this.$store.dispatch("booking/changeSeatInEditingMode", this.seat.id);

        const request = {
          id: this.seat.id,
          type: BusObjectTypes.Seat
        } as BusObject;

        await this.upgradeOrder(request);
      }
    },
    setHoveredObject(): void {
      if (this.isBySeats && this.seat.isAvailable) {
        this.$store.commit("booking/setHoveredObject", <BusSchemeItem>{
          table: this.table,
          seat: this.seat,
        });
      }
    },
    resetHoveredObject(): void {
      if (this.isBySeats && this.seat.isAvailable) {
        this.$store.commit("booking/setHoveredObject", <BusSchemeItem>{
          table: undefined,
          seat: undefined,
        });
      }
    },
  },
});
</script>