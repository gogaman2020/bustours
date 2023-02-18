<template>
    <div v-if="templateArgs.isActive" class="template-button-wrapper">
        <a :class="[templateArgs.class, 'button-position']" href="javascript:void(0)" @click="buttonClick">{{templateArgs.text}}</a>
    </div>
</template>

<script>
    export default {
        name: "buttonTemplate",
        components:{
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
    .template-button-wrapper{
        display: flex;
        justify-content: center;
    }

    .button-position {
        padding-left: 0;

        & span {
            margin-top: -0.5em;
            margin-left: -0.5em;
        }
    }

    .user-action {
        width: 21px;
        height: 18px;
        background-image: url("data:image/svg+xml,%3Csvg width='23' height='23' viewBox='0 0 23 23' fill='none' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M3.83341 18.2083H1.91675C1.91675 15.0326 4.49111 12.4583 7.66675 12.4583C10.8424 12.4583 13.4167 15.0326 13.4167 18.2083H11.5001C11.5001 16.0912 9.78384 14.3749 7.66675 14.3749C5.54966 14.3749 3.83341 16.0912 3.83341 18.2083ZM15.3277 14.7678L12.7344 12.1803L14.0876 10.8214L15.3257 12.0596L19.4466 7.94642L20.7997 9.30342L15.3257 14.7659L15.3277 14.7678ZM7.66675 11.4999C5.54966 11.4999 3.83341 9.78368 3.83341 7.66659C3.83341 5.54949 5.54966 3.83325 7.66675 3.83325C9.78384 3.83325 11.5001 5.54949 11.5001 7.66659C11.4974 9.78258 9.78275 11.4973 7.66675 11.4999ZM7.66675 5.74992C6.61967 5.75098 5.76722 6.59215 5.75222 7.63912C5.73722 8.68609 6.56521 9.55134 7.61183 9.58241C8.65844 9.61347 9.5363 8.79885 9.58342 7.75284V8.13617V7.66659C9.58342 6.60804 8.72529 5.74992 7.66675 5.74992Z' fill='%232E3A59'/%3E%3C/svg%3E");
    }
</style>