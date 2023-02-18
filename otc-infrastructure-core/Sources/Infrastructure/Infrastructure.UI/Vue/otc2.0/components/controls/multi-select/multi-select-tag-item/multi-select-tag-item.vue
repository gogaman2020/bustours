<template>
    <div :class="style.box">
        <div :class="style.text" v-tooltip.top-center="tooltipText">{{ text }}</div>
        <div :class="style.close" @click.stop="$emit('remove')"/>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import tooltip from 'EXT/otc2.0/components/common/tooltip/tooltip'

    const TEXT_OVERFLOW_ENDING = '...';

    export default {
        name: "multi-select-tag-item",

        components: {
            tooltip
        },
        props: {
            value: String,
            maxTextLength: {
                type: Number,
                default: 26
            }
        },
        data() {
            return {
                style: style
            }
        },
        computed: {
            maxtxt() {
                if (!this.maxTextLength || this.maxTextLength < 0) return null;
                return this.maxTextLength - TEXT_OVERFLOW_ENDING.length;
            },
            text() {
                let {value, maxtxt} = this;
                if (maxtxt && value && value.length > maxtxt)
                    return value.substring(0, maxtxt) + TEXT_OVERFLOW_ENDING;

                return value;
            },
            tooltipText() {
                return this.value && this.value.length > this.maxtxt ? this.value : '';
            }
        }
    }
</script>
