<template>
    <div>
        <h1>{{$t('booking.header')}}</h1>
        <div :class="s.order">

            <div :class="s.row" v-if="step < OrderStep.Payment">
                <control-item :class="s.topControl" :title="$t('booking.bookingType')">
                    <dropdown v-model="type" :items="orderTypes" />
                </control-item>                
            </div>

            <div :class="s.main">

                <component :is="currentComponent" />    

                <BookingSummary v-if="step > OrderStep.Form" :class="s.summary" />

            </div>    

        </div>  
    </div>
</template>

<script lang="ts">

	import Vue from "vue"
    import { mapActions, mapState, mapGetters, mapMutations } from "vuex"
    import { mapProps } from "@/store/helpers"
    import s from "./style.module.scss"
    import { SelectItem, Time } from "~/types/common"
    import { OrderType, OrderStep } from "@/types/booking"

    import controlItem from "@/components/controlItem/controlItem.vue"
    import bbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue"
    import dropdown from "@/components/controls/dropdown/dropdown.vue"

    import bookingGroup from "@/components/booking-admin/booking-group/booking-group.vue"
    import bookingPrivateHire from "@/components/booking-admin/booking-private-hire/booking-private-hire.vue"   
    import BookingSummary from "@/components/booking/booking-summary/booking-summary.vue"

    export default Vue.extend({
        middleware: "authy",
        name: "BookingAdmin",
        data() {
            return {
                s,
                ButtonTheme,
                OrderStep
            }
        },
        components: {
            bbButton,
            dropdown,
            controlItem,
            BookingSummary
        },  
        computed: {     
            ...mapProps(
                [	
                    "step",
                    "type",
                    "date",
                    "time"
                ], 
                "booking", 
                "order", 
                "setOrder"
            ),                
            ...mapGetters('common',['domainEnum']),
            orderTypes(): SelectItem[] {
                let orderTypes =  this.domainEnum('OrderType', (key: string) => this.$t(key));

                return orderTypes.filter((x: SelectItem) => x.value != OrderType.Regular && x.value != OrderType.Service);
            }, 
            currentComponent(): any {
                const mapping: any = {
                    [OrderType.RegularGroup]: bookingGroup,
                    [OrderType.PrivateHire]: bookingPrivateHire
                }
                return mapping[(this as any).type] ?? bookingGroup;
            }             
        },
        watch: {
            type(val, oldVal) {
                if (val != oldVal) {
                    this.step = OrderStep.Form
                }
            }
        },
        methods: {
            ...mapActions('booking', ['getMenuInfo','getBuses','getRouteInfo']),    
            ...mapMutations('booking', ['resetOrder'])       
        },

        async created() {

            this.resetOrder()

            this.date = new Date()

            this.type = OrderType.PrivateHire

            // this.type = OrderType.PrivateHire
            // this.step = OrderStep.Menu
        }
    })

</script>

