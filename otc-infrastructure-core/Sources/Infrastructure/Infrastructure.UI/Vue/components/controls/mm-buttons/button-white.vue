<template>
    <Button v-if="!IsHidden" :class="[style.buttonWhite]" :size="size" :disabled="IsDisabled" @click="handlerOnClick">
        <slot></slot>
    </Button>
</template>

<script>
    import Button from './button';
    import { click } from './mixin';
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

    export default {
        name: 'button-white',
        components: {
            Button,
            click
        },
        mixins: [click, ContractFormSettingsMixin]
    }
</script>