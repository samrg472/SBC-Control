using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Threading;

namespace SBC_Control
{
    public class CmdBottomBarView
    {
        private readonly Dispatcher _dispatcher;
        private readonly object _bottomBar;

        public bool IsHeadphone
        {
            get
            {
                var method = this._bottomBar.GetType().GetMethod("get_IsHeadphone");

                Trace.Assert(method != null, nameof(method) + " != null");
                return (bool) method.Invoke(this._bottomBar, new object[0]);
            }
            set
            {
                var method = this._bottomBar.GetType().GetMethod("set_IsHeadphone", new Type[] { typeof(bool) });

                Trace.Assert(method != null, nameof(method) + " != null");
                this._dispatcher.Invoke(() =>
                {
                    method.Invoke(this._bottomBar, new object[1] { value });
                });
            }
        }

        public CmdBottomBarView(object bottomBar)
        {
            Trace.Assert(bottomBar != null, nameof(bottomBar) + " != null");
            this._bottomBar = bottomBar;

            var type = this._bottomBar.GetType();
            this._dispatcher = (Dispatcher) type
                .GetField("mainThreadDispatcher", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(bottomBar);
            Trace.Assert(this._dispatcher != null, nameof(this._dispatcher) + " != null");
        }
    }
}
