<template>
  <div :class="s.bookingWidget">
    <div :class="s.container">
        <control-item :class="s.guests" :title="$t('booking.guests')" :text-left="true">
            <dropdown v-model="guestCount" :items="guestCounts" /> 
        </control-item>
        <control-item :class="s.seatings" :title="$t('booking.tableRequirements')" :text-left="true">
            <dropdown v-model="seatingType" :items="seatingTypes" /> 
        </control-item>  
        <control-item :title="''" :class="s.button" :noBack="true">
          <NuxtLink v-if="canProceed" :class="s.continueLink" :to="bookingTo">
            <BbButton :disabled="false" :theme="ButtonTheme.Black" :class="s.buttonBooking" @click="fillBookingInitialState">
              {{ $t("booking.continue") }}
            </BbButton>
          </NuxtLink>
          <BbButton  v-else :disabled="true" :theme="ButtonTheme.Black" :class="s.buttonBooking">
            {{ $t("booking.continue") }}
          </BbButton>
        </control-item>
    </div>
    <div v-if="!canProceed" :class="s.errorMessage">
      {{ $t("booking.widgetError.message") }}
      <nuxt-link :to="localePath('/contact-us')" :class="s.errorMessageLink">
        {{ $t("booking.widgetError.link") }}
      </nuxt-link>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue"
import style from "./style.module.scss"
import config from "@/config"
import { SeatingType } from "@/types/booking"

import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue"
import controlItem from "@/components/controlItem/controlItem.vue";
import dropdown from "@/components/controls/dropdown/dropdown.vue"
import { SelectItem } from "@/types/common"

export default Vue.extend({
  name: "booking-widget",
  components: {
    BbButton,
  },
  data() {
    return {
      s: style,
      config: config,
      seatingTypeEnum: SeatingType,
      ButtonTheme,
      dropdown,
      controlItem,
      widgetData: {
        guestCount: this.$store.state.booking.order.guestCount,
        seatingType: this.$store.state.booking.order.seatingType,
      },
    };
  },
  computed: {
    guestCounts() {
        let items: SelectItem[] = [];
        for(let i = 1; i < config.maxGuestCount; i++) {
            items.push(new SelectItem(i));
        }
        items.push(new SelectItem(config.moreThanMaxGuestCountValue, config.maxGuestCount + '+'));
        return items;
    },
    seatingTypes() {
        return [
            new SelectItem(SeatingType.SeparateTable, (this as any).$t("booking.separateTable")),
            new SelectItem(SeatingType.BySeats, (this as any).$t("booking.shareTable"))
        ];
    },
    bookingTo(): object {
      return { path: this.localePath('/booking'), query: { guestCount: this.guestCount, seatingType: this.seatingType } };
    },
    seatingType: {
      get(): string {
        return this.widgetData.seatingType;
      },
      set(value: SeatingType): void {
        this.widgetData.seatingType = value;
      },
    },
    guestCount: {
      get(): number {
        return this.widgetData.guestCount;
      },
      set(value: number): void {
        this.widgetData.guestCount = value;
      },
    },
    canProceed: {
      get(): boolean {
        return this.guestCount <= this.config.maxGuestCount;
      },
    },
  },
  methods: {
    fillBookingInitialState(): void {
      this.$store.commit("booking/setSeatingType", this.widgetData.seatingType);
      this.$store.commit("booking/setGuestCount", this.widgetData.guestCount);
    }
  }
});
</script>