import * as imported from "BasicExport";
export class TestClass2 {
    constructor() {
        this.a = new imported.TestClass();
    }
    Test2(v) {
        return this.a.Test1(v) + v;
    }
}