declare function RequireNative(typeName:string);

let api = RequireNative('ChakraCore.NET.Plugin.Common.EchoProvider,ChakraCore.NET.Plugin.Common');
export function echo(message:string) {
    api.echo(message);
}