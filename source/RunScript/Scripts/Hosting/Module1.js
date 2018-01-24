import { echo } from 'sdk@Echo';
import * as info from 'sdk@SysInfo';

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