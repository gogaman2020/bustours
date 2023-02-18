<template>
    <div :class="s.root">
        <div v-for="item in items" :key="item.value" :class="s.item">
            <label>
                <checkbox :textBackward="true" :value="value.some(x => x == item.value)" :label="item.text" @input="onChange(item.value, $event)" />
            </label>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { SelectItem } from "@/types/common";
import checkbox from "@/components/controls/checkbox/checkbox.vue";

export default Vue.extend({
  name: "multiselectCheckbox",
  props: {
    items: Array as () => SelectItem[],
    value: Array as () => number[],
  },
  components: {
      checkbox
  },
  data() {
    return {
      selected: this.value,
      s: style,
    };
  },
  watch: {
    value(val) {
        this.selected = val;
    }
  },
  methods: {
      onChange(itemId: number, checked: boolean) {
        if (checked) {
            this.selected.push(itemId);
        } else {
            var index = this.selected.map(x => x.toString()).indexOf(itemId.toString());
            if (index > -1) {
                this.selected.splice(index, 1);
            }            
        }

        this.$emit('input', this.selected);
      }
  }
});
</script>