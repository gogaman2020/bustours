<template>
	<div class="portlet box otcSubBox">
        <div v-show="!hiddenHeader" class="portlet-title" @click="hidden = !hidden">
            <div class="caption font-white">
            	<slot name="icon"></slot>
                <span class="caption-subject bold uppercase">
                	<slot name="header"></slot>
                </span>
            </div>
            <div class="tools" onclick="Common.onFormBoxToolsClick()">
                <a href="javascript:;" :class="['expandCollapseBtn', hidden ? 'collapse' : 'expand']"></a>
            </div>
        </div>
		<div v-show="hiddenHeader" style="border-top: 1px solid #bfbfbf"/>
        <transition name="slide">
        	<div v-show="!hidden" class="portlet-body">
				<slot></slot>
	        </div>
        </transition>
    </div>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: 'box',
		components: {

		},
		props: {
			hide: {
				type: Boolean,
				default: false,
				required: false
			},
			hideHeader:
			{
				type: Boolean,
				default: false,
				required: false
            }
		},
		data() {
			return {
				style: style,
				hidden: this.hide,
				hiddenHeader: this.hideHeader
			}
		}
	}
</script>