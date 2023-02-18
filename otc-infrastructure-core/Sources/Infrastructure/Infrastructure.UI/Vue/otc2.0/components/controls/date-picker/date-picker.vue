<template>
    <div :class="style.box">
        <div @click="onClick">
            <DateInput
                ref="dateInput"
                :tabindex="tabindex"
                :value="dateSelected"
                :classField="classField"
                :placeholder="placeholder"
                @input="handleInput"
                @keyup.esc="handleEscape"/>
        </div>
        <div v-if="visible" :class="style.calendarBox">
            <Calendar
                :class="style.calendar"
                :value="dateSelected"
                :format="calendarFormat"
                :clear-button="true"
                :placeholder="placeholder"
                :firstDayOfWeek="1"
                :hasInput="false"
                :onDayClick="onDaySelect" />

            <div :class="style.footer">
                <a :class="style.todayBtn" @click="todayClick">сегодня</a>
            </div>
        </div>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import { dateFormat } from 'EXT/utils/date'

    import Calendar from 'vue2-slot-calendar';
    import DateInput from "./date-input/date-input";

    const CALENDAR_LANG = {
        daysOfWeek: ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
        limit: "Limit reached ({{limit}} items max).",
        loading: "Загрузка...",
        minLength: "Min. Length",
        months: [
            "Январь",
            "Февраль",
            "Март",
            "Апрель",
            "Май",
            "Июнь",
            "Июль",
            "Август",
            "Сентябрь",
            "Октябрь",
            "Ноябрь",
            "Декабрь"
        ],
        notSelected: "Ничего не выбрано",
        required: "Required",
        search: "Search"
    };

    export default {
        name: "DatePicker",

        components: { 
            Calendar,
            DateInput 
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
                default: 'dd.mm.yyyy'
            },
            calendarFormat: {
                type: String,
                default: 'yyyy/MM/dd'
            }
        },
        data: function() {
            return {
                style : style,
                visible: false,
                dateSelected: null,
                dateSelectedText: "",
                lastValidValue: null,
                isValid: true
            };
        },
        computed: {
            listeners() {
                return {
                    ... this.$listeners,

                    input: dateSelectedText => this.handleInput(dateSelectedText)
                };
            }
        },
        watch: {
            value () {
                if (document.activeElement !== this.$refs.input) {
                    this.normalize();
                }
            }
        },
        created() {
            window.VueCalendarLang = () => { return CALENDAR_LANG; }

            document.addEventListener('click', this.tryClose);
            document.addEventListener('keyup', this.tryClose);
            this.normalize();
        },
        methods: {
            onClick: async function() {
                this.dateSelected = this.value || new Date();
                this.visible = !this.visible;
            },
            onDaySelect: async function(date) {
                this.visible = false;
                this.$emit('input', this.initDate(date));
            },
            todayClick: async function() {
                this.visible = false;

                let date = new Date();
                date.setHours(0, 0, 0, 0);
                this.$emit('input', this.initDate(date));
            },
            initDate: function(date){
                this.dateSelected = date;
                this.dateSelectedText = date && dateFormat(date, this.dateFotmat) || '';

                return date;
            },
            tryClose: function(e) {
                let el = e.target;

                while (el) {
                    if (el === this.$el) {
                        return;
                    }

                    el = el.parentElement;
                }

                this.visible = false;
            },
            normalize: function() {
                this.dateSelected = this.value;
                this.dateSelectedText = this.value && dateFormat(this.value, this.dateFormat) || '';
            },
            handleInput: function(value) {
                if (value) {
                    this.dateSelected = value;
                }
                this.$emit('input', value);
            },
            handleEscape() {
                this.visible = false;
                this.$refs.dateInput.blur();
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