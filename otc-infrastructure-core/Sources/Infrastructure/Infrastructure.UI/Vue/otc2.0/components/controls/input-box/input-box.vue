<template>
    <div :class="[style.inputBox]">
        <span :class="[style.inputBoxLabel]">{{label}}</span>

		<tooltip v-if="hint" :message="hint" :class="[style.inputBoxTooltip]" />

        <input type="text" :placeholder="placeholder" 
            :class="[style.inputBoxInput]" 
            :value="value" @input="$emit('input', $event.target.value)"
            :disabled="disabled">
	</div>
</template>

<script>
    import style from './style.module.scss'

	export default {
        name: 'input-box-control',

		props: {
            label: {
				type: String,
				default: ''
            },
			placeholder: {
				type: String,
				default: ''
            },
            hint: {
				type: String,
				default: ''
            },
            value: {
				type: String,
				default: ''
            },
            disabled: {
				type: Boolean,
				default: false
            }
		},
		data() {
			return {
                style: style
			}
        },
        methods: {
        }
	}
</script>