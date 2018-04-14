"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Net = require("net");
let s;
let isConnected = false;
const server = Net.createServer((socket) => {
    isConnected = true;
    if (s) {
        socket.write(s);
    }
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});
server.listen(1234);
//# sourceMappingURL=DebugAdapter.js.map