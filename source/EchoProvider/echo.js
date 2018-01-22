var api = RequireNative('EchoProvider.EchoProvider,EchoProvider');
export function echo(message) {
    api.echo(message);
}