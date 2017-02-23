var hold = true;
var result;
function SimplePromise() {
    return new Promise(function (resolve, reject) {
        for (var i = 0; i < 100000; i++) {

        }
        timer.setTimeout(function () {
            test.echo(10);
            resolve(1);
        },1);
    });
}

function CallAsync() {

    test.asyncFunction().then(function (x) {
        result = x;
        hold = false;
    });
}




