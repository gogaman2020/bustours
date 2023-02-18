<template>
    <div :class="s.page">   

        <h1>{{$t('currentBookings.title')}}</h1>

        <div :class="s.filter">

            <div :class="s.row">

                <control-item :title="$t('currentBookings.city')">
                    <dropdown v-model="cityId" :items="cityList" /> 
                </control-item> 

                <control-item  :title="$t('currentBookings.dateFrom')">
                    <calendar :lang="this.lang" v-model="dateFrom" :availableDates="allTourDates" :startAvailableDate="new Date()" :endAvailableDate="endAvailableDate"/> 
                </control-item>   

                <control-item  :title="$t('currentBookings.dateTo')">
                    <calendar :lang="this.lang" v-model="dateTo" :availableDates="allTourDates" :startAvailableDate="dateFrom" :endAvailableDate="endAvailableDate"/> 
                </control-item>                               

            </div> 

            <div :class="s.row">

                <control-item :class="s.multiselect" :title="$t('currentBookings.tourTypes')" :noBack="true">
                    <multiselectCheckbox :items="tourTypesList" :value="tourTypes" />
                    <checkbox v-model="conflict" :label="$t('currentBookings.conflict')" :text-backward="true" :class="s.box"/>
                    <checkbox v-model="group" :label="$t('currentBookings.group')" :text-backward="true"/>
                </control-item>  
                 

            </div>
        
            <bbButton @click="apply" :text="$t('currentBookings.filter')" :class="s.applyButton" :theme="buttonTheme.White" :disabled="applying" />         
        </div>

        <div :class="s.tours">
            <h2>{{$t('currentBookings.currentBookings')}}</h2>
            <BbGrid :data="tours" :class="s.grid" :grouping="{key: x => formatDate(x.departure) }" bodyMaxHeight="50vh">
                <!-- <BbGridColumn title="" field="check" width="60px" :variants="['white']"/> -->
                <BbGridColumn :title="$t('currentBookings.dateTime')" field="departure" :cellClass="s.firstColumn"/>
                <BbGridColumn :title="$t('currentBookings.number')" field="number" />                  
                <BbGridColumn :title="$t('currentBookings.type')" field="tourType" />  
				<BbGridColumn :title="$t('currentBookings.status')" field="tourState" />    
                <BbGridColumn :title="$t('currentBookings.itinerary')" field="itinerary" />      
                <BbGridColumn :title="$t('currentBookings.duration')" field="duration" /> 
                <BbGridColumn :title="$t('currentBookings.conflict')" field="conflicts" />  
                <BbGridColumn :title="$t('currentBookings.guest')" field="guests" />   
                <BbGridColumn :title="$t('currentBookings.extras')" field="extras" /> 
                <BbGridColumn :title="$t('currentBookings.paymentInformation')" field="paymentInformation" /> 
                <BbGridColumn field="Buttons" :cellClass="s.bbGridColumnCustom" />    
                <template v-slot:number="{row}">
                    {{row.number ? row.number :  ''}}
                </template>                                                     
                <template v-slot:departure="{row}">
                    <dateTime :value="row.departure" :type="displayType.Time"/>
                </template>         
                <template v-slot:itinerary="{row}">
                    {{row.itinerary ? row.itinerary[lang] : row.privateHireComment}}
                </template>    
                <template v-slot:duration="{row}">
                    <duration :value="row.duration" />
                </template>  
                <template v-slot:tourType="{row}">
                    {{$t(`enums.TourType.${TourType[row.tourType]}`)}}
                </template>   
                <template v-slot:tourState="{row}">
                    <div :class="[s.blockCellTemplate, s[`blockCellTemplate${getTourStateBackgroundColor(row.tourState)}`]]">
                        {{ row.tourState ? $t(`enums.TourState.${TourState[row.tourState]}`) : "" }}
                    </div>
                </template>   
                <template v-slot:extras="{row}">
                    {{ row.extras? $t('yes'):$t('no')}}
                </template> 
                <template v-slot:conflicts="{row}">
                    <div :class="[s.blockCellTemplate, s[`blockCellTemplate${getConflictBackgroundColor(row.conflicts)}`]]">
                        {{ row.conflicts? $t('yes'):$t('no')}}
                    </div>
                </template> 
                <template v-slot:guests="{row}">
                    {{row.guestsNumber}}{{$t('currentBookings.of')}}{{row.seatsNumber}}
                </template>   
                <template v-slot:paymentInformation="{row}">
                    {{paidInfo(row)}}<br/>{{waitingInfo(row)}}
                </template>                         

                <template #Buttons="{row}">
                    <div :class="s.gridRowButtonsWrapper">
                        <NuxtLink :to="localePath(`/tour-information/${row.id}`)"><BbButton>{{$t(`currentBookings.select`)}}</BbButton></NuxtLink>
                    </div>
			    </template>
            </BbGrid>
        </div>
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState } from "vuex"
import { AuthorityLevel, Roles } from "@/types/private"
import style from "./style.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime, { DisplayType } from "@/components/display/dateTime.vue"
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import multiselectCheckbox from "@/components/controls/multiselect-checkbox/multiselect-checkbox.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import { SelectItem } from "@/types/common"
import { TourFilter, City, CurrentBooking, TourType, TourState } from "@/types/tour"
import { Tour } from "@/types/booking"
import BbGrid from "@/components/bb-grid/bb-grid.vue"
import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue"
import moment from 'moment'
import calendar from "@/components/controls/calendar/calendarDate.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"
import duration from "@/components/display/duration.vue"

enum TourStateBackgroundColors {
	Green = "Green",
	Red = "Red",
    Yellow = "Yellow",
    Gray = "Gray",
    White = "White"
}

export default Vue.extend({
    middleware: "authy",
    name: "CurrentBookings",
    props: {
        
    },    
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        multiselectCheckbox,
        bbButton,
        BbGrid,
        BbGridColumn,
        currency,
        dateTime,
        dropdown,
        calendar,
        checkbox,
        duration
    },
    data() {
        return {
            s: style,
            applying: false,       
            checkedIds: [],     
            cityId: null as number | null,
            tourTypes: [] as number[],
            conflict: false as Boolean,
            conflictFlag: false as Boolean,
            group: false as Boolean,
            groupFlag: false as Boolean,
            dateFrom: new Date(),
            dateTo: new Date(),
            TourType: TourType,
            TourState: TourState
        };
    },    
    computed: {
        ...mapState('common', ['domainEnums']),
        ...mapState('tour', ['cities']),
        lang(): string {
            return this.$i18n.locale
        },
        tours: {
			get(): CurrentBooking[] {
                var tours = this.$store.state.tour.currentBookings
                if(this.conflictFlag)
                     tours = this.$store.state.tour.currentBookings?.filter((item:CurrentBooking)=> item.conflicts==true);
                if(this.groupFlag)
                     tours = this.$store.state.tour.currentBookings?.filter((item:CurrentBooking)=> item.hasGroupOrder==true);
				return tours;
			},
		},           
        buttonTheme() {
            return ButtonTheme;
        },
        displayType() {
            return DisplayType;
        },
        tourTypesList(): SelectItem[] {
            return this.domainEnums?.TourType
            .map((x: SelectItem) => new SelectItem(x.value, this.$t(`enums.TourType.${x.text}`).toString())) ?? [];
        },
        tourStatesList(): SelectItem[] {
            return this.domainEnums?.TourState
            .map((x: SelectItem) => new SelectItem(x.value, this.$t(`enums.TourState.${x.text}`).toString())) ?? [];
        },
        cityList(): SelectItem[] {
            return this.cities?.map((x: City) => new SelectItem(x.id, x.name[this.lang])) ?? [];
        },
		currentUserRole(): Roles {
			return this?.$auth?.user?.role;
		},        
        endAvailableDate(): Date | null {
            if (this.currentUserRole == Roles.Crew) {
                var date = new Date()
                date.setDate(this.dateFrom.getDate() + 2);
                return date;
            } else {
                return null;
            }
        },
		allTours(): Tour[] {
            return this.$store.state.booking.routeInfo.tours;
        },  
		allTourDates(): Date[] {
        	return this.allTours.map((x: Tour) => new Date(x.departure));
        },              
    },
    watch: {
        cityList() {
            this.setCityId()
        },
        dateFrom(from: Date) {
          if(from > this.dateTo) {
            this.dateTo = from;
          }
        },
    },
    methods: {
        ...mapActions('tour', ['getCities','currentBookings','deleteTours']), 
        async apply() {
            this.applying = true
            const filter: TourFilter = {
                cityId: this.cityId ?? undefined,
                tourTypes: this.tourTypes.length>0?this.tourTypes: undefined,
                departureDateFrom: this.dateFrom,
                departureDateTo: this.dateTo,
                hasOrders: false,
                withoutOrders: true,
                ids: undefined
            }
            await this.currentBookings(filter);
            this.conflictFlag=this.conflict;
            this.groupFlag=this.group;
            this.applying = false
        },
        formatDate (value: string, type: DisplayType = DisplayType.Date): string {
            if (value) {
                const configByType = (config.formats as any)[DisplayType[type].toLowerCase()]; 
                return moment(new Date(value)).locale(this.lang).format(configByType['short']);
            } else {
                return '';
            }
        },
        setCityId() {
            if (this.cityList?.length) {
                this.cityId = parseInt(this.cityList[0].value.toString());
            }
        },
        paidInfo(tour: CurrentBooking): string {
            return tour.tourType==TourType.PrivateHire?tour.paid?this.$t('currentBookings.paid').toString():""
                                                        :this.$t('currentBookings.paid')+tour.paid.toString();
        },
        waitingInfo(tour: CurrentBooking): string {
            return tour.tourType==TourType.PrivateHire?tour.waiting?this.$t('currentBookings.isWaiting').toString():""
                                                        :tour.waiting.toString()+this.$t('currentBookings.isWaiting');
        },
        getTourStateBackgroundColor(tourState?: TourState): TourStateBackgroundColors {
			switch (tourState) {
				case TourState.Draft: return TourStateBackgroundColors.Gray;
				case TourState.Active: return TourStateBackgroundColors.Green;
                case TourState.CancelRequest: return TourStateBackgroundColors.Yellow;
				case TourState.Canceled: return TourStateBackgroundColors.Yellow;
				case TourState.Deleted: return TourStateBackgroundColors.Yellow;			
				default: return TourStateBackgroundColors.White;
			}
		},
        getConflictBackgroundColor(isConflict: Boolean): TourStateBackgroundColors {
			switch (isConflict) {
				case true: return TourStateBackgroundColors.Red;
				case false: return TourStateBackgroundColors.White;			
				default: return TourStateBackgroundColors.White;
			}
		}
    },
    async created() {
        if (this.allTourDates?.length) {
            let minDate = this.allTourDates[0];
            this.allTourDates.forEach(x => {
                if (x < minDate) {
                    minDate = x;
                }
            })
            this.dateFrom = minDate;
        }
        this.tourTypesList.forEach(x => (this.tourTypes as any).push(x.value))
        this.setCityId()
        await this.apply()
    }        
})
</script>