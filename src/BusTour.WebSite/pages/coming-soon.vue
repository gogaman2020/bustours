<template>
  <div :class="s.overlay">
    <div :class="[s.row, s.rowFirst]">Luxury bus tours</div>
    <div :class="[s.row, s.rowSecond]">Coming soon...</div>
    <!-- <login :class="s.login"></login> -->
  </div>
</template>

<script lang="ts">
import Vue from "vue";

import style from "./coming-soon.module.scss";

import login from "@/components/controls/login/login.vue";

export default Vue.extend({
  components: {
    login,
  },
  data() {
    return {
      s: style,
    };
  },
});
</script>