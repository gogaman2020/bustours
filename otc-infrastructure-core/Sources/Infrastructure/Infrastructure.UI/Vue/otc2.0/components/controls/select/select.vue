<template>
	<div :class="[style.select]">
		<select-text-input v-show="!list" :placeholder="placeholder" v-if="true" />
		<select-dropdown v-if="list" />
	</div>
</template>

<script>
	import style from './select.module.scss'

	import selectTextInput  	from './select-text-input/select-text-input'
	import selectDropdown 		from './select-dropdown/select-dropdown'
	import selectTags 			from './select-tags/select-tags'

	export default {
		name: 'select-control',
		components: {
			selectTextInput,
			selectDropdown,
			selectTags
		},
		props: {
			placeholder: {
				type: String,
				default: ''
			},
			list: false
		},
		data() {
			return {
				style: style
			}
		},
	}
</script>