<template>
    <Button v-if="type == 'button'" :class="[style.buttonGreen]" :disabled="disabled" @click="handlerOnClick">
        <slot></slot>
    </Button>
    <button-link v-else-if="type == 'link'" :href="href" :class="[style.buttonGreen]" disabled="disabled">
        <slot></slot>
    </button-link>
    <div v-else>
        <p>Ошибка в параметрах зеленой кнопки</p>
    </div>
</template>

<script>
    import Button from './button';
    import { buttonMixin } from './mixin'

    export default {
        name: 'button-green',
        components: {
            Button,
            // click
        },
        href: {
            type: String,
            default: '',
            required: false
        },
        mixins: [buttonMixin]
    }
</script>