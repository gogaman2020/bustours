<template>
    <div :class="[style.inputBox]">
        <span :class="[style.inputBoxLabel]">{{label}}</span>

		<tooltip v-if="hint" :message="hint" :class="[style.inputBoxTooltip]" />

        <VueSuggestions
            :class="[style.inputBoxInput]"
            :model.sync="inputValue"
            :placeholder="placeholder"
            :options="suggestionOptions"
            @input="$emit('input', $event.target.value)">
        </VueSuggestions>
        <!-- <input type="text" :placeholder="placeholder" 
            :class="[style.inputBoxInput]" 
            :value="value" @input="$emit('input', $event.target.value)"
            :disabled="disabled"> -->
	</div>
</template>

<script>
    import style from './style.module.scss'
    import VueSuggestions from 'vue-suggestions';

	export default {
        name: 'input-box-dadata-control',

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
            },
            token: {
                type: String,
                requered: true
            },
            type: {
                type: String,
                requered: true
            },
            select: {
                type: Function,
                requered: true
            }
        },
        components:{
            VueSuggestions 
        },
		data() {
			return {
                style: style,
                inputValue: this.value,
                suggestionOptions: {
                    // @see https://confluence.hflabs.ru/pages/viewpage.action?pageId=207454318
                    token: this.token,
                    type: this.type,
                    scrollOnFocus: false,
                    triggerSelectOnBlur: true,
                    triggerSelectOnEnter: true,
                    addon: 'none',
                    // @see https://confluence.hflabs.ru/pages/viewpage.action?pageId=207454320
                    onSelect: this.onSelect
                },
                disableWatch: 0
			}
        },
        watch: {
            value: function(newValue){
                if(this.disableWatch > 0){
                    this.disableWatch += 1;
                }
                this.inputValue = this.value;
            },
            inputValue: function(newValue){
                if(this.disableWatch > 0){
                    this.disableWatch -= 1;
                    return;
                }

                this.$emit('input', this.inputValue);
            }
        },
        methods: {
            onSelect: function(suggestion){
                if(this.select){
                    this.inputValue = this.select(suggestion);
                } else {
                    this.inputValue = suggestion.value;
                }
                this.disableWatch = 1;

                this.$emit('input', this.inputValue);
                this.$forceUpdate();
            }
        }
	}
</script>