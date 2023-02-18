<template>
  <div :class="s.containerInput">
    <div :class="s.authorizationName">
      {{ $t("auth.login")
      }}<span v-show="validation.userName.empty" :class="s.red">&nbsp;*</span>
    </div>
    <input
      :class="[
        s.authorizationInput,
        validation.userName.invalid ? s.errorInput : '',
      ]"
      type="text"
      v-model="authData.userName"
    />
    <div
      v-show="validation.userName.invalid"
      :class="[s.errorFakeContainer, s.red]"
    >
      <div :class="s.errorContainer">
        <div :class="s.errorImg"></div>
        <div :class="s.errorMessage">{{ $t("auth.wrongLogin") }}</div>
      </div>
    </div>
    <div :class="s.authorizationName">
      {{ $t("auth.password")
      }}<span v-show="validation.password.empty" :class="s.red">&nbsp;*</span>
    </div>
    <input
      :class="[
        s.authorizationInput,
        validation.password.invalid ? s.errorInput : '',
      ]"
      type="password"
      v-model="authData.password"
      @keydown.enter="login"
    />
    <div
      v-show="validation.password.invalid"
      :class="[s.errorFakeContainer, s.red]"
    >
      <div :class="s.errorContainer">
        <div :class="s.errorImg"></div>
        <div :class="s.errorMessage">{{ $t("auth.wrongPassword") }}</div>
      </div>
    </div>
    <div :class="s.blockBtn">
      <BbButton @click="login" :theme="ButtonTheme.Black">
        {{ $t("booking.continue") }}
      </BbButton>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import BbButton, { ButtonTheme } from "~/components/controls/bb-button/bb-button.vue";

export default Vue.extend({
  name: "login",
  components: {
    BbButton,
  },
  props: {
    returnUrl: String
  },
  data() {
    return {
      s: style,
      authData: {
        userName: "",
        password: "",
      },
      validation: {
        userName: {
          empty: false,
          invalid: false,
        },
        password: {
          empty: false,
          invalid: false,
        },
      },
      ButtonTheme,
    };
  },
  methods: {
    async login() {
      if (this.validate()) {
        try {
          await this.$auth.loginWith("local", {
            data: this.authData,
          });
          this.$router.push(this.localePath(this.returnUrl ?? "current-bookings"));
        } catch (e) {
          if (e.response && e.response.data && e.response.data.message) {
            if (
              e.response.data.message.toLowerCase().indexOf("username") > -1
            ) {
              this.validation.userName.invalid = true;
            }
            if (
              e.response.data.message.toLowerCase().indexOf("password") > -1
            ) {
              this.validation.password.invalid = true;
            }
          }
        }
      }
    },
    validate(): boolean {
      let result = true;

      if (!this.authData.userName) {
        this.validation.userName.empty = true;
        result = false;
      }
      if (!this.authData.password) {
        this.validation.password.empty = true;
        result = false;
      }

      return result;
    },
  },
  watch: {
    "authData.userName": function (newVal) {
      if (newVal) {
        this.validation.userName.empty = false;
        this.validation.userName.invalid = false;
      }
    },
    "authData.password": function (newVal) {
      if (newVal) {
        this.validation.password.empty = false;
        this.validation.password.invalid = false;
      }
    },
  },
});
</script>