<template>
	<div :class="[style.selectDropdown]">
		<select-tags/>
		<input :class="[style.selectDropdownInput]" type="text"> 
		<!-- to do: когда инпут пустой повесить на него класс selectDropdownInputEmpty  -->
		<select-list :class="[style.selectDropdownList]" />
	</div>
	
</template>

<script>
	import style from './style.module.scss'

	import selectList from './select-list/select-list'
	import selectTags from '../select-tags/select-tags'

	export default {
		name: 'select-dropdown',
		components: {
			selectList,
			selectTags
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