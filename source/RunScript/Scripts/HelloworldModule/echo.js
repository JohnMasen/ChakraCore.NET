var api = RequireNative('EchoProvider');
export function echo(message) {
    api.echo(message);
}