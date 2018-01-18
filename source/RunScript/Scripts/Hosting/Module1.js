import { echo } from 'echo.js';
export class Test {
    Hello(value) {
        echo("Hello run hosted, value="+String(value));
        return {
            tag1: "This is object result",
            message: "This is message",
            value: value + 1
        };
    }
    //HelloAsync() {
    //    return new Promise((resolve, reject) => {
    //        echo("HelloAsync run hosted");
    //        resolve("HelloAsync Execution result");
    //    });
    //}
    Dispatch(name, para) {
        let result;
        let args = JSON.parse(para);
        result = this[name].apply(this,args);
        if (result) {
            return JSON.stringify(result);
        }
        else {
            return "null";
        }
    }
    //DispatchAsync(name, para) {
    //    let result;
    //    echo("name=" + name);
    //    echo("para = " + para);
    //    switch (name) {
    //        case "HelloAsync":
    //            result = this.HelloAsync();
    //            break;
    //        default:
    //    }
    //    return result;
    //}
}