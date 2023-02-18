/* jshint esversion: 8 */

import axios from 'axios';

export const dataLoader = {
	name: 'data-loader',

	data() {
		return {
            webApiUrl: process.env.VUE_APP_WEBAPI_URL
		};
	},
	methods: {
        downloadExcelFile: async function (method, parameters, callback) {
            var url = this.webApiUrl + '/Export/' + method + '?request=' + JSON.stringify(parameters);

            return await axios.get(url, {withCredentials: true, responseType: 'arraybuffer'})
                .then((response) => {
                    var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
                    var fileName = 'Отчёт.xlsx';
                    var disposition = response.headers["content-disposition"];

                    if (disposition && disposition.indexOf("attachment") !== -1) {
                        var fileNameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        var matches = fileNameRegex.exec(disposition);
            
                        if (matches != null && matches[1]) {
                            fileName = matches[1].replace(/['"]/g, "");
                        }
                    }

                    if (navigator.msSaveBlob) {
                        window.navigator.msSaveOrOpenBlob(blob, fileName);
                    } else {
                        var link = document.createElement('a');
                        link.href = window.URL.createObjectURL(blob);
                        link.download = decodeURI(fileName);
                        link.click();
                    }

                    callback();
                })
                .catch((error) => {
                    console.log(error);

                    callback();
                });
        }
	}
};