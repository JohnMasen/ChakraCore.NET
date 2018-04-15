import * as Net from 'net';
const port:number=Number(process.argv[2]);
const server = Net.createServer((socket) => {
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});

server.listen(port);