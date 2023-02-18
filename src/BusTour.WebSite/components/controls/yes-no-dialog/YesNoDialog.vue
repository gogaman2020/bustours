<template>
	<div v-if="modalVisible" :class="style.modal" @click.self="close(false)">
		<div :class="style.modalWrapper" :style="customStyle">
			<div :class="style.modalContainer">
				<div :class="style.modalContent">
					<slot></slot>

					<div :class="style.modalButtons">
						<BbButton :text="yes ? yes : $t('yes')" :theme="ButtonTheme.Black" @click="close(true)" />
						<BbButton v-if="!hideNo" :text="no ? no : $t('no')" :theme="ButtonTheme.White" @click="close(false)" />
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import ModalBase from "@/components/controls/modal/modal-base";

import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue";

export default Vue.extend({
	name: "YesNoDialog",
	
	mixins: [ModalBase],

	components: {
		BbButton
	},

	props: {
		width: {
			type: Number,
			required: false,
			default: 482,
		},
		height: {
			type: Number,
			required: false,
			default: 184,
		},
		yes: {
			type: String,
			default: null,
		},	
		no: {
			type: String,
			default: null,
		},	
		hideNo: Boolean		
	},

	data() {
		return {
			style: style,
			customStyle: {
				width: `${this.width}px`,
				height: `${this.height}px`,
			},
			ButtonTheme,
		};
	},
});
</script>