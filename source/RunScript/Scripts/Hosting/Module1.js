import { echo } from 'echo.js';
class d {
    Dispatch(name, para) {
        let result;
        let args = JSON.parse(para);
        result = this[name].apply(this, args);
        if (result) {
            return JSON.stringify(result);
        }
        else {
            return "null";
        }
    }
}
export class Test extends d {
    Hello(value1,value2) {
        echo("Hello run hosted, value1="+String(value1)+",value2="+String(value2));
        return {
            tag1: "This is object result",
            message: "This is message",
            value: value1 + value2
        };
    }
}