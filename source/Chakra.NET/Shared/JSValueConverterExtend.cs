using ChakraHost.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public class JSValueConverterExtend
    {
        
        private static readonly DateTime jsStart = new DateTime(1970, 1, 1);
        //private static JSValueWithContext dateConstructor;
        public static void Inject(ChakraContext context)
        {
            context.ValueConverter.RegisterConverter<DateTime>(
                (value, helper) =>
                {
                    var diff = value - jsStart;

                    var newDate = context.GlobalObjectWithContext.CallFunction<JavaScriptValue>("Date");
                    newDate.WithContext(context).CallMethod<double>("setTime", diff.TotalMilliseconds);
                    return newDate;
                },
                (value, helper) =>
                {
                    var s = value.WithContext(context).GetValue<string>();
                    var dateString = string.Join(" ", pickDatePart(s));
                    return DateTime.Parse(dateString);
                });

            #region another interesting implementation
            //        private const string DATE_SCRIPT = "(function (){var a=new Date();a.setTime({0});return a;})()";
            //    context.ValueConverter.RegisterConverter<DateTime>(
            //        (value, helper) =>
            //            {
            //                var diff = value - jsStart;
            //                return context.RunScript<JavaScriptValue>(string.Format(DATE_SCRIPT, diff.TotalMilliseconds));
            //            },
            //            (value, helper) =>
            //            {
            //                var s = value.WithContext(context).GetValue<string>();
            //var dateString = string.Join(" ", pickDatePart(s));
            //                return DateTime.Parse(dateString);     
            //            });
            #endregion

        }

        private static IEnumerable<string> pickDatePart(string dateString)
        {
            StringBuilder builder = new StringBuilder();
            var tmp = dateString.Split(' ');
            for (int i = 1; i < 5; i++)
            {
                yield return tmp[i];
            }
        }
    }
}
