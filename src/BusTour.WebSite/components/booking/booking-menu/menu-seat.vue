<template>
    <div :class="classSeat" 
    :style="{'top':orderInfoSeat.y+'px', 'left': orderInfoSeat.x+'px', 'transform': 'scale(' + seat.scaleX + ',' + seat.scaleY + ')'}"> 
    
  <p :class="s.schemeText" :style="{'transform': `${transformText}` }">{{ seat.name }}</p>
</div>
</template>
<script lang="ts">
import Vue from 'vue'
import style from "./style.module.scss";
import { mapGetters } from 'vuex'

import {
  OrderInfoSeat,
  OrderInfoTable,
  Table,
  Seat,
  OrderSeat,
  Order,
  SeatType,
  SchemeTableModel,
} from "@/types/booking";

export default Vue.extend({
    name: "menu-seat",
    props: {
        orderInfoSeat: {} as () => OrderInfoSeat,
        tableId:{
            type: Number
        },
        hasShowAlergy: Boolean,
    },
    data() {
        return {
            s: style,
            isSelected: this.orderInfoSeat.isSelected
        };
    },
    computed: {
        ...mapGetters({
            order: "booking/order",
            bookingTables: 'booking/tables',
            schemeTables: 'booking/schemeTables',
        }),
        transformText: {
          get(): string {
            return  this.seat.type === SeatType.Default ? `translateX(${4 * this.seat.scaleX}px)` :
                    this.seat.type === SeatType.Double ? `translate(${3 * this.seat.scaleX}px, ${1 * this.seat.scaleY}px)` :
                    this.seat.type === SeatType.Side ? `translateY(${3 * this.seat.scaleY}px)` :
                    this.seat.type === SeatType.Long ? `translate(${5 * this.seat.scaleX}px, ${3 * this.seat.scaleY}px)` :
                    `translateX(0)`
          }
        },
        seat: {
            get(): Seat {
                const s = this.bookingTables.filter((item: Table) => item.id == this.tableId)[0].seats
                .filter((item: Seat) => item.id == this.orderInfoSeat.id)[0];
                const schemeSeat = (this.schemeTables as SchemeTableModel[]).filter((item: SchemeTableModel) => item.id == this.tableId)[0].seats
                .filter((item: Seat) => item.id == this.orderInfoSeat.id)[0];

                let seat = {
                    id: s.id,
                    name: s.name,
                    x: s.x,
                    y: s.y,
                    isForward: s.isForward,
                    isBackward: s.isBackward,
                    price: s.price,
                    rotate: schemeSeat.rotate,
                    type: schemeSeat.type,
                    scaleX: schemeSeat.scaleX,
                    scaleY: schemeSeat.scaleY,
                } as Seat;
                return seat;
            },
        },
        isBackward(): boolean {
            return this.seat.isBackward;
        },
        classSeat(): string[] {
            let arrClass = [this.s.seat, this.s.defaultCursor];
           
            console.log(this.seat);
            

        if(this.isSelected === true) {
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
          else if (this.seat.type === SeatType.Disabled) {
            arrClass.push(this.s.seatSelectedDisabled);
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
          else if (this.seat.type === SeatType.Disabled) {
            arrClass.push(this.s.seatLockedDisabled);
          }          
          // else if (this.seat.type === SeatType.Сorner) {
          //   arrClass.push(this.s.seatLockedCorner);
          // }
        }
            // if(this.isSelected){
            //     arrClass.push(this.isBackward ? this.s.seatLeftWithPeople: this.s.seatRightWithPeople);
            // }else {
            //     arrClass.push(this.isBackward ? this.s.seatLeftLocked : this.s.seatRightLocked);
            // }
            // if(this.hasShowAlergy)
            // {                
            //     if(this.order.seats.find((x: OrderSeat) => x.seatId == this.seat.id)?.isAllergy === true)
            //     {
            //         arrClass.push(this.s.seatAlergy)
            //     }
            // }

            return arrClass;
        }
    },  
});

</script>