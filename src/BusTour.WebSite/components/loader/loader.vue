<template lang="html">
  <div :class="s.loadingPage" v-if="loading">
    <p>Loading...</p>
  </div>
</template>

<script>
    import style from "./style.module.scss";

    export default {
        data: () => ({
            loading: false,
            s: style
        }),
        methods: {
            start() {
                this.loading = true
            },
            finish() {
                this.loading = false
            }
        }
    }
</script>