function SimplePromise() {
    return new Promise(function (resolve, reject) {
        timer.setTimeout(function () {
            test.echo(1);
            resolve(1);
        },2000);
    });
}


function T1() {
    SimplePromise().then(function (value) {
        //do nothing
        test.echo(value);
    });
    //timer.setTimeout(function ()    {
    //    test.echo(2);
    //}, 1000);
    
}

