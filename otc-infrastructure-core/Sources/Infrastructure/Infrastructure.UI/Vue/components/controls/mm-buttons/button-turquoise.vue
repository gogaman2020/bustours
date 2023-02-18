<template>
    <Button v-if="!IsHidden" :class="[style.buttonTurquoise]" :size="size" :disabled="IsDisabled" @click="handlerOnClick">
        <slot></slot>
    </Button>
</template>

<script>
    import Button from './button';
    import { click } from './mixin'
    import ContractFormSettingsMixin from "EXT/components/mixins/contract-form-settings-mixin.js";

    export default {
        name: 'button-turquoise',
        components: {
            Button,
            click
        },
        mixins: [click, ContractFormSettingsMixin]
    }
</script>