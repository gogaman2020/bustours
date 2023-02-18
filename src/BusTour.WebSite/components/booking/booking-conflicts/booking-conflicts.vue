<template>
    <div>
        <BbGrid :data="conflicts" :class="s.grid">
            <!-- <BbGridColumn :title="$t('booking.conflicts.conflictType')" field="conflictType" />          -->
            <BbGridColumn :title="$t('booking.conflicts.orderNumber')" field="orderNumber" />                  
            <BbGridColumn :title="$t('booking.conflicts.tourType')" field="orderType" />  
            <BbGridColumn :title="$t('booking.conflicts.departure')" field="departure" />     
            <BbGridColumn :title="$t('booking.conflicts.arrival')" field="arrival" />             
            <BbGridColumn :title="$t('booking.conflicts.seatsNumbers')" field="seatsNumbers" />        
            <BbGridColumn v-if="!hideBlocking" :title="$t('booking.conflicts.isBlocking')" field="isBlocking" />     
            <!-- <template v-slot:conflictType>
                {{$t('booking.conflicts.place')}}
            </template>     -->
            <template v-slot:orderNumber="{row}">
                {{row.orderId}}
            </template>     
            <template v-slot:departure="{row}">
                <dateTime :value="departure(row.orderId)" :type="DisplayType.DateTime" />
            </template>        
            <template v-slot:arrival="{row}">
                <dateTime :value="arrival(row.orderId)" :type="DisplayType.DateTime" />
            </template>                                                  
            <template v-slot:orderType="{row}">
                {{orderType(row.orderId)}}
            </template>      
            <template v-slot:seatsNumbers="{row}">
                {{seatsNumbers(row.seatIds)}}
            </template>    
            <template v-slot:isBlocking="{row}">
                <span :class="s[row.isBlocking ? 'blocking' : '']">{{row.isBlocking ? $t('yes') : $t('no')}}</span>
            </template>                                                                                                  
        </BbGrid>  
    </div>
</template>

<script lang="ts">

	import Vue, { PropType } from "vue"
    import { mapActions, mapState, mapGetters, mapMutations } from "vuex"
    import { mapProps } from "@/store/helpers"
    import s from "./s.module.scss"
    import { OrdersConflictsResponse, OrderConflictsResponse, Order, OrderType } from "@/types/booking"

    import controlItem from "@/components/controlItem/controlItem.vue"
    import bbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue"
    import BbGrid from "@/components/bb-grid/bb-grid.vue"
    import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue" 
    import dateTime, { DisplayType } from "~/components/display/dateTime.vue"

    export default Vue.extend({
        middleware: "authy",
        name: "BookingConflicts",
        props: {
            conflictsReponse: {
                type: Object as PropType<OrdersConflictsResponse>
            },
            hideBlocking: Boolean
        },
        data() {
            return {
                s,
                ButtonTheme,
                DisplayType
            }
        },
        components: {
            bbButton,
            controlItem,
            BbGrid,
            BbGridColumn,
            dateTime
        },  
        computed: {     
            ...mapGetters('tour', ['seatFullName']),
            orders(): Order[] {
                return this.conflictsReponse?.orders ?? []
            },
            conflicts(): OrderConflictsResponse[] {
                return this.conflictsReponse?.conflicts ?? []
            },
            hasBlocking(): boolean {
                return this.conflicts.some(x => x.isBlocking)
            }         
        },
        methods: {
            approve() {
                this.$emit('approve')
            },
            orderType(orderId: number): string {
                const order = this.orders.find(x => x.id == orderId)
                return this.$t(`enums.OrderType.${OrderType[order?.type ?? OrderType.Regular]}`).toString()
            },
            seatsNumbers(seatIds: number[]): string {
                return seatIds.map(seatId => this.seatFullName(seatId)).sort().join(', ')
            },
            departure(orderId: number): Date {
                const order = this.orders.find(x => x.id == orderId)
                return new Date(order?.tour?.departure ?? new Date());          
            },
            arrival(orderId: number): Date {
                const order = this.orders.find(x => x.id == orderId)
                return new Date(order?.tour?.arrival ?? new Date());          
            },                
        },
    })

</script>

