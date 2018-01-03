__Sys.Echo("start");
var a = 1;
var c = {};
var m = __Sys.Import("passmodule.js");
//import("passmodule.js").then((m) => { Loader.Echo("imported"); });
//var a = 1;
//import 'passmodule.js';
__Sys.Echo("a=" + a);
__Sys.Echo("c.foo="+c.foo(a));