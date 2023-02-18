

<template>
    <div :class='s.claendar2Container'>
         <input
            :class="[s.settingInput, s.smoky]"
            autocomplete="off"
            type="text"
            readonly="readonly"
            aria-invalid="false"
            :value="valueStr"
            @click="openCalendar($event)"
        />

        <calendar-vue 
          :style="`
            ${showCalendar ? 'visibility: visible;' : 'visibility: hidden;'} 
            ${isOpenTop ? `top:-${topPosition}px; box-shadow: #111 2px -2px 3px;` : 'box-shadow: #111 2px 2px 3px;'}
          `" 
          :selectionDate="availableDatesStr"
          :unavailableDates="unavailableDatesStr"
          :value="valueISO"
          @input ="onInput" 
          :weekDays="weekDays"
          :startAvailableDate="startAvailableDate"
          :endAvailableDate="endAvailableDate"
          :isAvailableByDefault="true"
          :noSeatToursDates='noSeatToursDates'
        />

        <div v-if="showCalendar" :class='s.closeCalendar' @click="closeCalendar()"/>
        
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import calendarVue from "./calendar.vue";
import moment from 'moment'

export default Vue.extend({
  name: "calendarDate",
  components: {
    calendarVue
  },
  props:{
    availableDates: Array,
    unavailableDates: Array,  
    value: Date,
    startAvailableDate: {
      type: Date,
      default: null
    },
    endAvailableDate: {
      type: Date,
      default: null
    },
    lang: String,
    wrapClass: {
      type: String,
      default: null
    },
    noSeatToursDates: {
      type: Array,
      default: null
    }
  },
  data() {
    return {
      showCalendar: false,
      s: style,
      weekDays: this.$t("weekDays"),
      isOpenTop: false,
      topPosition: 0,
      oldWrapPadding: '',
    };
  },
  computed: {
    valueStr() {
      if (this.value) {
        return this.lang=='ru'?new Intl.DateTimeFormat('ru-RU', { month: "long", day: "numeric", year: "numeric"}).format(new Date(this.value)).toLowerCase()
						:new Intl.DateTimeFormat('en-GB', { month: "long", day: "numeric", year: "numeric"}).format(new Date(this.value)).toLowerCase();
      } else {
        return ''
      }
    },
    valueISO(): string {
        return (this as any).formatISO((this as any).value);
    },
    availableDatesStr() {
      return this.availableDates ? (this as any).availableDates.map((x: Date) => (this as any).formatISO(x)) : [];
    },
    unavailableDatesStr() {
      return this.unavailableDates ? this.unavailableDates.map(x => (this as any).formatDate(x)) : [];
    }    
  },
  methods:{
    async onInput(val: string){
      this.$emit('input', new Date(val));
      //this.closeCalendar();
    },
    formatISO(date: Date): string {
        return moment(date).format('YYYY-MM-DD')
    },
    formatDate(value: Date) : string {
      let date: Date = new Date(value);
      if (value) {
        var mm = date.getMonth() + 1;
        var dd = date.getDate();

        return [
                (dd>9 ? '' : '0') + dd,
                (mm>9 ? '' : '0') + mm,
                date.getFullYear(),
              ].join('.');
      } else {
        return '';
      }   
    },
    openCalendar(event: any) {
      if((this as any).wrapClass !== null) {
        const calendarInput = event.target as HTMLDivElement;
        const wrap = document.querySelector<HTMLDivElement>(`.${(this as any).wrapClass}`) as HTMLDivElement;
        const calendar = calendarInput.nextElementSibling as HTMLDivElement;

        const calendarHeight = calendar.scrollHeight as number;     
        const calendarWithInputHeight = calendar.getBoundingClientRect().bottom - calendarInput.getBoundingClientRect().top;

        const calendarInputTop = calendarInput.getBoundingClientRect().top;
        const calendarInputBottom = calendarInput.getBoundingClientRect().bottom;
        const magicMargin = 60; //отступ для расчета границ врапера (чтоб не в притык)
        const wrapTop = wrap.getBoundingClientRect().top + magicMargin;
        const wrapBottom = wrap.getBoundingClientRect().bottom - magicMargin;

        (this as any).oldWrapPadding = wrap.style.paddingBottom || '0px';

        if(calendarInputTop + calendarWithInputHeight > wrapBottom && calendarInputBottom - calendarWithInputHeight > wrapTop) {
          (this as any).isOpenTop = true;
          (this as any).topPosition = calendarHeight + 5;
        }
        else if(calendarInputTop + calendarWithInputHeight > wrapBottom + 10 || calendarInputBottom - calendarWithInputHeight < wrapTop  - 10) {
          (this as any).isOpenTop = false;
        
          if(wrap.style.transition === '') {
            wrap.style.transition = '.3s';
          }
          
          const newWrapPadding =  calendar.getBoundingClientRect().bottom - wrapBottom;
          wrap.style.paddingBottom = `calc(${(this as any).oldWrapPadding} + ${newWrapPadding}px)`;
        }
      }
      
      (this as any).showCalendar = true;
    },
    closeCalendar() {
      (this as any).showCalendar = false;
      
      if((this as any).wrapClass !== null) { 
        const wrap = document.querySelector<HTMLDivElement>(`.${(this as any).wrapClass}`) as HTMLDivElement;
        wrap.style.paddingBottom = (this as any).oldWrapPadding;
      }
    },
  }
})
</script>
