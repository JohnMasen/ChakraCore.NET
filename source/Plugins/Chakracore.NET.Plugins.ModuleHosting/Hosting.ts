declare function RequireNative(typeName: string);
let api = RequireNative('Hosting');

interface IDispatch {
    Dispatch(name: string, parameters: string);
}

export function CreateHosting(moduleName, className) {
    return api.CreateHosting(moduleName, className)
        .then(result => {
            return new RemoteProxy(result);
        });
}

export class RemoteProxy {
    proxy: IDispatch;
    constructor(value:IDispatch) {
        this.proxy = value;
    }
    Call(name, ...args) {
        let para = JSON.stringify(args);
        return this.proxy.Dispatch(name, para)
            .then(result => {
                return JSON.parse(result);
            }
            );
    }
}