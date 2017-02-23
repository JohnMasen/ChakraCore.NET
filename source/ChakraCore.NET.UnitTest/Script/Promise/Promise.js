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

function SimplePromise1() {
    return createPromiseStub(function (resolve, reject) {
        for (var i = 0; i < 100000; i++) {

        }
        timer.setTimeout(function () {
            test.echo(10);
            resolve(1);
        }, 1);
    });
}

function CallAsync() {

    test.asyncFunction().then(function (x) {
        result = x;
        hold = false;
    });
}


class PromiseStub
{
    constructor(promiseBody)
    {
        this.real_resolve = function () { };
        this.real_reject = function () { };
        this.body = promiseBody;
    }
    resolve_proxy(v) {
        real_resolve(v);
    }   
    reject_proxy(s) {
        real_reject(s);
    }

    create () {
        return new Promise(function (resolve, reject) {
            this.real_resolve = resolve;
            this.real_reject = reject;
            this.body(resolve_proxy, reject_proxy);
        });

    }
}
function createPromiseStub(body) {
    var tmp = new PromiseStub(body);
    return tmp.create();
}



