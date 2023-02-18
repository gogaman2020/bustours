<template>
	<BusLayout>
		<GiftCertificateManagement />
	</BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import GiftCertificateManagement from "~/components/giftCertificate/giftCertificateManagement/giftCertificateManagement.vue";
import { AuthorityLevel } from "@/types/private";

export default Vue.extend({
	middleware: "authy",
	meta: {
		auth: { authority: AuthorityLevel.Administrator },
	},	
	components: { 
		GiftCertificateManagement
	},
});
</script>