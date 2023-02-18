<template>
    <div :class="[style.pagination, 'row']" v-if="totalPages > 1">
        <div :class="[style.paginationPageNumbers, 'col-md-12', 'col-sm-12']">
            <div :class="[style.paginationPageNumbersWhite]" @click.prevent="moveTo(currentPage - 1)" v-if="prevEnabled">&lt;</div>
            <div :class="[currentPage == 1 ? style.paginationPageNumbersDisabled : style.paginationPageNumbersGray]" @click.prevent="moveTo(1)">1</div>
            <div v-if="pagesToShow[0] > 2"> ... </div>
            <div :class="[currentPage == pageNo ? style.paginationPageNumbersDisabled : style.paginationPageNumbersGray]" v-for="(pageNo, index) in pagesToShow" @click.prevent="moveTo(pageNo)" :key="index">{{pageNo}}</div>
            <div v-if="pagesToShow.slice(-1)[0] < totalPages - 1"> ... </div>
            <div :class="[currentPage == totalPages ? style.paginationPageNumbersDisabled : style.paginationPageNumbersGray]" @click.prevent="moveTo(totalPages)">{{totalPages}}</div>
            <div :class="[style.paginationPageNumbersWhite]" @click.prevent="moveTo(currentPage + 1)" v-if="nextEnabled">&gt;</div>
        </div>
    </div>
</template>

<script>
    import 'idempotent-babel-polyfill'

    import style from './pager.module.scss'

    const MAX_PAGES_TO_SHOW = 7;

    export default {
        data: function() {
            return {
                currentPage: 1,
                totalPages: 1,
                maxPagesToShow: MAX_PAGES_TO_SHOW,
                style: style
            };
        },
        computed: {
            prevEnabled: function() {
                return this.currentPage > 1
            },
            nextEnabled: function() {
                return this.currentPage < this.totalPages;
            },
            pagesToShow: function() {
                var startIndex = this.currentPage - Math.ceil((MAX_PAGES_TO_SHOW - 2) / 2);
                if (startIndex < 2) {
                    startIndex = 2;
                }

                var endIndex = this.currentPage + Math.ceil((MAX_PAGES_TO_SHOW - 2) / 2);
                if (endIndex > this.totalPages - 1) {
                    endIndex = this.totalPages - 1
                }

                var result = [];
                for (var i = startIndex; i <= endIndex; i++) {
                    result.push(i);
                }

                return result;
            }
        },
        methods: {
            moveTo: function(newPage) {
                if (newPage >= 1 && newPage <= this.totalPages) {
                    this.currentPage = newPage;
                    this.$emit('pageChanged', this.currentPage);
                }
            },
            setCurrentPage: function(currentPage) {
                this.currentPage = currentPage > this.totalPages ? this.totalPages : currentPage;
            },
            setTotalPages: function(totalPages) {
                if (this.currentPage > totalPages) {
                    this.currentPage = totalPages;
                }

                this.totalPages = totalPages;
            }
        }
    };
</script>