using ChakraCore.NET.API;
using ChakraCore.NET.SharedMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class SharedMemoryObjectsTest : UnitTestBase
    {
        private int payloadSize = 1024;
        private byte[] payloadToJS;
        private byte[] payloadFromJS;
        private byte[] target;
        protected override void SetupContext()
        {
            payloadToJS = new byte[payloadSize];
            payloadFromJS = new byte[payloadSize];
            target = new byte[payloadSize];
            for (int i = 0; i < payloadSize; i++)
            {
                payloadToJS[i] = 1;
                target[i] = (byte)(payloadToJS[i]+ payloadToJS[i]);
            }
        }



        [TestMethod]
        public void ArrayBufferReadWrite_CreateInDotnet()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoAdd(buffer, "arrayBufferAdd");
            buffer.Dispose();
        }

        [TestMethod]
        public void ArrayBufferReadWrite_FromExternal()
        {
            var p=Marshal.AllocHGlobal(payloadSize);
            JSArrayBuffer buffer = JSArrayBuffer.CreateFromExternal(p,(ulong)payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoAdd(buffer, "arrayBufferAdd");
            buffer.Dispose();
            Marshal.FreeHGlobal(p);
        }

        [TestMethod]
        public void ArrayBufferReadWrite_InJavascript()
        {
            JSArrayBuffer buffer = JSArrayBuffer.CreateInJavascript((uint)payloadSize, (b) => 
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoAdd(buffer, "arrayBufferAdd");
            buffer.Dispose();
        }

        [TestMethod]
        public void ArrayBufferSetGet_CreateInDotnet()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoSetGet(buffer);
            buffer.Dispose();
        }

        [TestMethod]
        public void ArrayBufferSetGet_FromExternal()
        {
            var p = Marshal.AllocHGlobal(payloadSize);
            JSArrayBuffer buffer = JSArrayBuffer.CreateFromExternal(p, (ulong)payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoSetGet(buffer);
            buffer.Dispose();
            Marshal.FreeHGlobal(p);
        }

        [TestMethod]
        public void ArrayBufferSetGet_InJavascript()
        {
            JSArrayBuffer buffer = JSArrayBuffer.CreateInJavascript((uint)payloadSize, (b) =>
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoSetGet(buffer);
            buffer.Dispose();
        }
        
        [TestMethod]
        public void TypedArrayAdd_CreateInJS()
        {
            JSTypedArray array=JSTypedArray.CreateInJS(JavaScriptTypedArrayType.Int8,(uint)payloadSize, (b) =>
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoAdd(array, "typedArrayAdd");
            array.Dispose();
        }

        [TestMethod]
        public void TypedArrayAdd_CreateFromArrayBuffer()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            JSTypedArray array = JSTypedArray.CreateFromArrayBuffer(JavaScriptTypedArrayType.Int8, buffer, 0, (uint)payloadSize);
            array.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoAdd(array, "typedArrayAdd");
            array.Dispose();
            buffer.Dispose();
        }

        [TestMethod]
        public void TypedArraySetGet_CreateInJS()
        {
            JSTypedArray array = JSTypedArray.CreateInJS(JavaScriptTypedArrayType.Int8, (uint)payloadSize, (b) =>
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoSetGet(array);
            array.Dispose();
        }

        [TestMethod]
        public void TypedArraySetGet_CreateFromArrayBuffer()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            JSTypedArray array = JSTypedArray.CreateFromArrayBuffer(JavaScriptTypedArrayType.Int8, buffer, 0, (uint)payloadSize);
            array.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            smoSetGet(array);
            array.Dispose();
            buffer.Dispose();
        }

        [TestMethod]
        public void DataViewReadWrite_CreateInDotnet()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            JSDataView view = JSDataView.CreateFromArrayBuffer(buffer,0,(uint)payloadSize,null);
            smoAdd(view, "dataViewAdd");
            view.Dispose();
            buffer.Dispose();
        }

        [TestMethod]
        public void DataViewReadWrite_CreateInJS()
        {
            JSArrayBuffer buffer = JSArrayBuffer.CreateInJavascript((uint)payloadSize, null);
            JSDataView view = JSDataView.CreateFromArrayBuffer(buffer, 0, (uint)payloadSize, (b)=>
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoAdd(view, "dataViewAdd");
            view.Dispose();
            buffer.Dispose();
        }

        [TestMethod]
        public void DataViewSetGet_CreateInDotnet()
        {
            JSArrayBuffer buffer = JSArrayBuffer.Create(payloadSize);
            buffer.Buffer.WriteArray(0, payloadToJS, 0, payloadSize);
            JSDataView view = JSDataView.CreateFromArrayBuffer(buffer, 0, (uint)payloadSize, null);
            smoSetGet(view);
            view.Dispose();
            buffer.Dispose();
        }

        [TestMethod]
        public void DataViewSetGet_CreateInJS()
        {
            JSArrayBuffer buffer = JSArrayBuffer.CreateInJavascript((uint)payloadSize, null);
            JSDataView view = JSDataView.CreateFromArrayBuffer(buffer, 0, (uint)payloadSize, (b) =>
            {
                b.WriteArray(0, payloadToJS, 0, payloadSize);
            });
            smoSetGet(view);
            view.Dispose();
            buffer.Dispose();
        }


        private void smoAdd<T>(T obj,string testMethod) where T:JSSharedMemoryObject
        {
            
            var s=runScript("SMOTest");
            context.GlobalObject.CallMethod<T>(testMethod,obj);
            obj.Buffer.ReadArray<byte>(0, payloadFromJS, 0, payloadSize);
            Assert.IsTrue(target.SequenceEqual(payloadFromJS));
        }

        private void smoSetGet<T>(T obj) where T:JSSharedMemoryObject
        {
            context.GlobalObject.WriteProperty<T>("buffer", obj);
            var b1 = context.GlobalObject.ReadProperty<T>("buffer");
            b1.Buffer.ReadArray<byte>(0, payloadFromJS, 0, payloadSize);
            Assert.IsTrue(payloadToJS.SequenceEqual(payloadToJS));
        }
    }
}
