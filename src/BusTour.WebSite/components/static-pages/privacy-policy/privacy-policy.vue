<template>
	<div :class="s.page">
		<h1 data-center>privacy policy</h1>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

export default Vue.extend({
	data() {
		return {
			s,
		}
	}
})
</script>