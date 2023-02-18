// Возвращает значение без единиц измерения
export function withoutUnits(value: string = ""): number {
	return parseInt(value.replaceAll(/[a-z]|%/gi, ""));
}

// Проверяет пройден ли заданный breakpoint для шага
export function isBreakpointPassed(breakpoint: string = ""): boolean {
	const displayResolution = getComputedStyle(document.body).getPropertyValue("width");

	return withoutUnits(breakpoint) >= withoutUnits(displayResolution);
}