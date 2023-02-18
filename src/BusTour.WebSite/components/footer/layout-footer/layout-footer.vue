<template>
	<div class="noPrint">
		<div v-if="!isLoggedIn" :class="s.footer">
			<div :class="s.footerBlock">
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('covid-19-update')">
						{{ $t("footer.covid19Update") }}
					</nuxt-link>
				</div>
				
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('customer-charter')">
						{{ $t("footer.customerCharter") }}
					</nuxt-link>
				</div>

				<div :class="s.footerItem">
					<nuxt-link :to="localePath('faqs')">
						{{ $t("footer.faqs") }}
					</nuxt-link>
				</div>
			</div>

			<div :class="s.footerBlock">
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('privacy-policy')">
						{{ $t("footer.privacyPolicy") }}
					</nuxt-link>
				</div>
				
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('cookie-policy')">
						{{ $t("footer.cookiePolicy") }}
					</nuxt-link>
				</div>
			</div>

			<div :class="s.footerBlock">
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('terms-and-conditions')">
						{{ $t("footer.termsAndConditions") }}
					</nuxt-link>
				</div>
				
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('contact-us')">
						{{ $t("footer.contactUs") }}
					</nuxt-link>
				</div>
			</div>

			<div :class="s.footerBlock">
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('careers')">
						{{ $t("footer.careers") }}
					</nuxt-link>
				</div>
				
				<div :class="s.footerItem">
					<nuxt-link :to="localePath('modern-slavery-statement')">
						{{ $t("footer.modernSlaveryStatement") }}
					</nuxt-link>
				</div>
			</div>
		</div>
		<div v-if="false" :class="s.footerPrivate"></div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export default Vue.extend({
	name: "layout-footer",
	props: {
		isLoggedIn: Boolean,
	},
	data() {
		return {
			s: style,
		};
	}
});
</script>