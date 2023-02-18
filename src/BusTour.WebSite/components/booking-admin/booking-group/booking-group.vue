<template>
    <div>
        <h1 v-if="editingMode">Group order #{{id}}</h1>

        <div v-if="step < OrderStep.Menu" :class="s.row">
            
            <control-item :title="$t('booking.date')" :class="[s.topControl, s.calendarControl]">
                <calendar :lang="this.lang" v-model="date" :availableDates="tourDates" />
            </control-item>

            <control-item :title="$t('booking.time')" :class="s.topControl">
                <timepicker v-model="time" :times="tourTimes" />
            </control-item>       

            <control-item :title="$t('booking.guests')" :class="s.topControlHalf">
                <!-- <numeric v-model="guestCount" :max="50" /> -->
                <dropdown v-model="guestCount" :items="guestCountList"/>
            </control-item>    

        </div>    

        <bbButton v-if="[OrderStep.Form, OrderStep.Seats].includes(step)" :disabled="!date || !time" @click="apply" :text="$t('booking.apply')" :class="s.newBlock" :theme="buttonTheme.White" />

        <!-- <bbButton @click="test" :text="'test'" :class="s.newBlock" :theme="buttonTheme.White" /> -->

        <yesNoDialog 
            ref="conflictDialog" 
            :hideNo="hasBlockingConflicts" :yes="hasBlockingConflicts ? $t('booking.clearOrderSeats'): $t('booking.accept')" 
            :no="$t('booking.clearOrderSeats')"
            :height="200"
        >
            {{$t('booking.haveConflict')}}
        </yesNoDialog>

        <div v-if="step == OrderStep.Seats">
            
            <div :class="[ss.seatsCounter, s.newHalfBlock]">{{`${$t('booking.seatsChosen')}: ${seats.length}/${guestCount}`}}</div>
            
            <BookingSeats 
                :class="s.newBlock" 
            />
            
            <div :class="s.newBlock">

                <div :class="s.row">
                    <control-item :title="$t('booking.giftCertificate')" :class="ss.bottomControl">
                        <textInput v-model="certificateId" />
                    </control-item>
                    
                    <control-item :title="$t('booking.promoCode')" :class="ss.bottomControl">
                        <textInput v-model="promoCode" />
                    </control-item>            
                </div>   

                <div :class="s.row">
                    <control-item :title="$t('booking.name')" :class="ss.bottomControlThird" :error="getError($v.fullName)">
                        <textInput v-model="$v.fullName.$model" />
                    </control-item>
                    
                    <control-item :title="$t('booking.phone')" :class="ss.bottomControlThird" :error="getError($v.phoneNumber)">
                        <textInput v-model="$v.phoneNumber.$model" mask="##############" />
                    </control-item>     

                    <control-item :title="$t('booking.email')" :class="ss.bottomControlThird" :error="getError($v.email)">
                        <textInput v-model="$v.email.$model" />
                    </control-item>                             
                </div>    

                <div v-if="hasConflicts" :class="[s.row,ss.conflicts]">
                    <h2>{{$t('booking.conflicts.title')}}</h2>
                    <BookingConflicts :conflictsReponse="conflictsResponse" />
                    <div :class="ss.buttons">
                        <bbButton 
                            @click="refreshConflicts" 
                            :text="$t('booking.refreshConflicts')" 
                            :theme="buttonTheme.White" 
                        />                      
                        <bbButton 
                            v-if="hasBlockingConflicts" 
                            @click="resetConflicts" 
                            :text="$t('booking.clearSelection')" 
                            :theme="buttonTheme.Black" 
                        />   
                    </div> 
                </div>

            </div>            
        </div>

        <div v-if="step == OrderStep.Menu" :class="s.newBlock">
            <BookingMenu />
            <BookingMenuExtra />    
        </div>

        <div v-if="step == OrderStep.Receipt" :class="s.newBlock">
            <OrderView :editing-mode="editingMode" @back-button-clicked="back" />
        </div>        

        <div v-if="step == OrderStep.Payment" :class="s.newBlock">
            <BookingPayment @click="back" :hideBackButton="false" />
        </div>

        <div :class="[s.newBlock,s.buttonBlock]" v-if="step > OrderStep.Form">
            <bbButton v-if="step > steps[1] && step < OrderStep.Payment" @click="back" :text="$t('booking.back')" :theme="buttonTheme.Black" />   
            <bbButton :disabled="!canContinue()" v-if="step < steps[steps.length - 2]" @click="next" :text="$t('booking.continue')" :theme="buttonTheme.White" />    
        </div>    

    </div>
</template>

<script lang="ts">

	import Vue from "vue"
    import { mapActions, mapState, mapGetters, mapMutations } from "vuex"
    import { mapProps } from "@/store/helpers"
    import s from "@/components/booking-admin/booking-admin/style.module.scss"
    import ss from "./s.module.scss"
    import { SelectItem, Time } from "@/types/common"
    import { TourType } from "@/types/tour"
    import { OrderStep, ResponseSelection, ResponseTable, ResponseSeat, Tour, OrdersConflictsResponse } from '@/types/booking'
    import config from "~/config"
    import moment from "moment"

    import BookingSeats from "@/components/booking/booking-seats/booking-seats.vue" 
    import BookingMenu from "@/components/booking/booking-menu/booking-menu.vue"
    import BookingMenuExtra from "@/components/booking/booking-menu/menu-extra.vue"
    import BookingSummary from "@/components/booking/booking-summary/booking-summary.vue"
    import BookingPayment from "@/components/booking/booking-payment/booking-payment.vue"
    import OrderView from "@/components/order/view/order-view.vue"

    import controlItem from "@/components/controlItem/controlItem.vue"
    import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
    import dropdown from "@/components/controls/dropdown/dropdown.vue"
    import calendar from "@/components/controls/calendar/calendarDate.vue"
    import timePicker from "@/components/controls/timepicker/timepicker.vue"
    import numeric from "@/components/controls/numeric/numeric.vue"
    import textInput from "~/components/controls/text-input/text-input.vue"
    import yesNoDialog from "~/components/controls/yes-no-dialog/YesNoDialog.vue"
    import  { emailValidator, required } from "@/utils/validators"

    export default Vue.extend({
        middleware: "authy",
        name: "BookingGroup",
        props: {
            editingMode: {
                type: Boolean,
                default: false,
            },
        },  
        provide() {
            return {
                editingMode: true,
            }
        },                
        data() {
            return {
                s,
                ss,
                OrderStep,
                showErrors: false,
                conflictsChecked: false,
                prevOrderSeatsCount: 0
            }
        },
        components: {
            bbButton,
            dropdown,
            controlItem,
            calendar,
            timePicker,
            numeric,
            textInput,
            yesNoDialog,
            BookingSeats,
            BookingMenu,
            BookingMenuExtra,
            BookingSummary,
            BookingPayment,
            OrderView
        },  
        validations() {
            return {
                fullName: { required },
                phoneNumber: { required },
                email: { required, email: emailValidator }
            }
        },             
        computed: {        
            ...mapProps(
                [	
                    "guestCount",
	                "date",
	                "time",
                    "seats",
                    "type",
                    "step",
                    "id",
                    "certificateId",
                    "promoCode",
                    'tourId',
                    'isGroup',
                    'conflictsResponse'
                ], 
                "booking", 
                "order", 
                "setOrder"
            ),            
            ...mapProps(
                [	
                    'fullName',
                    'phoneNumber',
                    'email'
                ], 
                "booking", 
                "order.client", 
                "setOrderClient"
            ),              
            ...mapGetters('common',['domainEnum']),
            lang(): string {
                return this.$i18n.locale
            },               
            tourTypes(): SelectItem[] {
                return this.domainEnum('TourType', (key: string) => this.$t(key));
            },
            buttonTheme(): any {
                return ButtonTheme;
            },   
            isSeatsFilled(): boolean {
                return (this as any).seats.length == (this as any).guestCount;
            },
            steps(): OrderStep[] {
                return [OrderStep.Form, OrderStep.Seats, OrderStep.Menu, OrderStep.Payment, OrderStep.Receipt];
            },
            selectionModel():ResponseSelection  {
                return this.$store.state.booking.selectionModel;
            },
            hasConflicts(): boolean {
                return !!this.conflictsResponse?.conflicts?.length;
            },
            hasBlockingConflicts(): boolean {
                return this.hasConflicts && !!this.conflictsResponse && this.conflictsResponse.conflicts.some((x:any) => x.isBlocking);
            },            
            allSeats(): ResponseSeat[] {
                return this.selectionModel.tables.reduce((arr: ResponseSeat[], table: ResponseTable) => arr.concat(table.seats), [])
            },
            orderId(): number {
                return this.id;
            },
            orderSeats(): any {
                return this.seats;
            },            
            tours(): Tour[] {
                return this.$store.state.booking.routeInfo.tours;
            },
            tourDates(): Date[] {
                 return this.tours.map((x: Tour) => new Date(x.departure));
            },
            tourTimes(): Time[] {
                const date = this.date;
                const time = this.time;

                if (!date) {
                    return [];
                } else {
                    let times = this.tours
                    .map(x => new Date(x.departure))
                    .sort((x,y) => x.getTime() - y.getTime())
                    .filter(x => moment(new Date(x)).format('DD.MM.YYYY') == moment(date).format('DD.MM.YYYY'))
                    .map(x => Time.fromDate(x));

                    let timesStr = times.map((x) => x.toString())      
                    timesStr = timesStr.filter((x, i) => timesStr.indexOf(x) === i)    
                    times = timesStr.map((x) => Time.fromString(x))     

                    if (!time || (times.length && !times.some(x => x.equals(time)))) {
                        (this as any).time = times[0];
                    }

                    return times;
                }
            },
            seatsCount(): number {
                return this.tours?.length ? this.tours[0].tables.reduce((acc, table) => acc + table.seats.reduce((acc, seat) => acc = acc + 1, 0), 0) : 1;
            },
            guestCountList(): SelectItem[] {
                const max = this.seatsCount + 1;
                const min = 1;

                let availableNumbers = Array<number>(max - min).fill(0).map((_, idx) => idx + 1);
                let dataSource: SelectItem[] = availableNumbers.map(item => new SelectItem(item, item.toString()));

                return dataSource;
            },            
        },
        watch: {
            async step(val: OrderStep) {

                if (val == OrderStep.Payment) {
                    this.setPaymentFromOrderClient()
                    await this.setForPayment()
                }
            },
            async guestCount(current, old) {
                if (current != old && this.id) {
                    if (current < this.seats.length) {
                        this.resetOrderSeats()
                    }                    
                    await this.createOrUpdateOrderDraft()
                    await this.setSchemeTables(this.id)
                }
            },
            async order() {
                if (this.id) {
                    this.getCalculationCostTour()
                }
            },
            async orderSeats() {
                const newCount = this.orderSeats?.length ?? 0;
                if (newCount != this.prevOrderSeatsCount) {
                    this.refreshConflicts()
                    this.prevOrderSeatsCount = newCount;
                }
            }

        },
        methods: {
            ...mapActions('booking', [
                'getMenuInfo',
                'getRouteInfo',
                'getSelectionModel',
                'createOrUpdateOrderDraft',
                'setForPayment',
                'setSchemeTables',
                'getCalculationCostTour',
                'getOrderConflicts'
            ]),
            ...mapMutations("booking", ["setTour",'goToOrderStep','clearSelectedObjects','removeSelectedObjects','resetOrder','createSchemeTables', 
            'resetOrderSeats',
            'setPaymentFromOrderClient'
            ]),
            async test() {
                const answer = await ((this.$refs.conflictDialog as any).open() as Promise<boolean>)
                if (!answer) {
                    this.clearSelectedObjects()
                    await this.getSelectionModel()
                }
            },
            getError(field: any) {
                if (!this.showErrors) {
                    return ''
                } else if (field.required === false) {
                    return (this as any).$t('validation.isRequired')
                } else if (field.email === false) {
                    return (this as any).$t('validation.incorrectEmail');                      
                } else {
                    return ''
                }                
            },            
            async apply() {
                this.step = OrderStep.Seats;
                this.setTour()
				this.createSchemeTables()
				await this.createOrUpdateOrderDraft()
				await this.setSchemeTables(this.id)
            },
            back() {
                const currentStep = this.step
                let nextStep: OrderStep | null = null
                this.steps.forEach(x => {
                    if (x < currentStep) {
                        nextStep = x
                    }
                })

                if (this.editingMode && nextStep == OrderStep.Payment) {
                    nextStep = OrderStep.Menu
                }

                if (nextStep != null) {
                    this.goToOrderStep(nextStep)
                }
            },            
            async next() {
                const currentStep = this.step;
                let nextStep: OrderStep | null = null;
                this.steps.forEach(x => {
                    if (x > currentStep && !nextStep) {
                        nextStep = x;
                    }
                })

                if (this.editingMode && nextStep == OrderStep.Payment) {
                    nextStep = OrderStep.Receipt
                }    

                this.showErrors = true
                this.$v.$touch()
                const isValid = !this.$v.$invalid; 

                if (nextStep && isValid) {
                    await this.createOrUpdateOrderDraft()
                    this.goToOrderStep(nextStep);
                }
            },
            async resetConflicts() {
                this.resetOrderSeats()
                await this.createOrUpdateOrderDraft()  
                await this.setSchemeTables(this.id)              
            },
            async refreshConflicts() {
                this.conflictsChecked = false
                this.conflictsResponse = await this.getOrderConflicts()
                this.conflictsChecked = true          
            },            
            canContinue(): boolean {
                const currentStep: OrderStep = this.step
                if (currentStep == OrderStep.Seats) {
                    return this.isSeatsFilled && !this.hasBlockingConflicts && this.conflictsChecked
                } else {
                    return true
                }
            }
        },

        async created() {
            if (!this.editingMode) {
                this.resetOrder({ type: this.type })
            } else {
                this.apply()
            }
            this.isGroup = true
        }
    })

</script>
