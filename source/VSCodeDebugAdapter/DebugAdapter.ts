import * as Net from 'net';
let s:string;
let isConnected=false;

const server = Net.createServer((socket) => {
   isConnected=true;
   if (s) {
       socket.write(s);
   }
    socket.pipe(process.stdout);
    process.stdin.pipe(socket);
});

server.listen(1234);