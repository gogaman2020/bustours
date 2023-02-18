export const ViewMode = Object.freeze({ "hidden":1, "view":2, "edit":3, "required":4 });

export const FormValueSettingsMixin = {
    props: {
        elName: String,
        additionalTester: Function,
		ignoreSettings: Boolean,
		readonly: Boolean
    },
    data: function() {
        return {
            viewMode: ViewMode.edit,
            eventBus: window.eventBus,
        }
    },
    created() {
        this.eventBus.$on('form-value-settings', (data) => {
            this.getViewMode(data);
        });
    },
    beforeDestroy:function(){
        this.eventBus.$off('form-value-settings');
    },
    computed: {        
        isView: function() {
            return this.isMode(ViewMode.view);
        },
        isHidden: function() {
            return this.isMode(ViewMode.hidden);
        },
        isEdit: function() {
            return this.isMode(ViewMode.edit);
        },
        isRequired: function() {
            return this.isMode(ViewMode.required);
        }
    },
    methods: {
        getViewMode: function(visibilitySettings) {
			if (this.ignoreSettings) {
				if (this.readonly) {
					this.viewMode = ViewMode.view;
				}
				return;
			}

            if(!visibilitySettings) {
                this.viewMode = ViewMode.view;
                return;
            }

            var isEdit = visibilitySettings["IsEdit"];
            if (isEdit !== undefined && isEdit !== null) {
                this.viewMode = isEdit.IsRequired ? ViewMode.edit : ViewMode.view;
                return;
            }

            if(this.additionalTester){
                var settings = this.additionalTester(this, visibilitySettings);
                if(settings){
                    this.applySettings(settings);
                    return;
                }
            }

            var settings = visibilitySettings[this.elName];
            this.applySettings(settings);
        },
        isMode: function(testValue) {
            if(this.viewMode === null){
                this.getViewMode();
            }

            return this.viewMode === testValue;
        },
        applySettings: function(settings){
            if (settings) {
                if (settings.IsHidden) {
                    this.viewMode = ViewMode.hidden;
                } else if (settings.IsDisabled) {
                    this.viewMode = ViewMode.view;
                } else if (settings.IsRequired) {
                    this.viewMode = ViewMode.required;
                    this.eventBus.$emit('form-value-validate', this.getValidationSettings());
                } else {
                    this.viewMode = ViewMode.edit;
                }
            } else {
                this.viewMode = ViewMode.view;
            }
        }
    }
}