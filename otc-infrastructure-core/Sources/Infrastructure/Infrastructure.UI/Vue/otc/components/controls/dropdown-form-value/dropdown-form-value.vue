<template>
    <div v-if="!isHidden" class="row formGroupRow form-group">
        <label v-if="!!label" :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ label }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <div :class="dropDownClass">
                    <button class="btn btn-primary dropdown-toggle" type="button" :id="id" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                        {{placeholder}}
                        <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" :aria-labelledby="id">
                        <li v-for="item in items" :key="item.Value"><a @click="onSelect(item.Value)" href="#">{{item.Text}}</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';
    import { FormValueSettingsMixin } from 'EXT/components/mixins/formValueSettingsMixin';
    import styles from 'EXT/otc/styles/error-style.module.scss'

    export default {
        name: 'dropdown-form-value',
        mixins: [FormValueCalcMixin, FormValueSettingsMixin],
        props: {
            label: String,
            items: Array,
            placeholder: String,
            className: String
        },
        data: function() {
            return {
                id: null,
                dropDownClass: this.className || 'dropdown'
            }
        },
        mounted() {
            this.id = 'dropdown_' + this._uid
        },
        methods: {
            onSelect(value) {
                this.$emit('select', value);
            }
        }
    }
</script>