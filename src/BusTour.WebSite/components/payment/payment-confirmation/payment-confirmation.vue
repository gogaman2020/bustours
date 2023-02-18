<template>

    <div :class="s.root">

        <div :class="s.mainWrapper">

            <h1>{{$t('payment.confirmation.title')}}</h1>

            <h2>{{$t('payment.confirmation.selectMethod')}}</h2>

            <div :class="s.paymentMethods">
                <div 
                    :class="[s.paymentMethod, method == paymentMethod ? s.selected : '']" 
                    v-for="method in paymentMethods" 
                    :key="method"
                    @click="changePaymentMethod(method)"
                >
                    <img :src="getPaymentMethodImage(method)" />
                </div>
            </div>

            <h2 :class="s.paymentMethodHeader">{{paymentMethodHeader}}</h2>

            <div :class="[s.fields,s.cardInfo]">
                
                <div v-if="isCardPayment">

                    <control-item :class="s.row" :title="$t('payment.cardholdersName')" :isRequired="$v.cardholdersName.required !== undefined" :error="getError($v.cardholdersName)">
                        <textInput :placeholder="'Garry Potter'" v-model="$v.cardholdersName.$model" /> 
                    </control-item>      

                    <control-item :class="s.row" :title="$t('payment.cardNumber')" :isRequired="$v.cardNumber.required !== undefined" :error="getError($v.cardNumber)">
                        <textInput v-model="$v.cardNumber.$model" :mask="'#### #### #### ####'" />
                    </control-item>

                    <div :class="[s.expiryDateWrapper,s.row]">
                        <control-item :title="$t('payment.confirmation.expiryDate')" :isRequired="$v.cardMonth.required !== undefined" :error="getError($v.cardMonth)">
                            <dropdown v-model="$v.cardMonth.$model" :items="cardMonthSelectItems" />
                        </control-item>
                        <control-item :title="''" :error="getError($v.cardYear)">
                            <dropdown v-model="$v.cardYear.$model" :items="cardYearSelectItems" />
                        </control-item>
                    </div>

                    <control-item :class="[s.securityCodeWrapper,s.row]" :title="$t('payment.confirmation.securityCode')" :isRequired="$v.cardCvcCode.required !== undefined" :error="getError($v.cardCvcCode)">
                        <textInput v-model="$v.cardCvcCode.$model" :mask="'###'" /> 
                    </control-item>    

                    <div :class="s.separator"></div>

                </div>

                <control-item :class="s.row" :title="$t('payment.confirmation.bookingName')" :isRequired="$v.name.required !== undefined" :error="getError($v.name)">
                    <textInput :placeholder="'Garry Potter'" v-model="$v.name.$model" /> 
                </control-item>     

                <control-item :class="s.row" :title="$t('payment.confirmation.email')" :isRequired="$v.email.required !== undefined" :error="getError($v.email)">
                    <textInput :placeholder="'g.potter@hogwarts.uk'" v-model="$v.email.$model" /> 
                </control-item>  

                <control-item :class="s.row" :title="$t('payment.confirmation.repeatEmail')" :isRequired="$v.repeatemail.required !== undefined" :error="getError($v.repeatemail)">
                    <textInput :placeholder="'g.potter@hogwarts.uk'" v-model="$v.repeatemail.$model" />  
                </control-item> 

                <phoneInput 
                    v-model="$v.phone.$model" 
                    :class="s.row" 
                    :title="$t('payment.confirmation.phone')" 
                    :isRequired="$v.phone.code.required !== undefined" 
                    :codeError="getError($v.phone.code)"
                    :numberError="getError($v.phone.number)" 
                />

                <control-item :labelPosition="'right'" :class="[s.row, s.chckbx]" :label="$t('payment.confirmation.agreePersonalData')" :isRequired="$v.agreePersonalData.required !== undefined" :error="getError($v.agreePersonalData)">
                    <checkbox v-model="$v.agreePersonalData.$model" />  
                </control-item>     

                <control-item :labelPosition="'right'" :class="[s.row, s.chckbx]" :label="' '" :isRequired="$v.agreeTermsOfService.required !== undefined" :error="getError($v.agreeTermsOfService)">
                    <checkbox v-model="$v.agreeTermsOfService.$model" />
                    <template #label>
                        {{$t('payment.confirmation.iAgreeTo')}}
                        <a href="/terms-and-conditions" target="_blank">{{ $t("payment.confirmation.termsOfService") }}</a>
                        <!-- <nuxt-link :to="localePath('terms-and-conditions')">
						    {{ $t("payment.confirmation.termsOfService") }}
					    </nuxt-link> -->
                    </template>
                </control-item>

                <control-item :labelPosition="'right'" :class="[s.row, s.chckbx]" :label="$t('payment.confirmation.agreeNotifications')" :isRequired="$v.agreeNotifications.required !== undefined" :error="getError($v.agreeNotifications)">
                    <checkbox v-model="$v.agreeNotifications.$model" />  
                </control-item>       

                <div :class="s.separator"></div>    
                
                <div :class="s.amountToPay" v-if="!!amount">
                    <div :class="s.title">{{$t('payment.confirmation.amountToPay')}}</div>
                    <div :class="s.amount"><currency :adjustFont="false" v-model="amount" /></div>
                </div>                 
                
                <div :class="s.buttonsWrapper">
                    <slot name="extra-button"></slot>
                    <bbButton :disabled="!allOptionsAreChecked" @click="pay" :theme="ButtonTheme.Black" :class="s.payButton" :text="$t('pay')" />
                </div>
            </div>
        </div>
        <div :class="s.sidepanel">
            <slot name="sidepanel"></slot>
        </div>
    </div>

</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { AuthorityLevel } from "@/types/private"
import { PaymentMethod, SelectItem } from "@/types/common"
import style from "./style.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime from "@/components/display/dateTime.vue"
import infoPanel from "@/components/controls/info-panel/info-panel.vue"
import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import numeric from "@/components/controls/numeric/numeric.vue"
import phoneInput from "@/components/controls/phone-input/phone-input-flags.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"
import { email, helpers } from 'vuelidate/lib/validators'
import { mapActions, mapState, mapGetters, mapMutations } from "vuex"
import { mapProps } from "@/store/helpers"
import  { emailValidator, cardNumberValidator } from "@/utils/validators"

const required = (value: any): boolean => value === 0 || !!value
const checked = (value: any): boolean => value === 0 || !!value

export default Vue.extend({
    middleware: "authy",
    name: "PaymentConfirmation",
    props: {
        amount: Number
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
        numeric,
        phoneInput,
        checkbox
    },
    data() {
        return {
            s: style,
            ButtonTheme,
            showErrors: false,
            paymentMethod: PaymentMethod[PaymentMethod.Visa],
            cardholdersName: null,
            cardNumber: null,
            cardMonth: null,
            cardYear: null,
            cardCvcCode: null,
            agreePersonalData: false,
            agreeTermsOfService: false,
            agreeNotifications: false,
            PaymentMethod

        };
    },    
    validations() {
        let validations: any =  {
            paymentMethod: { required },
            cardholdersName: { },
            cardNumber: { },
            cardMonth: { },
            cardYear: { },
            cardCvcCode: { },
            name: { required },
            email: { required, email: emailValidator },
            repeatemail: { 
                required,
                emailMatch: (val: any, vm: any) => { return val == vm.email},
                email: emailValidator
            },
            phone: {
                code:  { },
                number:  { }
            },
            agreePersonalData: { checked },
            agreeTermsOfService: { checked },
            agreeNotifications: { checked }

        }

        if (this.isCardPayment) {
            validations.cardholdersName.required = required;
            validations.cardNumber.required = required;
            validations.cardNumber.cardNumber = cardNumberValidator;
            validations.cardMonth.required = required;
            validations.cardYear.required = required;
            validations.cardCvcCode.required = required;
        }

        return validations;
    },       
    computed: {
        ...mapProps(
            [	
                'email',
                'repeatemail',
                'name',
                'phone',
            ], 
            'booking', 
            'payment', 
            'setPayment'
        ),         
        paymentMethods() {
            return Object.keys(PaymentMethod).filter(key => !isNaN(Number((PaymentMethod as any)[key])));
        },
        paymentMethodHeader(): string {
            return this.$t('payment.confirmation.payThis') + ' ' + this.paymentMethod.toString();
        },
        cardMonthSelectItems(): SelectItem[] {
            return [1,2,3,4,5,6,7,8,9,10,11,12].map(x => new SelectItem(x, x.toString().length > 1 ? x : '0' + x));
        },
        cardYearSelectItems(): SelectItem[] {
            let selectItems: SelectItem[] = [];
            let year = new Date().getFullYear();
            for (let i = 0; i < 9; i++) {
                selectItems.push(new SelectItem(year + i));
            }
            return selectItems;
        },        
        lang(): string {
            return this.$i18n.locale;
        },
        isCardPayment(): boolean {
            return [PaymentMethod[PaymentMethod.Visa],PaymentMethod[PaymentMethod.MasterCard]].includes(this.paymentMethod);
        },
        allOptionsAreChecked(): boolean {
            return !Object.keys(this.$v).some(x => (this as any).$v[x]?.checked === false);
        }
    },
    watch: {
        cardholdersName(value, old) {
            if (this.name == old) {
                this.name = value;
            }
        }
    },
    methods: {
        ...mapMutations('booking', ['resetPayment']),
        getPaymentMethodImage(paymentMethod: PaymentMethod): string {
            return `/images/icons/payments/${paymentMethod.toString().toLowerCase()}.png`;
        },
        changePaymentMethod(paymentMethod: string) {
            this.paymentMethod = paymentMethod;
        },
        getError(field: any) {
            if (!this.showErrors) {
                return '';
            } else if (field.required === false) {
                return (this as any).$t('validation.isRequired');
            } else if (field.cardNumber === false) {
                return (this as any).$t('payment.confirmation.incorrectCardNumber');
            } else if (field.email === false) {
                return (this as any).$t('validation.incorrectEmail');                
            // } else if (field.checked === false) {
            //     return (this as any).$t('validation.isRequiredCheck');
            } else if (field.emailMatch === false) {
                return (this as any).$t('validation.emailNotMatch');                            
            } else {
                return '';
            }
        },
        pay() {

            this.showErrors = true;

            this.$v.$touch();

            const isValid = !this.$v.$invalid;

            if (isValid) {
                this.$emit('pay');
            }
        }
    }   
})
</script>