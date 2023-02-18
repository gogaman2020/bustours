<template>
	<div :class="s.page">
		<h1 data-center>Terms and conditions</h1>
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