<template>
<div :class="s.settingItem">
	<input
	:class="s.settingInput"
	:value="value"
	:type="type"
	@input="emitInput($event)"
	@paste="validate" 
	@keypress="validate"
	/>
	<div :class="[s.eye, type === 'password' ? s.eyeShow : s.eyeHide]" @click="changeInputType" />
</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";

export default Vue.extend({
name: "password-input",
props: {
	value: String,
	isPassword: {
			type: Boolean,
			default: true,
		},
},
data() {
	return {
	type: this.isPassword ? "password" : "text",
	s: style,
	};
},
methods: {
	validate(event: any) {
		let regex = /[A-Za-z0-9!@#$%^&*()-_=+\[\{\}\]\:;"'<,>.?/№]/;
		
		if(event.type === 'keypress' && !regex.test(event.key)) {
			event.preventDefault();	
		}

		if(event.type === 'paste') {
			let array: string[] = event.clipboardData.getData('text').split('');
			array.forEach(item => {
				if(!regex.test(item)) {
					event.preventDefault();
					return;
				}
			})
		}
	},
	changeInputType() {
		this.type = this.type === "password" ? "text" : "password";
	},
	emitInput(e: InputEvent): void {
		this.$emit(
			"input",
			e.target && (<HTMLInputElement>e.target).value
			? (<HTMLInputElement>e.target).value
			: null
		);
	},
},
});
</script>>