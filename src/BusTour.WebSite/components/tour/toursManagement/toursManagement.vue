<template>
    <div :class="s.root">   

        <h1>{{$t('tours.management.title')}}</h1>

        <div :class="s.filter">

            <div :class="s.filterRow">

                <control-item :title="$t('tours.management.filterCity')">
                    <dropdown v-model="cityId" :items="cityList" /> 
                </control-item>    

                <control-item :title="$t('tours.management.route')" :class="s.double">
                    <dropdown v-model="routeId" :items="routeList" /> 
                </control-item>                 

            </div> 

            <div :class="s.filterRow">

                <control-item :title="$t('tours.management.filterDateFrom')">
                    <calendar :lang="this.lang" v-model="dateFrom" :availableDates="allTourDates" /> 
                </control-item>   

                <control-item :title="$t('tours.management.filterDateTo')">
                    <calendar :lang="this.lang" v-model="dateTo" :startAvailableDate="dateFrom" :availableDates="allTourDates" /> 
                </control-item> 

            </div>            

            <div :class="s.filterRow">

                <control-item :class="s.multiselect" :title="$t('tours.management.tourTypes')" :noBack="true">
                    <multiselectCheckbox :items="tourTypesList" :value="tourTypes" />
                </control-item>   

            </div>
        
            <bbButton @click="apply" :text="$t('tours.management.filterButton')" :class="s.applyButton" :theme="buttonTheme.White" :disabled="applying" />         
        </div>

        <div :class="s.tours">
            <h2>{{$t('tours.management.tours')}}</h2>
            <BbGrid :data="tours" :class="s.grid" :grouping="{key: x => formatDate(x.departure) }">
                <!-- <BbGridColumn :title="$t('tours.grid.id')" field="id" :template="{template: x => x.toString()}" width="60px" /> -->
                <BbGridColumn title="" field="check" width="60px" :variants="['white']"/>
                <BbGridColumn :title="$t('tours.grid.datetime')" field="departure" />
                <BbGridColumn :title="$t('tours.grid.type')" field="type" />      
                <BbGridColumn :title="$t('tours.grid.itinerary')" field="itinerary" />      
                <BbGridColumn :title="$t('tours.grid.duration')" field="duration" />                                           
                <template v-slot:departure="{row}">
                    <dateTime :value="row.departure" :type="displayType.Time" />
                </template>         
                <template v-slot:itinerary="{row}">
                    {{row.route.name[lang]}}
                </template>    
                <template v-slot:duration="{row}">
                    <duration :value="row.type==30? row.duration :row.route.duration" />
                </template>  
                <template v-slot:type="{row}">
                    {{$t(`enums.TourType.${TourType[row.type]}`)}}
                </template>                                
                <template v-slot:check="{row}">
                    <checkbox
                        v-if="row.ordersCount === 0"
                        :value="checkedIds.some(x => x == row.id)" 
                        @input="onTourChecked(checkedIds, row.id, $event)" 
                        variant="center"
                    />                    
                </template>                                         
            </BbGrid>

             <bbButton @click="deleteIds" :text="$t('tours.management.deleteButton')" :class="s.deleteButton" :theme="buttonTheme.White" :disabled="!checkedIds.length || applying" />             
        </div>
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"
import { mapActions, mapState } from "vuex"
import { AuthorityLevel } from "@/types/private"
import style from "./style.module.scss"
import currency from "@/components/display/currency.vue"
import dateTime, { DisplayType } from "@/components/display/dateTime.vue"
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import multiselectCheckbox from "@/components/controls/multiselect-checkbox/multiselect-checkbox.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import { SelectItem, Dictionary } from "@/types/common"
import { Tour, TourFilter, City, TourType } from "@/types/tour"
import { Route, Tour as BookingTour } from "@/types/booking"
import BbGrid from "@/components/bb-grid/bb-grid.vue"
import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue"
import JsonExcel from "vue-json-excel"
import moment from 'moment'
import calendar from "@/components/controls/calendar/calendarDate.vue"
import checkbox from "@/components/controls/checkbox/checkbox.vue"
import duration from "@/components/display/duration.vue"

export default Vue.extend({
    middleware: "authy",
    name: "ToursManagement",
    props: {
        id: {
            type: [Number, String],
            default: null
        }
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
        JsonExcel,
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
            routeId: null as number | null,
            tourTypes: [] as number[],
            tours: [] as Tour[],
            dateFrom: new Date(),
            dateTo: new Date(),
            TourType
        };
    },     
    computed: {
        ...mapState('common', ['domainEnums']),
        ...mapState('tour', ['cities', 'routes']),
        lang(): string {
            return this.$i18n.locale
        },           
        buttonTheme() {
            return ButtonTheme;
        },
        displayType() {
            return DisplayType;
        },
        tourTypesList(): SelectItem[] {
            return this.domainEnums?.TourType.filter((x:any)=>x.value!=TourType.PrivateHire)
            .map((x: SelectItem) => new SelectItem(x.value, this.$t(`enums.TourType.${x.text}`).toString())) ?? [];
        },
        cityList(): SelectItem[] {
            return this.cities?.map((x: City) => new SelectItem(x.id, x.name[this.lang])) ?? [];
        },
        routeList() : SelectItem[] {
            return (<Route[]>this.routes)?.filter(x => x.cityId == this.cityId).map(x => new SelectItem(x.id, x.name[this.lang])) ?? [];
        },
		allTours(): BookingTour[] {
            return this.$store.state.booking.routeInfo.tours;
        },  
		allTourDates(): Date[] {
        	return this.allTours.map(x => new Date(x.departure));
        },        
    },
    watch: {
        cityList() {
            this.setCityId()
        },
        routeList() {
            this.setRouteId()
        },        
        dateFrom(from) {
            if(from > this.dateTo) {
                this.dateTo = from;
            }
        }
    },
    methods: {
        ...mapActions('tour', ['getCities','filter','deleteTours']), 
        async deleteIds() {
            this.applying = true
            await this.deleteTours(this.checkedIds)
            this.checkedIds = [];
            this.applying = false
            this.apply()
        },
        async apply() {
            this.applying = true
            const filter: TourFilter = {
                cityId: this.cityId ?? undefined,
                routeId: this.routeId ?? undefined,
                tourTypes: this.tourTypes.length>0?this.tourTypes: undefined,
                departureDateFrom: this.dateFrom,
                departureDateTo: this.dateTo,
                hasOrders: undefined,
                withoutOrders: undefined,
                ids: undefined
            }
            this.tours = await this.filter(filter)
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
        formatAmount (value: string):string {
            return value ? `£ ${value}` : '';
        },
        setCityId() {
            if (this.cityList?.length) {
                this.cityId = parseInt(this.cityList[0].value.toString());
            }
        },
        setRouteId() {
            if (this.routeList?.length) {
                this.routeId = parseInt(this.routeList[0].value.toString());
            }
        },        
        onTourChecked(items: any[], itemId: number, checked: boolean) {
            if (checked) {
                items.push(itemId);
            } else {
                const index = items.map(x => x.toString()).indexOf(itemId.toString());
                if (index > -1) {
                    items.splice(index, 1);
                }            
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
        await this.apply()
        this.tourTypesList.forEach(x => (this.tourTypes as any).push(x.value))
        this.setCityId()
        this.setRouteId()

    }        
})
</script>