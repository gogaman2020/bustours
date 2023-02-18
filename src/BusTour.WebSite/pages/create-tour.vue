<template>
  <BusLayout :class="s.root">
    <h1>{{$t("createTour.title")}}</h1>

    <div :class="s.itemsRow">
      <control-item :class="[s.controlItem,s.double]" :title="$t('createTour.route')" :isRequired="$v.routeId.required !== undefined" :error="getError($v.type)">
        <dropdown v-model="$v.routeId.$model" :items="routeItems" /> 
      </control-item>
    </div>    

    <div :class="s.itemsRow">
      <control-item :class="s.controlItem" :title="$t('createTour.dateStart')" :isRequired="$v.dateStart.required !== undefined" :error="getError($v.dateStart)">
        <calendar :lang="this.lang" v-model="$v.dateStart.$model" :unavailableDates="unavailableDates" :startAvailableDate="new Date()" /> 
      </control-item>
      <control-item :class="s.controlItem" :title="$t('createTour.dateEnd')" :isRequired="$v.dateEnd.required !== undefined" :error="getError($v.dateEnd)">
        <calendar :lang="this.lang" v-model="$v.dateEnd.$model" :unavailableDates="unavailableDates" :startAvailableDate="dateStart"/> 
      </control-item>
    </div>

    <div :class="s.itemsRow">
      <control-item :class="s.controlItem" :title="$t('createTour.type')" :isRequired="$v.type.required !== undefined" :error="getError($v.type)">
        <dropdown v-model="$v.type.$model" :items="tourTypes" /> 
      </control-item>
      <control-item :noBack="true" :class="s.controlItem" :title="$t('createTour.chooseWeekdays')">
        <checkbox v-model="$v.chooseWeekdays.$model" /> 
      </control-item>
    </div>    

    <div v-for="(tour, tourIndex) in $v.tours.$each.$iter" :key="'tour_' + tourIndex" :class="s.tour">

      <div :class="s.itemsRow">
        <control-item :class="s.controlItem" :title="$t('createTour.weekdayStart')" :isRequired="tour.weekdayStart.required !== undefined" v-if="chooseWeekdays" :error="getError(tour.weekdayStart, tourIndex)">
          <dropdown v-model="tour.weekdayStart.$model" :items="weekdays" @input="commit" /> 
        </control-item>
        <control-item :class="s.controlItem" :title="$t('createTour.weekdayEnd')" :isRequired="tour.weekdayEnd.required !== undefined" v-if="chooseWeekdays" :error="getError(tour.weekdayEnd, tourIndex)">
          <dropdown v-model="tour.weekdayEnd.$model" :items="weekdays" @input="commit" /> 
        </control-item>
        <div v-if="isRegular" :class="s.innerRow">
          <control-item 
            :class="s.controlItem"
            :title="$t('createTour.setPeriodTime')" 
            v-for="(time, timeIndex) in tour.times.$each.$iter" 
            :isRequired="time.required !== undefined" 
            :error="getError(time, tourIndex, timeIndex)" 
            :key="'time_' + timeIndex"
          >
            <timepicker :startHour="8" :endHour="22" v-model="tour.times.$each.$iter[timeIndex].$model" @input="commit" /> 
          </control-item>
          <control-item :noBack="true">
            <button @click="addTime(tour)" :class="[s.addButton]">{{$t('createTour.addTime')}}</button> 
          </control-item>    
          <control-item v-if="tour && tour.times && tour.times.$model && tour.times.$model.length > 1" :noBack="true">
            <button @click="removeTime(tour)" :class="[s.addButton, s.remove, s.removeTimeButton]">{{$t('createTour.removeTime')}}</button>               
          </control-item>                    
        </div>
        <div v-else :class="s.innerRow">
            <control-item :class="s.controlItem" v-if="!isRegular" :title="$t('createTour.serviceStart')" :isRequired="tour.serviceStart.required !== undefined" :error="getError(tour.serviceStart)">
                <timepicker v-model="tour.serviceStart.$model" :startHour="8" :endHour="22" @input="commit" /> 
            </control-item>      
            <control-item :class="s.controlItem" v-if="!isRegular" :title="$t('createTour.serviceEnd')" :isRequired="tour.serviceEnd.required !== undefined" :error="getError(tour.serviceEnd)">
                <timepicker v-model="tour.serviceEnd.$model" :startHour="8" :endHour="22" @input="commit" /> 
            </control-item>
        </div>       
      </div>   

      <div :class="s.itemsRow" v-if="isRegular">
        <!-- <control-item :title="$t('createTour.price')" :mandatory="true">
          <numeric v-model="tour.price" @input="commit" /> 
        </control-item> -->
        <control-item :class="s.controlItem" :title="$t('createTour.seatPrice')" :isRequired="tour.seatPrice.required !== undefined" :error="getError(tour.seatPrice)">
          <text-input :class="s.numberInput" :mask="currencyVueMask" v-model="tour.seatPrice.$model" @input="commit" /> 
        </control-item>  
        <control-item :class="s.controlItem" :title="$t('createTour.vipPrice')" :isRequired="tour.vipPrice.required !== undefined" :error="getError(tour.vipPrice)">
          <text-input :class="s.numberInput" v-mask="currencyVueMask" v-model="tour.vipPrice.$model" @input="commit" /> 
        </control-item>  
        <control-item :class="s.controlItem" :title="$t('createTour.discount')" :isRequired="tour.discount.required !== undefined" :error="getError(tour.discount)">
          <text-input :class="s.numberInput" v-mask="percentVueMask" v-model="tour.discount.$model" @input="commit" /> 
        </control-item>                            
      </div>

      <div :class="[s.itemsRow,s.menuToggler]" v-if="isRegular">
        <control-item :title="$t('createTour.hasMenu')" :noBack="true">
          <checkbox v-model="tour.$model.hasMenu" @input="commit" /> 
        </control-item>        
        <control-item :title="$t('createTour.hasBeverages')" :noBack="true">
          <checkbox v-model="tour.$model.hasBeverages" @input="commit" /> 
        </control-item>
      </div>        

      <div :class="[s.itemsRow, s.menuBeverage]" v-if="isRegular">
          <div v-if="tour.$model.hasBeverages">
            <h2>{{$t('common.beverages')}}</h2>
            <div :class="s.multicheck">
                <div v-for="item in 2" :key="'column1_' + item" :class="[s.multicheckItem, s.columnTitles]">
                    <div :title="$t('createTour.menuInTicket')">{{$t('createTour.menuInTicketColumn')}}</div>
                    <div :title="$t('createTour.menuExtra')">{{$t('createTour.menuExtraColumn')}}</div> 
                </div>              
                <div v-for="item in beverageItems" :key="'beverage_' + item.value" :class="s.multicheckItem">
                    <div>{{item.text}}</div>
                    <checkbox :title="$t('createTour.menuInTicket')" :class="s.multicheckBox" :value="tour.beverages.$model.some(x => x == item.value)" :label="''" @input="onItemCheck(tour.beverages.$model, item.value, $event)" />
                    <checkbox :title="$t('createTour.menuExtra')" :class="s.multicheckBox" :value="tour.beveragesExtra.$model.some(x => x == item.value)" :label="''" @input="onItemCheck(tour.beveragesExtra.$model, item.value, $event)" />  
                </div>
            </div>
          </div>      
          <div v-if="tour.$model.hasMenu"> 
            <h2>{{$t('common.menu')}}</h2>
            <div :class="s.multicheck">
                <div v-for="item in 2" :key="'column2_' + item" :class="[s.multicheckItem, s.columnTitles]">
                    <div :title="$t('createTour.menuInTicket')">{{$t('createTour.menuInTicketColumn')}}</div>
                    <div :title="$t('createTour.menuExtra')">{{$t('createTour.menuExtraColumn')}}</div> 
                </div>                 
                <div v-for="item in menuItems" :key="'menu_' + item.value" :class="s.multicheckItem">
                    <div>{{item.text}}</div>
                    <checkbox :title="$t('createTour.menuInTicket')" :class="s.multicheckBox" :value="tour.menus.$model.some(x => x == item.value)" :label="''" @input="onItemCheck(tour.menus.$model, item.value, $event)" :disabled="isExtraMenu(item.value)" />
                    <checkbox :title="$t('createTour.menuExtra')" :class="s.multicheckBox" :value="tour.menusExtra.$model.some(x => x == item.value)" :label="''" @input="onItemCheck(tour.menusExtra.$model, item.value, $event)" :disabled="!isExtraMenu(item.value)" />  
                </div>
            </div>            
          </div>
      </div>      

    </div>

    <button @click="addTour" :class="[s.addButton,s.addTourButton]">{{$t('createTour.addSchedule')}}</button>

    <button @click="removeTour" v-if="$v.tours.$model.length > 1" :class="[s.addButton, s.remove, s.removeTourButton]">{{$t('createTour.removeSchedule')}}</button>  

    <div :class="s.createMessages" v-if="createError || createSuccess">
        <div v-if="createError" :class="s.createError">{{createError}}</div>
        <div v-if="createSuccess" :class="s.createSuccess">{{createSuccess}}</div>
    </div>   

    <bbButton :theme="ButtonTheme.Black" @click="create" :text="$t('createTour.create')" :class="s.createButton"/>    

  </BusLayout>
</template>

<script lang="ts">
import Vue from "vue";
import config from "@/config";
import { Roles, AuthorityLevel } from "@/types/private";
import style from "./create-tour.module.scss";
import ControlItem from "@/components/controlItem/controlItem.vue";
import calendar from "@/components/controls/calendar/calendarDate.vue";
import checkbox from "@/components/controls/checkbox/checkbox.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue";
import timepicker from "@/components/controls/timepicker/timepicker.vue";
import numeric from "@/components/controls/numeric/numeric.vue";
import multiselectCheckbox from "@/components/controls/multiselect-checkbox/multiselect-checkbox.vue"
import textInput from "@/components/controls/text-input/text-input.vue";
import bbButton, { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import { mapProps } from "@/store/helpers";
import { mapActions, mapState } from "vuex";
import { SelectItem, WeekDays, Time } from "@/types/common";
import { MenuInfo, Menu, Beverage, MenuTypeEnum, Route } from "@/types/booking";
import { minLength, helpers } from 'vuelidate/lib/validators'
import moment from "moment"
import { TourType } from "~/types/tour";
import { currencyVueMask, percentVueMask } from '@/utils/mask';

interface Period {
    weekday: number;
    time: Time;
    tourIndex: number;
    timeIndex: number;
}

const required = (value: any): boolean => value === 0 || !!value
const notZero = (value: any): boolean => !!value && parseInt(value) !== 0

export default Vue.extend({
  middleware: "authy",
  meta: {
    auth: { authority: AuthorityLevel.Administrator },
  },
  components: {
    ControlItem,
    calendar,
    checkbox,
    dropdown,
    timepicker,
    numeric,
    multiselectCheckbox,
    bbButton,
    textInput
  },
  data() {
    return {
      selectionDate: ['2021-11-19'],
      s: style,
      showErrors: false,
      ButtonTheme: ButtonTheme,
      createError: '',
      createSuccess: '',
      currencyVueMask,
      percentVueMask
    };
  },  
  validations() {
    return {
        dateStart: {
            required,
            datesRange: (val: any, vm: any) => { return val <= vm.dateEnd}
        },
        dateEnd: {
            required,
            datesRange: (val: any, vm: any) => val >= vm.dateStart
        },       
        type: { required },    
        routeId: { required },
        chooseWeekdays: {},       
        tours: {
            required,
            minLength: minLength(1),
            $each: {
                weekdayStart: {
                    required: (value: any, tour: any): boolean => !this.chooseWeekdays || required(value),
                    weekdaysRange: (val: any, tour: any) => !tour.chooseWeekdays || val <= tour.weekdayEnd,
                    weekdaysInPeriod: (val: any) => true 
                },
                weekdayEnd: {
                    required: (value: any, tour: any): boolean => !this.chooseWeekdays || required(value),
                    weekdaysRange: (val: any, tour: any) => !tour.chooseWeekdays || val >= tour.weekdayStart,
                    weekdaysInPeriod: (val: any) => true                
                },  
                times: {
                    $each: {
                        required: (value: any): boolean => this.type != TourType.Regular || required(value),
                        crossPeriod: (val: any) => true 
                    }
                },                         
                seatPrice: {
                    required: (value: any): boolean => this.type != TourType.Regular || notZero(value),
                },
                vipPrice: {
                    required: (value: any): boolean => this.type != TourType.Regular || notZero(value),
                },
                discount: {},
                serviceStart: {
                    required: (value: any): boolean => this.type != TourType.Service || required(value),
                    datesRange: (val: any, vm: any): boolean => this.type != TourType.Service || Time.diff(val, vm.serviceEnd) > 0  
                },
                serviceEnd: {
                    required: (value: any): boolean => this.type != TourType.Service || required(value),
                    datesRange: (val: any, vm: any): boolean => this.type != TourType.Service || Time.diff(val, vm.serviceStart) < 0  
                },
                hasMenu: {},
                hasBeverages: {},
                menus: {},
                menusExtra: {},
                beverages: {},
                beveragesExtra: {}
            }
        }
    }
  },  
  computed: {
    lang(): string {
      return this.$i18n.locale;
    },    
    toursCreation(): any {},
     ...mapProps(
      ['dateStart', 'dateEnd', 'routeId', 'unavailableDates', 'chooseWeekdays', 'type', 'tours'],
      'tour', 
      'toursCreation',
      'setToursCreation'
    ),
    ...mapState('tour', ['routes']),
    userRole(): Roles {
			return this.$auth.user?.role;
		},
    tourTypes(): SelectItem[] {
      switch (this.$auth.user.role) {
        case Roles.Supervisor:
            return [
              new SelectItem(TourType.Regular , this.$t('createTour.Regular') as string),
              new SelectItem(TourType.Service, this.$t('createTour.Service') as string)
            ];
        case Roles.Administrator:
          this.$v.type.$model = TourType.Service;
          return [
              new SelectItem(TourType.Service, this.$t('createTour.Service') as string)
            ];
        default:
            return [];
      }
    },
    weekdays(): SelectItem[] {
      let selectItems = SelectItem.fromEnum(WeekDays, this.$t('weekDaysNames'));
      selectItems.forEach(x => {
        x.text = x.text
      });
      return selectItems;
    },
    menuInfo(): MenuInfo {
      return this.$store.state['booking'].menuInfo as MenuInfo;
    },
    menuItems(): SelectItem[] {
      return this.menuInfo.menus.map((x: Menu) => new SelectItem(x.id, x.name[this.lang]));
    },
    beverageItems(): SelectItem[] {
      return this.menuInfo.beverages.map((x: Beverage) => new SelectItem(x.id, x.name[this.lang]));
    },
    routeItems(): SelectItem[] {
      return (this.routes ?? []).map((x: Route) => new SelectItem(x.id, x.name[this.lang]));
    },    
    isRegular(): boolean {
      return this.type == TourType.Regular;
    },
    crossedPeriods() {
        const periods: Period[] = [];
        const crossedPeriods: Period[] = [];
        this.toursCreation.tours.forEach((tour: any, tourIndex: number) => {
            for(let weekday = tour.weekdayStart; weekday <= tour.weekdayEnd; weekday++) {
                tour.times.forEach((time: any, timeIndex: number) => {
                    const timeObj = Time.fromPlain(time);
                    periods.push({
                        weekday,
                        time: timeObj,
                        tourIndex,
                        timeIndex
                    })                    
                    const crossed = periods.filter(x => x.weekday == weekday && x.time && timeObj && x.time.equals(timeObj))
                    if (crossed.length > 1 && !crossedPeriods.some(x => x.tourIndex == tourIndex && x.timeIndex == timeIndex)) {
                        crossed.forEach(x => crossedPeriods.push(x));
                    }
                });
            }
        });   
        return crossedPeriods;     
    },
    toursWithWeekdaysOuOfPeriod(): number[] {
        const res = [] as number[];
        const weekdays = [] as number[];

        if (this.toursCreation.dateStart && this.toursCreation.dateEnd) {
            let dateStart = moment(this.toursCreation.dateStart).startOf('day')
            const dateEnd = moment(this.toursCreation.dateEnd).startOf('day')
            while(dateStart <= dateEnd && !weekdays.includes(dateStart.day())) {
                weekdays.push(dateStart.day());
                dateStart = dateStart.add(1, 'days');
            }
        }

        this.toursCreation.tours.forEach((tour: any, tourIndex: number) => {
            if (tour.weekdayStart && tour.weekdayEnd && !weekdays.some(x => x >= tour.weekdayStart && x <= tour.weekdayEnd)) {
                res.push(tourIndex);
            }
        });          

        return res;
    }
  },
  methods: {
    ...mapActions('booking', ['getMenuInfo']),
    getError(field: any, tourIndex: any, timeIndex: any) {
        if (!this.showErrors) {
            return '';
        } else if (field.required === false) {
            return (this as any).$t('validation.isRequired');
        } else if (field.weekdaysRange === false) {
            return (this as any).$t('createTour.wrongWeekdaysRange');           
        } else if (field.datesRange === false) {
            return (this as any).$t('createTour.wrongDatesRange');              
        } else if (field.crossPeriod !== undefined && this.crossedPeriods.some(x => x.tourIndex == tourIndex && x.timeIndex == timeIndex)) {
            return (this as any).$t('createTour.crossPeriod');       
        } else if (field.weekdaysInPeriod !== undefined && this.toursWithWeekdaysOuOfPeriod.includes(parseInt(tourIndex))) {
            return (this as any).$t('createTour.weekdaysOuOfPeriod');                    
        } else {
            return '';
        }
    },
    numberValidator(event: any) {
      let regex = /[0-9]/;
      if(event.type === 'keypress' && !regex.test(event.key)) {
        event.preventDefault();	
      }

      if(event.type === 'paste') {
        let array: string[] = event.clipboardData.getData('text').split('');
        array.forEach(item => {
          if(!regex.test(item)) {
            event.preventDefault();
            return;
          }
        })
      }
      if (Number(event.target.value) > 10000) {
        event.target.value = 10000;
        event.preventDefault();
      }
    },
    commit() {
      this.$store.commit('tour/setToursCreation', this.toursCreation);     
    },
    async create() {
        this.showErrors = true;
        this.createSuccess = '';

        this.$v.$touch();

        const isInvalid = this.$v.$invalid || (this.type == TourType.Regular && this.crossedPeriods.length > 0)

        if (!isInvalid) {
            this.createError = '';
            try {
                const response = await this.$store.dispatch('tour/createTours');
                this.createSuccess = this.$t('createTour.success').toString() + ': ' + response.map((x: any) => x.id).join(', ')
            }
            catch (error) {
                if ((error as any).data?.hasDuplicateTours) {
                    this.createError += this.$t('createTour.duplicateTours').toString() + 
                    ':\n' + (error as any).data.duplicateTours.map((x: any) => `${moment(x.departure).format('DD.MM.YYYY HH:mm')}-${moment(x.arrival).format('HH:mm')}`).join('\n');
                }
                if ((error as any).data?.hasBlockingTours) {
                    this.createError += this.$t('createTour.blockingTours').toString() + 
                    ':\n' + (error as any).data.blockingTours.map((x: any) => `${moment(x.departure).format('DD.MM.YYYY HH:mm')}-${moment(x.arrival).format('HH:mm')}`).join('\n');
                }                
            }
        }
    },
    addTour() {
      this.$store.commit('tour/addTourCreation', {
        menus: this.menuInfo.menus.map(x => x.id).filter(x => !this.isExtraMenu(x)),
        menusExtra: this.menuInfo.menus.map(x => x.id).filter(x => this.isExtraMenu(x)),
        beverages: this.menuInfo.beverages.map(x => x.id),
        beveragesExtra: this.menuInfo.beverages.map(x => x.id),
        hasMenu: true,
        hasBeverages: true,
        times: [null]
      });
    },    
    removeTour() {
      this.toursCreation.tours.pop();
      this.commit();
    },
    addTime(period: any) {
      this.toursCreation.tours.find((x: any) => x == period.$model).times.push(null);
      this.commit();
    },
    removeTime(period: any) {
      this.toursCreation.tours.find((x: any) => x == period.$model).times.pop();
      this.commit();
    },    
    onItemCheck(items: any[], itemId: number, checked: boolean) {
      if (checked) {
          items.push(itemId);
      } else {
          var index = items.map(x => x.toString()).indexOf(itemId.toString());
          if (index > -1) {
              items.splice(index, 1);
          }            
      }
      this.commit();
    },
    isExtraMenu(menuId: number): boolean {
        let menuTypeId = this.menuInfo?.menus.find(x => x.id == menuId)?.menuType?.id
        return !!menuTypeId && menuTypeId != MenuTypeEnum.Main
    }
  },
  async created() {
    this.dateStart = new Date();
    this.dateEnd = new Date();
    this.chooseWeekdays = true;
    this.type = (TourType.Regular).toString();
    if (!this.tours?.length) {
        this.addTour();
    }
    if (this.routes?.length) {
      this.routeId = this.routes[0].id;
    }    
  }
});
</script>