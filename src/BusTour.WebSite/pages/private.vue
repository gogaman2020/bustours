<template>
  <administrator-control />
</template>

<script lang="ts">
import Vue from "vue";
import administratorControl from "@/components/private/administrator-control.vue";
import { AuthorityLevel } from "@/types/private";
import config from "@/config";

export default Vue.extend({
  name: "private",
  components: {
    administratorControl,
  },
  middleware: "authy",
  meta: {
    auth: { authority: AuthorityLevel.Administrator },
  },
});
</script>