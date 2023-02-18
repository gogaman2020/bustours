<template>
	<ul :class="[style.selectList]">
		<li>
			<checkbox :class="style.selectListCheck"><span>Текст - 1</span></checkbox>
		</li>
		<li>
			<checkbox :class="style.selectListCheck"><span>Текст - 2</span></checkbox>
		</li>
	</ul>
</template>

<script>
	import style from './style.module.scss'

	import checkbox from 'EXT/otc2.0/components/controls/checkbox/checkbox'

	export default {
		name: 'select-list',
		components: {
			checkbox
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