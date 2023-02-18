<template>
    <div :class="s.container" v-if="showForm">
        <login :returnUrl="$route.query.returnUrl" />
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./authorization.module.scss";

import login from "@/components/controls/login/login.vue";

export default Vue.extend({
    name: "authorization",
    components: {
        login,
    },
    data() {
        return {
            s: style,
            showForm: false
        };
    },
    async created() {
        if (this.$route?.query?.returnUrl) {
            if (!this.$auth.loggedIn) {
                try {
                    await this.$auth.fetchUser()
                } catch { }
            } 
            if (this.$auth.loggedIn) {
                this.$router.push(this.localePath(this.$route.query.returnUrl.toString()))
            } else {
                this.showForm = true
            }
        } else {
            this.showForm = true
        }
    }
});
</script>