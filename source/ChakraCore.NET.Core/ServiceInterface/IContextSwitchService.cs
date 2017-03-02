using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// Switchs chakracore context into/leave current thread, the context is retrived from current service node
    /// </summary>
    public interface IContextSwitchService:IService,IDisposable
    {
        /// <summary>
        /// Run an action with the chakracore context
        /// </summary>
        /// <typeparam name="T">Result type of function <paramref name="a"/></typeparam>
        /// <param name="a">The action be called with a specified chakra context</param>
        void With(Action a);

        /// <summary>
        /// run a function with the chakracore context and returns the function result
        /// </summary>
        /// <typeparam name="T">Result type of function <paramref name="f"/></typeparam>
        /// <param name="f">The function be called with a specified chakra context</param>
        /// <returns>Result of function <paramref name="f"/></returns>
        T With<T>(Func<T> f);
        /// <summary>
        /// Try switch context to current thread
        /// </summary>
        /// <returns>true if release is required, false if context already running at current thread(no release call required)</returns>
        bool Enter();
        /// <summary>
        /// Release the context from current thread, this method should be called before you call <see cref="Enter"/> on another thread
        /// </summary>
        void Leave();
        /// <summary>
        /// If chakracore is running at current thread
        /// <para>True if context is running at current thread, otherwise false</para>
        /// </summary>
        bool IsCurrentContext { get; }
    }
}
