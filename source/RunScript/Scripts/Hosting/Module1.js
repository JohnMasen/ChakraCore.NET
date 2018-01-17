import { echo } from 'echo.js';
export class Test {
    Hello() {
        echo("Hello run hosted");
        return {
            tag1: "This is object result",
            message:"This is message"
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
        echo("name=" + name);
        echo("para = " + para);
        switch (name) {
            case "Hello":
                result = this.Hello();
                echo("Hello=" + result);
                break;
            default:
        }
        if (result) {
            return JSON.stringify(result);
        }
        else {
            return "";
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