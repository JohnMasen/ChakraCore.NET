var api = RequireNative('Hosting');
export function CreateHosting(moduleName, className) {
    return api.CreateHosting(moduleName, className)
        .then(result => {
            return new RemoteProxy(result);
        });
}

export class RemoteProxy {
    constructor(value) {
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