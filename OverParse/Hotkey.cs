using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

//sourcecode from
//https://gist.github.com/EmissaryFromSunday/1308531

namespace HotKeyFrame
{
    class HotKey
    {
        private const int WM_HOTKEY = 0x0312;
        private IntPtr _windowHandle;
        private Dictionary<int, EventHandler> _hotkeyEvents;

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int MOD_KEY, int VK);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public HotKey(Window window)
        {
            WindowInteropHelper _host = new WindowInteropHelper(window);
            _windowHandle = _host.Handle;

            ComponentDispatcher.ThreadPreprocessMessage
                += ComponentDispatcher_ThreadPreprocessMessage;

            _hotkeyEvents = new Dictionary<int, EventHandler>();
        }

        public void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY) return;
            var hotkeyID = msg.wParam.ToInt32();
            if (!_hotkeyEvents.Any((x) => x.Key == hotkeyID)) return;

            new ThreadStart(
                () => _hotkeyEvents[hotkeyID](this, EventArgs.Empty)
            ).Invoke();
        }

        public void Regist(ModifierKeys modkey, Key trigger, EventHandler eh,int i)
        {

            var imod = modkey.ToInt32();
            var itrg = KeyInterop.VirtualKeyFromKey(trigger);

            while ((++i < 0xc000) && RegisterHotKey(_windowHandle, i, imod, itrg) == 0) ;

            if (i < 0xc000)
            {
                _hotkeyEvents.Add(i, eh);
            }
        }

        public void Unregist()
        {
            foreach (var hotkeyid in _hotkeyEvents.Keys)
            {
                UnregisterHotKey(_windowHandle, hotkeyid);
            }
        }
    }

    static class Extention
    {
        public static Int32 ToInt32(this ModifierKeys m)
        {
            return (Int32)m;
        }
    }
}
