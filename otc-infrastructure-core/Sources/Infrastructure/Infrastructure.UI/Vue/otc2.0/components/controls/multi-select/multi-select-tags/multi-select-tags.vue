<template>
    <div :class="style.box">
        <div :class="style.tagBox">
            <MultiSelectTagItem v-for="tag in visibleTags" :key="tag.Value" :value="tag.Text" 
                @remove="$emit('remove', tag.Value)" :class="[style.tag]"/>

            <div v-if="showMoreItems" :class="style.showSelector" @click.stop="onChangeShowAllItems(true)">{{showMoreItemsString}}</div>
            <div v-if="showOnlyFirstItems" :class="style.showSelector" @click.stop="onChangeShowAllItems(false)">{{showOnlyFirstItemsString}}</div>
        </div>
    </div>
</template>

<script>
    import style from './style.module.scss'

    import { getCorrect } from 'EXT/utils/stringHelper'
    
    import MultiSelectTagItem from '../multi-select-tag-item/multi-select-tag-item';

    export default {
        name: "multi-select-tags",

        components: { 
            MultiSelectTagItem 
        },
        props: {
            tags: Array,
            maxVisibleSelectedItems: {
				type: Number,
				default: 5
            },
            showAllTags: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                style: style,
                showAllTagsValue: false
            }
        },
        computed: {
            visibleTags() {
                if (this.showAllTagsValue || this.maxVisibleSelectedItems == 0 || !this.tags || this.tags.length <= this.maxVisibleSelectedItems) {
                    return this.tags;
                }
                return this.tags.slice(0, this.maxVisibleSelectedItems);
            },
            showMoreItemsString() {
                if (!this.showMoreItems) return '';

                var moreItemsCount = this.tags.length - this.maxVisibleSelectedItems;

                return 'Показать ещё ' + moreItemsCount + getCorrect(moreItemsCount, ' запись', ' записи', ' записей');
			},
            showOnlyFirstItemsString() {
                if (!this.showOnlyFirstItems) return '';

                return 'Показать только ' + getCorrect(this.maxVisibleSelectedItems, ' первую ', ' первые ', ' первые ') +
                    this.maxVisibleSelectedItems + getCorrect(this.maxVisibleSelectedItems, ' запись', ' записи', ' записей');
            },
            showMoreItems() {
                if (this.maxVisibleSelectedItems == 0 || !this.tags || this.tags.length <= this.maxVisibleSelectedItems) {
                    return false;
                }

                return !this.showAllTagsValue;
            },
            showOnlyFirstItems() {
                if (this.maxVisibleSelectedItems == 0 || !this.tags || this.tags.length <= this.maxVisibleSelectedItems) {
                    return false;
                }

                return this.showAllTagsValue;
            }
        },
        watch: {
            showAllTags () {
                this.showAllTagsValue = this.showAllTags;
            }
        },
        methods: {
            onChangeShowAllItems: function(newValue) {
                this.showAllTagsValue = newValue;
                this.$emit('update:showAllTags', newValue);
            }
        },
        mounted() {
            this.showAllTagsValue = this.showAllTags;
        }
    }
</script>
