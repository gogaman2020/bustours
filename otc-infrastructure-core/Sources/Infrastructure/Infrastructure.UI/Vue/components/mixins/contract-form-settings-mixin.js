import { mapGetters } from "vuex";

export default {
    name: "contract-form-settings-mixin",

    props: {
        elName: { type: String, default: "default" },
        elGroupName: { type: String, default: "default" },
        isDisabled: { type: Boolean, default: undefined },
        forceDisabled: { type: Boolean, default: undefined },
    },
    computed: {
        ...mapGetters({
            FormSettings: "formSettings/get",
            IsShowError: "formSettings/isShowError",
        }),

        keyName() {
            return this.elName.split("/");
        },
        keyGroupName() {
            return this.elGroupName.split("/");
        },

        IsDisabled() {
            if (this.forceDisabled === true) {
                return true
            }
            else {
                //приоритет у правила, переданного с бэка
                var checkResult = this.checkRuls("IsDisabled", false);
                return !checkResult && typeof this.isDisabled !== "undefined" ? this.isDisabled : checkResult;
            }
         },
        IsHidden() {
            return this.checkRuls("IsHidden", false);
        },
        IsRequired(){ 
            return this.checkRuls("IsRequired", false);
        }
    },
    methods: {
        isEmptyObj(obj) {
            for (var key in obj) {
                return false;
            }
            return true;
        },
        checkRuls(ruls, defaultValue){
            var currentSettings = defaultValue;
            if (!this.isEmptyObj(this.FormSettings) && this.keyName.length > 1 && this.FormSettings[this.keyName[0]] && typeof this.FormSettings[this.keyName[0]][this.keyName[1]] !== "undefined") {
                currentSettings = this.FormSettings[this.keyName[0]][this.keyName[1]][ruls];
            } else if (!this.isEmptyObj(this.FormSettings) && this.keyGroupName.length > 1 && this.FormSettings[this.keyGroupName[0]] && typeof this.FormSettings[this.keyGroupName[0]][this.keyGroupName[1]] !== "undefined") {
                currentSettings = this.FormSettings[this.keyGroupName[0]][this.keyGroupName[1]][ruls];
            }

            return currentSettings!= undefined? currentSettings : defaultValue;
        }
    },
};
