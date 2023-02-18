export const GetPermissionsMixin = {
    data: function() {
        return {
            eventBus: window.eventBus,
            webApiUrl: process.env.VUE_APP_WEBAPI_URL,
            permissions: null,
            permissionsLoading: null
        };
    },
    created: function() {
        this.getPermissions();
    },
    methods: {
        getPermissions: function() {
            var self = this;
            self.permissionsLoading = true;
            $.ajax({
                method: 'GET',
                url: self.webApiUrl + '/Permission/GetPermission',
                contentType: 'application/json',
                dataType: 'json',
                xhrFields: {
                    withCredentials: true
                },
                success: function (response) {
                    self.permissionsLoading = null;
                    if (response) {
                        self.permissions = response;
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.permissionsLoading = null;
                    self.eventBus.$emit('show-message', { header: 'Ошибка', message: textStatus });
                }
            });
        }
    }
}