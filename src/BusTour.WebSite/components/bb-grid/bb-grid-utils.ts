import { Func } from "~/types/common";
import { IArrayGroup, IGroupable } from "./bb-grid-types";

export function *keyGenerator(): Generator<number, number, number> {
	let key = 0;

	while (true) {
		yield key++;
	}
}

export function groupArrayItemsBy(array: any[], callbackFn: Func<object, any>): IGroupable {
	let groups = array.reduce((accum: IArrayGroup[], curr) => {
		const key = callbackFn(curr);

		if (accum.find(x => x.key == key)) {
			return accum;
		}

		const items = array.filter(x => callbackFn(x) == key);

		accum.push({ key, items });

		return accum;
	}, []);

	return groups;
}