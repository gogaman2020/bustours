<template>
	<div class="[style.selectTags]">
		taaag
	</div>
</template>

<script>
	import style from './style.module.scss'

	import tag from 'EXT/otc2.0/components/common/tag/tag'

	export default {
		name: 'select-tags',
		components: {
			tag
		},
		props: {

		},
		data() {
			return {
				style: style
			}
		},
	}
</script>