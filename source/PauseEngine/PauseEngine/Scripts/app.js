import { echo } from 'sdk@Echo';
export class app {
    loop() {
        let count = 1;
        while (true) {
            
            delay(1000);
            echo("hello " + count);
            count++;
        }
    }
}
function delay(ms) {
    let last = (new Date()).getTime();
    while (true) {
        let now = (new Date()).getTime();
        if (now-last>=ms) {
            return;
        }
    }
}