<template>
    <div :class="[ss.root, editingMode ? ss.editingMode : '']">

        <h1 v-if="editingMode">Private hire order #{{id}}</h1>

        <div v-if="step == OrderStep.Form">

            <div :class="s.row">
                <control-item :title="$t('booking.date')" :class="[s.topControl, s.calendarControl]" :error="getError($v.date)">
                    <calendar v-model="$v.date.$model" />
                </control-item>                
            </div>

            <div :class="s.row">
                <control-item :title="$t('booking.isAllDay')" :class="s.topControl">
                    <checkbox v-model="$v.isAllDay.$model" /> 
                </control-item>                

                <control-item v-if="!isAllDay" :title="$t('booking.timeFrom')" :class="s.topControl" :error="getError($v.timeFrom)">
                    <timepicker v-model="$v.timeFrom.$model" />
                </control-item>       

                <control-item v-if="!isAllDay" :title="$t('booking.timeTo')" :class="s.topControl" :error="getError($v.timeTo)">
                    <timepicker v-model="$v.timeTo.$model" />
                </control-item>                         

            </div>  

            <div :class="s.row" v-if="!isAllDay">
                
                <control-item :title="$t('booking.blockBookingForDraft')" :class="s.topControl">
                    <checkbox v-model="$v.blockBookingForDraft.$model" /> 
                </control-item>                

                <control-item :title="$t('booking.timeFrom')" :class="s.topControl" :error="getError($v.blockBookingTimeFrom)">
                    <timepicker v-model="$v.blockBookingTimeFrom.$model" />
                </control-item>       

                <control-item :title="$t('booking.timeTo')" :class="s.topControl" :error="getError($v.blockBookingTimeTo)">
                    <timepicker v-model="$v.blockBookingTimeTo.$model" />
                </control-item>                         

            </div>       

            <div :class="s.row">
                
                <control-item :title="$t('booking.name')" :class="s.topControl" :error="getError($v.fullName)">
                    <textInput v-model="$v.fullName.$model" /> 
                </control-item>                

                <control-item :title="$t('booking.phone')" :class="s.topControl" :error="getError($v.phoneNumber)">
                    <textInput v-model="$v.phoneNumber.$model" mask="##############" />
                </control-item>      

                <control-item :title="$t('booking.email')" :class="s.topControl" :error="getError($v.email)">
                    <textInput v-model="$v.email.$model" />
                </control-item>                    

            </div>      

            <div :class="s.row">
                
                <control-item :title="$t('booking.departurePoint')" :class="s.topControlOneAndHalf" :error="getError($v.departurePoint)">
                    <textInput v-model="$v.departurePoint.$model" /> 
                </control-item>                

                <control-item :title="$t('booking.arrivalPoint')" :class="s.topControlOneAndHalf" :error="getError($v.arrivalPoint)">
                    <textInput v-model="$v.arrivalPoint.$model" />
                </control-item>       

            </div>      

            <div :class="s.row">
                
                <control-item :title="$t('booking.route')" :class="s.topControlDouble" :error="getError($v.routeId)">
                    <dropdown v-model="$v.routeId.$model" :items="routesList"/> 
                </control-item>                

            </div>       

            <div :class="s.row">
                
                <control-item :class="s.topControlDouble" :title="$t('booking.comment')" :autoHeight="true" :error="getError($v.comment)">
                    <textInput :class="ss.comment" v-model="$v.comment.$model" :multiline="true" />
                </control-item>          

            </div>          

            <div :class="s.row">
                
                <control-item :class="[s.topControlDouble, ss.stops]" :title="$t('booking.stops')" :autoHeight="true" :noBack="true" :error="getError($v.stops)">
                    <control-item v-for="(stop, index) in stops" :key="index" :class="[s.topControlOneAndHalf, ss.stop]" :title="(index + 1).toString()">
                        <textInput :value="stop" @input="changeStop(index, $event)" />
                    </control-item>
                </control-item>   

            </div>   

            <div :class="[s.row,ss.addButtons]">
                <control-item :noBack="true">
                    <button @click="addStop()" :class="[s.addButton]">{{$t('booking.addStop')}}</button> 
                </control-item>   
                <control-item v-if="stops.length" :noBack="true" :class="ss.removeStop">
                    <button @click="removeStop()" :class="[s.addButton, s.remove]">{{$t('booking.removeStop')}}</button>               
                </control-item>                                 
            </div>    

            <div :class="s.row">
                
                <control-item :class="s.topControl" :title="$t('booking.tourPrice')" :error="getError($v.tourPrice)">
                    <numeric v-model="$v.tourPrice.$model" />
                </control-item>          

            </div>              

            <div v-if="checkedConflicts" :class="ss.conflicts">
                <h2>{{$t('booking.conflicts.title')}}</h2>
                <BookingConflicts :conflictsReponse="ordersConflictsResponse" />
            </div>

        </div>
        
        <div v-if="step == OrderStep.Menu">

            <div :class="s.row">
                
                <control-item :title="$t('booking.guests')" :class="s.topControlHalf">
                    <dropdown v-model="guestCount" :items="guestCountList"/>
                </control-item>                    
            </div>    

            <div v-if="guestCount > 0">

                <div :class="ss.menus">
                    <div>

                        <h2>{{$t('menuBooking.menusSelection')}}</h2>

                        <div v-for="(menu, index) in regularMenus" :key="menu.menuId" :class="ss.menuBeverageItem">
                            <control-item :title="''" :class="s.topControlDouble">
                                <dropdown :items="menuList" :value="menu.menuId" @input="setMenuId(index, $event)" :disabled="!enableAddMenu" />
                            </control-item>
                            <control-item :title="''" :class="s.topControlHalf">
                                <dropdown :items="guestCountListMenu" :value="menu.amount" @input="setMenuAmount(menu.menuId, $event)" />
                            </control-item>                        
                        </div>

                    </div>

                    <div :class="[s.row,ss.addButtons]" v-if="enableAddMenu">
                        <control-item :noBack="true">
                            <button @click="addMenu()" :class="[s.addButton]">{{$t('booking.addMenu')}}</button> 
                        </control-item>   
                        <control-item v-if="menus.length" :noBack="true" :class="ss.removeStop">
                            <button @click="removeMenu()" :class="[s.addButton, s.remove]">{{$t('booking.removeMenu')}}</button>               
                        </control-item>                  
                    </div>

                </div>

                <div :class="ss.menus">
                    <div>

                        <h2>{{$t('menuBooking.drinksSelection')}}</h2>

                        <div v-for="beverageGroup in beverageGroups" :key="beverageGroup.name">

                            <div :class="ss.beverageGroupName">{{$t('menuBooking.' + beverageGroup.name)}}</div>

                            <div v-for="(beverage, index) in beverageGroup.beverages" :key="beverage.beverageId" :class="ss.menuBeverageItem">
                                <control-item :title="''" :class="s.topControlDouble">
                                    <dropdown :items="beverageList" :value="beverage.beverageId" @input="setBeverageId(index, $event)" :disabled="!enableAddMenu" />
                                </control-item>
                                <control-item :title="''" :class="s.topControlHalf">
                                    <dropdown :items="guestCountListMenu" :value="beverage.amount" @input="setBeverageAmount(beverage.beverageId, $event)" />
                                </control-item>                        
                            </div>

                        </div>

                    </div>

                    <div :class="[s.row,ss.addButtons]" v-if="enableAddMenu">
                        <control-item :noBack="true">
                            <button @click="addBeverage()" :class="[s.addButton]">{{$t('booking.addBeverage')}}</button> 
                        </control-item>   
                        <control-item v-if="menus.length" :noBack="true" :class="ss.removeStop">
                            <button @click="removeBeverage()" :class="[s.addButton, s.remove]">{{$t('booking.removeBeverage')}}</button>               
                        </control-item>                  
                    </div>
                    
                </div>   

                <div :class="ss.menus">
                    <div>

                        <h2>{{$t('menuBooking.extrasSelection')}}</h2>

                        <div v-for="(menu, index) in extraMenus" :key="menu.menuId" :class="ss.menuBeverageItem">
                            <control-item :title="''" :class="s.topControlDouble">
                                <dropdown :items="menuList" :value="menu.menuId" @input="setMenuId(index, $event)" :disabled="!enableAddMenu" />
                            </control-item>
                            <control-item :title="''" :class="s.topControlHalf">
                                <dropdown :items="guestCountListMenu" :value="menu.amount" @input="setMenuAmount(menu.menuId, $event)" />
                            </control-item>                        
                        </div>

                    </div>

                </div>      


                <div :class="ss.menus">
                    <h2>{{$t('booking.specialRequests')}}</h2>   

                    <control-item :class="s.topControlDouble" :title="''" :autoHeight="true">
                        <textInput :class="ss.comment" v-model="specialRequests" :multiline="true" />
                    </control-item>                    
                </div>

            </div>

        </div>

        <div v-if="step == OrderStep.Receipt">
            <OrderView :editing-mode="editingMode" @back-button-clicked="back" />
        </div>          

        <div v-if="step == OrderStep.Payment">
            <BookingPayment @click="back" :hideBackButton="false" />
        </div>  

        <div v-if="error" :class="ss.error">
            {{error}}
        </div>         

        <div :class="[s.row, ss.bottomButtons]">
            <bbButton v-if="step == OrderStep.Form" :theme="ButtonTheme.White" @click="checkConflicts">{{$t('booking.checkConflicts')}}</bbButton>
            <bbButton :theme="ButtonTheme.White" @click="approveConflicts" v-if="checkedConflicts && !hasBlockingConflicts && !approvedConflicts">{{$t('booking.approveConflicts')}}</bbButton>   
            <bbButton v-if="step > steps[0] && step < OrderStep.Payment" @click="back" :text="$t('booking.back')" :theme="ButtonTheme.Black" />               
            <bbButton v-if="step < steps[steps.length - 2] && approvedConflicts" :theme="ButtonTheme.Black" @click="next">{{$t('booking.next')}}</bbButton>
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
    import { MenuTypeEnum, Menu, Beverage, OrderMenu, OrderBeverage, OrderStep, ResponseSelection, ResponseTable, ResponseSeat, Tour, Route, OrdersConflictsResponse, OrderConflictsResponse } from '@/types/booking'
    import config from "~/config"
    import moment from "moment"

    import BookingSeats from "@/components/booking/booking-seats/booking-seats.vue" 
    import BookingMenu from "@/components/booking/booking-menu/booking-menu.vue"
    import BookingMenuExtra from "@/components/booking/booking-menu/menu-extra.vue"
    import BookingSummary from "@/components/booking/booking-summary/booking-summary-private-hire.vue"
    import BookingPayment from "@/components/booking/booking-payment/booking-payment.vue"
    import BookingConflicts from "@/components/booking/booking-conflicts/booking-conflicts.vue"
    import OrderView from "@/components/order/view/order-view.vue"

    import controlItem from "@/components/controlItem/controlItem.vue"
    import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
    import dropdown from "@/components/controls/dropdown/dropdown.vue"
    import calendar from "@/components/controls/calendar/calendarDate.vue"
    import timePicker from "@/components/controls/timepicker/timepicker.vue"
    import numeric from "@/components/controls/numeric/numeric.vue"
    import textInput from "~/components/controls/text-input/text-input.vue"
    import yesNoDialog from "~/components/controls/yes-no-dialog/YesNoDialog.vue"
    import checkbox from "@/components/controls/checkbox/checkbox.vue"
    import  { emailValidator, required } from "@/utils/validators"

    export default Vue.extend({
        middleware: "authy",
        name: "BookingPrivateHire",
        props: {
            editingMode: {
                type: Boolean,
                default: false,
            },
        },    
        data() {
            return {
                s,
                ss,
                OrderStep,
                conflictSeatsIds: [] as number[],
                conflictOrdersIds: [] as number[],
                ButtonTheme,
                ordersConflictsResponse: null as OrdersConflictsResponse | null,
                checkedConflicts: false,
                approvedConflicts: false,
                hasBlockingConflicts: false,
                enableAddMenu: false,
                showErrors: false,
                error: ''
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
            checkbox,
            BookingSeats,
            BookingMenu,
            BookingMenuExtra,
            BookingSummary,
            BookingPayment,
            BookingConflicts,
            OrderView
        },  
        validations() {
            return {
                date: { required },
                isAllDay: {},
                timeFrom: { 
                    required: (value: any, vm: any) => vm.isAllDay || !!value,
                    datesRange: (val: any, vm: any) => vm.isAllDay || Time.diff(val, vm.timeTo) > 0
                },
                timeTo: { 
                    required: (value: any, vm: any) => vm.isAllDay || !!value,
                    datesRange: (val: any, vm: any) => vm.isAllDay || Time.diff(val, vm.timeFrom) < 0
                },
                blockBookingForDraft: { },
                blockBookingTimeFrom: { 
                    required: (value: any, vm: any) => vm.isAllDay || !!value,
                    datesRange: (val: any, vm: any) => vm.isAllDay || Time.diff(val, vm.blockBookingTimeTo) > 0                  
                },
                blockBookingTimeTo: { 
                    required: (value: any, vm: any) => vm.isAllDay || !!value,
                    datesRange: (val: any, vm: any) => vm.isAllDay || Time.diff(val, vm.blockBookingTimeFrom) < 0                   
                },
                fullName: { required },
                phoneNumber: { required },
                email: { required, email: emailValidator },
                departurePoint: { required },
                arrivalPoint: { required },
                routeId: {},
                comment: { required: (value: any, vm: any) => vm.routeId > 0 || !!value },
                stops: {},
                tourPrice: { required: (value: any): boolean => !!value && value != 0 }
            }
        },          
        computed: {   
            ...mapProps(
                [	
                    'id',
                    'step',
                    'comment',
                    'guestCount',
                    'menus',
                    'beverages',
                    'specialRequests',
                    'type'
                ], 
                "booking", 
                "order", 
                "setOrder"
            ),                    
            ...mapState('tour', ['routes']),    
            ...mapProps(
                [	
                    'date',
                    'timeFrom',
                    'timeTo',
                    'isAllDay',
                    'blockBookingForDraft',
                    'blockBookingTimeFrom',
                    'blockBookingTimeTo',
                    'departurePoint',
                    'arrivalPoint',
                    'routeId',
                    'stops',
                    'tourPrice'
                ], 
                "booking",
                "order.privateHire",
                "setOrderPrivateHire"
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
            ...mapGetters('booking', ['seats']),
            ...mapState('booking', ['menuInfo']),
            lang(): string {
                return this.$i18n.locale
            },              
            routesList(): SelectItem[] {

                const list: SelectItem[] = this.routes.map((x: Route) => new SelectItem(x.id, x.name[this.lang]));
                list.push(new SelectItem(0, this.$t('booking.otherRoute').toString()));

                return list;
            },
            steps(): OrderStep[] {
                return [OrderStep.Form, OrderStep.Menu, OrderStep.Payment, OrderStep.Receipt];
            },  
            guestCountList(): SelectItem[] {
                const max = this.seats.length + 1;
                const min = 0;

                let availableNumbers = Array<number>(max - min).fill(0).map((_, idx) => idx);
                let dataSource: SelectItem[] = availableNumbers.map(item => new SelectItem(item, item.toString()));

                return dataSource;
            },
            guestCountListMenu(): SelectItem[] {
                const max = this.seats.length + 1;
                const min = 0;

                let availableNumbers = Array<number>(max - min).fill(0).map((_, idx) => idx);
                let dataSource: SelectItem[] = availableNumbers.map(item => new SelectItem(item, item.toString()));

                return dataSource;
            },            
            menuList(): SelectItem[] {
                return this.menuInfo.menus.map((x: Menu) => new SelectItem(x.id, x.name[this.lang]))
            },
            beverageList(): SelectItem[] {
                return this.menuInfo.beverages.map((x: Beverage) => new SelectItem(x.id, x.name[this.lang]))
            },
            beverageGroups(): any[] {
                const groups: any[] = [
                    {
                        name: 'softDrinks',
                        beverages: [],
                        checker: (beverage: Beverage) => !beverage.alcoholByVolume && !beverage.isHot
                    },
                    {
                        name: 'hotDrinks',
                        beverages: [],
                        checker: (beverage: Beverage) => !beverage.alcoholByVolume && beverage.isHot
                    },
                    {
                        name: 'alcoDrinks',
                        beverages: [],
                        checker: (beverage: Beverage) => !!beverage.alcoholByVolume
                    }                                      
                ]

                const that = this

                this.beverages.forEach((b: OrderBeverage) => {
                    const group = groups.find(g => { 
                        const beverage = that.menuInfo.beverages.find((z: Beverage) => z.id == b.beverageId)
                        return beverage ? g.checker(beverage) : false
                    })
                    if (group) {
                        group.beverages.push(b)
                    }
                })

                return groups;
            },
            extraMenus(): OrderMenu[] {
                return this.menus
                    .filter((x: OrderMenu) => {
                        const menu = this.menuInfo.menus.find((m: Menu) => m.id == x.menuId)
                        return menu?.menuType?.id != MenuTypeEnum.Main })
            },
            regularMenus(): OrderMenu[] {
			    return this.menus.filter((x: OrderMenu) => !this.extraMenus.some(e => e.menuId == x.menuId))
            }                    
        },
        watch: {
            timeFrom(val: Time, oldValue: Time) {
                const current = Time.fromPlain(val)
                const old = Time.fromPlain(oldValue)
                if (current && !current.equals(old)) {
                    this.checkedConflicts = false
                    this.approvedConflicts = false
                    try {
                        this.blockBookingTimeFrom = new Time(val.hours - 2, val.minutes, val.seconds);
                    } catch(e) {
                        this.blockBookingTimeFrom = null;
                    }
                }
            },
            timeTo(val: Time, oldValue: Time) {
                const current = Time.fromPlain(val)
                const old = Time.fromPlain(oldValue)
                if (current && !current.equals(old)) {
                    this.checkedConflicts = false
                    this.approvedConflicts = false                    
                    try {                    
                        this.blockBookingTimeTo = new Time(val.hours + 2, val.minutes, val.seconds);
                    } catch(e) {
                        this.blockBookingTimeTo = null;
                    }                    
                }
            },
            date(val: Date, old: Date) {
                if (val?.getTime() != old?.getTime()) {
                    this.checkedConflicts = false
                    this.approvedConflicts = false 
                }
            },
            isAllDay (val: boolean, old: boolean) {
                if (val != old) {
                    this.checkedConflicts = false
                    this.approvedConflicts = false 
                }
            },            
        },
        methods: {
            ...mapActions('booking', ['getOrderConflicts', 'createPrivateHireOrder']),
            ...mapMutations('booking', ['goToOrderStep','resetOrder']),
            getError(field: any) {
                if (!this.showErrors) {
                    return ''
                } else if (field.required === false) {
                    return (this as any).$t('validation.isRequired')
                } else if (field.email === false) {
                    return (this as any).$t('validation.incorrectEmail');                      
                } else if (field.datesRange === false) {
                    return (this as any).$t('createTour.wrongDatesRange')                     
                } else {
                    return ''
                }                
            },
            addStop() {
                this.stops.push('')
                this.commitPrivateHire()
            },
            removeStop() {
                this.stops.pop()
                this.commitPrivateHire()
            },
            changeStop(index: number, e: any) {
                this.stops[index] = e
                this.commitPrivateHire()
            },
            addMenu() {
                this.menus.push({ amount: 1 })
                this.commitOrder()
            },
            removeMenu() {
                this.menus.pop()
                this.commitOrder()
            },
            setMenuId(index: number, e: any) {
                this.menus[index].menuId = parseInt(e)
                this.commitOrder()
            },
            setMenuAmount(menuId: number, e: any) {
                this.menus.find((x: OrderMenu) => x.menuId == menuId).amount = parseInt(e)
                this.commitOrder()
            },       
            addBeverage() {
                this.beverages.push({ amount: 1 })
                this.commitOrder()
            },
            removeBeverage() {
                this.beverages.pop()
                this.commitOrder()
            },
            setBeverageId(index: number, e: any) {
                this.beverages[index].beverageId = parseInt(e)
                this.commitOrder()
            },
            setBeverageAmount(beverageId: number, e: any) {
                this.beverages.find((x: OrderBeverage) => x.beverageId == beverageId).amount = parseInt(e)
                this.commitOrder()
            },                  
            commitPrivateHire() {
                this.$store.commit('booking/setOrderPrivateHire', (this as any).privateHire)
            },    
            commitOrder() {
                this.$store.commit('booking/setOrder', (this as any).order)
            },                
            async checkConflicts() {
                if (this.validate()) {
                    this.ordersConflictsResponse = await this.getOrderConflicts()
                    this.checkedConflicts = true
                    this.approvedConflicts = (this.ordersConflictsResponse?.conflicts?.length ?? 0) === 0
                    this.hasBlockingConflicts = this.ordersConflictsResponse?.conflicts?.some((x: OrderConflictsResponse) => x.isBlocking) ?? false
                }
            },
            approveConflicts() {
                this.approvedConflicts = !this.hasBlockingConflicts && true;
            },
            validate(): boolean {

                this.showErrors = true
                this.$v.$touch()
                const isInvalid = this.$v.$invalid;   
                this.error = '';

                if (this.step >= OrderStep.Menu) {
                    const menusAmount = this.regularMenus.reduce((sum: number, menu: OrderMenu) => sum += menu.amount, 0)
                    if (this.guestCount != menusAmount) {
                        this.error = this.$t('booking.menusAndGuestsError').toString()
                        return false
                    }
                }
                
                return !isInvalid;
            },
            async next() {
                let nextStep: OrderStep | null = null;

                this.steps.forEach(x => {
                    if (x > this.step && !nextStep) {
                        nextStep = x;
                    }
                })

                if (this.editingMode && nextStep == OrderStep.Payment) {
                    nextStep = OrderStep.Receipt
                }

                if (nextStep && this.validate()) {
                    this.goToOrderStep(nextStep)
                    // @ts-ignore
                    if (nextStep == OrderStep.Payment && !this.id) {
                        await this.createPrivateHireOrder()
                    }
                }
            },
            back() {
                let nextStep: OrderStep | null = null
                this.steps.forEach(x => {
                    if (x <  this.step) {
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
            setRouteId() {
                if (this.routes?.length && !this.routeId) {
                    this.routeId = this.routes[0].id
                }
            }                
        },
        async created() {
            if (!this.editingMode) {
                this.resetOrder({ type: this.type })
            }
            this.setRouteId()            
            this.menuInfo.menus.forEach((x: Menu) => {
                if (!this.menus.some((m: OrderMenu) => m.menuId == x.id)) {
                    this.menus.push({ menuId: x.id, amount: 0 });
                }
            })
            this.menuInfo.beverages.forEach((x: Beverage) => {
                if (!this.beverages.some((m: OrderBeverage) => m.beverageId == x.id)) {
                    this.beverages.push({ beverageId: x.id, amount: 0 });
                }
            })            
            this.commitOrder()            
        }
    })

</script>
