<template>
    <div :class="s.root">   

        <h1>{{$t('giftCertificate.management.title')}}</h1>

        <div :class="s.filter">

            <div :class="s.filterRow">
                <control-item :class="s.filterCity" :title="$t('giftCertificate.management.filterCity')">
                    <dropdown v-model="cityId" :items="cityList" /> 
                </control-item>    
            </div>           
            
            <div :class="s.filterRow">
                <control-item :title="$t('giftCertificate.management.statusFilter')" :noBack="true">
                    <multiselectCheckbox :items="statusList" :value="statuses" />
                </control-item>   
            </div>

            <bbButton @click="apply" :text="$t('giftCertificate.management.filterButton')" :class="s.applyButton" :theme="buttonTheme.White" :disabled="applying" />         
        </div>

        <div :class="s.certificates">
            <BbGrid :data="certificates" :class="s.grid" bodyMaxHeight="50vh">
                <BbGridColumn :title="$t('giftCertificate.grid.id')" field="id" />
                <BbGridColumn :title="$t('giftCertificate.grid.number')" field="number" />
                <BbGridColumn :title="$t('giftCertificate.grid.amount')" field="amount" />        
                <BbGridColumn :title="$t('giftCertificate.grid.purchaseDate')" field="purchaseDate" />                  
                <BbGridColumn :title="$t('giftCertificate.grid.dateStart')" field="dateStart" /> 
                <BbGridColumn :title="$t('giftCertificate.grid.dateEnd')" field="dateEnd" />
                <BbGridColumn :title="$t('giftCertificate.grid.status')" field="status" />   
                <BbGridColumn :title="$t('giftCertificate.grid.redeemedDate')" field="redeemedDate" />
                <BbGridColumn :title="$t('giftCertificate.grid.bookingNumber')" field="bookingNumber" />  
                <BbGridColumn :title="$t('giftCertificate.grid.balance')" field="balance" />    
                <template v-slot:id="{row}">
                    {{row.id.toString()}}
                </template>     
                <template v-slot:bookingNumber="{row}">
                    {{row.order ? row.order.id : ''}}
                </template>     
                <template v-slot:dateStart="{row}">
                    <dateTime :value="row.dateStart" />
                </template>                                                         
                <template v-slot:purchaseDate="{row}">
                    <dateTime :value="row.dateStart" />
                </template>    
                <template v-slot:dateEnd="{row}">
                    <dateTime :value="row.dateEnd" />
                </template>     
                <template v-slot:amount="{row}">
                    <currency :value="row.amount ? row.amount : row.amountVariant.amount" :adjustFont="false" />
                </template>    
                <template v-slot:redeemedDate="{row}">
                    <dateTime :value="row.redeemedDate" :type="displayType.DateTime" />
                </template>      
                <template v-slot:balance="{row}">
                    <currency :value="row.balance" :adjustFont="false" />
                </template>     
                <template v-slot:status="{row}">
                    {{domainStatuses.find(x => x.value == row.status).text}}
                </template>                                                                                                   
            </BbGrid>

            <json-excel :data="certificates" :fields="exportCertificatesFields" :class="s.exportButton" name="certificates.xls">
                <bbButton text="Export to excel"></bbButton>
            </json-excel>            
        </div>

        <div :class="s.total">
            <h2>{{$t('giftCertificate.management.total')}}</h2>
            <BbGrid :data="statusesTotals" :class="s.grid">
                <BbGridColumn :title="$t('giftCertificate.grid.status')" field="status" />    
                <BbGridColumn :title="$t('giftCertificate.grid.count')" field="count" />                                    
                <BbGridColumn :title="$t('giftCertificate.grid.amount')" field="amount" />        
                <BbGridColumn :title="$t('giftCertificate.grid.balance')" field="balance" />                 
                <template v-slot:amount="{row}">
                    <currency :value="row.amount ? row.amount : row.amountVariant.amount" :adjustFont="false" />
                </template>      
                <template v-slot:balance="{row}">
                    <currency :value="row.balance" :adjustFont="false" />
                </template>    
                <template v-slot:count="{row}">
                    {{row.count}}
                </template>                   
                <template v-slot:status="{row}">
                    {{domainStatuses.find(x => x.value == row.status).text}}
                </template>                                                                                                    
            </BbGrid>   

            <json-excel :data="statusesTotals" :fields="exportTotalsFields" :class="s.exportButton" name="totals.xls">
                <bbButton text="Export to excel"></bbButton>
            </json-excel>         

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
import { SelectItem } from "@/types/common"
import { CertificateFilter, GiftCertificate, GiftCertificateStatusTotals, GiftCertificateStatus } from "@/types/giftCertificate"
import BbGrid from "@/components/bb-grid/bb-grid.vue"
import BbGridColumn from "@/components/bb-grid/bb-grid-column/bb-grid-column.vue"
import JsonExcel from "vue-json-excel"
import moment from 'moment'
import { City } from "@/types/tour"

export default Vue.extend({
    middleware: "authy",
    name: "GiftCertificateManagement",
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
        JsonExcel
    },
    data() {
        return {
            s: style,
            statuses: [] as string[],
            prevAll: false,
            applying: false,
            certificates: [] as GiftCertificate[],
            statusesTotals: [] as GiftCertificateStatusTotals[],
            cityId: null as number | null
        };
    },     
    computed: {
        ...mapState('common', ['domainEnums']),
        ...mapState('tour', ['cities']),
        lang(): string {
            return this.$i18n.locale
        },           
        buttonTheme() {
            return ButtonTheme;
        },
        displayType() {
            return DisplayType;
        },
        domainStatuses(): SelectItem[] {
            return this.domainEnums?.GiftCertificateStatus
            //.filter((x: SelectItem) => x.value != 99)
            .map((x: SelectItem) => new SelectItem(x.value, this.$t(`enums.GiftCertificateStatus.${x.text}`).toString())) ?? [];
        },
        statusList(): any[] {
            return this.domainStatuses.length ? [new SelectItem('all', 'All')].concat(this.domainStatuses) : [];
        },
        cityList(): SelectItem[] {
            return this.cities?.map((x: City) => new SelectItem(x.id, x.name[this.lang])) ?? [];
        },        
        exportCertificatesFields(): any {
            return {
                [(this as any).$t('giftCertificate.grid.id')]: 'id', 
                [(this as any).$t('giftCertificate.grid.amount')]: {
                    field: "amount",
                    callback: (value: string) => this.formatAmount(value),
                },
                [(this as any).$t('giftCertificate.grid.purchaseDate')]: {
                    field: "dateStart",
                    callback: (value: string) => this.formatDate(value),
                },      
                [(this as any).$t('giftCertificate.grid.dateStart')]: {
                    field: "dateStart",
                    callback: (value: string) => this.formatDate(value),
                },  
                [(this as any).$t('giftCertificate.grid.dateEnd')]: {
                    field: "dateEnd",
                    callback: (value: string) => this.formatDate(value),
                },   
                [(this as any).$t('giftCertificate.grid.status')]: {
                    field: "status",
                    callback: (status: string) => this.domainStatuses.find(x => x.value == status)?.text,
                }, 
                [(this as any).$t('giftCertificate.grid.redeemedDate')]: {
                    field: "redeemedDate",
                    callback: (value: string) => this.formatDate(value, DisplayType.DateTime),
                },   
                [(this as any).$t('giftCertificate.grid.bookingNumber')]: {
                    field: "bookingNumber",
                    callback: (value: string) => '',
                },   
                [(this as any).$t('giftCertificate.grid.balance')]: {
                    field: "balance",
                    callback: (value: string) => this.formatAmount(value),
                },                                                                                                                           
            };
        },
        exportTotalsFields(): any {
            return {
                [(this as any).$t('giftCertificate.grid.status')]: {
                    field: "status",
                    callback: (status: string) => this.domainStatuses.find(x => x.value == status)?.text,
                }, 
                [(this as any).$t('giftCertificate.grid.count')]: 'count',                  
                [(this as any).$t('giftCertificate.grid.amount')]: {
                    field: "amount",
                    callback: (value: string) => this.formatAmount(value),
                },                
                [(this as any).$t('giftCertificate.grid.balance')]: {
                    field: "balance",
                    callback: (value: string) => this.formatAmount(value),
                },                                                                                                                                           
            };
        }             
    },
    watch: {
        cityList() {
            this.setCityId()
        },        
        statuses() {
            const add = (status: string) => {
                if (!this.statuses.some(x => x == status)) {
                    this.statuses.push(status);
                }
            };
            const remove = (status: string) => {
                if (this.statuses.some(x => x == status)) {
                    this.statuses = this.statuses.filter(x => x != status);
                }
            };  
            const isAll = this.statuses.some(x => x == 'all');

            if (this.prevAll != isAll && isAll) {
                this.domainStatuses.forEach((x: SelectItem) => add(x.value.toString()));
            }
            if (this.statuses.filter(x => x != 'all').length < this.domainStatuses.length) {
                remove('all');
            } 
            else {
                add('all');
            }

            this.prevAll = isAll;
        }
    },
    methods: {
        ...mapActions('giftCertificate', ['getGiftCertificates','getGiftCertificateStatusesTotals']), 
        ...mapActions('tour', ['getCities']),
        async apply() {
            this.applying = true
            const filter: CertificateFilter = {
                number: null,
                statuses: this.statuses.filter(x => x != 'all').map(x => GiftCertificateStatus[<keyof typeof GiftCertificateStatus> x])
            }
            this.certificates = await this.getGiftCertificates(filter)
            this.applying = false
        },
        async getStatusesTotals() {
            this.statusesTotals = await this.getGiftCertificateStatusesTotals();
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
            if (this.cityList.length) {
                this.cityId = parseInt(this.cityList[0].value.toString());
            }
        },          
    },

    async created() {
        this.setCityId()
        await Promise.all([
            this.getStatusesTotals(),
            this.apply(),
        ]);
        this.statusList.forEach(x => (this.statuses as any).push(x.value))
    }        
})
</script>