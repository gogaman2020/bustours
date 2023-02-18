<template>
    <div :class="[s.root, !!error ? s.error : '', s[labelPosition], s[slotTag], s[variant]]">
        <label v-if="!label && !isHiddenTitle" :class="[s.title, {[s.titleTextLeft]: textLeft}]">
            {{title ? title : '&nbsp;'}}
            <sup v-if="isRequired" :class="s.required">*</sup>
        </label>
        <div :class="[s.control, isNoBack ? s.noBack : '', isDropdown ? s.dropdown : '', autoHeight ? s.autoHeight : '']">
            <label v-if="!!label" :class="[s.label]">
                <slot></slot>
                <span>
                    <slot name="label">
                        {{label}}
                    </slot>
                </span>
            </label>    
            <slot v-else></slot>        
        </div>
        <div v-if="error" :class="s.error">
            {{error}}
        </div>   
        <div v-else-if="success" :class="s.success">
            {{success}}
        </div>             
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./controlItem.module.scss";

export default Vue.extend({
  name: "ControlItem",
  props: {
    title: String,
    label: String,
    isRequired: Boolean,
    error: String,
    success: String,
    noBack: Boolean,
    textLeft: Boolean,
    variant: String,
    labelPosition: {
        type: String,
        default: 'top'
    },
    isHiddenTitle: {
        type: Boolean,
        default: false
    },
    autoHeight: Boolean
  },  
  data() {
    return {
      s: style,
    };
  },
  computed: {
    slotTag(): string {
      //return (this?.$slots as any).default[0]?.componentOptions?.tag
      return (this as any).$slots?.default?.[0]?.componentOptions?.Ctor?.extendOptions?.name ?? '';
    },
    isDropdown(): boolean {
      return ['dropdown','timepicker','calendarDate'].includes(this.slotTag);
    },
    isNoBack(): boolean {
        return this.noBack || ['checkbox'].includes(this.slotTag);
    } 
  }
});
</script>