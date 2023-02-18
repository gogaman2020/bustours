<template>
    <div :class="s.main">
      <video :class="s.video" loop autoplay muted>
        <source src="/videos/main-bg-video.mp4" type="video/mp4" />
      </video>
      <div :class="s.subTitleContainer">
        <div :class="s.title">{{ $t("home.title") }}</div>
        <div :class="s.subTitle">{{ $t("home.subTitle") }}</div>
      </div>
      <div :class="s.linkToPhotos">
        <nuxt-link :to="localePath('on-board')">{{
          $t("home.morePhotos")
        }}</nuxt-link>
      </div>
      <div :class="s.bookingWidgetContainer">
        <booking-widget></booking-widget>
      </div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./index.module.scss";
import config from "@/config";
import { Route } from "~/types/booking";
import BookingWidget from "@/components/booking/booking-widget/booking-widget.vue";

import { AuthorityLevel } from "@/types/private";
import { mapMutations } from "vuex"

export default Vue.extend({
  //
  middleware: "authy",
  meta: {
    auth: { authority: AuthorityLevel.User, disabled: !config.showComingSoon },
  },
  //
  components: {
    BookingWidget,
  },
  data() {
    return {
      s: style,
    };
  },
  computed: {
    lang: {
      get(): string {
        return this.$i18n.locale;
      },
    },
    route: {
      get(): Route {
        return this.$store.state.booking.routeInfo.route;
      },
    },
  },
	methods: {
		...mapMutations('booking', ['resetOrder', 'clearSelectedObjects']),
	},
	created() {
        this.clearSelectedObjects()
		this.resetOrder()
	}  
});
</script>