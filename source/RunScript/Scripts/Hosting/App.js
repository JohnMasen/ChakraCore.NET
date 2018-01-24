import { echo } from 'sdk@Echo';
import * as info from 'sdk@SysInfo';
import { CreateHosting } from 'sdk@Hosting';
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