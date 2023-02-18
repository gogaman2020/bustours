<template>
	<BusLayout>
        <no-ssr>
		    <BookingAdmin :key="hash" />
        </no-ssr>
	</BusLayout>
</template>

<script lang="ts">
	import Vue from "vue";
    import BookingAdmin from "@/components/booking-admin/booking-admin/booking-admin.vue"
    import { AuthorityLevel } from "@/types/private"
    import config from "@/config";

	export default Vue.extend({
        middleware: "authy",
        meta: {
            auth: { authority: AuthorityLevel.Administrator },
        },        
        components: {
            BookingAdmin
        },
        computed: {
            hash() {
                return this.$route.hash;
            }
        }
    });
</script>