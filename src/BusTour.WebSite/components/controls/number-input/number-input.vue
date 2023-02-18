<template>
  <div :class="style.numberInput">
    <numeric
      @input="emitInput($event)"
      :value="model"
      :min="min"
      :max="max"
      :precision="0"
      :controls="false"
      size="100%"
    />
    <div :class="style.numberInputButtons">
      <button @click="up" type="button"></button>
      <button @click="down" type="button"></button>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

import numeric from "vue-numeric-input";

export default Vue.extend({
  name: "number-input",
  components: {
    numeric,
  },
  props: {
    value: Number,
    min: Number,
    max: Number,
  },
  data() {
    return {
      style: style,
    };
  },
  methods: {
    up(): void {
      const newValue = this.model + 1;
      if (newValue <= this.max) {
        this.$emit("input", newValue);
      }
    },
    down(): void {
      const newValue = this.model - 1;
      if (newValue >= this.min) {
        this.$emit("input", newValue);
      }
    },
    emitInput(newValue: number): void {
      this.$emit("input", isNaN(newValue) ? null : newValue);
    },
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