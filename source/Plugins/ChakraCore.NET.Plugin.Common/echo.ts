declare function RequireNative(typeName:string);

let api = RequireNative('instance@Echo');
export function echo(message:string) {
    api.echo(message);
}