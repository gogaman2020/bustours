<template>
	<div v-if="modalVisible" :class="s.overlay" @click.self="close(false)">
		<div :class="s.modal">
			<span :class="s.cross" @click="close(false)">&times;</span>

			<div :class="s.title">
				<slot name="title">{{ modalTitle }}</slot>
			</div>

			<div :class="s.modalBody">
				<slot name="body">{{ modalBody }}</slot>
			</div>

			<div :class="s.modalFooter">
				<slot name="footer" :returnTrue="close.bind(this, true)" :returnFalse="close.bind(this, false)">
					<BbButton text="Close" :theme="ButtonTheme.Black" @click="close(true)" />
				</slot>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";
import ModalBase from "./modal-base";

import BbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue";

export default Vue.extend({
	mixins: [ModalBase],	

	components: {
		BbButton,
	},

	data() {
		return {
			s,
			ButtonTheme,
		}
	},
})
</script>