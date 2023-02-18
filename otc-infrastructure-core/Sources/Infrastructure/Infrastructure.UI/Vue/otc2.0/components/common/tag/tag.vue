<template>
	<div :class="classNames">
		<slot></slot>
	</div>
</template>

<script>
	import style from './tag.module.scss'

	export default {
		name: 'tag',
		components: {

		},
		props: {
			theme: {
				type: String,
				default: 'white'
			},
			isActive: {
				type: Boolean,
				default: false
			}
		},
		data() {
			return {
				style: style
			}
		},

	    computed: {
	    	classNames() {
		        let classNames = [this.style.tag, this.isActive ? this.style.tagActive : ''];

		        this.theme == 'white' ? classNames.push(this.style.tagWhite) : classNames.push(this.style.tagBlack);


		        switch (this.size) {
		            case 'lg':
		                classNames.push(this.style.tagLg);
		                break;
		            case 'sm':
		                classNames.push(this.style.tagSm);
		                break;
		            default:
		                classNames.push(this.style.tagMid);
		                break;
		        }

		        if(this.disabled) classNames.push(this.style.tagDisabled)

		        return classNames;
		    }
	    }
	}
</script>