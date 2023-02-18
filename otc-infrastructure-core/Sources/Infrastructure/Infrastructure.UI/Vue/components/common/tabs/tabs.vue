<template>
    <div :class="style.tabs">
        <template v-for="(item) in tabs">
            <div :value="item.Key" :key="item.Key" v-if="item.IsVisible" :class="[style.tab, isActive(item.Key) ? style.active:'' ]" @click="setActive(item.Key)">
                <a aria-expanded="true" data-toggle="tab">
                    {{ item.Text }}
                </a>
            </div>
        </template>
    </div>
</template>

<script>
	import style from './style.module.scss'

	export default {
		name: 'tabs',
		components: {

		},
		props: {
            tabs: {
                type: Array,
                required: true
            },
            selectedTab: {
                required: true
            }
        },
		data() {
			return {
				style: style
			}
		},
        methods: {
            isActive: function (key) {
                return this.selectedTab === key;
            },
            setActive: function (key) {
                this.$emit("setActiveTab", key);
            }
        }
	}
</script>