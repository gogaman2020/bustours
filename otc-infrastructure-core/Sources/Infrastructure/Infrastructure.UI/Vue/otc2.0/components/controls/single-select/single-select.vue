<template>
    <div :class="[style.select]" @click="onClick">
        <span v-if="label" :class="[style.selectLabelText]">{{label}}</span>

		<tooltip v-if="hint" :message="hint" :class="[style.selectLabelTooltip]" />

        <div v-if="open" :class="[style.selectDropdown]" @keyup.stop="onKeyUpDropDown" >
            <input v-if="allowSearch" type="text" :placeholder="placeholderOpened ? placeholderOpened : placeholder" 
                :class="[style.selectDropdownInput, !keywords ? style.selectDropdownInputEmpty : '']" 
                v-model="keywords" @click.stop="onClickInputDropDown" v-focus>

            <ul :class="[style.selectDropdownList]" @click.stop="onClickDropDown">
                <li v-for="item in visibleItems" :key="item.Value" :value="item.Value" 
                    :class="[item.Disabled ? style.selectDropdownListItemDisabled : style.selectDropdownListItemEnabled]"
                    @click.stop="onClickDropDownItem(item)">

                    <div :class="[style.selectDropdownListItemText]">{{ item.Text }}</div>
                </li>
            </ul>
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
        name: 'single-select-control',

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
            items: {},
            selectedKey: null,
            allowSearch: {
				type: Boolean,
				default: true
            }
		},
		data() {
			return {
                style: style,
                open: false,
                keywords: '',
                selectedItem: null
			}
        },
        created() {
            document.addEventListener('click', this.onClickOutside);
            document.addEventListener('keyup', this.onKeyUpGlobal);
        },
        mounted(){
            this.initSelectedItem();
        },
        methods: {
            show() {
                if (this.open === true) return;
                this.open = true;
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
                if (!this.items || this.items.length === 0 || !this.selectedKey) {
                    this.selectedItem = null;
                    return;
                }

                var filtered = this.items.filter(item => item.Value == this.selectedKey);
                if (filtered && filtered.length > 0) {
                    this.selectedItem = filtered[0];
                }
            }
        },
        computed: {
            visibleItems() {
                if (!this.keywords) return this.items;

                var keyword = this.keywords.trim().toLowerCase();
                if (!keyword) return this.items;

                var filtered = this.items.filter(item => item.Disabled || item.Text.toLowerCase().indexOf(keyword) !== -1);

                return filtered;
            }
        },
        watch: {
            selectedItem: function(val) {
                var selectedKey = val ? val.Value : null;
                var intValue = selectedKey ? parseInt(selectedKey, 10) : null;

                this.$emit('update:selectedKey', intValue ? intValue : selectedKey);
            },
            items: function(val) {
                this.initSelectedItem();
            },
            selectedKey: function(val) {
                this.initSelectedItem();
            }
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