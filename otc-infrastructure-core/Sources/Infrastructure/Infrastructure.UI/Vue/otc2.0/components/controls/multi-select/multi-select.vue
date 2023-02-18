<template>
    <div :class="[style.select]" @click="onClick">
        <span v-if="label" :class="[style.selectLabelText]">{{label}}</span>

		<tooltip v-if="hint" :message="hint" :class="[style.selectLabelTooltip]" />

        <div v-if="open" :class="[style.selectDropdown]" @keyup.stop="onKeyUpDropDown">
            <MultiSelectTags v-if="showTags" :tags="selectedItems" @remove="onTagRemove"
                :maxVisibleSelectedItems="maxVisibleSelectedItems" :showAllTags.sync="showAllTags" />

            <input type="text" :placeholder="placeholderOpened ? placeholderOpened : placeholder" 
                :class="[style.selectDropdownInput, !keywords ? style.selectDropdownInputEmpty : '']" 
                v-model="keywords" @click.stop="onClickInputDropDown" v-focus>

            <ul :class="[style.selectDropdownList]" @click.stop="onClickDropDown">
                <li v-for="item in visibleItems" :key="item.Value" :value="item.Value" 
                    :class="[item.Disabled ? style.selectDropdownListItemDisabled : style.selectDropdownListItemEnabled]">

                    <div v-if="item.Disabled" :class="[style.selectDropdownListItemText]">{{ item.Text }}</div>
                    <checkbox v-else :class="[style.selectDropdownListItemCheckbox]" v-model="selectedItems" :value="item">
                        <span>{{ item.Text }}</span>
                    </checkbox>
                </li>
            </ul>

            <div :class="style.selectorBoxWrapper">
                <div :class="style.selectorBox">
                    <div :class="style.selectorItem" @click.stop="onSelectAll()">Выбрать все</div>
                    <div :class="style.selectorItem" @click.stop="onUnselectAll()">Очистить выбор</div>
                </div>
            </div>
        </div>

        <div :class="[style.selectText]">
            <MultiSelectTags v-if="showTags" :tags="selectedItems" @remove="onTagRemove"
                :maxVisibleSelectedItems="maxVisibleSelectedItems" :showAllTags.sync="showAllTags" />

            <div :class="[style.selectTextPlaceHolder]" v-show="!showTags">{{placeholderClosed ? placeholderClosed : placeholder}}</div>
        </div>
	</div>
</template>

<script>
    import style from './style.module.scss'
    
    import tooltip from 'EXT/otc2.0/components/common/tooltip/tooltip'
    import checkbox from 'EXT/otc2.0/components/controls/checkbox/checkbox'
    import MultiSelectTags from './multi-select-tags/multi-select-tags'

    import { checkIsArraysEquals } from 'EXT/utils/arrayHelper'

	export default {
        name: 'multi-select-control',

		components: {
            tooltip,
            checkbox,
            MultiSelectTags
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
            selectedKeysString: {
				type: String,
				default: ''
            },
            maxVisibleSelectedItems: {
				type: Number,
				default: 5
            },
		},
		data() {
			return {
                style: style,
                open: false,
                keywords: '',
                selectedItems: [], 
                showAllTags: false
			}
        },
        created() {
            document.addEventListener('click', this.onClickOutside);
            document.addEventListener('keyup', this.onKeyUpGlobal);
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
            onTagRemove: function(key) {
                if (!this.selectedItems) return;

                var filtered = this.selectedItems.filter(item => item.Value != key);

                this.selectedItems = filtered;
            },
            onSelectAll: function() {
                if (!this.selectedItems) {
                    this.selectedItems = [];
                }

                var filtered = this.visibleItems.filter(item => !item.Disabled);

                this.selectedItems = [... new Set([...this.selectedItems, ...filtered])];
            },
            onUnselectAll: function() {
                if (!this.selectedItems) return;

                var component = this;

                var filtered = this.selectedItems.filter( function(el) {
                    return component.visibleItems.indexOf(el) < 0;
                } );

                this.selectedItems = filtered;
            },
            onSelectAll: function() {
                var filtered = this.items.filter(item => !item.Disabled);
                this.selectedItems = filtered;
            },
            onUnselectAll: function() {
                this.selectedItems = [];
            },
            initSelectedItems: function() {
                if (!this.items || this.items.length === 0 || !this.selectedKeysString) {
                    this.selectedItems = [];
                    return;
                }

                var selectedKeys = JSON.parse(this.selectedKeysString);

                var component = this;

                var selectedItems = selectedKeys ? selectedKeys.reduce(
                    function (acc, key) {

                        var filtered = component.items.filter(item => item.Value == key);
                        if (filtered && filtered.length > 0) {
                            acc.push(filtered[0]);
                        }
                        return acc;
                    },
                    []
                ) : [];

                var isEqual = checkIsArraysEquals(this.selectedItems, selectedItems, (a, b) => {return a.Value === b.Value});

                if (!isEqual) {
                    this.selectedItems = selectedItems;
                }
            }
        },
        computed: {
            showTags() {
                return this.selectedItems && this.selectedItems.length > 0;
            },
            visibleItems() {
                if (!this.keywords) return this.items;

                var keyword = this.keywords.trim().toLowerCase();
                if (!keyword) return this.items;

                var filtered = this.items.filter(item => item.Disabled || item.Text.toLowerCase().indexOf(keyword) !== -1);

                return filtered;
            }
        },
        watch: {
            selectedItems: function(val) {
                var keys = val ? val.reduce(
                    function (acc, obj) {
                        var intValue = parseInt(obj.Value, 10);
                        acc.push(intValue ? intValue : obj.Value);
                        return acc;
                    },
                    []
                ) : [];

                var selectedKeysString = keys && keys.length > 0 ? JSON.stringify(keys) : null;

                this.$emit('update:selectedItems', val);
                this.$emit('update:selectedKeysString', selectedKeysString);
            },
            items: function(val) {
                this.initSelectedItems();
            },
            selectedKeysString: function(val) {
                this.initSelectedItems();
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