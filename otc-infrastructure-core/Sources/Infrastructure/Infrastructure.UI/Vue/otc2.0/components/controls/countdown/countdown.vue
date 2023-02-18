<template>
    <startCountdown v-if="internalStartDelayMilliseconds > 0" 
                    :time="internalStartDelayMilliseconds" 
                    @countdownend="countdownend" 
                    :transform="transform">
        <template slot-scope="props">
            осталось {{ props.days }} {{ props.hours }}:{{ props.minutes }}:{{ props.seconds }}
        </template>
    </startCountdown>
</template>

<script>
    import 'idempotent-babel-polyfill'

    import countdown from '@chenfengyuan/vue-countdown';

    export default {
        props: {
            startDelayMilliseconds: {
                type: Number,
                required: true
            },
            cautionIntervalHour: {
                type: Number,
                required: false
            }
        },
        data: function () {
            return {
                internalStartDelayMilliseconds: this.startDelayMilliseconds,
            };
        },
        components: {
            startCountdown: countdown,
        },
        methods: {
            transform(props) {
                  Object.entries(props).forEach(([key, value]) => {
                      
                      const digits = key != "days" ?
                          value < 10 ? `0${value}` : value :
                          value;

                      if (key != "days") {
                          props[key] = `${digits}`;
                      }
                      else {
                          const word = this.getNumEnding(value, ["день", "дня", "дней"])
                          props[key] = `${digits} ${word}`;
                      }
                  });

                  return props;
            },
            countdownend: function () {
                if (this.internalStartDelayMilliseconds > 0) {
                    this.internalStartDelayMilliseconds = 0;
                }
            },
            /**
             * Функция возвращает окончание для множественного числа слова на основании числа и массива окончаний
             * @param  iNumber Integer Число на основе которого нужно сформировать окончание
             * @param  aEndings Array Массив слов или окончаний для чисел (1, 4, 5),
             *         например ['яблоко', 'яблока', 'яблок']
             * @return String
             */
            getNumEnding: function (iNumber, aEndings)
            {
                var sEnding, i;
                iNumber = iNumber % 100;
                if (iNumber>=11 && iNumber<=19) {
                    sEnding=aEndings[2];
                }
                else {
                    i = iNumber % 10;
                    switch (i)
                    {
                        case (1): sEnding = aEndings[0]; break;
                        case (2):
                        case (3):
                        case (4): sEnding = aEndings[1]; break;
                        default: sEnding = aEndings[2];
                    }
                }
                return sEnding;
            }
        }
    };
</script>