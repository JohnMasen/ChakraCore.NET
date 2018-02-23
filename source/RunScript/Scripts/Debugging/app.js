import { V1,F1 } from 'Module1';
import { echo } from 'sdk@Echo';
export class app {
    main() {
        let a = V1;
        let b = a;
        let c = a + b;
        let f=this.abc(c);
        let o = { a: a, b: b, c: c, d: 123 };
        let o2 = { o: o, text: "text" };
        let ff = function (a) {
            return a + a;
        };
        //eval("let cc=10;"); //eval function will cause arguments handle to 0
        let ff2=F1(1,2);
        ff(1);
        //exceptionFunction();
        echo("---Script Finish---");
    }
    abc(v){
        return v+v;
    }
}