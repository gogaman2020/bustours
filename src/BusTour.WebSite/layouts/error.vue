<template>
  <div>
    <h1>Error</h1>
    <a target="_self" href="/">Go to Home page</a>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

import config from "@/config";

export default Vue.extend({
  //
  mounted() {
    if (config.showComingSoon) this.$router.push(this.localePath("/"));
  },
  //
});
</script>