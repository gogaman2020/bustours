<template>
    <div>
        <paymentConfirmation 
            @pay="pay()" 
            :amount="addedCertificate.amountVariant.amount"
        >
            <template #sidepanel>
                <div :class="s.sidepanel">
                    <giftCertificateSummary />
                    <div :class="s.howItWorks">
                        {{$t('addCertificate.howItWorks')}}
                    </div>
                </div>
            </template>
            <template #extra-button>
                <bbButton v-if="!hideBackButton" :text="$t('buttons.back')" :theme="ButtonTheme.Black" @click="back" />
            </template>                
        </paymentConfirmation>
        <div v-if="paymentError" :class="s.error">
            <div>Payment failure: <b>{{paymentError}}</b></div>
        </div>           
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState } from "vuex";
import { mapProps } from "@/store/helpers";
import { AuthorityLevel } from "@/types/private"
import style from "./style.module.scss"
import ControlItem from "@/components/controlItem/controlItem.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import currency from "@/components/display/currency.vue"
import dateTime from "@/components/display/dateTime.vue"
import { MenuInfo, Surprise, Payment } from "@/types/booking"
import { AmountVariant, CertificateSurprise, CertificateFilter, GiftCertificate, GiftCertificateStatus } from "@/types/giftCertificate"
import { SelectItem } from "@/types/common"
import paymentConfirmation from "@/components/payment/payment-confirmation/payment-confirmation.vue"
import giftCertificateSummary from "@/components/giftCertificate/giftCertificateSummary/giftCertificateSummary.vue"

const required = (value: any): boolean => value === 0 || !!value

export default Vue.extend({
    middleware: "authy",
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        ControlItem,
        checkbox,
        dropdown,
        bbButton,
        textInput,
        currency,
        paymentConfirmation,
        dateTime,
        giftCertificateSummary
    },
    props: {
        hideBackButton: Boolean,
        error: String
    },
    data() {
        return {
            s: style,
            certificatePayment: null as Payment | null,
            ButtonTheme
        };
    },   
    computed: {
        ...mapProps(
            [
                'addedCertificate'
            ],
            'giftCertificate', 
            'certificateAdding',
            'setCertificateAdding'
        ),           
        ...mapState('booking', ['payment']), 
        lang(): string {
            return this.$i18n.locale;
        },        
        paymentError(): string | null {
            return this.certificatePayment?.details?.error ?? this.error ?? '';
        },        
    },
    methods: {
        ...mapActions('giftCertificate', [
            'sendCertificatePaymentSuccess',
            'sendCertificatePaymentFail'
        ]),
        async pay() {
			// if (this.payment.email.startsWith('a')) {
			// 	this.certificatePayment = await this.sendCertificatePaymentFail('Incufficient funds')
			// } else {
				this.certificatePayment = await this.sendCertificatePaymentSuccess()
                this.$emit('paid')
			// } 
        },
        back() {
            this.$emit('back')
        }
    },    
})
</script>