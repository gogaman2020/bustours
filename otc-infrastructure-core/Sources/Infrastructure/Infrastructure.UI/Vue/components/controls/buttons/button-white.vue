<template>
    <Button v-if="type == 'button' || type == 'submit'" :class="[style.buttonBlue]" :type="type" :disabled="disabled" @click="handlerOnClick">
        <slot></slot>
    </Button>
    <button-link v-else-if="type == 'link'" :href="href" :class="[style.buttonBlue]" disabled="disabled">
        <slot></slot>
    </button-link>
    <div v-else>
        <p>Ошибка в параметрах кнопки</p>
    </div>
</template>

<script>
    import Button from './button';
    import { buttonMixin } from './mixin'

    export default {
        name: 'button-white',
        components: {
            Button
        },
        href: {
            type: String,
            default: '',
            required: false
        },
        mixins: [buttonMixin]
    }
</script>