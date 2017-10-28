var lib1 = require("TestLib");
function t2(source) {
    return "lib2" + lib1.t1(source) ;
}
exports.t2 = t2;



