<template>
    <div :class="[s.root, adjustFont ? s.adjustFont : '']">
        <div :class="s.sign" v-if="marginTop !== null" :style="{'margin-top': marginTop + 'px'}">&#163;</div>       
        <div :class="s.sign" v-else>&#163;</div> 
        <div :class="s.amount">{{value ? value.toFixed(digits) : 0}}</div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./currency.module.scss"

export default Vue.extend({
  name: "currency",
  props: {
    value: Number,
    adjustFont: {
        type: Boolean,
        default: true
    },
    digits: {
        type: Number,
        default: 0
    },
    marginTop: {
        type: Number,
        default: null
    }
  },
  data() {
    return {
        s: style
    };
  },
});
</script>

