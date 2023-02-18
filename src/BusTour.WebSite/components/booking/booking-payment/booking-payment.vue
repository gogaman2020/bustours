<template>
	<Payment :amount="totalPrice+calculationCostTour.vat" @pay="pay">
		<template #extra-button>
			<BbButton v-if="!hideBackButton" :text="$t('buttons.back')" :theme="ButtonTheme.Black" @click="$emit('click')" />
		</template>
	</Payment>
</template>

<script lang="ts">
import Vue from "vue";

import Payment from "~/components/payment/payment-confirmation/payment-confirmation.vue"
import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue"
import { mapGetters, mapActions } from "vuex"
import { mapProps } from "@/store/helpers"

export default Vue.extend({
	props: {
		hideBackButton: Boolean
	},
	components: {
		Payment,
		BbButton,
	},

	data() {
		return {
			ButtonTheme,
		}
	},

	computed: {
        ...mapProps(
            [	
                'email',
            ], 
            'booking', 
            'payment', 
            'setPayment'
        ),		
        ...mapProps(
            [	
                'id',
				'hash'
            ], 
            'booking', 
            'order', 
            'setOrder'
        ),			
		...mapGetters({
			totalPrice: "booking/totalPrice",
			calculationCostTour: "booking/calculationCostTour"
		}),
        lang(): string {
			return this.$i18n.locale
		},		
	},

	methods: {
		...mapActions('booking', ['sendOrderPaymentSuccess', 'sendOrderPaymentFail','addNotification']),		
		async pay() {
			if (this.email.startsWith('a')) {
				await this.sendOrderPaymentFail('Incufficient funds')
			} else {
				await this.sendOrderPaymentSuccess()
				await this.addNotification({email:this.email, lang:this.lang});		
			}
			this.$router.push(`/order/${this.hash}`)
			this.$emit('paid')
		}
	}
});
</script>