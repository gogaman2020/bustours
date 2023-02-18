<template>
    <div class="check-wrapper">
        <checkbox v-model="templateArgs.value"
                class="check-position"
                elName="IsEdited"
                :disabled="!templateArgs.isEnable"
                @input="buttonClick">
        </checkbox>
    </div>
</template>

<script>
    import Checkbox from 'EXT/components/controls/checkbox/checkbox'

    export default {
        name: "checkTemplate",
        components:{
            Checkbox
        },
        methods: {
            buttonClick: function(e) {
                var gridWidget = this.templateArgs.parentComponent.kendoWidget();
                var data = gridWidget.dataItem($(this.$el).closest('tr'));
                this.templateArgs.click(data);
            }
        },
        data() {
            return {
                templateArgs: {}
            };
        }
    };
</script>

<style lang="scss">
    .check-wrapper{
        display: flex;
        justify-content: center;
    }


    .check-position {
        padding-left: 0;

        & span {
            margin-top: -0.5em;
            margin-left: -0.5em;
        }
    }
</style>