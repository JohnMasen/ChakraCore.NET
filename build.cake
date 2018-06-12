var target = Argument("target", "Default");

Task("Default")
  .Does(() =>
{
    MSBuild("./source/ChakraCore.NET.sln");
});

RunTarget(target);