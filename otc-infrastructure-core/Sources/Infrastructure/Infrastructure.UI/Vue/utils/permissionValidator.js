export const RoutePermission = Object.freeze({ "procedureList":1, "procedure":2, "request":3, 'commission':4, 'chat':5 });

export const permissionValidator = {
    _permissionList: null,
    _url: process.env.VUE_APP_WEBAPI_URL,

    hasAccess: function(permission, callback){
        if (this._permissionList !== null) {
            callback(_.contains(this._permissionList, permission));
            return;
        }

        var self = this;
        $.ajax({
            method: 'GET',
            url: self._url + '/Permission/RouterPermissions',
            contentType: 'application/json',
            dataType: 'json',
            xhrFields: {
                withCredentials: true
            },
            success: function (data) {
                self._permissionList = data;
                self.hasAccess(permission, callback);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                callback(false);
            }
        });
    }
};