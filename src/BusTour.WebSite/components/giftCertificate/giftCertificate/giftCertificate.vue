<template>
    <div :class="s.root">
        <div v-if="addedCertificate && addedCertificate.id">
            <div v-if="!isDraft">
                <h1 class="noPrint">You have successfully bought gift sertificate</h1>
                <div :class="s.inner">
                    
                    <giftCertificateSummary />

                    <bbButton @click="print" :text="$t('giftCertificate.print')" class="noPrint" :class="s.printButton" :theme="ButtonTheme.Black" />

                    <div class="noPrint" :class="s.emailWrapper">
                        <div :class="s.title">{{$t('giftCertificate.emailSend')}}</div>
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

                    <bbButton 
                        @click="send(emailSend)" 
                        :text="$t('giftCertificate.send')" 
                        class="noPrint" 
                        :class="s.sendButton" 
                        :theme="ButtonTheme.Black"
                     />   

                    <div :class="s.sendSuccess" v-if="showSuccess">
                        {{$t('giftCertificate.sendSuccess')}}
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

            <div v-else>
                <giftCertificatePayment :hideBackButton="true" @paid="paid()" :error="paymentError" />
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
import style from "./style.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime from "@/components/display/dateTime.vue"
import { Surprise } from "@/types/booking"
import { GiftCertificate, GiftCertificateStatus } from "@/types/giftCertificate"
import infoPanel from "@/components/controls/info-panel/info-panel.vue"
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import moment from 'moment'
import giftCertificateSummary from "@/components/giftCertificate/giftCertificateSummary/giftCertificateSummary.vue"
import giftCertificatePayment from "@/components/giftCertificate/giftCertificatePayment/giftCertificatePayment.vue"

export default Vue.extend({
    middleware: "authy",
    name: "GiftCertificate",
    props: {
        id: {
            type: [Number, String],
            default: null
        },
        number: {
            type: [Number, String],
            default: null
        },        
        email: {
            type: String,
            default: 'email@gmail.com'
        }        
    },    
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        currency,
        infoPanel,
        dateTime,
        bbButton,
        textInput,
        giftCertificateSummary,
        giftCertificatePayment
    },
    data() {
        return {
            s: style,
            emailSend: this.email,
            supportEmail: config.supportEmail ?? 'email@gmail.com',
            emailMode: 'view',
            ButtonTheme,
            showSuccess: false
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
        isDraft(): boolean {

            return this.addedCertificate?.status == GiftCertificateStatus.Draft
        },
        paymentError(): string | null {
            return this.addedCertificate?.payment?.details?.error ?? null;
        },           
    },
    methods: {
        ...mapActions('giftCertificate', [
            'getGiftCertificate', 
            'getGiftCertificates', 
            'sendCertificateOnEmail',
            'getCertificatePdf',
        ]),
        ...mapMutations('giftCertificate',['setAddedCertificate','resetAddedCertificate']),
        async print() {
            const pdfBase64Content = await this.getCertificatePdf(this.id);
            const pdfContent = Uint8Array.from(
                atob(pdfBase64Content)
                    .split('')
                    .map(ch => ch.charCodeAt(0))
            );
            const pdfFile = new Blob([pdfContent], { type: "application/pdf" });
            const pdfUrl = URL.createObjectURL(pdfFile);
            const pdfWindow = window.open(pdfUrl);
            pdfWindow?.print();
        },
        async send(email: any) {
            this.showSuccess = false;
            await this.sendCertificateOnEmail(email)
            this.showSuccess = true;
        },
        paid() {
            this.setAddedCertificate({ status:  GiftCertificateStatus.Active })
        } 
    },
    
    async created() {
        //this.resetAddedCertificate()

        let certifcate: GiftCertificate | null = null;

        if (this.id) {
            certifcate = await this.getGiftCertificate(this.id)
        } else if (this.number) {
            const certifcates = await this.getGiftCertificates({ number: this.number })
            certifcate = certifcates.length ? certifcates[0] : null
        }

        if (certifcate) {
            this.setAddedCertificate(certifcate)
        }
    }
})
</script>