var text = $("td").text();

var regExp = /(([0-9]{2,3}(\D))){5,}/g;
var matches = regExp.exec(text);
var numbers = matches[0];
var sep = matches[3];

regExp = /Shift: ([-]{0,1}\d*)/;
matches = regExp.exec(text);
var shift = parseInt(matches[1]);
var result = '';
$.each(numbers.split(sep), function (i, item) {
    if (item != "") {
        result += String.fromCharCode(parseInt(item, 10) - shift);
    }
});
$("input[name='solution']").val(result);
$("form[name='submitform']").submit();