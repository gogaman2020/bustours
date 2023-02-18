<template>
    <div :class="style.wrapper">
        <span v-if="label" :class="style.labelText">{{label}}</span>

        <tooltip v-if="hint" :message="hint" :class="style.labelTooltip" />

        <div :class="style.box">
            <div :class="style.item">
                <date-picker
                    :value="value.from"
                    :placeholder="'с'"
                    :tabindex="minTabindex"
                    :classField="classField"
                    @input="handleInputFrom"/>
            </div>
            <div :class="style.separate">
                <svg width="17" height="1" viewBox="0 0 17 1" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <line y1="0.5" x2="16.0312" y2="0.5" :stroke="'black'"></line>
                </svg>
            </div>
            <div :class="style.item">
                <date-picker
                    :value="value.to"
                    :placeholder="'по'"
                    :tabindex="maxTabindex"
                    :classField="classField"
                    @input="handleInputTo"/>
            </div>
        </div>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import tooltip from 'EXT/otc2.0/components/common/tooltip/tooltip'
    import datePicker from 'EXT/otc2.0/components/controls/date-picker/date-picker'

    export default {
        name: "DateRange",

        components: {
            tooltip,
            datePicker
        },
        props: {
            label: {
				type: String,
				default: ''
            },
            hint: {
				type: String,
				default: ''
            },
            minTabindex: Number,
            maxTabindex: Number,
            name: String,
            classField: String,
            value: {
                type: Object,
                default() {
                    return {
                        from: null,
                        to: null
                    }
                }
            }
        },
        data() {
            return {
                style: style
            }
        },
        methods: {
            handleInputFrom(valueFrom) {
                const value = { ... this.value, from: valueFrom };

                if (value.from && value.to && value.from.getTime() > value.to.getTime()) {
                    value.to = new Date(value.from);
                }
                this.$emit('input', value);
            },

            handleInputTo(valueTo) {
                const value = { ... this.value, to: valueTo };

                if (value.from && value.to && value.from.getTime() > value.to.getTime()) {
                    value.from = new Date(value.to);
                }

                this.$emit('input', value);
            }
        }
    }
</script>
