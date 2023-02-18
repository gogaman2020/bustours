<template>
    <Button v-if="!IsHidden" :class="[style.buttonBlue]" :size="size" :disabled="IsDisabled" @click="handlerOnClick">
        <slot></slot>
    </Button>
</template>

<script>
    import Button from './button';
    import { click } from './mixin'
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

    export default {
        name: 'button-blue',
        components: {
            Button,
            click
        },
        mixins: [click, ContractFormSettingsMixin]
    }
</script>