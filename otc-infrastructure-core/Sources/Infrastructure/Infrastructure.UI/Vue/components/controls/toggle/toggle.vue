<template>
    <div :class="style.toggleBlock">
        <div v-if="leftLabel" :class="[style.toggleLabel, state ? '' : style.toggleLabelactive]">{{leftLabel}}</div>
        <div :class="[style.toggle,this.stateClass]"
             @click.self="onClick">
            <div :class="style.draggable"
                 @mousedown.prevent="dragStart"
                 :style="togglestyle">
            </div>
        </div>
        <div v-if="rightLabel" :class="[style.toggleLabel, state ? style.toggleLabelactive : '']">{{rightLabel}}</div>
    </div>
</template>
<script>
    import style from './style.module.scss'
    export default {
        name: 'toggle',

        props: {
            value: {
                type: Boolean,
                default: false
            },
            leftLabel: {
                type: String
            },
            rightLabel: {
                type: String
            }
        },

        data() {
            return {
                style: style,
                width: 100,
                state: false,
                pressed: 0,
                position: 0
            }
        },
        mounted() {
            this.toggle(this.value)
        },
        computed: {
            togglestyle() {
                return {
                    transform: `translateX(${this.posPercentage})`
                }
            },
            posPercentage() {
                return `${this.position / this.width * 100}%`
            },
            stateClass() {
                if (this.state) {
                    return this.style.toggleactive;
                }
            }
        },
        watch: {
            position() {
                this.state = this.position >= 50
            }
        },
        methods: {
            onClick() {
                this.toggle(!this.state)
                this.emit()
            },
            toggle(state) {
                this.state = state
                this.position = !state
                    ? 0
                    : 100
            },
            dragging(e) {
                const pos = e.clientX - this.$el.offsetLeft
                const percent = pos / this.width * 100
                this.position = percent <= 0
                    ? 0
                    : percent >= 100
                        ? 100
                        : percent
            },
            dragStart(e) {
                this.startTimer()
                window.addEventListener('mousemove', this.dragging)
                window.addEventListener('mouseup', this.dragStop)
            },
            dragStop() {
                window.removeEventListener('mousemove', this.dragging)
                window.removeEventListener('mouseup', this.dragStop)
                this.resolvePosition()
                clearInterval(this.$options.interval)
                if (this.pressed < 30) {
                    this.toggle(!this.state)
                }
                this.pressed = 0
                this.emit()
            },
            startTimer() {
                this.$options.interval = setInterval(() => {
                    this.pressed++
                }, 1)
            },
            resolvePosition() {
                this.position = this.state
                    ? 100
                    : 0
            },
            emit() {
                this.$emit('input', this.state)
            }
        }
    }
</script>