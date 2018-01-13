import { s } from 'world.js';
import { echo } from 'echo.js';
RequireNative('ImageSharpProvider');

export class app {
    main() {
        echo("[Module] hello "+s);
    }
}
