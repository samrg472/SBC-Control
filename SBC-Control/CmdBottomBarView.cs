using System;
using System.Diagnostics;
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

                Debug.Assert(method != null, nameof(method) + " != null");
                return (bool) method.Invoke(this._bottomBar, new object[0]);
            }
            set
            {
                var method = this._bottomBar.GetType().GetMethod("set_IsHeadphone", new Type[] { typeof(bool) });

                Debug.Assert(method != null, nameof(method) + " != null");
                this._dispatcher.Invoke(() =>
                {
                    method.Invoke(this._bottomBar, new object[1] { value });
                });
            }
        }

        public CmdBottomBarView(Dispatcher dispatcher, object bottomBar)
        {
            this._dispatcher = dispatcher;
            this._bottomBar = bottomBar;
        }
    }
}
