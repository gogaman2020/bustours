<template>
    <div :class="[s.root, variant ? s[variant] : '']">

        <control-item :class="s.code" :title="title" :isRequired="isRequired" :error="codeError">
            <dropdown :items="countries" v-model="value.code">
                <template #option="{option}">
                    <div>{{option.text}}</div>
                </template>      
                <template #selected-option="{option}">
                    <div>{{option.label}}</div>               
                </template>                 
            </dropdown>
        </control-item>

        <control-item :class="s.number" :title="''" :error="numberError">
            <textInput v-model="value.number" :mask="'##############'" />
        </control-item>     

    </div>
</template>

<script lang="ts">
import Vue from "vue"
import style from "./style.module.scss"
import { countries, Country } from 'countries-list'
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import textInput from "@/components/controls/text-input/text-input.vue"
import { SelectItem } from "@/types/common"

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
        countries(): SelectItem[] {
            let countr = Object.keys(countries).map(x => (countries as any)[x] as Country);
            countr = countr.sort((a,b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0));
            return countr.map(x => new SelectItem(`+${x.phone}`, `${x.name} (${x.native})`));
        },
    },
    components: {
        dropdown,
        textInput
    },
    methods: {
        // getOptionText(code: string) {
        //     let country = this.countries.find((x: any) => x.phone == code);
        //     return country
        // },
        emitInput(e: InputEvent): void {
            this.$emit(
            "input",
            e.target && (<HTMLInputElement>e.target).value
                ? (<HTMLInputElement>e.target).value
                : null
            );
        },
    },
});
</script>>