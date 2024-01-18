// JavaScript source code


const text = document.querySelector('input[type="text"]');
const buttons = document.querySelectorAll('input[type="button"]');
buttons.forEach(x => x.onclick = () => execute(x.value));

async function execute(v) {
    switch (v) {
        case 'C':
            text.value = '';
            break;
        case '=':
            text.value = math.evaluate(text.value);
            break;
        case 'GBP':
        case 'EUR':
        case 'USD':
            const response = await fetch(`http://api.nbp.pl/api/exchangerates/rates/a/${v}/2023-12-22/?format=json`);
            const json = await response.json();
            const rate = json.rates[0].mid;
            text.value = math.evaluate(text.value + '*' + rate);
            break;
        default:
            text.value += v;
    };
}