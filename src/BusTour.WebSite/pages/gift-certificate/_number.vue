<template>
	<BusLayout>
		<GiftCertificate :number="$route.params.number" />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import GiftCertificate from "~/components/giftCertificate/giftCertificate/giftCertificate.vue"
import { mapMutations } from "vuex"
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Administrator },
	},
	components: { 
		GiftCertificate
	},
	methods: {
		...mapMutations('giftCertificate', ['resetAddedCertificate']),
	},
	created() {
		this.resetAddedCertificate()
	}
});
</script>