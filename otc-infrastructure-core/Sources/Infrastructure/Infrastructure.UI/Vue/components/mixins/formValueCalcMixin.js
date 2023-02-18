export const FormValueCalcMixin = {
    props: {
        labelLen: Number,
        valueLen: Number
    },
    data: function() {
        return {
            labelSize: process.env.VUE_APP_LABEL_SIZE,
            fullSize: process.env.VUE_APP_FULL_SIZE,
            localLabelLen: this.labelLen,
            localValueLen: this.valueLen,
        };
    },
    computed: {
        calcLabelSize: function() {
            if (this.labelLen === undefined || this.labelLen === null || isNaN(this.labelLen)) {
                this.localLabelLen = this.labelSize;
            }

            return 'col-sm-' + this.localLabelLen;
        },

        calcValueSize: function() {
            if (this.valueLen === undefined || this.valueLen === null || isNaN(this.valueLen)) {
                this.localValueLen = this.fullSize - this.localLabelLen;
            }

            return 'col-sm-' + this.localValueLen;
        }
    }
}