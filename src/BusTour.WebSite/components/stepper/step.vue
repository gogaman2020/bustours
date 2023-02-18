<template>
	<div v-if="isActiveInCurrentStep">
		<slot v-if="defaultSlotVisible"></slot>
		<slot name="always"></slot>

		<div v-for="bp in breakpoints" :key="bp" :class="s.slot">
			<slot v-if="isSlotVisible(bp)" :name="`breakpoint${bp}`"></slot>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";
import { isBreakpointPassed, withoutUnits } from "./stepper-helper";

type breakpoint = string | number;

export default Vue.extend({
	props: {
		steps: [Array, Function],
		breakpoints: {
			type: Array,
			default: () => [],
			validator(breakpoints): boolean {
				if (Array.isArray(breakpoints)) {
					return breakpoints.every(el => { 
						return ["string", "number"].includes(typeof el) && el.toString().match(/[0-9]/g) != null
					});
				}

				return false;
			},
		},
	},

	data() {
		return {
			s,
		}
	},

	computed: {
		isActiveInCurrentStep(): boolean {
			const currentStep = (this.$parent as any).step;

			if (Array.isArray(this.steps)) {
				return this.steps.includes(currentStep);
			}
				
			return this.steps(currentStep);
		},
		defaultSlotVisible(): boolean {
			return !this.breakpoints.some(bp => isBreakpointPassed((bp as breakpoint).toString()));
		}
	},

	methods: {
		isSlotVisible(breakpoint: breakpoint): boolean {
			return isBreakpointPassed(breakpoint.toString()) && !this.smallerPassedBreakpointsExists(breakpoint);
		},
		smallerPassedBreakpointsExists(breakpoint: breakpoint): boolean {
			const breakpointsWithoutUnits = (this.breakpoints as breakpoint[]).map(bp => withoutUnits(bp.toString()));
			const smallerBreakpoints = breakpointsWithoutUnits.filter(bp => bp < withoutUnits(breakpoint.toString()));
			
			return smallerBreakpoints.some(bp => isBreakpointPassed(bp.toString()));
		},
	}
})
</script>