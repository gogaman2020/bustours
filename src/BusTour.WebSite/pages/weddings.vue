<template>
  <div>Weddings</div>
</template>

<script lang="ts">
import Vue from "vue";
import config from "@/config";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
  middleware: "authy",
  meta: {
    auth: { comingSoon: true },
  },
});
</script>