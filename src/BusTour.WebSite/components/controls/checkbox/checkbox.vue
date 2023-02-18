<template>
  <label :class="[s.root, { [s.textBackward]: textBackward }, s[variant]]">
    <div :class="s.label" v-if="label">{{label}}</div>    
    <div :class="s.container" :title="title ? title : ''">
      <input :class="s.input" type="checkbox" :disabled="disabled" :checked="value === true" @change="$emit('input', $event.target.checked)" />
      <span  :class="s.span"></span>
    </div>
  </label>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./checkbox.module.scss";

export default Vue.extend({
  name: "checkbox",
  props: {
    value: Boolean,
    label: String,
    textBackward: Boolean,
    title: String,
    variant: {
        type: String,
        default: ''
    },
    disabled: Boolean
  },
  data() {
    return {
      s: style,
    };
  },
});
</script>