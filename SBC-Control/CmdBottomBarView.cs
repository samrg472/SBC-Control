using System;
using System.ComponentModel;
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
                if (value == IsHeadphone)
                {
                    return;
                }

                var method = _bottomBar.GetType().GetMethod("set_IsHeadphone", new[] { typeof(bool) });
                Trace.Assert(method != null, nameof(method) + " != null");

                _dispatcher.Invoke(() =>
                {
                    method.Invoke(_bottomBar, new object[] { value });
                    OnPropChanged(this, new PropertyChangedEventArgs("IsHeadphone"));
                });
            }
        }

        public float MasterVolume
        {
            get
            {
                var method = _bottomBar.GetType().GetMethod("get_Volume");

                Trace.Assert(method != null, nameof(method) + " != null");
                return (float) method.Invoke(_bottomBar, new object[0]);
            }
            private set
            {
                var method = _bottomBar.GetType().GetMethod("set_Volume", new[] { typeof(float) });

                Trace.Assert(method != null, nameof(method) + " != null");
                _dispatcher.Invoke(() =>
                {
                    method.Invoke(_bottomBar, new object[] { value });
                });
            }
        }

        public bool MasterMute
        {
            get
            {
                var method = _bottomBar.GetType().GetMethod("get_Mute");

                Trace.Assert(method != null, nameof(method) + " != null");
                return (bool) method.Invoke(_bottomBar, new object[0]);
            }
            private set
            {
                var method = _bottomBar.GetType().GetMethod("set_Mute", new[] { typeof(bool) });

                Trace.Assert(method != null, nameof(method) + " != null");
                _dispatcher.Invoke(() =>
                {
                    method.Invoke(_bottomBar, new object[] { value });
                });
            }
        }

        /// <summary>
        /// Used to determine whether the property change handler is using the same value or raising a different value.
        /// </summary>
        private bool _prevIsHeadphoneVal;

        public CmdBottomBarView(object bottomBar)
        {
            Trace.Assert(bottomBar != null, nameof(bottomBar) + " != null");
            _bottomBar = bottomBar;
            _prevIsHeadphoneVal = IsHeadphone;

            var type = bottomBar.GetType();

            _dispatcher = (Dispatcher) type
                .GetField("mainThreadDispatcher", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(bottomBar);
            Trace.Assert(_dispatcher != null, nameof(_dispatcher) + " != null");

            {
                // Perform configuration updates
                var config = GlobalConfig.Config;
                if (GlobalConfig.ConfigIsNew)
                {
                    config.HeadphoneMasterVolume = MasterVolume;
                    config.HeadphoneMasterMute = MasterMute;
                    config.SrsMasterVolume = MasterVolume;
                    config.SrsMasterMute = MasterMute;
                }

                if (IsHeadphone)
                {
                    config.HeadphoneMasterVolume = MasterVolume;
                    config.HeadphoneMasterMute = MasterMute;
                }
                else
                {
                    config.SrsMasterVolume = MasterVolume;
                    config.SrsMasterMute = MasterMute;
                }

                config.Save();
            }

            {
                // Install our property change handler
                var propChangedEvt = type.GetEvent("PropertyChanged", BindingFlags.Public | BindingFlags.Instance);
                Trace.Assert(propChangedEvt != null, nameof(propChangedEvt) + " != null");
                var propChangedDel = propChangedEvt.EventHandlerType;
                var handler =
                    typeof(CmdBottomBarView).GetMethod("OnPropChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                var d = Delegate.CreateDelegate(propChangedDel, this, handler ?? throw new InvalidOperationException());

                var propChangedAddMethod = propChangedEvt.GetAddMethod();
                propChangedAddMethod.Invoke(bottomBar, new object[] {d});
            }
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void OnPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsHeadphone") return;
            if (IsHeadphone == _prevIsHeadphoneVal) return;
            _prevIsHeadphoneVal = IsHeadphone;

            var config = GlobalConfig.Config;
            if (IsHeadphone)
            {
                // Save to SRS config
                config.SrsMasterVolume = MasterVolume;
                config.SrsMasterMute = MasterMute;

                MasterVolume = config.HeadphoneMasterVolume;
                MasterMute = config.HeadphoneMasterMute;
            }
            else
            {
                // Save to headphone config
                config.HeadphoneMasterVolume = MasterVolume;
                config.HeadphoneMasterMute = MasterMute;

                MasterVolume = config.SrsMasterVolume;
                MasterMute = config.SrsMasterMute;
            }

            config.Save();
        }
    }
}
