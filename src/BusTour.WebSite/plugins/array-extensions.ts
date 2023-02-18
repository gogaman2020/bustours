import { Predicate } from "~/types/common";

const _baseDescriptor: PropertyDescriptor = {
	configurable: true,
	enumerable: false,
	writable: true,
}

Object.defineProperties(Array.prototype, {
	"findOrDefault": {
		value(callback: Predicate, defaultValue: any = 0) { 
			const array = this;
			let found;
			
			for (let index = 0; index < array.length; index++) {
				const element = array[index];
				const result = callback(element, index, array);

				if (result) {
					found = element;
					break;
				}
			};

			return found ?? defaultValue;
		},
		..._baseDescriptor,
	},
});