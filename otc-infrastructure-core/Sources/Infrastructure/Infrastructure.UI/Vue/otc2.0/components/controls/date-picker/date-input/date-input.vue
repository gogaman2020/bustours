<template>
    <div :class="style.box">
        <input
            ref="input"
            :tabindex="tabindex"
            :value="dateString"
            :placeholder="placeholder"
            :class="[style.dateValue, classField]"
            v-on="listeners"
            @focus="handleFocus"
            @blur="handleBlur"/>
        <div :class="style.iconBox" @click="focus">
            <DateIcon :class="style.icon" />
        </div>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import DateIcon from './date-icon'
    import { dateParse, dateFormat } from 'EXT/utils/date'

    export default {
        name: "DateInput",

        components: {
            DateIcon
        },
        props: {
            tabindex: Number,
            value: Date,
            classField: String,
            placeholder: String
        },
        data() {
            return {
                dateString: null,
                isValid: true,
                lastValidValue: null,

                style: style
            }
        },
        computed: {
            listeners() {
                return {
                    ... this.$listeners,

                    input: dateString => this.handleInput(dateString)
                };
            }
        },
        watch: {
            value() {
                if (document.activeElement !== this.$refs.input) {
                    this.normalize();
                }
            }
        },
        created() {
            this.normalize();
        },
        methods: {
            handleFocus: function() {
                this.lastValidValue = this.value;
            },
            handleInput: function(e) {
                const dateString = e.target.value;

                this.dateString = dateString;

                const parseResult = dateParse(dateString);

                this.isValid = parseResult.isValid;

                if (this.isValid) {
                    this.lastValidValue = parseResult.dateValue;
                }

                this.$emit('input', parseResult.dateValue);
            },
            handleBlur: function() {
                if (!this.isValid) {
                    this.$emit('input', this.lastValidValue);
                } else {
                    this.normalize();
                }
            },
            normalize() {
                this.dateString = this.value && dateFormat(this.value, 'dd.mm.yyyy') || '';
            },
            focus() {
                this.$refs.input.focus();
            },
            blur() {
                this.$refs.input.blur();
            }
        }
    }
</script>
