using Chakra.NET.API;
namespace Chakra.NET
{
    public  static partial class JSHelper
    {
        //public static JavaScriptValue SetProperty(this JavaScriptValue target, string name, JavaScriptValue value)
        //{
        //    using (context)
        //    {

        //    }
        //    target.SetProperty(JavaScriptPropertyId.FromString(name), value, true);
        //    return target;
        //}
        

        //public static JavaScriptValue GetProperty(this JavaScriptValue target, string name)
        //{
        //    return target.GetProperty(JavaScriptPropertyId.FromString(name));
        //}

        public static JSValueWithContext WithContext(this JavaScriptValue target, ChakraContext context)
        {
            return new JSValueWithContext(target, context);
        }
    }
}
