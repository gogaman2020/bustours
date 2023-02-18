<template>
  <div>Corporate events</div>
</template>

<script lang="ts">
import Vue from "vue";

import style from "./coming-soon.module.scss";
import config from "@/config";

import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
  //
  middleware: "authy",
  meta: {
    auth: { disabled: !config.showComingSoon },
  },
  //
  data() {
    return {
      s: style,
    };
  },
});
</script>