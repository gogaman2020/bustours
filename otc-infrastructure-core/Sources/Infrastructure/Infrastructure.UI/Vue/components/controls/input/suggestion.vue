<template>
    <ul v-if="list.length > 0" :class="style.suggestion">
        <li v-for="(item, index) in list" @click="select(item)" :class="style.suggestionListItem" :key="index">
            <slot :item="item"></slot>
        </li>
    </ul>
</template>

<script>
    import axios from 'axios';
    import style from './style.module.scss'

    export default {
        name: 'suggestion',
        props: {
            query: String,
            url: String,
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
                style: style
            }
        },
        methods: {
            get() {
                if(!this.query) return;

                axios.get(`${this.url}?q=${encodeURI(this.query)}`, { withCredentials: true })
                .then((response) => {
                    if(response.data) {
                        this.list = response.data;
                    }
                })
                .catch(message => {
					console.log(message)
                })
            },
            select(data) {
                this.$emit('select', data)
                this.list = [];
            }
        },
        computed: {
            getDebounce() {return _.debounce(this.get, this.delay);},
        },
        watch: {
            query(val) {
                if(!val) {
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