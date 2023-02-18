<template>
    <div :class="[s.root, variant ? s[variant] : '']">
        <input
            v-if="currentMode == 'edit'"
            :value="value"
            @input="emitInput($event)"
            :placeholder="placeholder"
        />
        <span v-else :class="s.viewspan">{{value ?  value : placeholder}}</span>
        <img v-if="hasModeToggler && currentMode == 'view'" @click="currentMode = 'edit'" src="/images/icons/pen.png" />
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export default Vue.extend({
  name: "email",
  props: {
    value: String,
    variant: String,
    placeholder: String,
    hasModeToggler: Boolean,
    mode: {
        type: String,
        default: 'edit'
    }
  },
  data() {
    return {
      s: style,
      currentMode: this.mode
    };
  },
  methods: {
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