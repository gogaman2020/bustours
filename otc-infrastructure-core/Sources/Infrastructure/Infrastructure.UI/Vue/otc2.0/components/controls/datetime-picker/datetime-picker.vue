<template>
    <div :class="style.datetimeBox">
        <span v-if="label" :class="style.datetimeBoxLabel">{{label}}</span>

        <tooltip v-if="hint" :message="hint" :class="style.datetimeBoxTooltip" />

        <date-picker :class="style.date"
            v-model="dateConvert"
            :type="'datetime'"
            :format="calendarFormat"
            :inputClass="style.dateValue"
            :popupClass="style.datePopup"
            :showSecond="false"
            :minuteStep="5"
            @input="handleInput"/>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import { dateFormat } from 'EXT/utils/date'

    import DatePicker from 'vue2-datepicker';
    import 'vue2-datepicker/index.css';
    import 'vue2-datepicker/locale/ru';


    export default {
        name: "DateTimePicker",

        components: { 
            DatePicker
        },
        props: {
            tabindex: Number,
            classField: String,
            placeholder:{
                type: String,
                default: ""
            },
            value: {
                type: Date,
                default: new Date()
            },
            dateFormat: {
                type: String,
                default: 'dd.mm.yyyy HH:mm'
            },
            calendarFormat: {
                type: String,
                default: 'DD.MM.YYYY HH:mm'
            },
            hint: {
				type: String,
				default: ''
            },
            label: {
				type: String,
				default: ''
            },
        },
        data: function() {
            return {
                style : style,
                visible: false,
                dateSelected: this.value,
                isValid: true
            };
        },
        computed: {
            listeners() {
                return {
                    ... this.$listeners,

                    input: dateSelectedText => this.handleInput(dateSelectedText)
                };
            },
            dateConvert: function() {
                return this.dateSelected ? new Date(this.dateSelected) : null;
            }
        },
        watch: {
            value () {
                this.normalize();
            }
        },
        created() {
            this.normalize();
        },
        methods: {
            normalize: function() {
                this.dateSelected = this.value ? new Date(this.value) : null;
            },
            handleInput: function(value) {
                if (value) {
                    var now_utc =  Date.UTC(value.getUTCFullYear(), value.getUTCMonth(), value.getUTCDate(),
                                            value.getUTCHours(), value.getUTCMinutes(), value.getUTCSeconds());

                    this.dateSelected = new Date(now_utc);
                } else {
                    this.dateSelected = null;
                }
                this.$emit('input', this.dateSelected ? this.dateSelected.toISOString() : '');
            }
        }
    }
</script>

<style lang="scss">
    .datepicker div {
        display: block;
    }
    .datepicker-body p {
        cursor: pointer;
    }
    .datepicker-popup {
        border: none;
        border-radius: 0;
        box-shadow: none;
        min-width: 0 !important;
    }
</style>