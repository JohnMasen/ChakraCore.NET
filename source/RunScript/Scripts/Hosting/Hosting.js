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
    Call(name, para) {
        return this.proxy.Dispatch(name, JSON.stringify(para))
            .then(result => {
                if (result != "") {
                    return JSON.parse(result);
                }
                else {
                    return undefined;
                }
            });
    }
}