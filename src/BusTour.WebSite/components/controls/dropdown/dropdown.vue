<template>
    <div :class="[s.root, disabled ? s.disabled : '']">
        <!-- <v-select
            v-model="selected"
            :items="items"
            item-text="text"
            item-value="value"
            :menu-props="{ offsetY: true, closeOnClick: true }"
            :close-on-select="true"
            :attach="true"
        ></v-select> -->
        <vue-select
            v-if="type == 'vueselect'"
            v-model="selected"
            :options="items"
            :clearable="false"
            :searchable="false"
            :appendToBody="true"
            :calculate-position="withPopper"
            :disabled="disabled"
        >
            <template #option="option">
                <slot name="option" :option="option" >
                    <div :class="longname?s.selected:''" :data-title="option.text">{{option.text}}</div>
                </slot>
            </template>      
            <template #selected-option="option">
                <slot name="selected-option" :option="option">
                    <div :class="longname?s.selected:''" :data-title="getText(option)">{{getText(option)}}</div>
                </slot>                
            </template>     
            <template #open-indicator="{ attributes }">
              <span v-bind="attributes">
                <div :class="s.opener"></div>
                <!-- <img src="http://cdn1.iconfinder.com/data/icons/cc_mono_icon_set/blacks/16x16/br_down.png"> -->
              </span>
            </template>                        
        </vue-select>
        <select :class="s.select" v-model="selected" v-else>
            <option v-for="item in items" :key="item.value" :value="item.value">
            {{item.text}}
            </option>
        </select>     
    </div>
</template>

<script lang="ts">
    import Vue from "vue"
    import style from "./dropdown.module.scss"
    import vueSelect from 'vue-select'
    import 'vue-select/dist/vue-select.css'
    import { createPopper, Placement } from '@popperjs/core'

export default Vue.extend({
  name: "dropdown",
  props: {
    items: {
      type: Array
    },
    value: {
      type: [Number, String]
    },
    type: {
        type: String,
        default: 'vueselect'
    },
    disabled: Boolean,
    longname: {
        type: Boolean,
        default: false
    },
  },
  data() {
    return {
      s: style,
      placement: 'bottom'
    };
  },
  components: {
      vueSelect
  },
  computed: {
    selected: {
      get(): string | number | undefined {
        return this.value;
      },
      set(val: any) {
        this.$emit('input', val.value);
      }
    }
  },
    methods: {
        getText(item: any): string {
            let found = item ? (this as any).items.find((x: any) => x.value == item.label) : null;
            return found ? found.text : '';
        },
        withPopper(dropdownList: any, component: any, { width }: any) {
        /**
         * We need to explicitly define the dropdown width since
         * it is usually inherited from the parent with CSS.
         */
        dropdownList.style.width = width

        /**
         * Here we position the dropdownList relative to the $refs.toggle Element.
         *
         * The 'offset' modifier aligns the dropdown so that the $refs.toggle and
         * the dropdownList overlap by 1 pixel.
         *
         * The 'toggleClass' modifier adds a 'drop-up' class to the Vue Select
         * wrapper so that we can set some styles for when the dropdown is placed
         * above.
         */
        const popper: any = createPopper(component.$refs.toggle, dropdownList, {
            placement: this.placement as Placement,
            modifiers: [
            {
                name: 'offset',
                options: {
                offset: [0, -1],
                },
            },
            {
                name: 'toggleClass',
                enabled: true,
                phase: 'write',
                fn({ state }) {
                component.$el.classList.toggle(
                    'drop-up',
                    state.placement === 'top'
                )
                },
            },
            ],
        })

        /**
         * To prevent memory leaks Popper needs to be destroyed.
         * If you return function, it will be called just before dropdown is removed from DOM.
         */
        return () => popper.destroy()
        },        
    }
});
</script>

<style>
.v-select.drop-up.vs--open .vs__dropdown-toggle {
  border-radius: 0 0 4px 4px;
  border-top-color: transparent;
  border-bottom-color: rgba(60, 60, 60, 0.26);
}

[data-popper-placement='top'] {
  border-radius: 4px 4px 0 0;
  border-top-style: solid;
  border-bottom-style: none;
  box-shadow: 0 -3px 6px rgba(0, 0, 0, 0.15);
}
</style>