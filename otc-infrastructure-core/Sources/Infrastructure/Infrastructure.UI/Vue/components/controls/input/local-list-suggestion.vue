<template>
    <ul v-if="list.length > 0" v-show="showSuggestions" :class="style.suggestion">
        <li v-for="(item, index) in list" @click="select(item)" :class="style.suggestionListItem" :key="index">
            <slot :item="item"></slot>
        </li>
    </ul>
</template>

<script>
    import style from './style.module.scss'

    export default {
        name: 'localListSuggestion',
        props: {
            query: String,
            items: Array,
            filterByStartsWith: Boolean,
            delay: {
                type: Number,
                default: 300,
                required: false
            },
            charsToStartFinding: {
                type: Number,
                default: 1,
                required: false
            }
        },
        data() {
            return {
                list: [],
                style: style,
            }
        },
        methods: {
            get() {
                if (!this.query) return;
                if (this.filterByStartsWith) {
                    this.list = this.items.filter(x => x.startsWith(this.query));
                }
                else {
                    this.list = this.items.filter(x => x.includes(this.query));
                }
            },
            select(data) {
                this.$emit('select', data);
                this.list = [];
            }
        },
        computed: {
            getDebounce() { return _.debounce(this.get, this.delay); },

            showSuggestions() {
                if (!this.query || !this.items || this.items.length < 1) {
                    return true
                }

                return !this.items.some(x => x === this.query);
            }
        },
        watch: {
            query(val) {
                if (!val) {
                    this.list = [];
                    return;
                }

                if (val.length < this.charsToStartFinding) {
                    this.list = [];
                    return;
                }

                this.getDebounce();
            }
        }
    }
</script>