import { s } from 'world.js';
import { echo } from 'sdk@ChakraCore.NET.Plugin.Common.EchoProvider,ChakraCore.NET.Plugin.Common';
import * as info from 'info.js';
//RequireNative('ImageSharpProvider');//Test provider references to other dlls

export class app {
    main() {
        echo("[Module] hello " + s);
        echo('CommandArguments: "' + info.CommandArguments + '"');
        echo("Is64BitCPU: " + info.Is64BitProcess);
        echo("CurrentPath: " + info.CurrentPath);
        echo("CurrentThread: " + info.GetCurrentThread());
    }
}
