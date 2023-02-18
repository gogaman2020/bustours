export function checkIsArraysEquals(arrayA, arrayB, comparer) {
    if (arrayA.length !== arrayB.length) return false;

    var aSubB = arrayA.every(a => arrayB.some(b => comparer(a, b)));

    return aSubB;
}