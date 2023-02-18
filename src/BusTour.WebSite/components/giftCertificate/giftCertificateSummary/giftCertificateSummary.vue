<template>
    <div :class="s.root">
        <div v-if="certificate && certificate.id">

                <h2>Your Order Summary</h2>

                <info-panel :class="s.panel">
                    <div data-bold>{{$t('giftCertificate.number')}}</div>
                    <div>
                        <!-- <u><a :href="'/gift-certificate/' + certificate.number" target="__blank">{{certificate.number}}</a></u> -->
                        {{certificate.number}}
                    </div>                      
                    <div data-bold>{{$t('giftCertificate.amount')}}</div>
                    <div><currency :value="amountVariant" /></div>
                    <div data-bold>{{$t('giftCertificate.dateEnd')}}</div>
                    <div><dateTime :variant="'long'" :value="certificate.dateEnd" /></div>
                    <!-- <div data-bold>{{$t('giftCertificate.comment')}}</div>
                    <div>{{certificate.comment}}</div> -->
                </info-panel>

                <info-panel :class="s.panel" v-if="surprisesQuantities && surprisesQuantities.length">
                    <div data-bold>{{$t('giftCertificate.surprises')}}</div>
                    <div>
                        <div v-for="surprisesQuantity in surprisesQuantities" :key="surprisesQuantity.id">
                            {{surprisesQuantity.name[lang]}} / {{surprisesQuantity.quantity}}
                        </div>    
                    </div>
                </info-panel>      

                <info-panel :class="s.panel">
                    <div data-bold>{{$t('giftCertificate.total')}}</div>
                    <div :class="s.totalAmount"><currency :adjustFont="false" :value="totalAmount" /></div>
                    <div data-shift>{{$t('giftCertificate.includingVat')}}</div>
                    <div><currency :value="totalAmount/1.2*0.2" /></div>                                        
                </info-panel>

        </div>     
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState } from "vuex"
import { mapProps } from "@/store/helpers"
import { AuthorityLevel } from "@/types/private"
import style from "./style.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime from "@/components/display/dateTime.vue"
import { Surprise } from "@/types/booking"
import { GiftCertificate } from "@/types/giftCertificate"
import infoPanel from "@/components/controls/info-panel/info-panel.vue"
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import moment from 'moment'

export default Vue.extend({
    middleware: "authy",
    name: "giftCertificateSummary",
    props: {
    },    
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        currency,
        infoPanel,
        dateTime,
        bbButton,
        textInput
    },
    data() {
        return {
            s: style,
            email: 'email@gmail.com',
            supportEmail: process.env.VUE_SUPPORT_EMAIL ?? 'email@gmail.com'
        };
    },     
    computed: {
        ButtonTheme() {
            return ButtonTheme;
        },
        certificateAdding(): any {},
        certificateSurprises(): any {},
        ...mapProps(
            [
                'addedCertificate'
            ],
            'giftCertificate', 
            'certificateAdding',
            'setCertificateAdding'
        ),            
        ...mapState('giftCertificate', ['amountVariants']),
        certificate(): GiftCertificate {
            return this.addedCertificate
        },
        lang(): string {
            return this.$i18n.locale;
        },
        surprises(): Surprise[] {
            return this.$store.state['booking'].menuInfo.surprises;
        },
        surprisesQuantities(): any {
            let certificate = (this.certificate as any) as GiftCertificate;
            return certificate?.certificateSurprises?.length > 0
            ? certificate.certificateSurprises.filter(x => x.quantity > 0).map(x => 
            {
                let surprise = this.surprises.find(z => z.id == x.surpriseId);
                return { 
                    id: x.id,
                    name: surprise?.name,
                    quantity: x.quantity,
                    amount: x.quantity * (surprise?.price ?? 0)
                }
            })
            : [];
        },
        amountVariant(): number {
            let certificate = (this.certificate as any) as GiftCertificate
            return certificate.amountVariant ? certificate.amountVariant.amount : 0
        },        
        totalAmount(): number {
            let suprisesAmount = this.surprisesQuantities && this.surprisesQuantities.reduce((prev: number, curr: any) => prev + curr.amount, 0)
            return this.amountVariant + suprisesAmount;
        }
    },
    methods: {
        ...mapActions('booking', ['getMenuInfo']),
        ...mapActions('giftCertificate', ['getAmountVariants','getGiftCertificate']),
        getSurprise(id: number): Surprise | undefined {
            return this.surprises.find(x => x.id == id);
        },
        getError(field: any) {
            return '';
        },
    },
    async created() {
        await this.getAmountVariants()
    }      
})
</script>