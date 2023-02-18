<template>
    <BusLayout :class="s.root">

        <div v-if="step == Steps.Add">
            <h1>{{$t("addCertificate.title")}}</h1>

            <!-- <div :class="[s.message,s.howItWorks]" v-html="$t('addCertificate.howItWorks')"></div>             -->

            <div :class="[s.itemsRow, s.firstRow]">
                <control-item :class="[s.amountVariant, s.control]" :title="$t('addCertificate.amountVariantId')" :isRequired="true" :error="getError($v.amountVariantId)">
                    <dropdown v-model="$v.amountVariantId.$model" :items="amountVariantsSelectItems">
                        <template #option="{option}">
                            <currency v-if="getAmountVariant(option.value)" :class="s.amountVariantOption" :adjustFont="true" :value="getAmountVariant(option.value).amount" />
                        </template>          
                        <template #selected-option="{option}">
                            <currency v-if="getAmountVariant(option.label)" :class="s.amountVariantOption" :adjustFont="true" :value="getAmountVariant(option.label).amount" />
                        </template>                                       
                    </dropdown>
                </control-item>
                <control-item :title="$t('addCertificate.hasSurprises')" :class="s.right" v-if="false">
                    <checkbox v-model="$v.hasSurprises.$model" @input="commit" />
                </control-item>
                <control-item :class="[s.validUntil, s.control]" :title="$t('addCertificate.vaidUntil')">
                    <dateTime :variant="'long'" :value="validUntil" />
                </control-item>
            </div>

            <div :class="s.surprisesWrapper" v-if="!!$v.hasSurprises.$model">
                <h2>{{$t("addCertificate.extras")}}</h2>
                <div :class="s.surprises">
                    <template :class="surprise" v-for="certificateSurprise in $v.certificateSurprises.$each.$iter">
                        <div :class="[s.name, s.left]">{{getSurprise(certificateSurprise.surpriseId.$model).name[lang]}}</div>
                        <currency :class="s.price" v-model="getSurprise(certificateSurprise.surpriseId.$model).price" />
                        <dropdown :class="s.quantity" v-model="certificateSurprise.quantity.$model" :items="quantitiesSelectItems" @input="commit" /> 
                    </template>
                </div>
            </div>

            <div :class="s.itemsRow" v-if="false">
                <control-item :class="s.commentWrapper" :title="$t('addCertificate.commentTitle')">
                    <textInput v-model="$v.comment.$model" @input="commit" />
                </control-item>
            </div>

            <bbButton @click="create" :text="$t('addCertificate.continue')" :class="s.createButton" :theme="buttonTheme.Black" :disabled="creating" />
        </div>

        <div v-if="step == Steps.Confirm && addedCertificate.id" :class="s.stepConfirm">
            <giftCertificateSummary :id="addedCertificate.id" />
            <bbButton @click="goto(Steps.Pay)" :text="$t('addCertificate.confirm')" :class="s.createButton" :theme="buttonTheme.Black" />
        </div>

        <div v-if="(step == Steps.Confirm || step == Steps.Pay) && addedCertificate.id" :class="[s.stepPay, s['step' + step]]">
            <giftCertificatePayment @paid="paid()" @back="back()" />
        </div>

        <div v-if="step == Steps.Success && addedCertificate.id">
            <giftCertificate :id="addedCertificate.id" :email="payment.email" />
        </div>        

    </BusLayout>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState, mapMutations } from "vuex";
import { mapProps } from "@/store/helpers";
import { AuthorityLevel } from "@/types/private"
import style from "./add-gift-certificate.module.scss"
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
import giftCertificate from "@/components/giftCertificate/giftCertificate/giftCertificate.vue"
import giftCertificateSummary from "@/components/giftCertificate/giftCertificateSummary/giftCertificateSummary.vue"
import giftCertificatePayment from "@/components/giftCertificate/giftCertificatePayment/giftCertificatePayment.vue"

const required = (value: any): boolean => value === 0 || !!value

enum Steps
{
    Add, Confirm, Pay, Success
}

export default Vue.extend({
    middleware: "authy",
    meta: {
        auth: { comingSoon: true },
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
        giftCertificate,
        giftCertificateSummary,
        giftCertificatePayment
    },
    data() {
        return {
            selectionDate: ['2021-11-19'],
            s: style,
            showErrors: false,
            creating: false,
            buttonTheme: ButtonTheme,
            step: Steps.Add,
            Steps,
            certificatePayment: null as Payment | null
        };
    },
    validations() {
        return {
            amountVariantId: {},
            hasSurprises: {},
            comment: {},
            certificateSurprises: {
                $each: {
                    surpriseId: {},
                    quantity: {}
                }
            }
        }
    },      
    computed: {
        certificateAdding(): any {},
        ...mapProps(
            [
                'amountVariantId', 
                'hasSurprises', 
                'certificateSurprises', 
                'comment', 
                'addedCertificate'
            ],
            'giftCertificate', 
            'certificateAdding',
            'setCertificateAdding'
        ),           
        ...mapState('giftCertificate', ['amountVariants']),
        ...mapState('booking', ['payment']), 
        lang(): string {
            return this.$i18n.locale;
        },        
        surprises(): Surprise[] {
            return this.$store.state['booking'].menuInfo.surprises;
        },
        amountVariantsSelectItems(): SelectItem[] {
            return (this.amountVariants as AmountVariant[]).map(x => new SelectItem(x.id, '£ ' + x.amount.toString()));
        },        
        quantitiesSelectItems(): SelectItem[] {
            return [0,1,2,3,4,5,6,7,8,9,10].map(x => new SelectItem(x, x));
        },
        validUntil() {
            let date = new Date();
            date.setDate(1);
            date.setMonth(date.getMonth() + 4);
            date.setDate(date.getDate() - 1);
            return date;
        },
        paymentError(): string | null {
            return this.certificatePayment?.details?.error ?? null;
        },        
    },
    methods: {
        ...mapActions('booking', ['getMenuInfo']),
        ...mapActions('giftCertificate', [
            'getAmountVariants', 
            'postCertificateAdding', 
            'getGiftCertificates',
            'sendCertificatePaymentSuccess',
            'sendCertificatePaymentFail'
        ]),
        ...mapMutations('giftCertificate', ['resetAddedCertificate','setAddedCertificate']),
        getSurprise(id: number): Surprise | undefined {
            return this.surprises.find(x => x.id == id);
        },
        commit() {
            this.$store.commit('giftCertificate/setCertificateAdding', this.certificateAdding);     
        },
        goto(step: Steps): void {
            this.step = step;
        },
        async create() {
            this.showErrors = true;
            this.$v.$touch();
            if (!this.$v.$invalid) {
                this.creating = true;
                const createdCertificate = await this.postCertificateAdding();
                this.creating = false;
                if (createdCertificate.id) {
                    //this.$router.push({ name: `gift-certificate___${this.lang}`, params: { id: createdCertificate.id } });
                    //this.$router.push(`/gift-certificate/${createdCertificate.number}`);
                    this.setAddedCertificate(createdCertificate)
                    this.goto(Steps.Confirm);
                }                
            }
        },
        getError(field: any, tourIndex: any, timeIndex: any) {
            if (!this.showErrors) {
                return '';
            } else if (field.required === false) {
                return this.$t('validation.isRequired');
            } else {
                return '';
            }
        },
        getAmountVariant(id: number): AmountVariant {
            return this.amountVariants.find((x: AmountVariant) => x.id == id);
        },
        back() {
            this.goto(Steps.Add)
        },
        paid() {
            this.goto(Steps.Success)
        }
    },
    watch: {
        '$route.hash'() {
            this.goto(Steps.Add);
        }
    },
    async created() {

        this.resetAddedCertificate()

        await this.getAmountVariants()

        this.certificateSurprises = this.surprises.map(x => { 
            return {
                surpriseId: x.id,
                quantity: '0' as any
            } as CertificateSurprise;
        });
    }      
})
</script>