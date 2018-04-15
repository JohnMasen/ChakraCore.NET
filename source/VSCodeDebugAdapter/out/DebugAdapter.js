"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Net = require("net");
const port = Number(process.argv[2]);
const server = Net.createServer((socket) => {
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});
server.listen(port);
//# sourceMappingURL=DebugAdapter.js.map