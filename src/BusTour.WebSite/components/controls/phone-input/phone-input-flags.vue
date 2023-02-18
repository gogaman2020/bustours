<template>
    <div :class="[s.root, s.withFlags, variant ? s[variant] : '']">
        <control-item :class="s.controlItem" :title="title" :isRequired="isRequired" :error="codeError">
            <VuePhoneNumberInput 
                v-model="valueStr" 
                default-country-code="GB"
                @update="emitInput($event)" 
                :dark="true"
                :dark-color="'#f5f5f5'"
                :translations="{
                    countrySelectorLabel: '',
                    countrySelectorError: '',
                    phoneNumberLabel: '',
                    example: ''                    
                }"
            />   
        </control-item>
    </div>
</template>

<script lang="ts">
import Vue from "vue"
import style from "./style.module.scss"
const VuePhoneNumberInput = require('vue-phone-number-input')
import 'vue-phone-number-input/dist/vue-phone-number-input.css'

export default Vue.extend({
    name: "PhoneInput",
    props: {
        value: Object,
        variant: String,
        title: String,
        isRequired: Boolean,
        codeError: String,
        numberError: String
    },
    data() {
        return {
            s: style
        };
    },
    computed: {
        valueStr: {
            get(): string {
                return this.value.number ?? '';
            },
            set() {}
        }
    },
    components: {
        VuePhoneNumberInput
    },
    methods: {
        validate(value: String): Boolean {
            let result = true;
            if(value !== undefined) {
                let regex = /[0-9]/;
    
                [...value].forEach(item => {
                    if(!regex.test(item)) {
                        console.log(item);
                        
                        result = false;
                    }
                });
            }
            return result;
        },
        emitInput(e: any): void {
            if (this.validate(e.phoneNumber)) {
                this.$emit("input", { 
                    code: e.countryCallingCode,
                    number: e.phoneNumber
                });
            }
            
        },
    },
});
</script>>