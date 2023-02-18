<template>
    <div class="row formGroupRow form-group">
        <label :class="[calcLabelSize, 'text-right', 'formFieldLabel']"><span class="originalText">{{ elLabel }}</span></label>
        <div :class="calcValueSize">
            <div class="col-sm-12 no-space formFieldValueDiv">
                <a :href="href" :target="target">{{ elValue }}</a>
            </div>
        </div>
    </div>
</template>

<script>
    import { FormValueCalcMixin } from 'EXT/components/mixins/formValueCalcMixin';

    export default {
        name: 'LinkFormElement',
        mixins: [FormValueCalcMixin],
        props: {
            elLabel: String,
            elValue: String,
            href: String,
            target: String
        },
        data: function() {
            return {
                eventBus: window.eventBus,
            }
        },
        computed: {
            value: function () {
                return this.elValue;
            }
        }
    }
</script>