<template>
  <div :class="s.numeric">
    <vuenumeric
      v-model="model"
      :min="min"
      :max="max"
      :precision="0"
      :controls="false"
      size="100%"
    />
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./numeric.module.scss";

import vuenumeric from "vue-numeric-input";

export default Vue.extend({
  name: "numeric",
  components: {
    vuenumeric,
  },
  props: {
    value: Number,
    min: {
      type: Number,
      default: 0
    },
    max: { 
      type: Number,
      default: 10000
    }
  },
  data() {
    return {
      s: style,
    };
  },
  computed: {
    model: {
      get(): number {
        return this.value;
      },
      set(value: number): void {
        this.$emit("input", value);
      },
    },
  },
});
</script>