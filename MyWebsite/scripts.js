// JavaScript source code


const text = document.querySelector('input[type="text"]');
const buttons = document.querySelectorAll('input[type="button"]');
buttons.forEach(x => x.onclick = () => execute(x.value));

const incorrect = 'incorrect expression';

async function execute(v) {
    if (text.value === incorrect) {
        text.value = '';
    }

    switch (v) {
        case 'C':
            text.value = '';
            break;
        case '=':
            text.value = tryEvaluate(text.value);
            break;
        case 'GBP':
        case 'EUR':
        case 'USD':
            const response = await fetch(`http://api.nbp.pl/api/exchangerates/rates/a/${v}/2023-12-22/?format=json`);
            const json = await response.json();
            const rate = json.rates[0].mid;
            text.value = tryEvaluate(text.value + '*' + rate);
            break;
        default:
            text.value += v;
    };
}

function tryEvaluate(expession) {
    let result;
    try {
        result = math.evaluate(expession);
    } catch (error) {
        result = incorrect;
    }
    return result;
}