import style from './style.module.scss';

export const click = {
	props: {
		size: {
			type: String,
			default: 'mid',
			required: false
		},
		disabled: {
			type: Boolean,
			default: false,
			required: false
		}
	},
	data() {
		return {
			style: style
		}
	},
	methods: {
		handlerOnClick(e) {
			this.$emit('click', e);
		}
	}
}