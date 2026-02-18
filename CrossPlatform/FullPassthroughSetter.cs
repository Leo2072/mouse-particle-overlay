using Godot;
#if GODOT_WINDOWS
using System;
using System.Runtime.InteropServices;
#elif GODOT_LINUXBSD
using DisplayHandle = nint;
using WindowHandle = ulong;
using RegionHandle = nint;
using System.Runtime.InteropServices;
#endif


/// <summary>
/// Helper autoload class for
/// making a <c>Window</c> fully ignore mouse inputs
/// without affecting how it appears on-screen.
/// <para/>
/// 
/// <b>NOTE:</b>
/// <br/>
/// This functionality is only supported for Windows, Linux (X11), and MacOS Desktop Windows.
/// <br/>
/// This functionality is only available for Borderless <c>Window</c>s.
/// <para/>
/// <b>Windows</b>:
/// This functionality may cause visual artifacts when
/// blending semi-transparent pixels with the contents below the affected <c>Window</c>s.
/// </summary>
[GlobalClass]
public partial class FullPassthroughSetter : RefCounted
{
    #if GODOT_WINDOWS
#region WINDOWS_IMPLEMENTATION
    // If the platform is Windows, setting the Window Region to an empty polygon will cause the window to not render.
    // Thus, we instead set the WS_EX_LAYERED and WS_EX_TRANSPARENT extended window styles.

    // WS_EX_LAYERED makes the Window ignore Mouse Inputs when used with WS_EX_TRANSPARENT.
    // (On its own, WS_EX_LAYERED provides alpha blending for transparent and semi-transparent Window parts,
    // and lets you specify the alpha value threshold at which a pixel will ignore mouse inputs.)

    // WARNING:
    // Godot sets the CS_OWNDC flag for the Style Class used to make ALL of its Windows.
    // WS_EX_LAYERED (especially its blending functionalities) does not work properly
    // when used on Windows with the CS_OWNDC or CS_CLASSDC flags set for their Style Classes.


    // Import the Windows API methods to get and set the extended window style.
    // Source: https://stackoverflow.com/questions/9282284/setwindowlong-getwindowlong-and-32-bit-64-bit-cpus
    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    public static extern UInt32 GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    public static extern int SetWindowLong(IntPtr hwnd, int index, UInt32 newStyle);

    // Flag for the Transparent Extended Window Style.
    public const UInt32 WS_EX_TRANSPARENT = 0x00000020;

    // Flag for the Layered Extended Window Style.
    public const UInt32 WS_EX_LAYERED = 0x00080000;


    // Enumeration to indicate that the Extended Window Style flag is being operated on
    // when using GetWindowLong and SetWindowLong.
    public const int GWL_EXSTYLE = -20;


    /// <summary>
    /// Make mouse inputs completely pass through and ignored by
    /// the <paramref name="Target"/>, without affecting its visibility.
    /// <br/>
    /// To make the <paramref name="Target"/> able to intercept mouse inputs again,
    /// use <c>DisableFullPassthrough</c>.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the
    /// <c>WS_EX_TRANSPARENT</c> and <c>WS_EX_LAYERED</c> Extended Window Style flags
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting Extended Window Style and Window Class flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will set the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void EnableFullPassthrough(Window Target)
    {
        IntPtr hwnd = (IntPtr)DisplayServer.WindowGetNativeHandle
        (
            DisplayServer.HandleType.WindowHandle, Target.GetWindowId()
        );
        UInt32 currentExStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

        // Activate the WS_EX_TRANSPARENT flag.
        SetWindowLong(hwnd, GWL_EXSTYLE, currentExStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        return;
    }


    /// <summary>
    /// Make the <paramref name="Target"/> intercept mouse inputs again.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the <c>WS_EX_TRANSPARENT</c> Extended Window Style flag
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting the Extended Window Style flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Windows</b>:
    /// Parts of the <paramref name="Target"/> outside of its <c>MousePassthroughPolygon</c>
    /// will still not intercept mouse inputs.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will clear the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>
    /// and make all parts of it intercept mouse inputs again.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void DisableFullPassthrough(Window Target)
    {
        IntPtr hwnd = (IntPtr)DisplayServer.WindowGetNativeHandle
        (
            DisplayServer.HandleType.WindowHandle, Target.GetWindowId()
        );
        UInt32 currentExStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

        // Disable the WS_EX_TRANSPARENT flag.
        SetWindowLong(hwnd, GWL_EXSTYLE, currentExStyle & (~WS_EX_TRANSPARENT));
        return;
    }
#endregion



    #elif GODOT_LINUXBSD
#region LINUX_IMPLEMENTATION
    // On Linux, parts of the Window outside of its Window Region are still rendered,
    // so we only need to set said Region to an Empty Polygon.

    // Additional care must be taken as the engine does not automatically call
    // the update region code when running on Wayland, even for X11 Windows.


    // Import the LibX11 and LibXext API methods to Create, Set, and Destroy Window Regions.
    // Source: https://github.com/godotengine/godot/blob/master/platform/linuxbsd/x11/display_server_x11.cpp#L2265
    [DllImport("libX11.so", EntryPoint = "XCreateRegion", SetLastError = true)]
    public static extern RegionHandle CreateRegion();


    [DllImport("libX11.so", EntryPoint = "XDestroyRegion", SetLastError = true)]
    public static extern void DestroyRegion(RegionHandle Target);


    [DllImport("libXext.so", EntryPoint = "XShapeCombineRegion", SetLastError = true)]
    private static extern void CombineWindowInputRegion
    (
        DisplayHandle DisplayPtr, WindowHandle Target, int DestinationKind,
        int OffsetX, int OffsetY, RegionHandle InputRegion, int CombineOpCode
    );

    
    [DllImport("libXext.so", EntryPoint = "XShapeCombineMask", SetLastError = true)]
    private static extern void CombineWindowInputMask
    (
        DisplayHandle DisplayPtr, WindowHandle Target, int DestinationKind,
        int OffsetX, int OffsetY, RegionHandle InputRegion, int CombineOpCode
    );


    // Enumeration to specify that the destination region is written to.
    const int ShapeInputDest = 2;

    // Enumeration to specify that the shape of the first region
    // is set to that of the second one in the shape operation.
    const int ShapeSetOp = 0;


    /// <summary>
    /// Make mouse inputs completely pass through and ignored by
    /// the <paramref name="Target"/>, without affecting its visibility.
    /// <br/>
    /// To make the <paramref name="Target"/> able to intercept mouse inputs again,
    /// use <c>DisableFullPassthrough</c>.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the <c>WS_EX_TRANSPARENT</c> Extended Window Style flag
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting the Extended Window Style flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will set the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void EnableFullPassthrough(Window Target)
    {
        // Set the passthrough polygon to a region outside of the Bounds of the Windows
        // so that no mouse clicks will fall into it.
        Target.MousePassthroughPolygon = [new(-1, -1), new(-1, -1), new(-1, -1)];

        // Additionally set the input region using the engine source code,
        // as the mouse passthrough polygon has no effects for X11 windows
        // when the device runs on Wayland (for some reasons).
        int WindowID = Target.GetWindowId();
        RegionHandle EmptyRegion = CreateRegion();
	    CombineWindowInputRegion
        (
            (DisplayHandle)DisplayServer.WindowGetNativeHandle
            (
                DisplayServer.HandleType.DisplayHandle, WindowID
            ),
            (WindowHandle)DisplayServer.WindowGetNativeHandle
            (
                DisplayServer.HandleType.WindowHandle, WindowID
            ),
            ShapeInputDest, 0, 0, EmptyRegion, ShapeSetOp
        );
	    DestroyRegion(EmptyRegion);
        return;
    }


    /// <summary>
    /// Make the <paramref name="Target"/> intercept mouse inputs again.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the <c>WS_EX_TRANSPARENT</c> Extended Window Style flag
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting the Extended Window Style flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Windows</b>:
    /// Parts of the <paramref name="Target"/> outside of its <c>MousePassthroughPolygon</c>
    /// will still not intercept mouse inputs.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will clear the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>
    /// and make all parts of it intercept mouse inputs again.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void DisableFullPassthrough(Window Target)
    {
        // Clear the passthrough polygon so that it falls back to the default (covers the whole window).
        Target.MousePassthroughPolygon = [];

        // Additionally set the input region using the engine source code,
        // as the mouse passthrough polygon has no effects for X11 windows
        // when the device runs on Wayland (for some reasons).
        int WindowID = Target.GetWindowId();
    	CombineWindowInputMask
        (
            (DisplayHandle)DisplayServer.WindowGetNativeHandle
            (
                DisplayServer.HandleType.DisplayHandle, WindowID
            ),
            (WindowHandle)DisplayServer.WindowGetNativeHandle
            (
                DisplayServer.HandleType.WindowHandle, WindowID
            ),
            ShapeInputDest, 0, 0, 0, ShapeSetOp
        );
        return;
    }
#endregion



    #else
#region OTHER_IMPLEMENTATIONS
    // On MacOS, parts of the Window outside of its Window Region are still rendered,
    // so we only need to set said Region to an Empty Polygon.

    /// <summary>
    /// Make mouse inputs completely pass through and ignored by
    /// the <paramref name="Target"/>, without affecting its visibility.
    /// <br/>
    /// To make the <paramref name="Target"/> able to intercept mouse inputs again,
    /// use <c>DisableFullPassthrough</c>.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the <c>WS_EX_TRANSPARENT</c> Extended Window Style flag
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting the Extended Window Style flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will set the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void EnableFullPassthrough(Window Target)
    {
        #if !GODOT_MACOS
        // The Window Region (MousePassthroughPolygon) is only implemented for Windows, MacOS, and Linux(X11).
        // For all other platforms, it may have no effects.
        GD.PushWarning("Mouse Passthrough may not be implemented on your platform.");
        #endif

        // Set the passthrough polygon to a region outside of the Bounds of the Windows
        // so that no mouse clicks will fall into it.
        Target.MousePassthroughPolygon = [new(-1, -1), new(-1, -1), new(-1, -1)];
        return;
    }


    /// <summary>
    /// Make the <paramref name="Target"/> intercept mouse inputs again.
    /// <para/>
    /// 
    /// <b>NOTE:</b>
    /// <br/>
    /// This method is only supported on Windows, Linux (X11), and MacOS.
    /// <para/>
    /// <b>Windows</b>:
    /// This method will set the <c>WS_EX_TRANSPARENT</c> Extended Window Style flag
    /// of the <paramref name="Target"/>.
    /// Thus, the user should be careful when setting the Extended Window Style flags
    /// for <c>Window</c>s that will use this method.
    /// <para/>
    /// <b>Windows</b>:
    /// Parts of the <paramref name="Target"/> outside of its <c>MousePassthroughPolygon</c>
    /// will still not intercept mouse inputs.
    /// <para/>
    /// <b>Linux</b> and <b>MacOS</b>:
    /// This method will clear the <c>MousePassthroughPolygon</c> of the <paramref name="Target"/>
    /// and make all parts of it intercept mouse inputs again.
    /// Thus, this method should not be used for <c>Window</c>s that will have their
    /// <c>MousePassthroughPolygon</c> or <c>MousePassthrough</c> fields controlled by other sources.
    /// </summary>
    /// 
    /// <param name="Target">The <c>Window</c> to modify.</param>
    public static void DisableFullPassthrough(Window Target)
    {
        // Clear the passthrough polygon so that it falls back to the default (covers the whole window).
        Target.MousePassthroughPolygon = [];
        return;
    }
#endregion
    #endif
}
