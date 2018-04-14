"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
console.log('Hello world');
var net = require('net');
var server = net.createServer(function (socket) {
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});
server.listen(4711);
//# sourceMappingURL=app.js.map