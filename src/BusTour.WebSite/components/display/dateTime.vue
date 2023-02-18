<template>
    <div>{{formatted}}</div>
</template>

<script lang="ts">

import Vue from "vue"
import config from "@/config"
import moment from 'moment'

export enum DisplayType {
  Date,
  Time,
  DateTime,
}

export default Vue.extend({
  name: "dateTime",
  props: {
    value: {
      type: [Date, String],
      default: null
    },
    variant: {
      type: String,
      default: 'short'
    },
    type: {
      type: Number,
      default: () => DisplayType.Date
    }
  },
  data() {
    return {
    };
  },
  computed: {
    lang(): string {
        return this.$i18n.locale
    },    
    formatted(): string {
      if (this.value) {
        const date = new Date(this.value);
        const configByType = (config.formats as any)[DisplayType[this.type].toLowerCase()]; 
        const format = configByType[this.variant];

        return moment(date).locale(this.lang).format(format);
      } else {
        return '';
      }
    }
  }
});
</script>

