<template>
    <div :class="s.root">
        <div v-if="order.id" :class="s.order">
            <!-- <div>
                <h1>Order #{{order.id}}</h1>                
                <div v-if="isPaid" :class="s.success">
                    Successfully paid
                </div>
                <div v-else-if="isWaitingForPayment">
                    <div v-if="error" :class="s.error">
                        <div>{{error}}</div>
                    </div>
                    <BookingPayment v-if="!tourIsExpired" :hideBackButton="true" @paid="paid" /> 
                </div>
                <div v-else-if="isCanceled" :class="s.error">
                    Order is canceled
                </div>                
            </div> -->
            <div>
                <BookingSummary />

                <bbButton @click="print" :text="$t('giftCertificate.print')" class="noPrint" :class="s.printButton" :theme="ButtonTheme.Black" />

                <div class="noPrint" :class="s.emailWrapper">
                    <div :class="s[emailMode]">
                        <textInput 
                            v-model="emailSend" 
                            variant='uppercase'
                            :hasModeToggler="true"
                            :mode="emailMode"
                            @changeMode="mode => emailMode = mode"
                        />
                    </div>
                </div>

                <bbButton @click="send" :text="$t('giftCertificate.send')" class="noPrint" :class="s.sendButton" :theme="ButtonTheme.Black" />

                <div :class="s.sendSuccess" v-if="showSuccess" class="noPrint">
                    {{$t('booking.sendSuccess')}}
                </div>

                <div class="noPrint" :class="s.customerService">
                    <div :class="s.title">{{$t('giftCertificate.customerService')}}</div>
                    <textInput 
                        v-model="supportEmail" 
                        variant='uppercase'
                        :hasModeToggler="false"
                        :mode="'view'"
                    />                    
                </div>
            </div>
        </div>     
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState, mapMutations } from "vuex"
import { mapProps } from "@/store/helpers"
import { AuthorityLevel } from "@/types/private"
import style from "./s.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime from "@/components/display/dateTime.vue"
import { Surprise, Order, OrderState } from "@/types/booking"
import infoPanel from "@/components/controls/info-panel/info-panel.vue"
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import moment from 'moment'
import giftCertificateSummary from "@/components/giftCertificate/giftCertificateSummary/giftCertificateSummary.vue"
import Payment from "~/components/payment/payment-confirmation/payment-confirmation.vue"
import BookingPayment from "@/components/booking/booking-payment/booking-payment.vue"
import BookingSummary from "@/components/booking/booking-summary/booking-summary.vue"

const defaultEmail = 'email@gmail.com';

export default Vue.extend({
    middleware: "authy",
    name: "OrderCard",
    props: {
        hash: {
            type: String,
            default: null
        },  
        email: {
            type: String,
            default: defaultEmail
        }     
    },    
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        BookingPayment,
        BookingSummary,
        bbButton,
        textInput,
    },
    data() {
        return {
            s: style,
            emailSend: this.email,
            supportEmail: config.supportEmail ?? defaultEmail,
            emailMode: 'view',
            ButtonTheme,
            showSuccess: false,
        };
    },     
    computed: {
        ...mapState('booking', ['order','payment']),
        lang: {
			get(): string {
				return this.$i18n.locale;
			},
		},
        error(): string | null {
            return this.paymentError ?? (this.tourIsExpired ? 'Order is outdated' : '')
        },
        tourIsExpired(): boolean {
            return new Date(this.order.tour?.departure ?? new Date()) < new Date()
        },
        isPaid(): boolean {
            return this.order?.orderState == OrderState.Paid
        },
        isWaitingForPayment(): boolean {
            return this.order?.orderState == OrderState.WaitingForPayment
        },       
        isCanceled(): boolean {
            return this.order?.orderState == OrderState.Canceled
        },           
        paymentError(): string | null {
            return this.order?.payment?.details?.error ?? null;
        },
    },
    methods: { 
        ...mapActions('booking', ['getOrderByHash', 'addNotification']),
        ...mapMutations('booking', ['setOrder']),
        async getOrder() {
            const order = await this.getOrderByHash(this.hash)
            order.date = order.tour?.departure ?? order.date
            order.guestsWithDisabilities = order.disabledGuestCount >= 1
            this.setOrder(order)    
            this.emailSend = order?.client?.email ?? defaultEmail
            if (order.promoCode?.id) {
                this.$store.commit("booking/setOrderPromoCode", order.promoCode);
            }
            if (order.giftCertificate?.id) {
                this.$store.commit("booking/setOrderCertificate", order.giftCertificate);
            }
            await this.$store.dispatch("booking/getCalculationCostTour");
        },
        paid() {
            if (this.order?.id) {
                this.getOrder()
            }
        },
        async send() {
            this.showSuccess = false;
            await this.addNotification({email:this.emailSend, lang:this.lang});
            this.showSuccess = true;

        },
        print() {
            window.print();
        },
    },

    created() {
        this.getOrder()
        if (this.payment?.email && this.emailSend == defaultEmail) {
            this.emailSend = this.payment.email
        }
    }
})
</script>