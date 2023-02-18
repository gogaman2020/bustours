<template>
  <dropdown v-model="selected" :items="timesCalculated" />
  <!-- <select :class="s.select" v-model="selected">
    <option v-for="item in timesCalculated" :key="item.value" :value="item.value">
      {{item.text}}
    </option>
  </select> -->
</template>

<script lang="ts">
import Vue, { PropType } from "vue";
import style from "./timepicker.module.scss";
import { SelectItem, Time } from "@/types/common";
import dropdown from "@/components/controls/dropdown/dropdown.vue";

export default Vue.extend({
  name: "timepicker",
  props: {
    times: {
      type: Array,
      default: null
    },
    value: {
      type: Object,
      default: null
    },
    startHour: {
      type: Number,
      default: 0
    },
    endHour: {
      type: Number,
      default: 24
    },  
    step: {
      type: Number,
      default: 30
    }
  },
  data() {
    return {
      s: style
    };
  },
  components: {
    dropdown
  },
  computed: {
    timesCalculated() {
      let times: any[] = [];

      if (this.times) {
        times = this.times;
      } else {
        let currentTime = new Time(this.startHour, 0, 0);
        const endTime = new Time(this.endHour, 0, 0);
        let lastTime = currentTime;
        while(true) {
          if (currentTime.date.getTime() > endTime.date.getTime() || currentTime.date.getTime() < lastTime.date.getTime()) {
            break
          } else {
            lastTime = currentTime
            times.push(currentTime);
            currentTime = currentTime.addTime(new Time(0, this.step, 0));
          }
        }
      }

      return times.map(x => new SelectItem(x.toString(), x.toShortString()));
    },
    selected: {
      get(): string | null {
        return this.value ? Time.fromPlain(this.value).toString() : null;
      },
      set(value: string) {
        this.$emit('input', Time.fromString(value + ':00'));
      }
    },    
  } 
});
</script>