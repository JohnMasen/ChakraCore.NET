let info = RequireNative('RunScript.SysInfoPluginInstaller,RunScript');
export let CommandArguments = info.CommandArguments;
export let Is64BitProcess = info.Is64BitProcess;
export let CurrentPath = info.CurrentPath;
export function GetCurrentThread(){
    return info.CurrentThread();
}