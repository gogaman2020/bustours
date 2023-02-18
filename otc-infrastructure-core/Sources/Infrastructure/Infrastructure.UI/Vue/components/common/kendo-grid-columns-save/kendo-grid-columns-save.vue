<template>
    <div :style="{visibility: saveInProgress ? 'visible' : 'hidden'}" >
        <span style="color:gray; font-size: 12px; height:12px">Настройки сохраняются...</span>
    </div>
</template>
<script>
	import axios from 'axios';
	export default {
		name: 'kendo-grid-columns-save',
		components: {

		},
        props: {
            nodeKey: Number,            
            nodeKeyName: String,            
            kendoGridRef: String,
            skipColumnCount: Number
		},
		data() {
            return {
                grid: null,
                queue: [],
                timeoutId: null,
                firstLoad: true,
                saveInProgress: false,
                eventBus: window.eventBus,
                webApiUrl: process.env.VUE_APP_WEBAPI_URL,
                tenderUrl: process.env.VUE_APP_TENDER_URL,
                getSettingsUrl: process.env.VUE_APP_PR_GET_KENDO_SETTINGS,
                saveSettingsUrl: process.env.VUE_APP_PR_SAVE_KENDO_SETTINGS
            }
        },
        computed: {
            skipCount: function () {
                return this.skipColumnCount ? this.skipColumnCount : 0;
            },
            nodeKeyId: function () {
                return this.nodeKeyName ? this.nodeKeyName : this.nodeKey;
            }
        },
        beforeDestroy() {
            clearTimeout(this.timeoutId);
        },
        mounted() {
            let self = this;
            this.timeoutId = setTimeout(this.queueConcumer.bind(this), 1);
            this.$nextTick(function () {
                let kendoGrid = self.$parent.$refs[self.kendoGridRef];
                if (!kendoGrid) {
                    return;
                }
                self.grid = kendoGrid.kendoWidget();                
                self.grid.setOptions({
                    columnShow: self.onColumnsUpdate.bind(this),
                    columnHide: self.onColumnsUpdate.bind(this),
                    columnReorder: self.onReorder.bind(this),
                });
                var config = { withCredentials: true };
                axios.post(this.tenderUrl + this.getSettingsUrl, { pageNodeKey: this.nodeKeyId }, config)
                    .then(function (result) {
                        var settings = result.data.settings;
                        if (settings.length == 0) {
                            return;
                        }
                        var currentColumns = self.grid.columns;
                        var newColumns = [];                       

                        for (var i = 0; i < self.skipCount; i++) {
                            newColumns.push(currentColumns[i]);
                        }

                        for (var i = 0; i < settings.length; i++) {
                            var currentColumn = _.find(currentColumns, function (item) {
                                return item.field == settings[i].Name;
                            });

                            if (currentColumn) {
                                currentColumn.hidden = settings[i].Visible === false ? true : undefined;
                                newColumns.push(currentColumn);
                            }
                        }

                        _.each(currentColumns, function(currentColumn){
                            var newColumn = _.find(newColumns, function(newColumn) {
                                return newColumn.field == currentColumn.field;
                            });

                            if (newColumn === undefined) {
                                currentColumn.hidden = true;
                                newColumns.push(currentColumn);
                            }
                        });

                        self.grid.setOptions({
                            columns: newColumns
                        })
                    });
            });
        },
        methods: {
            queueConcumer: async function () {
                while (true) {                    
                    while (this.queue.length != 0) {
                        this.saveInProgress = true;
                        if (this.queue.length > 1) {
                            while (this.queue.length != 1) {
                                this.queue.shift();
                            }
                        }                        
                        let item = this.queue.shift();
                        await this.onColumnChange(item, this);
                        this.saveInProgress = false;
                    }
                    await this.timeout(100);
                }                
            },
            onReorder: function (event) {                
                let self = this;
                let oldIndex = event.oldIndex;                
                let newIndex = event.newIndex;
                if (newIndex <= self.skipCount - 1 || oldIndex <= self.skipCount - 1) {                    
                    setTimeout(function () {
                        self.grid.reorderColumn(oldIndex, event.column);
                    });
                    event.preventDefault();
                    return;
                }
                this.onColumnsUpdate();
            },
            onColumnsUpdate: function () {
                let self = this;           
                setTimeout(function () {
                    let columns = self.grid.columns;
                    self.queue.push(columns);
                });
            },
            onColumnChange: function (currentColumns, self) {
                let columns = currentColumns.map(function (item, index) {
                    return {
                        Name: item.field,
                        Visible: item.hidden === undefined || item.hidden === false,
                        Order: index
                    }
                });
                var config = { withCredentials: true };
                var body = {
                    infos: [
                        {
                            NodeKey: this.nodeKeyId,
                            ColumnInfos: columns
                        }
                    ]
                };
                columns.splice(0, this.skipCount);
                return axios.post(self.tenderUrl + self.saveSettingsUrl, body, config);
            },
            timeout: function (timeout) {
                let promise = new Promise(function (resolve, reject) {
                    setTimeout(function () {
                        resolve(true);
                    }, timeout);
                });
                return promise;
            }
		}
	}
</script>