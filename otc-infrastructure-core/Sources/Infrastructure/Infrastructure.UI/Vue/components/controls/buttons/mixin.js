import style from './style.module.scss';

export const buttonMixin = {
    props: {
        disabled: {
            type: Boolean,
            default: false,
            required: false
        },
        type: {
            type: String,
            default: 'button',
            required: false
        },
    },
    data() {
        return {
            style: style
        };
    },
    methods: {
        handlerOnClick(e) {
            this.$emit('click', e);
        }
    }
};