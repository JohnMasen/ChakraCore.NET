import { echo } from 'echo.js';
import { CreateHosting } from 'Hosting.js';
export class app {
    main() {
        echo("start");
        CreateHosting("Module1", "Test").then(target => {
            for (var i = 0; i < 10; i++) {
            this.test1(target);
            }
            
        });
    }
    test1(remoteModule) {
        let para = { name: "ddd" };
        remoteModule.Call("Hello", para)
            .then(result => {
                echo("Result=" + String(result));
                });
        //remoteModule.DispatchAsync("HelloAsync", JSON.stringify(para)).then(result => {
        //    echo("AsyncResult=" + result);
            //let resultObj = undefined;
            //if (result != "") {
            //    resultObj = JSON.parse(result);
            //}
            //echo("Result=" + result);
            //echo("ResultObj=" + resultObj);
        //});
    }
}