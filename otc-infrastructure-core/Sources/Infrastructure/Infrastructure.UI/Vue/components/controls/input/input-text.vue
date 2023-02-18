<template>
	<label :class="[style.input, 'row formGroupRow form-group']" :id="id">
		<span :class="[calcLabelSize, style.inputLabel, 'formFieldLabel text-right']">
			<slot></slot>
		</span>
		<div :class="[calcValueSize, style.inputField]">
			<div class="col-sm-12 no-space formFieldValueDiv">
				<input autocomplete="off" :class="[!valid ? style.inputError : '', 'form-control']" :id="id" :name="''" type="text"
				v-bind:value="value"
				:disabled="disabled"
				:placeholder="placeholder"
				@keyup="$emit('input', $event.target.value)"
				@blur="$emit('blur', $event)">
				<div :class="style.inputSuggestion">
					<slot name="suggestion"></slot>
				</div>
			</div>	
			<div :class="style.inputErrors">
				<slot name="error"></slot>
			</div>	
		</div>
		
	</label>
</template>

<script>

	import style from './style.module.scss'
	import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';

	export default {
		name: 'input-text',
		mixins: [FormValueCalcMixin],
		components: {

		},
		props: {
			placeholder: {
				type: String,
				default: '',
				required: false
			},
			value: {
				type: String,
				default: '',
				required: false
			},
			disabled: {
				type: Boolean,
				default: false,
				required: false
			},
			valid: {
				type: Boolean,
				default: true,
				required: false
			},
			mask: {
				type: String,
				default: '',
				required: false
			}
		},
		
		data() {
			return {
				style: style,
				id: `f${(~~(Math.random()*1e8)).toString(16)}`
			}
		},
		mounted() {
			if(this.mask.length > 0) {
				$(this.$el).find('input').inputmask(this.mask)
			}
		}

	}
</script>