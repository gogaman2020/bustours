<template>
    <div :class="[style.select]" @click="onClick">
        <span v-if="label" :class="[style.selectLabelText]">{{label}}</span>

		<tooltip v-if="hint" :message="hint" :class="[style.selectLabelTooltip]" />

        <div v-if="open" :class="[style.selectDropdown]" @keyup.stop="onKeyUpDropDown" >
            <input v-if="allowSearch" type="text" :placeholder="placeholderOpened ? placeholderOpened : placeholder" 
                :class="[style.selectDropdownInput, !keywords ? style.selectDropdownInputEmpty : '']" 
                v-model="keywords" @click.stop="onClickInputDropDown" v-focus>

            <ul v-if="visibleItems && visibleItems.length > 0" :class="[style.selectDropdownList]" @click.stop="onClickDropDown">
                <li v-for="item in visibleItems" :key="item.Value" :value="item.Value" 
                    :class="[item.Disabled ? style.selectDropdownListItemDisabled : style.selectDropdownListItemEnabled]"
                    @click.stop="onClickDropDownItem(item)">

                    <div :class="[style.selectDropdownListItemText]">{{ item.Text }}</div>
                </li>
            </ul>
            <span v-else-if="visibleItems && visibleItems.length == 0" :class="[style.selectDropdownList]">Не найдено</span>
            <span v-else :class="[style.selectDropdownList]">{{message}}</span>
        </div>

        <div :class="[style.selectText]" :role="'input'">
            <div v-if="selectedItem">{{selectedItem.Text}}</div>
            <div v-if="!selectedItem" :class="[style.selectTextPlaceHolder]">{{placeholderClosed ? placeholderClosed : placeholder}}</div>
        </div>
	</div>
</template>

<script>
    import style from './style.module.scss'
    
    import tooltip from 'EXT/otc2.0/components/common/tooltip/tooltip'

	export default {
        name: 'single-select-search-control',

		components: {
            tooltip
		},
		props: {
            label: {
				type: String,
				default: ''
            },
			placeholder: {
				type: String,
				default: ''
            },
            placeholderOpened: {
				type: String,
				default: ''
            },
            placeholderClosed: {
				type: String,
				default: ''
            },
            hint: {
				type: String,
				default: ''
            },
            search: {
				type: Function,
                required: true
            },
            selectedKey: null,
            allowSearch: {
				type: Boolean,
				default: true
            },
            allowCreate: {
				type: Boolean,
				default: false
            },
            term: {
				type: String,
				default: ''
            },
		},
		data() {
			return {
                style: style,
                open: false,
                keywords: '',
                selectedItem: null,
                timer: null,
                timeout: 500,
                visibleItems: null,
                message: ''
			}
        },
        created() {
            document.addEventListener('click', this.onClickOutside);
            document.addEventListener('keyup', this.onKeyUpGlobal);
        },
        mounted(){
            this.initSelectedItem();
            this.keywords = this.term;
        },
        methods: {
            show() {
                if (this.open === true) return;
                this.open = true;

                this.beginSearch();
            },
            close() {
                if (this.open === false) return;
                this.open = false;
            },
            onClick: function() {
                this.open = !this.open;
            },
            onClickDropDown: function() {
            },
            onClickInputDropDown: function() {
            },
            onClickDropDownItem: function(item) {
                this.selectedItem = item && item.Value ? item : null;
                this.close();
            },
            onClickOutside: function(e) {
                let el = e.target;

                while (el) {
                    if (el === this.$el) {
                        return;
                    }

                    el = el.parentElement;
                }

                this.close();
            },
            onKeyUpDropDown: function(e) {
                if (e.code === "Escape") {
                    this.close();
                } 
            },
            onKeyUpGlobal: function(e) {
                if (e.code === "Tab" || e.code === "Escape") {
                    this.close();
                } 
            },
            initSelectedItem: function() {
                if (!this.visibleItems || this.visibleItems.length === 0 || !this.selectedKey) {
                    this.selectedItem = null;
                    return;
                }

                var filtered = this.visibleItems.filter(item => item.Value == this.selectedKey);
                if (filtered && filtered.length > 0) {
                    this.selectedItem = filtered[0];
                }
            },
            initTimer: function(callback){
                this.deinitTimer();
                this.timer = setTimeout(callback, this.timeout);
            },
            deinitTimer: function(){
                if(this.timer){
                    clearTimeout(this.timer);
                    this.timer = null;
                }
            },
            beginSearch: async function(){
                let self = this;
                this.deinitTimer();
                this.initTimer(async function(){
                    self.message = 'Поиск...';
                    self.visibleItems = null;
                    let result = await self.search({term: self.keywords});

                    if(result){
                        self.visibleItems = result;
                    } else {
                        self.visibleItems = [];
                    }

                    if (self.allowCreate) {
                        self.visibleItems.push({Value: -1, Text: '< создать нового >'});
                    }

                    self.message = '';
                });
            },
            getInput: function(){
                return this.keywords;
            }
        },
        // computed: {
        //     visibleItems() {
        //         if (!this.keywords) return this.items;

        //         var keyword = this.keywords.trim().toLowerCase();
        //         if (!keyword) return this.items;

        //         var filtered = this.items.filter(item => item.Disabled || item.Text.toLowerCase().indexOf(keyword) !== -1);

        //         return filtered;
        //     }
        // },
        watch: {
            selectedItem: function(val) {
                var selectedKey = val ? val.Value : null;
                var intValue = selectedKey ? parseInt(selectedKey, 10) : null;

                this.$emit('update:selectedKey', intValue ? intValue : selectedKey);
                this.$emit('select', val);
            },
            selectedKey: function(val) {
                this.initSelectedItem();
            },
            keywords: function(val) {
                this.beginSearch();
            },
        },
        directives: {
            focus: {
                inserted: function(el) {
                    el.focus();
                }
            }
        }
	}
</script>