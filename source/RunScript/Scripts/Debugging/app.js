import { V1 } from 'Module1';
import { echo } from 'sdk@Echo';
export class app {
    main() {
        let a = "aaa";
        let b = a;
        let c = a + b;
        let f=this.abc(c);
        let o = { a: a, b: b, c: c, d: 123 };
        let o2 = { o: o, text: "text" };
        echo("---Script Finish---");
    }
    abc(v){
        return v+v;
    }
}