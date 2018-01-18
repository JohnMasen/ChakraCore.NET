import { echo } from 'echo';
import * as info from 'info.js';
import { CreateHosting } from 'Hosting.js';
export class app {
    main() {
        echo("[" + info.GetCurrentThread()+"]"+"start");
        CreateHosting("Module1", "Test").then(target => {
            for (let i = 0; i < 10; i++) {
            this.test1(target,i);
            }
            
        });
    }
    test1(remoteModule,value) {
        remoteModule.Call("Hello", value,value+1)
            .then(result => {
                echo("[" + info.GetCurrentThread() + "]" +"Result=" + JSON.stringify(result));
                });
    }
}