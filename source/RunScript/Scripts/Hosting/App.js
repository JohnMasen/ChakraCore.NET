import { echo } from 'sdk@ChakraCore.NET.Plugin.Common.EchoProvider,ChakraCore.NET.Plugin.Common';
import * as info from 'info.js';
import { CreateHosting } from 'sdk@ChakraCore.NET.Plugin.ModuleHosting.HostingSDK,ChakraCore.NET.Plugin.ModuleHosting';
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