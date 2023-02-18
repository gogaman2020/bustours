<template>
	<div>
		<slot></slot>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { Func } from "~/types/common";
import { isBreakpointPassed } from "./stepper-helper";

export interface IStep {
	value: number;
	breakpoint?: number | string;
}

export default Vue.extend({
	props: {
		steps: {
			type: Array,
			default: [],
			required: true,
		},
		value: Number,
	},

	data() {
		return {
			step: this.value,
		}
	},

	computed: {
		firstStep(): number {
			return (this.steps as IStep[])[0].value;
		},
		lastStep(): number {
			return (this.steps as IStep[])[this.steps.length - 1].value;
		}
	},

	methods: {
		next(stepShift: number = 1): void {
			const newStep = this.availableStepRecursively(
				stepShift, 
				(shift: number) => Math.min(this.lastStep, this.step + shift),
				this.lastStep
			);

			this.step = newStep;
			this.$emit("input", this.step);
		},
		prev(stepShift: number = 1): void {
			const newStep = this.availableStepRecursively(
				stepShift,
				(shift: number) => Math.max(this.firstStep, this.step - shift),
				this.firstStep
			);

			this.step = newStep;
			this.$emit("input", this.step);
		},
		reset(): void {
			this.step = this.firstStep
			this.$emit("input", this.step);
		},		
		// Возвращает номер следующего шага, доступного для текущего разрешения, если такого нет - текущий шаг.
		// Параметры:
		//     stepShift 		- Текущий сдвиг шага, по умолчанию - 1 (следующий / предыдущий)
		// 	   transitionRule 	- Правило перехода шага (направление степпера - вперед / назад)
		// 	   borderlineStep	- Граничный шаг (последний в текущем направлении, т.е. первый или последний)
		availableStepRecursively(stepShift: number = 1, transitionRule: Func<number, number>, borderlineStep: number): number {
			const stepNumber = transitionRule(stepShift);
			const step = (this.steps)[stepNumber] as IStep;

			if (step.breakpoint) {
				if (isBreakpointPassed(step?.breakpoint.toString())) {
					return stepNumber;
				} else if (stepNumber == borderlineStep) {
					return 0;
				} else {
					// Если шаг не подходит под текущее разрешение - ищем следующий
					return this.availableStepRecursively(stepShift + 1, transitionRule, borderlineStep); 
				}
			}

			return stepNumber;
		},
	},

	created(): void {
		this.$parent.$on("stepper-next", this.next);
		this.$parent.$on("stepper-prev", this.prev);
		this.$parent.$on("stepper-reset", this.reset);
	},
})
</script>