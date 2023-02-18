<template>
  <div :class="s.wrapper">
    <layout-header :isLoggedIn="isUserLoggedIn" :userRole="userRole" />
    <div :class="s.content">
      <Nuxt />
    </div>
    <layout-footer :is-logged-in="isUserLoggedIn" />
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import LayoutHeader from "@/components/header/layout-header/layout-header.vue";
import LayoutFooter from "@/components/footer/layout-footer/layout-footer.vue";

import { Roles } from "@/types/private";

export default Vue.extend({
  components: {
    LayoutHeader,
    LayoutFooter,
  },
  data() {
    return {
      s: style,
    };
  },
  computed: {
    isUserLoggedIn: {
      get(): boolean {
        return this.$auth.loggedIn;
      },
    },
    isUserRole: {
      get() {
        return this.$auth.user.role === Roles.User;
      },
    },
    userRole(): Roles | null {
      return this.$auth?.user?.role;
    }
  },
});
</script>