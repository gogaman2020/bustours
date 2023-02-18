<template>
    <div>{{formatted}}</div>
</template>

<script lang="ts">

import Vue from "vue"

export default Vue.extend({
    name: "duration",
    props: {
        value: {
            type: String,
            default: null 
        },
        variant: {
            type: String,
            default: 'short'
        }
    },
    data() { 
        return { }; 
    },
    computed: {
        lang(): string {
            return this.$i18n.locale
        },    
        formatted(): string {
            if (this.value) {
                const split = this.value.split(':');
                return `${parseInt(split[0])} ${this.$t('common.duration.hour')} ${split[1]} ${this.$t('common.duration.minute')}`
            } else {
                return '';
            }
        }
    }
});
</script>

