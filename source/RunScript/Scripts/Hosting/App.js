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
        remoteModule.Call("Hello", 1,2)
            .then(result => {
                echo("Result=" + JSON.stringify(result));
                });
    }
}