import { s } from 'world.js';
import { echo } from 'sdk@Echo';
import * as info from 'sdk@SysInfo';
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
