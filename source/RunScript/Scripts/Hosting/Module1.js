import { echo } from 'echo.js';
import * as info from 'info.js';

export class Test  {
    Hello(value1,value2) {
        echo("[" + info.GetCurrentThread() + "]" +"Hello run hosted, value1="+String(value1)+",value2="+String(value2));
        return {
            tag1: "This is object result",
            message: "This is message",
            value: value1 + value2
        };
    }
}