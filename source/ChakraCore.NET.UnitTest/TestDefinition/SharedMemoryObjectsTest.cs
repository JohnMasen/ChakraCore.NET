using ChakraCore.NET.Extension.SharedMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChakraCore.NET.UnitTest.TestDefinition
{
    public abstract class SharedMemoryObjectsTest : UnitTestBase
    {
        public SharedMemoryObjectsTest(bool shareRuntime, bool shareContext) : base(shareRuntime, shareContext)
        {

        }
        protected override void SetupContext()
        {
            SharedMemoryValueConvertHelper.Inject(context);
        }



        [TestMethod]
        public void ArrayBufferReadWrite()
        {
            int buffersize = 1024 * 1024 * 10;
            JSArrayBuffer buffer = JSArrayBuffer.Create(buffersize);
            context.RootObject.WriteProperty<JSArrayBuffer>("buffer", buffer);
            byte[] tmp = new byte[buffersize];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = 0x10;
            }
            byte[] target = new byte[buffersize];
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = 0x0f;
            }
            buffer.Buffer.WriteArray<byte>(0, tmp, 0, tmp.Length);
            runScript("ArrayBufferReadWrite");
            Assert.IsFalse(tmp.SequenceEqual(target));
            buffer.Buffer.ReadArray<byte>(0, tmp, 0, tmp.Length);
            Assert.IsTrue(tmp.SequenceEqual(target));
            buffer.Dispose();
            runtime.CollectGarbage();
        }

        [TestMethod]
        public void ArrayBufferSetGet()
        {
            int buffersize = 1024 * 1024 * 10;
            JSArrayBuffer buffer = JSArrayBuffer.Create(buffersize);
            context.RootObject.WriteProperty<JSArrayBuffer>("buffer1", buffer);
            byte[] tmp = new byte[buffersize];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = 0x10;
            }
            byte[] target = new byte[buffersize];
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = 0x0f;
            }

            buffer.Buffer.WriteArray<byte>(0, tmp, 0, tmp.Length);
            runScript("JSArrayBufferSetGet");

            JSArrayBuffer buffer1 = context.RootObject.ReadProperty<JSArrayBuffer>("buffer1");
            buffer1.Buffer.ReadArray<byte>(0, tmp, 0, tmp.Length);

            Assert.IsTrue(tmp.SequenceEqual(target));
            buffer.Dispose();
            buffer1.Dispose();
            runtime.CollectGarbage();
        }

        [TestMethod]
        public void TypedArrayReadWrite()
        {
            uint size = 100;
            byte[] initdata = new byte[size];
            for (int i = 0; i < size; i++)
            {
                initdata[i] = 1;
            }

            Action<SharedMemoryBuffer> init = (x) => { x.WriteArray(0, initdata, 0, initdata.Length); };
            JSTypedArray array = JSTypedArray.CreateInJS(API.JavaScriptTypedArrayType.Int8, size, init);
            context.RootObject.WriteProperty<JSTypedArray>("array1", array);
            runScript("JSTypedArrayReadWrite");
            array.Buffer.ReadArray(0, initdata, 0, initdata.Length);
            array.Dispose();
            foreach (var item in initdata)
            {
                Assert.AreEqual<int>(2, item);
            }
        }

        [TestMethod]
        public void DataViewReadWrite()
        {
            uint size = 100;
            byte[] initdata = new byte[size];
            for (int i = 0; i < size; i++)
            {
                initdata[i] = 1;
            }
            JSArrayBuffer buffer = JSArrayBuffer.CreateInJavascript(size, null);
            JSDataView view = JSDataView.CreateFromArrayBuffer(buffer, 0, size, (b) => { b.WriteArray(0, initdata, 0, initdata.Length); });
            context.RootObject.WriteProperty<JSDataView>("dv1", view);
            runScript("JSDataViewReadWrite");
            JSDataView view1 = context.RootObject.ReadProperty<JSDataView>("dv1");
            view1.Buffer.ReadArray(0, initdata, 0, initdata.Length);
            foreach (var item in initdata)
            {
                Assert.AreEqual<byte>(2, item);
            }
        }
    }
}
