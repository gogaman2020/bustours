const dtfrmt = require('dateformat');

const dateRegex = /^([0-9]+)\.([0-9]+)\.([0-9]+)$/;

const minValidYear = 1970;
const maxValidYear = 3000;

const maxValidDay = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

const monthNames = [ 'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь' ];

export function monthName(date) {
  return date && date.getMonth && monthNames[date.getMonth() - 1] || null;
}

export function datePart(date) {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

export function dateFormat(date, format) {
  if (!date) return '';
  return dtfrmt(date, format);
}

export function dateParse(dateString) {
  const invalidResult = {
    dateValue: null,
    isValid: false
  };

  function validResult(dateValue) {
    return {
      dateValue,
      isValid: true
    };
  }

  if (!dateString) {
    return validResult(null);
  }

  if (!dateString.match) {
    return invalidResult;
  }

  const match = dateString
    .trim()
    .replace(/\s*\.\s*/g, '.')
    .match(dateRegex);

  if (!match) {
    return invalidResult;
  }

  const year = parseInt(match[3]);

  if (year < minValidYear || year > maxValidYear)
    return invalidResult;

  const month = parseInt(match[2]) - 1;

  if (month < 0 || month > 11)
    return invalidResult;

  const incMaxValidDay = month == 1 && ((year % 400) == 0 || ((year % 4) == 0 && (year % 100) != 0)) && 1 || 0;

  const day = parseInt(match[1]);

  if (day < 1 || day > maxValidDay[month] + incMaxValidDay)
    return invalidResult;

  return validResult(new Date(year, month, day));
}

export function addDays(date, days) {
  const result = new Date(date);
  if (days !== 0)
    result.setDate(date.getDate() + days);

  return result;
}

export function getCalendarDays(date) {
  const currentMonth = date.getMonth();
  const monthStart = new Date(date.getFullYear(), date.getMonth(), 1);
  const monthStartDay = (monthStart.getDay() + 6) % 7;
  const today = datePart(new Date());

  // первый день календарика
  const startDate = addDays(monthStart, -monthStartDay);

  const dates = [];

  do {
    const week = Array(7).fill().map((x, index) => addDays(startDate, index));

    startDate.setDate(startDate.getDate() + 7);

    dates.splice(dates.length, 0, ... week);
  }
  while (startDate.getMonth() == currentMonth);

  const days = dates.map(date => new Object({
    date: date,
    isInMonth: date.getMonth() == currentMonth,
    today: date.getTime() == today.getTime()
  }));

  return days;
}

export function pad(val, len) {
  val = String(val);
  len = len || 2;
  while (val.length < len) {
    val = '0' + val;
  }
  return val;
}

export function parseJsonUtcDate(dateStr) {
  if (!(typeof dateStr === 'string' || dateStr instanceof String)) {
    return null;
  }

  const date = new Date(dateStr.endsWith('Z') ? dateStr : dateStr + 'Z');

  return !isNaN(date) && date || null;
}
