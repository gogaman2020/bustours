<template>
	<div>
		<div v-if="isLoggedIn" :class="style.logoutBlock">
			<div :class="style.logout">
				<img src="~static/images/icons/logout.svg" />
				<button @click="logout">{{ $t("menu.logout") }}</button>
			</div>
			<div :class="style.userRole">
				{{ userRole }}
			</div>
		</div>

		<YesNoDialog ref="YesNoDialog">{{ $t("auth.logoutConfirmation") }}</YesNoDialog>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import { Roles } from "@/types/private";

import YesNoDialog from "~/components/controls/yes-no-dialog/YesNoDialog.vue";

export default Vue.extend({
	name: "auth-control",
	components: {
		YesNoDialog,
	},
	data()  {
		return {
			style,
		}
	},
	computed: {
		userRole(): Roles | null {
			return this.$auth?.user?.role;
		},
		isLoggedIn(): Boolean {
			return this.$auth.loggedIn;
		}
	},
	methods: {
		async logout(): Promise<void> {
			const answer = await ((this.$refs.YesNoDialog as any).open() as Promise<boolean>);

			if (answer) {
				await this.$auth.logout();
				this.$router.push(this.localePath("/"));
			}
		},
	}
})
</script>