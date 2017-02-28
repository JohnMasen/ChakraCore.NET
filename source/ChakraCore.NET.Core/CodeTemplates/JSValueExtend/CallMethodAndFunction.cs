
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
namespace ChakraCore.NET
{
public partial class JSValue
{
        public void CallMethod(string name )
        {
            converter.RegisterMethodConverter();
            var a = valueService.ReadProperty<Action>(ReferenceValue,name);
            a();
        }

        public TResult CallFunction<TResult>(string name , bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<TResult>();
            var a = valueService.ReadProperty<Func<bool ,TResult>>(ReferenceValue,name);
            return a(isConstructCall);
        }



        public void CallMethod<T1>(string name ,T1 para1)
        {
            converter.RegisterMethodConverter<T1>();
            var a = valueService.ReadProperty<Action<T1>>(ReferenceValue,name);
            a(para1);
        }

        public TResult CallFunction<T1,TResult>(string name ,T1 para1, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1);
        }



        public void CallMethod<T1,T2>(string name ,T1 para1,T2 para2)
        {
            converter.RegisterMethodConverter<T1,T2>();
            var a = valueService.ReadProperty<Action<T1,T2>>(ReferenceValue,name);
            a(para1,para2);
        }

        public TResult CallFunction<T1,T2,TResult>(string name ,T1 para1,T2 para2, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2);
        }



        public void CallMethod<T1,T2,T3>(string name ,T1 para1,T2 para2,T3 para3)
        {
            converter.RegisterMethodConverter<T1,T2,T3>();
            var a = valueService.ReadProperty<Action<T1,T2,T3>>(ReferenceValue,name);
            a(para1,para2,para3);
        }

        public TResult CallFunction<T1,T2,T3,TResult>(string name ,T1 para1,T2 para2,T3 para3, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,T3,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,T3,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2,para3);
        }



        public void CallMethod<T1,T2,T3,T4>(string name ,T1 para1,T2 para2,T3 para3,T4 para4)
        {
            converter.RegisterMethodConverter<T1,T2,T3,T4>();
            var a = valueService.ReadProperty<Action<T1,T2,T3,T4>>(ReferenceValue,name);
            a(para1,para2,para3,para4);
        }

        public TResult CallFunction<T1,T2,T3,T4,TResult>(string name ,T1 para1,T2 para2,T3 para3,T4 para4, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,T3,T4,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,T3,T4,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2,para3,para4);
        }



        public void CallMethod<T1,T2,T3,T4,T5>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5)
        {
            converter.RegisterMethodConverter<T1,T2,T3,T4,T5>();
            var a = valueService.ReadProperty<Action<T1,T2,T3,T4,T5>>(ReferenceValue,name);
            a(para1,para2,para3,para4,para5);
        }

        public TResult CallFunction<T1,T2,T3,T4,T5,TResult>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,T3,T4,T5,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,T3,T4,T5,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2,para3,para4,para5);
        }



        public void CallMethod<T1,T2,T3,T4,T5,T6>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6)
        {
            converter.RegisterMethodConverter<T1,T2,T3,T4,T5,T6>();
            var a = valueService.ReadProperty<Action<T1,T2,T3,T4,T5,T6>>(ReferenceValue,name);
            a(para1,para2,para3,para4,para5,para6);
        }

        public TResult CallFunction<T1,T2,T3,T4,T5,T6,TResult>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,T3,T4,T5,T6,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2,para3,para4,para5,para6);
        }



        public void CallMethod<T1,T2,T3,T4,T5,T6,T7>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7)
        {
            converter.RegisterMethodConverter<T1,T2,T3,T4,T5,T6,T7>();
            var a = valueService.ReadProperty<Action<T1,T2,T3,T4,T5,T6,T7>>(ReferenceValue,name);
            a(para1,para2,para3,para4,para5,para6,para7);
        }

        public TResult CallFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7, bool isConstructCall=false)
        {
            converter.RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>();
            var a = valueService.ReadProperty<Func<bool ,T1,T2,T3,T4,T5,T6,T7,TResult>>(ReferenceValue,name);
            return a(isConstructCall,para1,para2,para3,para4,para5,para6,para7);
        }




    }
}
