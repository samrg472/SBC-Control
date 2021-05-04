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
                var method = _bottomBar.GetType().GetMethod("get_IsHeadphone");

                Trace.Assert(method != null, nameof(method) + " != null");
                return (bool) method.Invoke(_bottomBar, new object[0]);
            }
            set
            {
                var method = _bottomBar.GetType().GetMethod("set_IsHeadphone", new[] { typeof(bool) });

                Trace.Assert(method != null, nameof(method) + " != null");
                _dispatcher.Invoke(() =>
                {
                    method.Invoke(_bottomBar, new object[1] { value });
                });
            }
        }

        public CmdBottomBarView(object bottomBar)
        {
            Trace.Assert(bottomBar != null, nameof(bottomBar) + " != null");
            _bottomBar = bottomBar;

            var type = bottomBar.GetType();
            _dispatcher = (Dispatcher) type
                .GetField("mainThreadDispatcher", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(bottomBar);
            Trace.Assert(_dispatcher != null, nameof(_dispatcher) + " != null");
        }
    }
}
