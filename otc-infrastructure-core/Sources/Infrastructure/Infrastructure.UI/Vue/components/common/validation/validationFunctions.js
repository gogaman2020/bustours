const validationRegexs = {
	phoneRegex:	/^[+]{1}[0-9]{1} [(]{1}[0-9]{3}[)]{1} [0-9]{3}[-]{1}[0-9]{2}[-]{1}[0-9]{2}$/,
	emailRegex: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
}

export const validateInn = function (inn, error) {
	var result = false;
	if (typeof inn === 'number') {
		inn = inn.toString();
	} else if (typeof inn !== 'string') {
		inn = '';
	}
	var checkDigit = function (inn, coefficients) {
		var n = 0;
		for (var i in coefficients) {
			n += coefficients[i] * inn[i];
		}
		return parseInt(n % 11 % 10);
	};
	switch (inn.length) {
		case 10:
			var n10 = checkDigit(inn, [2, 4, 10, 3, 5, 9, 4, 6, 8]);
			if (n10 === parseInt(inn[9])) {
				result = true;
			}
			break;
		case 12:
			var n11 = checkDigit(inn, [7, 2, 4, 10, 3, 5, 9, 4, 6, 8]);
			var n12 = checkDigit(inn, [3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8]);
			if ((n11 === parseInt(inn[10])) && (n12 === parseInt(inn[11]))) {
				result = true;
			}
			break;
		default: 
			return true
	}

	return result;
}

export const validateKpp = function(kpp, error) {
	var result = false;
	if (typeof kpp === 'number') {
		kpp = kpp.toString();
	} else if (typeof kpp !== 'string') {
		kpp = '';
	}

	// debugger;

	if (kpp.length !== 9) {
		return true;
	} else if (!/^[0-9]{4}[0-9A-Z]{2}[0-9]{3}$/.test(kpp)) {
		result = false;
	} else {
		result = true;
	}

	return result;
}

export const validateOgrn = function(ogrn, error) {
	var result = false;
	if (typeof ogrn === 'number') {
		ogrn = ogrn.toString();
	} else if (typeof ogrn !== 'string') {
		ogrn = '';
	}
	if (ogrn.length !== 13) {
		return true;
	} else {
		var n13 = parseInt((parseInt(ogrn.slice(0, -1)) % 11).toString().slice(-1));

		if (n13 === parseInt(ogrn[12])) {
			result = true;
		} else {
			result = false;
		}
	}
	return result;
}

export const validatePhone = function (phone, error) {
	if (!phone) {
		return true;
    }

	return validationRegexs.phoneRegex.test(phone);
}

export const validateEmail = function (email, error) {
	if (!email) {
		return true;
	}

	return validationRegexs.emailRegex.test(email);
}