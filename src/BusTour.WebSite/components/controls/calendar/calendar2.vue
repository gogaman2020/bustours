<template>
    <div :class='s.claendar2Container'>
         <input
            :class="[s.settingInput, s.smoky]"
            autocomplete="off"
            type="text"
            readonly="readonly"
            aria-invalid="false"
            :value="value"
            @click="showCalendar = true"
        />

        <calendar-vue v-if="showCalendar" :selectionDate="selectionDate" v-model='date' @input ="onInput" :weekDays="weekDays"/>
        <div v-if="showCalendar" :class='s.closeCalendar' @click="showCalendar = false"/>
        
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import calendarVue from "./calendar.vue";

export default Vue.extend({
  name: "calendar2",
  components: {
    calendarVue
  },
  props:{
    selectionDate:{
        type: Array,
        default: []
    },
    value: String,
  },
  data() {
    return {
      showCalendar: false,
      s: style,
      date: this.value,
      weekDays: this.$t("weekDays")
    };
  },
  methods:{
    async onInput(val: string){
      this.$emit('input', val);
    }
  }
})
</script>
