<template>
    <div class="blockUI blockMsg blockPage loading">
        <div class="loading-message loading-message-boxed">
            <img :src="`${siteUrl}/spin.gif`" align="">
            <span><slot></slot></span>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'loading',
        data() {
            return {
                siteUrl: process.env.VUE_APP_SITE_URL
            }
        }
    }
</script>

<style>
    .loading {
        z-index: 1011;
        position: absolute;
        padding: 0px;
        margin: 0px;
        width: 100%;
        height: 100%;
        top: 0;
        left: 0;
        text-align: center;
        color: rgb(0, 0, 0);
        border: 0px; cursor: wait;
        background-color: #bebebe88;
    }

    .loading .loading-message {
        position: fixed;
        padding: 5px;
        margin: 0;
        width: 30%;
        top: 50%;
        left: 50%;
        transform: translate(-35%, -50%);
        text-align: center;
        color: rgb(0, 0, 0);
        border: 0px; cursor: wait;
    }
    .loading .loading-message img {
        margin-right: 5px;
    }
</style>