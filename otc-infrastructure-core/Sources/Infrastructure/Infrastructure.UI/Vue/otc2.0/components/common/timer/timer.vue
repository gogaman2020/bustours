<template>
    <div v-if="timer" :class="style.timer">
        <span>Осталось: {{timerValue}}</span>
    </div>
</template>

<script>
    import style from './timer.module.scss'
	import { getCorrect, declOfNum } from 'EXT/utils/stringHelper'

	export default {
		name: 'timer',
		components: {

		},
		props: {
			date: {
				type: Date,
				default: null
			}
		},
		data() {
			return {
                style: style,
                timer: null,
                timerValue: null
			}
        },
        mounted: function(){
            this.init();
        },
        destroyed(){
            this.deinit();
        },
        watch:{
            date: function(value){
                if(value){
                    this.init();
                } else {
                    this.deinit();
                }
            }
        },
        methods: {
            init: function(){
                let self = this;
                if(self.timer){
                    return;
                }

                self.timer = setInterval(() => {
                    self.timerValue = self.getTimerValue(self.date);
                }, 1000);
            },
            deinit: function(){
                if(this.timer){
                    clearInterval(this.timer);
                    this.timer = null;
                    this.timerValue = undefined;
                }
            },
            getTimerValue: function(value){
                let now = new Date();

                if(value < now){
                    this.deinit();
                    return;
                }

                let interval = value - now;
                let date = new Date(interval);
                let result = '';

                //начинается с 1970
                let years = date.getUTCFullYear() - 1970;
                if(years > 0) {
                    result += years + getCorrect(years, ' год ', ' года ', ' лет ');
                }

                //0 - 11
                let month = date.getUTCMonth();
                if(month > 0) {
                    result += month + getCorrect(month, ' месяц ', ' месяца ', ' месяцев ');
                }

                //1 - 31
                let days = date.getUTCDate() - 1;
                if(days > 0) {
                    result += days + getCorrect(days, ' день ', ' дня ', ' дней ');
                }

                //0-23
                let hours = date.getUTCHours();
                if(hours > 0) {
                    result += hours + getCorrect(hours, ' час ', ' часа ', ' часов ');
                }

                //0-59
                let minutes = date.getUTCMinutes();
                if(minutes > 0) {
                    result += minutes + getCorrect(minutes, ' минута ', ' минуты ', ' минут ');
                }

                //0-59
                let seconds = date.getUTCSeconds();
                if(seconds > 0) {
                    result += seconds + getCorrect(seconds, ' секунда ', ' секунды ', ' секунд ');
                }

                return result.trim();
            }
        }
	}
</script>