<template>
    <div :class="[s.root, variant ? s[variant] : '']">
        <div v-if="multiline" :class="s.subRoot">
            <textarea :value="value" @input="emitInput($event)" :placeholder="placeholder" />
        </div>
        <div v-else :class="s.subRoot">
          <input
              v-if="currentMode == 'edit'"
              :value="value"
              @input="emitInput($event)"
              @blur="$emit('blur')"
              :placeholder="placeholder"
              v-mask="mask"
          />
          <span v-else :class="s.viewspan">{{value ?  value : placeholder}}</span>
          <img v-if="hasModeToggler && currentMode == 'view'" @click="currentMode = 'edit'" src="/images/icons/pen.png" />
        </div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export default Vue.extend({
  name: "textInput",
  props: {
    value: [String, Number],
    variant: String,
    placeholder: String,
    hasModeToggler: Boolean,
    mode: {
        type: String,
        default: 'edit'
    },
    mask: {
      type: [ String, Function ],
      default: ''
    },
    filter: {
      type: String,
      default: ''
    },
    multiline: Boolean
  },
  data() {
    return {
      s: style,
      currentMode: this.mode,
      currentValue: ''
    };
  },
  watch: {
      currentMode(mode) {
          this.$emit('changeMode', mode);
      }
  },
  methods: {
    emitInput(e: InputEvent): void {

      const target = <HTMLInputElement>e.target;
      
      if (this.currentValue != target.value) {

        this.currentValue = target.value;

        this.$emit(
          "input",
          e.target && target.value
            ? target.value
            : null
        );
      }
    },
  },
});
</script>>