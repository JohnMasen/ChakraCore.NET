# ChakraCore.NET
A dotnet hosting library for chakracore (javascript) engine to provide 
scripting capability to dotnet applications. The primary target is make it easy to learn and use.
## Platform
This library is build with NetStandard 1.4 and Chakracore 1.4.1 

Support following platform:
* .NET Framework 4.6.1
* UWP (windows 10 , Windows 10 IoT , Windows 10 Mobile)
* Net Core 1.0
* Mono/Xamarin

As DotnetCore and Chakracore 1.4.1 supports cross platform, idealy you can use this library at any OS which supported by these 2 libraries.
This library is created and tested on windows 10 platform. 
If you're using it on other platform than Windows 10, feel free to send me a feed back.

## nuget:
https://www.nuget.org/packages/ChakraCore.NET/


## Key features
### Read/Write javascript values
### Call javascript function from dotnet, support callback to dotnet
### Expose dotnet function to javascript, support callback to javascript
### Flexible value converter system
* Transfer dotnet class instance to a proxy object in javascript
* Transfer dotnet structure to a javascript value
* Build-in value converters support String,int,double,single,bool,byte,decimal,Guid

### Support ArrayBuffer,TypedArray,DataView
### Support Task <-> Promise convert
### Support "require" feature

### Sample

Samples are still under development, suggest read the unit test code for a start by now.
here's some simple code which demostrates the basic usage of the library

Setup runtime and context, then run a script
```   
    ChakraRuntime runtime=ChakraRuntime.Create();
    ChakraContext context=runtime.CreateContext(true);
    context.RunScript("var a=1;");
```
write 1 to variable a
```
    context.GlobalObject.WriteProperty<int>("a", 1); //js: var a=1;
```
read variable a
```
    int value=context.GlobalObject.ReadProperty<T>("a"); //js: return a;
```
call js function
```
    context.RunScript("function add(v){return v+1}");  //run the script to create function add
    var b=context.GlobalObject.CallFunction<int, int>("add", 1); //js: return add([value]);
```
expose function 
```
    public int Add(int value)
    {
        return value + 1;
    }
    context.GlobalObject.Binding.SetFunction<int,int>("add", Add); //js: function add(v){[native code]}

```

call js function with callback
```
    public void echo(string s)
    {
        Debug.WriteLine(s);
    }
    context.RunScript("function test(callback){callback('hello world')})");  //init js function
    context.ServiceNode.GetService<IJSValueConverterService>().RegisterMethodConverter<string>();  //regiser callback method type
    context.GlobalObject.CallMethod<Action<string>>("test", echo);   //js: test([native function]);
```
map a dotnet object instance to js
```
    public class DebugEcho
    {
        public void Echo(String s)
        {
            Debug.WriteLine(s);
        }
    }

    context.ServiceNode.GetService<IJSValueConverterService>().RegisterProxyConverter<DebugEcho>( //register the object converter
        (binding, instance, serviceNode) =>
        {
            binding.SetMethod<string>("echo",instance.Echo); //js: [object].echo=function(s){[native code]}
        });
    DebugEcho obj = new DebugEcho();
    context.GlobalObject.WriteProperty<DebugEcho>("debugEcho", obj); //js: var debugEcho=[native object]

```
using "require"
```
c#	
	JSRequireLoader.EnableRequire(context,"Script\\Require"); //enable require, set root folder to "Script\Require"
	runScript("RequestTest"); 

RequestTest.js

	var p = require("TestLib");
	var output = p.t1("abc");

TestLib.js

	function t1(source) {
		return source + source;
	}
	exports.t1 = t1;

```