<template>
  <select
    :class="[s.select, isWhite ? s.white : '']"
    :value="value"
    @input="emitInput($event)"
  >
    <option v-for="role in roles" :key="role" :value="role">
      {{ role }}
    </option>
  </select>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { Roles } from "@/types/private";

export default Vue.extend({
  name: "role-select",
  props: {
    value: String,
    isWhite: Boolean,
  },
  data() {
    return {
      s: style,
      roles: Object.keys(Roles).filter(x => x !== 'User'),
    };
  },
  methods: {
    emitInput(e: Event): void {
      this.$emit(
        "input",
        e.target && (<HTMLInputElement>e.target).value
          ? (<HTMLInputElement>e.target).value
          : null
      );
    },
  },
});
</script>