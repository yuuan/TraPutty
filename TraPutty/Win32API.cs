using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TraPutty
{
	class Win32API
	{
		public const int GWL_ID = -12;
		public const int GWL_STYLE = -16;
		public const int GWL_EXSTYLE = -20;
		public const int WS_EX_LAYERED = 0x80000;
		public const int LWA_ALPHA = 0x2;
		public const int LWA_COLORKEY = 0x1;

		// Window Styles
		public enum WS : uint {
			OVERLAPPED = 0,
			POPUP = 0x80000000,
			CHILD = 0x40000000,
			MINIMIZE = 0x20000000,
			VISIBLE = 0x10000000,
			DISABLED = 0x8000000,
			CLIPSIBLINGS = 0x4000000,
			CLIPCHILDREN = 0x2000000,
			MAXIMIZE = 0x1000000,
			CAPTION = 0xC00000,      // WS_BORDER or WS_DLGFRAME  
			BORDER = 0x800000,
			DLGFRAME = 0x400000,
			VSCROLL = 0x200000,
			HSCROLL = 0x100000,
			SYSMENU = 0x80000,
			THICKFRAME = 0x40000,
			GROUP = 0x20000,
			TABSTOP = 0x10000,
			MINIMIZEBOX = 0x20000,
			MAXIMIZEBOX = 0x10000,
			TILED = WS.OVERLAPPED,
			ICONIC = WS.MINIMIZE,
			SIZEBOX = WS.THICKFRAME,
		}

		// Extended Window Styles
		public enum WS_EX : uint {
			DLGMODALFRAME = 0x0001,
			NOPARENTNOTIFY = 0x0004,
			TOPMOST = 0x0008,
			ACCEPTFILES = 0x0010,
			TRANSPARENT = 0x0020,
			MDICHILD = 0x0040,
			TOOLWINDOW = 0x0080,
			WINDOWEDGE = 0x0100,
			CLIENTEDGE = 0x0200,
			CONTEXTHELP = 0x0400,
			RIGHT = 0x1000,
			LEFT = 0x0000,
			RTLREADING = 0x2000,
			LTRREADING = 0x0000,
			LEFTSCROLLBAR = 0x4000,
			RIGHTSCROLLBAR = 0x0000,
			CONTROLPARENT = 0x10000,
			STATICEDGE = 0x20000,
			APPWINDOW = 0x40000,
			OVERLAPPEDWINDOW = (WS_EX.WINDOWEDGE | WS_EX.CLIENTEDGE),
			PALETTEWINDOW = (WS_EX.WINDOWEDGE | WS_EX.TOOLWINDOW | WS_EX.TOPMOST),
			LAYERED = 0x00080000,
			NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
			LAYOUTRTL = 0x00400000, // Right to left mirroring
			COMPOSITED = 0x02000000,
			NOACTIVATE = 0x08000000,
		}

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern bool GetLayeredWindowAttributes(IntPtr hwnd, uint crKey, out byte bAlpha, uint dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	}
}
