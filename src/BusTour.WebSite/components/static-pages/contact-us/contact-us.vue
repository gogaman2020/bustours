<template>
	<div :class="s.page">
		<h1 data-center>Contact us</h1>
		
		<control-item :class="s.row" :title="$t('staticPages.contactUs.subject')" >
			<TextInput :class="s.control" v-model="$v.subject.$model"/>
		</control-item>

		<control-item :class="s.rowTextarea"  :autoHeight="true" :title="$t('staticPages.contactUs.text')" >
			<TextInput :class="s.textArea" v-model="$v.text.$model" :multiline="true"/>
		</control-item>

		<phoneInput 
			:class="s.row"
			v-model="$v.phone.$model" 
			:title="$t('staticPages.contactUs.phoneNumber')"
		/>
		
		<bbButton :disabled="$v.validationGroup.$error" @click="send()" :theme="ButtonTheme.Black" :text="'Send'" />

		<div v-if="isSuccess" :class="s.success">
			{{ $t('staticPages.contactUs.success') }}
		</div>

	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";
import phoneInput from "@/components/controls/phone-input/phone-input-flags.vue";
import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import TextInput from "~/components/controls/text-input/text-input.vue";
import { required } from "vuelidate/lib/validators";

export default Vue.extend({
	components: {
        phoneInput,
		bbButton,
		TextInput,
    },
	data() {
		return {
			s,
			ButtonTheme,
			phone: {
                code: null,
                number: null
            },
			text: null,
			subject: null,
			isSuccess: false
		}
	},
	created() {
		this.$v.$touch();
	},
	validations: {
        phone: {
			code:  { required },
			number:  { required }
        },
		text: { required },
		subject: { required },
		validationGroup: ['phone','text', 'subject']
    },
	methods: {
		async send() {
			const request = {
				phone: '+' + this.phone.code + this.phone.number,
				text: this.text,
				subject: this.subject
			};

			await this.$store.dispatch('common/contactUs', request);

			this.isSuccess = true;

			this.phone.number = null;
			this.text = null;
			this.subject = null;
		}
	},
})
</script>