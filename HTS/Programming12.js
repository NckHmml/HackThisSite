function isPrime(x) {
    if (x == 1) return false;
    if (x == 2) return true;

    for (var i = 2; i < x; i++)
        if (x % i == 0) return false;
    return true;
}

var elements = document.getElementsByTagName('input');
var solution = '';
var value = '';
for (var i = 0; i < elements.length; i++) {
    var el = elements[i];
    if (el.getAttribute('value') != null && el.getAttribute('value').length == 640) {
        value = el.getAttribute('value');
    }

    if (el.getAttribute('name') == 'solution') {
        solution = el;
    }
}

var result = '';
var primeSum = 0;
var sum = 0;
var count = 0;
for (var i = 0; i < value.length; i++) {
    var x = parseInt(value[i]);
    if (!isNaN(x)) {
        if (x <= 1) continue;
        if (isPrime(x))
            primeSum += x;
        else
            sum += x;
    } else {
        if (count++ >= 25) continue;
        result += String.fromCharCode(value.charCodeAt(i) + 1)
    }
}

result += sum * primeSum;

solution.value = result;