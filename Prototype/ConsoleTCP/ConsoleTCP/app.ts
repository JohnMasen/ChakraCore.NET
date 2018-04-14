import { Socket } from "net";
import { Server } from "http";

console.log('Hello world');
let net = require('net');
const server:Server = net.createServer((socket) => {
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});

server.listen(4711);