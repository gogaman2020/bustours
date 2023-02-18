<template>
    <div :class="s.page">
        <div :class="s.tour">
            <h1>
                {{$t('itineraries.title')}}
            </h1>
        </div>

        <div :class="s.town">
            {{$t('itineraries.subTitle')}}
        </div>

        <div>
			<v-tabs v-model="tab" :class="s.tabs" :hide-slider="true">
				<v-tabs-slider></v-tabs-slider>

				<v-tab :class="s.tab" :active-class="s.tabActive">{{ $t("itineraries.tab1") }}</v-tab>
				<v-tab :class="s.tab" :active-class="s.tabActive">{{ $t("itineraries.tab2") }}</v-tab>
			</v-tabs>

			<v-tabs-items v-model="tab" :class="s.tabsContent">
				<v-tab-item>
					<picture>
                        <source srcset="/images/route/1/wesminster_half.jpg" media="(max-width: 576px)">
                        <source srcset="/images/route/1/wesminster_orig.jpg">
                        <img srcset="/images/route/1/wesminsterrrr.jpg" :class="s.img">
                    </picture>

                    <div :class="s.text">
                        {{$t('itineraries.text')}}
                    </div>

                    <div :class="s.info">
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.duration')}}
                            </div>

                            <div :class="s.item1">
                                {{$t('itineraries.duration1')}}
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.languages')}}
                            </div>
                            <div :class="s.item1">
                                {{$t('itineraries.languages1')}}
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.departurePoint')}}
                            </div>
                            <div :class="s.item1">
                                {{$t('itineraries.departurePoint1')}}
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.departureTime')}}
                            </div>
                            <div :class="s.item1">
                                <div :class="s.timerow">
                                    <div :class="s.time">10:00</div>
                                    <div :class="s.time">13:00</div>
                                    <div :class="s.time">17:00</div>
                                    <div :class="s.time">20:00</div>
                                </div>
                                {{$t('itineraries.departureTime1')}}
                            </div>
                        </div>

                        <div :class="s.row">  
                            <div :class="s.item">{{$t('itineraries.location')}}</div><div :class="s.item1">{{$t('itineraries.location1')}}</div>
                        </div>
                    </div>
				</v-tab-item>

				<v-tab-item>
                    <picture>
                        <source srcset="/images/route/1/tower_half.jpg" media="(max-width: 576px)">
                        <source srcset="/images/route/1/tower_orig.jpg">
                        <img srcset="/images/route/1/towerrrr.jpg" :class="s.img">
                    </picture>

                    <div :class="s.text">
                        {{$t('itineraries.text')}}
                    </div>

                    <div :class="s.info">
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.duration')}}
                            </div>

                            <div :class="s.item1">
                                
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.languages')}}
                            </div>
                            <div :class="s.item1">
                                
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.departurePoint')}}
                            </div>
                            <div :class="s.item1">
                                
                            </div>
                        </div>
                        <div :class="s.row">
                            <div :class="s.item">
                                {{$t('itineraries.departureTime')}}
                            </div>
                            <div :class="s.item1">
                                <div :class="s.timerow">
                                    
                                </div>
                                
                            </div>
                        </div>

                        <div :class="s.row">  
                            <div :class="s.item">{{$t('itineraries.location')}}</div><div :class="s.item1"></div>
                        </div>
                    </div>
				</v-tab-item>
			</v-tabs-items>
		</div>

        <!-- <div :class="s.tab">
            <div :class="s.btnTab">
                {{$t('itineraries.tab1')}}
            </div>
            <div :class="s.btnTab">
                {{$t('itineraries.tab2')}}
            </div>
        </div> -->

        

        <div :class="s.button">
            <div :class="booking">
                <bbButton :text="$t('booking.booking')" :theme="ButtonTheme.Black" @click="booking"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">  

import Vue from "vue"
import config from "@/config"

import { AuthorityLevel } from "@/types/private"
import style from "./style.module.scss"
import custom from "./vuetify-custom.module.scss";
import bbButton from "@/components/controls/bb-button/bb-button.vue"
import { ButtonTheme } from "@/components/controls/bb-button/bb-button.vue"

export default Vue.extend({
    name: "iti",
    middleware: "authy",   
    meta: {
        auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
    },
    components: {
        bbButton
    },
    data() {
        return {
            s: Object.assign(style, custom),
            tab: null
        };
    },
    computed: {
        ButtonTheme() {
            return ButtonTheme;
        }
    },
    methods: {
        booking() {
            this.$router.push(this.localePath("/booking"));
        }        
    },      
})
</script>