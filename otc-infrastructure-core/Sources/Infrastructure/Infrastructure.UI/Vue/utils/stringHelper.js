function getCorrect(number, ending1, ending2, ending5)
{
    if (!ending1) ending1 = '';
    if (!ending2) ending2 = '';
    if (!ending5) ending5 = '';

    if (!number) number = 0;

    if (number === 0)
        return ending5;

    number = number % 100;
    if (number >= 11 && number <= 19)
        return ending5;

    var i = number % 10;
    switch (i)
    {
        case 1:
            return ending1;
        case 2:
        case 3:
        case 4:
            return ending2;
        default:
            return ending5;
    }
}

function declOfNum(number, titles)
{  
    const cases = [2, 0, 1, 1, 1, 2];  
    return titles[ (number%100>4 && number%100<20)? 2 : cases[(number%10<5)?number%10:5] ];  
}

function formatAmount(amount, decimalCount = 2, decimal = ".", thousands = " ") {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;

        return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
    }
}

export { getCorrect, declOfNum, formatAmount }