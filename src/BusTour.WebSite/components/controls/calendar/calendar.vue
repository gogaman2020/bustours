<template>
  <div :class="s.calendarContainer">
    <div :class="s.calendarHeader">
      <span :class="s.prevMonthBtn" @click="toPrevMonth">
        <svg width="20" height="20" viewBox="0 0 16 16" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
          <g class="transform-group">
            <g transform="scale(0.015625, 0.015625)">
              <path
                d="M671.968 912c-12.288 0-24.576-4.672-33.952-14.048L286.048 545.984c-18.752-18.72-18.752-49.12 0-67.872l351.968-352c18.752-18.752 49.12-18.752 67.872 0 18.752 18.72 18.752 49.12 0 67.872l-318.016 318.048 318.016 318.016c18.752 18.752 18.752 49.12 0 67.872C696.544 907.328 684.256 912 671.968 912z"
                fill="#ccc"
              ></path>
            </g>
          </g>
        </svg>
      </span>
      <span>{{ currentDisplayYearMonth }}</span>
      <span :class="s.nextMonthBtn" @click="toNextMonth">
        <svg width="20" height="20" viewBox="0 0 16 16" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
          <g class="transform-group">
            <g transform="scale(0.015625, 0.015625)">
              <path
                d="M761.056 532.128c0.512-0.992 1.344-1.824 1.792-2.848 8.8-18.304 5.92-40.704-9.664-55.424L399.936 139.744c-19.264-18.208-49.632-17.344-67.872 1.888-18.208 19.264-17.376 49.632 1.888 67.872l316.96 299.84-315.712 304.288c-19.072 18.4-19.648 48.768-1.248 67.872 9.408 9.792 21.984 14.688 34.56 14.688 12 0 24-4.48 33.312-13.44l350.048-337.376c0.672-0.672 0.928-1.6 1.6-2.304 0.512-0.48 1.056-0.832 1.568-1.344C757.76 538.88 759.2 535.392 761.056 532.128z"
                fill="#ccc"
              ></path>
            </g>
          </g>
        </svg>
      </span>
    </div>
    <ul :class="s.dateTable">
      <li v-for="day in weekDays" :key="day">{{ day }}</li>
      <li
        v-for="date in currentDisplayDates"
        :key="date.tag + date.value"
        :class="[s.dateCell ,
            isNoSeats(date.value) ? s.noSeats : '',
            date.tag === 'current-month'? s.currentMonth: s.currentMonth,
            isAvailable(date.value) ? s.aviableDate : s.notAviableDate,
            checkChecked(checkedDate, date.value) ? s.isChecked : '']"
        @click="checkDate(date)"
      >
        {{ displayDate(date.value) }}
      </li>
    </ul>
  </div>
</template>

<script>
import style from "./style.module.scss";

export default {
  name: 'calendar',
  props: {
    value: String,
    selectionDate:{
        type: Array,
        default: []
    },
    weekDays:{
      type: Array,
      default: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
    },
    unavailableDates:{
        type: Array,
        default: []
    },    
    isAvailableByDefault: Boolean,
    startAvailableDate: {
      type: Date,
      default: null
    },
    endAvailableDate: {
      type: Date,
      default: null
    },
    noSeatToursDates: {
      type: Array,
      default: null
    }
  },
  data() {
    return {
      s: style,
      currentDisplayYear: null,
      currentDisplayMonth: null,
      currentDisplayDates: [],
      checkedDate: this.value,
    };
  },
  computed: {
    currentDisplayYearMonth() {
      return `${this.currentDisplayYear}.${this.zeroPadding(this.currentDisplayMonth)}`;
    },
    displayDate() {
      return date => (date[8] === '0' ? date.substr(9) : date.substr(8));
    },
    
  },
  created() {
    this.renderCalendar(this.value);
  },
  watch: {
    value(val) {
      this.checkedDate = val;
      this.renderCalendar(val);
    },
  },
  methods: {
    /**
     * Calendar render method
     */
    renderCalendar(_Date) {
      this.currentDisplayDates = [];
      const _date = new Date(_Date ?? new Date());
      this.currentDisplayYear = _date.getFullYear();
      this.currentDisplayMonth = _date.getMonth() + 1;
      const currentDisplayMonth_1th_Date = new Date(`${this.currentDisplayYear}-${this.currentDisplayMonth}-1`).getDay(); // what day is the first day of the current display month
      const currentDisplayMonth_Date_Num = new Date(this.currentDisplayYear, this.currentDisplayMonth, 0).getDate(); // total days in current month
      const lastDisplayMonth_Date_Num = new Date(this.currentDisplayYear, this.currentDisplayMonth - 1, 0).getDate(); // total days in prev month
      const lastDisplayMonth = this.currentDisplayMonth === 1 ? 12 : this.currentDisplayMonth - 1; // prev display month
      const lastDisplayYear = lastDisplayMonth === 12 ? this.currentDisplayYear - 1 : this.currentDisplayYear; // prev display year
      const nextDisplayMonth = this.currentDisplayMonth === 12 ? 1 : this.currentDisplayMonth + 1; // next display month
      const nextDisplayYear = nextDisplayMonth === 1 ? this.currentDisplayYear + 1 : this.currentDisplayYear; // next display year
      // Push prev month date
      for (let i = 0; i < currentDisplayMonth_1th_Date; i++) {
        this.currentDisplayDates.unshift({
          tag: 'last-month',
          value: this.getFullDate(lastDisplayYear, lastDisplayMonth, lastDisplayMonth_Date_Num - i)
        });
      }
      // Push current month date
      for (let j = 0; j < currentDisplayMonth_Date_Num; j++) {
        this.currentDisplayDates.push({
          tag: 'current-month',
          value: this.getFullDate(this.currentDisplayYear, this.currentDisplayMonth, j + 1)
        });
      }
      // Push next month date
      for (let k = 0; k < 42 - currentDisplayMonth_1th_Date - currentDisplayMonth_Date_Num; k++) {
        this.currentDisplayDates.push({
          tag: 'next-month',
          value: this.getFullDate(nextDisplayYear, nextDisplayMonth, k + 1)
        });
      }
    },
    /**
     * Render prev month calendar
     */
    toPrevMonth() {
      this.currentDisplayMonth -= 1;
      if (this.currentDisplayMonth === 0) {
        this.currentDisplayYear -= 1;
        this.currentDisplayMonth = 12;
      }
      this.renderCalendar(`${this.currentDisplayYear}-${this.currentDisplayMonth}`);
    },
    /**
     * Render next month calendar
     */
    toNextMonth() {
      this.currentDisplayMonth += 1;
      if (this.currentDisplayMonth === 13) {
        this.currentDisplayYear += 1;
        this.currentDisplayMonth = 1;
      }
      this.renderCalendar(`${this.currentDisplayYear}-${this.currentDisplayMonth}`);
    },
    /**
     * Click date to select
     */
    checkDate(date) {
      if (date.tag === 'current-month' || true) {
        this.checkedDate = date.value;
        this.$emit('input', date.value);
      }
    },
    getFullDate(year, month, date) {
      return `${year}-${this.zeroPadding(month)}-${this.zeroPadding(date)}`;
    },
    /**
     * Zero padding method
     */
    zeroPadding(n) {
      n = n + '';
      return n[1] ? n : '0' + n;
    },
    isAvailable(dateValue) {
      let startAvailableDate = null;
      if (this.startAvailableDate) {
        startAvailableDate = new Date(this.startAvailableDate);
        startAvailableDate.setDate(startAvailableDate.getDate());
        startAvailableDate.setHours(0, 0, 0, 0);
      }
      let endAvailableDate = null;
      if (this.endAvailableDate) {
        endAvailableDate = new Date(this.endAvailableDate);
        endAvailableDate.setDate(endAvailableDate.getDate());
        endAvailableDate.setHours(0, 0, 0, 0);
      }

      return (!this.selectionDate || !this.selectionDate.length || this.selectionDate.includes(dateValue)) 
      && !this.unavailableDates.includes(dateValue)
      && (!startAvailableDate || new Date(dateValue) >= startAvailableDate && (!endAvailableDate || !startAvailableDate || new Date(dateValue) <= endAvailableDate));
    },
    checkChecked(a,b) {
      return a === b;
    },
    isNoSeats(value) {
      let result = false;
      
      if(this.noSeatToursDates !== null) {
        this.noSeatToursDates.forEach(date => {
           if(new Date(date).getDate() == new Date(value).getDate()) {
             result = true;
             return;
           }
        });
      }
      return result;
    }
  }
};
</script>

<style>

</style>