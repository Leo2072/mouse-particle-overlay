using Godot;


#if GODOT_WINDOWS
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
#elif GODOT_LINUXBSD
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using DisplayHandle = nint;
using WindowHandle = ulong;
using ScanCode = byte;
// using KeySym = ulong;
using ButtonMask = uint;
#elif GODOT_MACOS
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using EventSourceStateID = int;
using ScanCode = ushort;
using MouseButtonCode = uint;
#endif



[GlobalClass]
public partial class GlobalKeyStateCache : RefCounted
{
    #if GODOT_WINDOWS
#region WINDOWS_IMPLEMENTATION
    // Import the relevant methods and constants from User32.dll (winuser.h):
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardstate

    // Mapping between Godot Key Codes and Windows Scan Codes (Windows Virtual Key Codes):
    // https://github.com/godotengine/godot/blob/f0aeea26fb9d930be42f1cdc5cb55b3acf9589bf/platform/linuxbsd/x11/key_mapping_x11.h



    // Functionalities for querying whether or not a key is pressed. 
    #region Key State Queries
    /// <summary>
    /// Helper method to get the logical state (which buttons are pressed)
    /// of the a given physical keyboard or mouse button.
    /// </summary>
    /// <param name="KeyCode">
    /// The Key Code of the Windows Virtual Key to check.
    /// </param>
    /// <return>
    /// A bit-field with the most significant bit set if the key is currently pressed,
    /// and the least significant bit set if the key was not pressed
    /// during the last call to <C>GetAsyncKeyState</c> from <c>user32.dll</c> by ANY PROCESS.
    /// </return>
    [DllImport("user32.dll", EntryPoint = "GetAsyncKeyState", SetLastError = true)]
    public static extern ushort GetKeyState(int KeyCode);


    /// <summary>
    /// A Dictionary mapping a Windows Virtual Key to its corresponding <c>Godot.Key</c>
    /// (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<byte, Key> WinVKeyToGodot = new
    (
        new Dictionary<byte, Key>()
        {
            { 0x08 , Key.Backspace },
            { 0x09 , Key.Tab },
            { 0x0C , Key.Clear },
            { 0x0D , Key.Enter },
            { 0x10 , Key.Shift },
            { 0x11 , Key.Ctrl },
            { 0x12 , Key.Alt },
            { 0x13 , Key.Pause },
            { 0x14 , Key.Capslock },
            { 0x1B , Key.Escape },
            { 0x20 , Key.Space },
            { 0x21 , Key.Pageup },
            { 0x22 , Key.Pagedown },
            { 0x23 , Key.End },
            { 0x24 , Key.Home },
            { 0x25 , Key.Left },
            { 0x26 , Key.Up },
            { 0x27 , Key.Right },
            { 0x28 , Key.Down },
            { 0x2A , Key.Print },
            { 0x2C , Key.Print },
            { 0x2D , Key.Insert },
            { 0x2E , Key.Delete },
            { 0x2F , Key.Help },
            { 0x30 , Key.Key0 },
            { 0x31 , Key.Key1 },
            { 0x32 , Key.Key2 },
            { 0x33 , Key.Key3 },
            { 0x34 , Key.Key4 },
            { 0x35 , Key.Key5 },
            { 0x36 , Key.Key6 },
            { 0x37 , Key.Key7 },
            { 0x38 , Key.Key8 },
            { 0x39 , Key.Key9 },
            { 0x41 , Key.A },
            { 0x42 , Key.B },
            { 0x43 , Key.C },
            { 0x44 , Key.D },
            { 0x45 , Key.E },
            { 0x46 , Key.F },
            { 0x47 , Key.G },
            { 0x48 , Key.H },
            { 0x49 , Key.I },
            { 0x4A , Key.J },
            { 0x4B , Key.K },
            { 0x4C , Key.L },
            { 0x4D , Key.M },
            { 0x4E , Key.N },
            { 0x4F , Key.O },
            { 0x50 , Key.P },
            { 0x51 , Key.Q },
            { 0x52 , Key.R },
            { 0x53 , Key.S },
            { 0x54 , Key.T },
            { 0x55 , Key.U },
            { 0x56 , Key.V },
            { 0x57 , Key.W },
            { 0x58 , Key.X },
            { 0x59 , Key.Y },
            { 0x5A , Key.Z },
            { 0x5B , Key.Meta },
            { 0x5C , Key.Meta },
            { 0x5D , Key.Menu },
            { 0x5F , Key.Standby },
            { 0x60 , Key.Kp0 },
            { 0x61 , Key.Kp1 },
            { 0x62 , Key.Kp2 },
            { 0x63 , Key.Kp3 },
            { 0x64 , Key.Kp4 },
            { 0x65 , Key.Kp5 },
            { 0x66 , Key.Kp6 },
            { 0x67 , Key.Kp7 },
            { 0x68 , Key.Kp8 },
            { 0x69 , Key.Kp9 },
            { 0x6A , Key.KpMultiply },
            { 0x6B , Key.KpAdd },
            { 0x6C , Key.KpPeriod },
            { 0x6D , Key.KpSubtract },
            { 0x6E , Key.KpPeriod },
            { 0x6F , Key.KpDivide },
            { 0x70 , Key.F1 },
            { 0x71 , Key.F2 },
            { 0x72 , Key.F3 },
            { 0x73 , Key.F4 },
            { 0x74 , Key.F5 },
            { 0x75 , Key.F6 },
            { 0x76 , Key.F7 },
            { 0x77 , Key.F8 },
            { 0x78 , Key.F9 },
            { 0x79 , Key.F10 },
            { 0x7A , Key.F11 },
            { 0x7B , Key.F12 },
            { 0x7C , Key.F13 },
            { 0x7D , Key.F14 },
            { 0x7E , Key.F15 },
            { 0x7F , Key.F16 },
            { 0x80 , Key.F17 },
            { 0x81 , Key.F18 },
            { 0x82 , Key.F19 },
            { 0x83 , Key.F20 },
            { 0x84 , Key.F21 },
            { 0x85 , Key.F22 },
            { 0x86 , Key.F23 },
            { 0x87 , Key.F24 },
            { 0x90 , Key.Numlock },
            { 0x91 , Key.Scrolllock },
            { 0x92 , Key.Equal },
            { 0xA0 , Key.Shift },
            { 0xA1 , Key.Shift },
            { 0xA2 , Key.Ctrl },
            { 0xA3 , Key.Ctrl },
            { 0xA4 , Key.Menu },
            { 0xA5 , Key.Menu },
            { 0xA6 , Key.Back },
            { 0xA7 , Key.Forward },
            { 0xA8 , Key.Refresh },
            { 0xA9 , Key.Stop },
            { 0xAA , Key.Search },
            { 0xAB , Key.Favorites },
            { 0xAC , Key.Homepage },
            { 0xAD , Key.Volumemute },
            { 0xAE , Key.Volumedown },
            { 0xAF , Key.Volumeup },
            { 0xB0 , Key.Medianext },
            { 0xB1 , Key.Mediaprevious },
            { 0xB2 , Key.Mediastop },
            { 0xB3 , Key.Mediaplay },
            { 0xB4 , Key.Launchmail },
            { 0xB5 , Key.Launchmedia },
            { 0xB6 , Key.Launch0 },
            { 0xB7 , Key.Launch1 },
            { 0xBA , Key.Semicolon },
            { 0xBB , Key.Equal },
            { 0xBC , Key.Comma },
            { 0xBD , Key.Minus },
            { 0xBE , Key.Period },
            { 0xBF , Key.Slash },
            { 0xC0 , Key.Quoteleft },
            { 0xDB , Key.Bracketleft },
            { 0xDC , Key.Backslash },
            { 0xDD , Key.Bracketright },
            { 0xDE , Key.Apostrophe },
            { 0xE2 , Key.Bar },
            { 0xE3 , Key.Help },
            { 0xE6 , Key.Clear },
            { 0xF6 , Key.Escape },
            { 0xF7 , Key.Tab },
            { 0xFA , Key.Mediaplay },
            { 0xFE , Key.Clear }
        }
    );

    /// <summary>
    /// A Dictionary mapping a <c>Godot.Key</c> to a list of Windows Virtual Keys
    /// that corresponds to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<Key, byte[]> GodotToWinVKey = new
    (
        new Dictionary<Key, byte[]>()
        {
            { Key.Backspace , [0x08] },
            { Key.Tab , [0x09, 0xF7] },
            { Key.Clear , [0x0C, 0xE6, 0xFE] },
            { Key.Enter , [0x0D] },
            { Key.Shift , [0x10, 0xA0, 0xA1] },
            { Key.Ctrl , [0x11, 0xA2, 0xA3] },
            { Key.Alt , [0x12] },
            { Key.Pause , [0x13] },
            { Key.Capslock , [0x14] },
            { Key.Escape , [0x1B, 0xF6] },
            { Key.Space , [0x20] },
            { Key.Pageup , [0x21] },
            { Key.Pagedown , [0x22] },
            { Key.End , [0x23] },
            { Key.Home , [0x24] },
            { Key.Left , [0x25] },
            { Key.Up , [0x26] },
            { Key.Right , [0x27] },
            { Key.Down , [0x28] },
            { Key.Print , [0x2A, 0x2C] },
            { Key.Insert , [0x2D] },
            { Key.Delete , [0x2E] },
            { Key.Help , [0x2F, 0xE3] },
            { Key.Key0 , [0x30] },
            { Key.Key1 , [0x31] },
            { Key.Key2 , [0x32] },
            { Key.Key3 , [0x33] },
            { Key.Key4 , [0x34] },
            { Key.Key5 , [0x35] },
            { Key.Key6 , [0x36] },
            { Key.Key7 , [0x37] },
            { Key.Key8 , [0x38] },
            { Key.Key9 , [0x39] },
            { Key.A , [0x41] },
            { Key.B , [0x42] },
            { Key.C , [0x43] },
            { Key.D , [0x44] },
            { Key.E , [0x45] },
            { Key.F , [0x46] },
            { Key.G , [0x47] },
            { Key.H , [0x48] },
            { Key.I , [0x49] },
            { Key.J , [0x4A] },
            { Key.K , [0x4B] },
            { Key.L , [0x4C] },
            { Key.M , [0x4D] },
            { Key.N , [0x4E] },
            { Key.O , [0x4F] },
            { Key.P , [0x50] },
            { Key.Q , [0x51] },
            { Key.R , [0x52] },
            { Key.S , [0x53] },
            { Key.T , [0x54] },
            { Key.U , [0x55] },
            { Key.V , [0x56] },
            { Key.W , [0x57] },
            { Key.X , [0x58] },
            { Key.Y , [0x59] },
            { Key.Z , [0x5A] },
            { Key.Meta , [0x5B, 0x5C] },
            { Key.Menu , [0x5D, 0xA4, 0xA5] },
            { Key.Standby , [0x5F] },
            { Key.Kp0 , [0x60] },
            { Key.Kp1 , [0x61] },
            { Key.Kp2 , [0x62] },
            { Key.Kp3 , [0x63] },
            { Key.Kp4 , [0x64] },
            { Key.Kp5 , [0x65] },
            { Key.Kp6 , [0x66] },
            { Key.Kp7 , [0x67] },
            { Key.Kp8 , [0x68] },
            { Key.Kp9 , [0x69] },
            { Key.KpMultiply , [0x6A] },
            { Key.KpAdd , [0x6B] },
            { Key.KpPeriod , [0x6C, 0x6E] },
            { Key.KpSubtract , [0x6D] },
            { Key.KpDivide , [0x6F] },
            { Key.F1 , [0x70] },
            { Key.F2 , [0x71] },
            { Key.F3 , [0x72] },
            { Key.F4 , [0x73] },
            { Key.F5 , [0x74] },
            { Key.F6 , [0x75] },
            { Key.F7 , [0x76] },
            { Key.F8 , [0x77] },
            { Key.F9 , [0x78] },
            { Key.F10 , [0x79] },
            { Key.F11 , [0x7A] },
            { Key.F12 , [0x7B] },
            { Key.F13 , [0x7C] },
            { Key.F14 , [0x7D] },
            { Key.F15 , [0x7E] },
            { Key.F16 , [0x7F] },
            { Key.F17 , [0x80] },
            { Key.F18 , [0x81] },
            { Key.F19 , [0x82] },
            { Key.F20 , [0x83] },
            { Key.F21 , [0x84] },
            { Key.F22 , [0x85] },
            { Key.F23 , [0x86] },
            { Key.F24 , [0x87] },
            { Key.Numlock , [0x90] },
            { Key.Scrolllock , [0x91] },
            { Key.Equal , [0x92, 0xBB] },
            { Key.Back , [0xA6] },
            { Key.Forward , [0xA7] },
            { Key.Refresh , [0xA8] },
            { Key.Stop , [0xA9] },
            { Key.Search , [0xAA] },
            { Key.Favorites , [0xAB] },
            { Key.Homepage , [0xAC] },
            { Key.Volumemute , [0xAD] },
            { Key.Volumedown , [0xAE] },
            { Key.Volumeup , [0xAF] },
            { Key.Medianext , [0xB0] },
            { Key.Mediaprevious , [0xB1] },
            { Key.Mediastop , [0xB2] },
            { Key.Mediaplay , [0xB3, 0xFA] },
            { Key.Launchmail , [0xB4] },
            { Key.Launchmedia , [0xB5] },
            { Key.Launch0 , [0xB6] },
            { Key.Launch1 , [0xB7] },
            { Key.Semicolon , [0xBA] },
            { Key.Comma , [0xBC] },
            { Key.Minus , [0xBD] },
            { Key.Period , [0xBE] },
            { Key.Slash , [0xBF] },
            { Key.Quoteleft , [0xC0] },
            { Key.Bracketleft , [0xDB] },
            { Key.Backslash , [0xDC] },
            { Key.Bracketright , [0xDD] },
            { Key.Apostrophe , [0xDE] },
            { Key.Bar , [0xE2] }
        }
    );


    /// <summary>
    /// A Dictionary mapping a <c>Godot.MouseButtonMas</c> to a Windows Virtual Key
    /// that corresponds to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<MouseButtonMask, byte> GodotMouseToWinVKey = new
    (
        new Dictionary<MouseButtonMask, byte>()
        {
            { MouseButtonMask.Left, 0x01 },
            { MouseButtonMask.Right, 0x02 },
            { MouseButtonMask.Middle, 0x04 },
            { MouseButtonMask.MbXbutton1, 0x05 },
            { MouseButtonMask.MbXbutton2, 0x06 }
        }
    );
    #endregion



    // Functionalities for taking a snapshot of the state of the keyboard and buttons.
    #region Snapshot
    /// <summary>
    /// A cache of the set of <c>Godot.Key</c>s pressed.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the keyboard
    /// taken during the last call to <c>UpdateKeyStateCache</c>.
    /// </summary>
    private HashSet<Key> GodotKeyStateCache = new();

    /// <summary>
    /// Take a snapshot of the logical state of the physical keyboard in the Display
    /// and update the <c>KeyStateCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On Windows, this does nothing.
    /// </param>
    public void UpdateKeyStateCache(Window Target)
    {
        GodotKeyStateCache.Clear();
        foreach (var GodotWinVKeyPair in WinVKeyToGodot)
        {
            if ((GetKeyState(GodotWinVKeyPair.Key) >>> 8) != 0)
            {
                GodotKeyStateCache.Add(GodotWinVKeyPair.Value);
            }
        }
    }


    /// <summary>
    /// A cache of the <c>Godot.MouseButtonMask</c> for all mouse buttons presssed.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the Godot Mouse Button Mask
    /// taken during the last call to <c>UpdateButtonMaskCache</c>.
    /// </summary>
    private MouseButtonMask GodotMouseMaskCache = 0;

    /// <summary>
    /// Take a snapshot of the Buttons being pressed
    /// and update the <c>GodotMouseMaskCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On Windows, this does nothing.
    /// </param>
    public void UpdateButtonMaskCache(Window Target)
    {
        GodotMouseMaskCache = 0;
        foreach (var GodotMouseWinVKeyPair in GodotMouseToWinVKey)
        {
            if ((GetKeyState(GodotMouseWinVKeyPair.Value) >>> 8) != 0)
            {
                GodotMouseMaskCache |= GodotMouseWinVKeyPair.Key;
            }
        }
    }


    /// <summary>
    /// Take a snapshot of the logical state of all input devices
    /// and update the cache with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard and button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On Windows, this does nothing.
    /// </param>
    public void UpdateInputStateCache(Window Target)
    {
        UpdateKeyStateCache(Target);
        UpdateButtonMaskCache(Target);
    }
    #endregion



    // Functionalities for querying information
    // from a snapshot of the keyboard and buttons.
    #region Snapshot Query
    /// <summary>
    /// Check if a given <c>Godot.Key</c> is being pressed (held down)
    /// using the snapshot saved in the <c>VirtualKeyStateCache</c>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.Key</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsKeyPressed(Key GodotKeyCode)
    {
        return GodotKeyStateCache.Contains(GodotKeyCode);
    }

    /// <summary>
    /// Get a list of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the <c>VirtualKeyStateCache</c>.
    /// </summary>
    /// <returns>
    /// An <c>Array[Key]</c> of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the <c>VirtualKeyStateCache</c>.
    /// </returns>
    public Godot.Collections.Array<Key> GetKeysPressed()
    {
        return [..GodotKeyStateCache];
    }


    /// <summary>
    /// Check if the mouse button corresponding to
    /// a given <c>Godot.MouseButtonMask</c> is being pressed (held down)
    /// using the snapshot saved in the <c>VirtualKeyStateCache</c>.
    /// <para/>
    /// <b>Note:</b>
    /// This method does not work when <paramref name="ButtonMask"/>
    /// is a combination of 2 or more <c>Godot.MouseButtonMask</c>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.MouseButtonMask</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsMouseButtonPressed(MouseButtonMask ButtonMask)
    {
        return (GodotMouseMaskCache & ButtonMask) != 0;
    }

    /// <summary>
    /// Get the <c>Godot.MouseButtonMask</c> being pressed (held down)
    /// in the snapshot saved in the <c>VirtualKeyStateCache</c>.
    /// </summary>
    /// <returns>
    /// The <c>Godot.MouseButtonMask</c>
    /// corresponding to the Godot Mouse Buttons being pressed (held down)
    /// in the snapshot saved in the <c>ButtonMaskCache</c>.
    /// </returns>
    public MouseButtonMask GetMouseButtonMask()
    {
        return GodotMouseMaskCache;
    }
    #endregion
#endregion





    #elif GODOT_LINUXBSD
#region LINUX_IMPLEMENTATION
    // Import the relevant methods from Xlib:
    // https://github.com/ajnewlands/X11.Net/blob/master/X11/xcb/base.cs#L10

    // Mapping between Godot Key Codes and Xlib Scancodes (X Key Codes):
    // https://github.com/godotengine/godot/blob/f0aeea26fb9d930be42f1cdc5cb55b3acf9589bf/platform/linuxbsd/x11/key_mapping_x11.h



    // Functionalities for querying whether or not a key is pressed. 
    #region Keyboard State Queries
    /// <summary>
    /// Helper method to get the logical state (which buttons are pressed)
    /// of the physical keyboard in a given Display as a 256-bit bit field.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the keyboard of interest.
    /// </param>
    /// <param name="Buffer">
    /// A buffer with a size of 256 bits
    /// where each bit indicates whether or not a physical button is pressed.
    /// <para/>
    /// For instance, if the n bit is 1,
    /// then the button represented by the X Key Code n is currently pressed.
    /// </param>
    [DllImport("libX11.so.6", EntryPoint = "XQueryKeymap", SetLastError = true)]
    public static extern void QueryPhysicialKeyPresses(
        DisplayHandle DisplayPtr, [MarshalAs(UnmanagedType.LPArray), Out] byte[] Buffer
    );


    /// <summary>
    /// A Dictionary mapping an X Key Code (Scan Code)
    /// to its corresponding <c>Godot.Key</c> (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<ScanCode, Key> XKeyCodeToGodot = new
    (
        new Dictionary<ScanCode, Key>()
        {
            { 0x09 , Key.Escape },
            { 0x0A , Key.Key1 },
            { 0x0B , Key.Key2 },
            { 0x0C , Key.Key3 },
            { 0x0D , Key.Key4 },
            { 0x0E , Key.Key5 },
            { 0x0F , Key.Key6 },
            { 0x10 , Key.Key7 },
            { 0x11 , Key.Key8 },
            { 0x12 , Key.Key9 },
            { 0x13 , Key.Key0 },
            { 0x14 , Key.Minus },
            { 0x15 , Key.Equal },
            { 0x16 , Key.Backspace },
            { 0x17 , Key.Tab },
            { 0x18 , Key.Q },
            { 0x19 , Key.W },
            { 0x1A , Key.E },
            { 0x1B , Key.R },
            { 0x1C , Key.T },
            { 0x1D , Key.Y },
            { 0x1E , Key.U },
            { 0x1F , Key.I },
            { 0x20 , Key.O },
            { 0x21 , Key.P },
            { 0x22 , Key.Bracketleft },
            { 0x23 , Key.Bracketright },
            { 0x24 , Key.Enter },
            { 0x25 , Key.Ctrl },
            { 0x26 , Key.A },
            { 0x27 , Key.S },
            { 0x28 , Key.D },
            { 0x29 , Key.F },
            { 0x2A , Key.G },
            { 0x2B , Key.H },
            { 0x2C , Key.J },
            { 0x2D , Key.K },
            { 0x2E , Key.L },
            { 0x2F , Key.Semicolon },
            { 0x30 , Key.Apostrophe },
            { 0x31 , Key.Quoteleft },
            { 0x32 , Key.Shift },
            { 0x33 , Key.Backslash },
            { 0x34 , Key.Z },
            { 0x35 , Key.X },
            { 0x36 , Key.C },
            { 0x37 , Key.V },
            { 0x38 , Key.B },
            { 0x39 , Key.N },
            { 0x3A , Key.M },
            { 0x3B , Key.Comma },
            { 0x3C , Key.Period },
            { 0x3D , Key.Slash },
            { 0x3E , Key.Shift },
            { 0x3F , Key.KpMultiply },
            { 0x40 , Key.Alt },
            { 0x41 , Key.Space },
            { 0x42 , Key.Capslock },
            { 0x43 , Key.F1 },
            { 0x44 , Key.F2 },
            { 0x45 , Key.F3 },
            { 0x46 , Key.F4 },
            { 0x47 , Key.F5 },
            { 0x48 , Key.F6 },
            { 0x49 , Key.F7 },
            { 0x4A , Key.F8 },
            { 0x4B , Key.F9 },
            { 0x4C , Key.F10 },
            { 0x4D , Key.Numlock },
            { 0x4E , Key.Scrolllock },
            { 0x4F , Key.Kp7 },
            { 0x50 , Key.Kp8 },
            { 0x51 , Key.Kp9 },
            { 0x52 , Key.KpSubtract },
            { 0x53 , Key.Kp4 },
            { 0x54 , Key.Kp5 },
            { 0x55 , Key.Kp6 },
            { 0x56 , Key.KpAdd },
            { 0x57 , Key.Kp1 },
            { 0x58 , Key.Kp2 },
            { 0x59 , Key.Kp3 },
            { 0x5A , Key.Kp0 },
            { 0x5B , Key.KpPeriod },
            { 0x5E , Key.Section },
            { 0x5F , Key.F11 },
            { 0x60 , Key.F12 },
            { 0x67 , Key.Comma },
            { 0x68 , Key.KpEnter },
            { 0x69 , Key.Ctrl },
            { 0x6A , Key.KpDivide },
            { 0x6B , Key.Print },
            { 0x6C , Key.Alt },
            { 0x6D , Key.Enter },
            { 0x6E , Key.Home },
            { 0x6F , Key.Up },
            { 0x70 , Key.Pageup },
            { 0x71 , Key.Left },
            { 0x72 , Key.Right },
            { 0x73 , Key.End },
            { 0x74 , Key.Down },
            { 0x75 , Key.Pagedown },
            { 0x76 , Key.Insert },
            { 0x77 , Key.Delete },
            { 0x79 , Key.Volumemute },
            { 0x7A , Key.Volumedown },
            { 0x7B , Key.Volumeup },
            { 0x7D , Key.Equal },
            { 0x7F , Key.Pause },
            { 0x80 , Key.Launch0 },
            { 0x81 , Key.Comma },
            { 0x84 , Key.Yen },
            { 0x85 , Key.Meta },
            { 0x86 , Key.Meta },
            { 0x87 , Key.Menu },
            { 0xA6 , Key.Back },
            { 0xA7 , Key.Forward },
            { 0xB5 , Key.Refresh },
            { 0xBF , Key.F13 },
            { 0xC0 , Key.F14 },
            { 0xC1 , Key.F15 },
            { 0xC2 , Key.F16 },
            { 0xC3 , Key.F17 },
            { 0xC4 , Key.F18 },
            { 0xC5 , Key.F19 },
            { 0xC6 , Key.F20 },
            { 0xC7 , Key.F21 },
            { 0xC8 , Key.F22 },
            { 0xC9 , Key.F23 },
            { 0xCA , Key.F24 },
            { 0xCB , Key.F25 },
            { 0xCC , Key.F26 },
            { 0xCD , Key.F27 },
            { 0xCE , Key.F28 },
            { 0xCF , Key.F29 },
            { 0xD0 , Key.F30 },
            { 0xD1 , Key.F31 },
            { 0xD2 , Key.F32 },
            { 0xD3 , Key.F33 },
            { 0xD4 , Key.F34 },
            { 0xD5 , Key.F35 }
        }
    );

    /// <summary>
    /// A Dictionary mapping a <c>Godot.Key</c> to a list of X Key Codes (Scan Codes)
    /// that corresponds to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<Key, ScanCode[]> GodotToXKeySym = new
    (
        new Dictionary<Key, ScanCode[]>()
        {
            { Key.Escape , [0x09] },
            { Key.Key1 , [0x0A] },
            { Key.Key2 , [0x0B] },
            { Key.Key3 , [0x0C] },
            { Key.Key4 , [0x0D] },
            { Key.Key5 , [0x0E] },
            { Key.Key6 , [0x0F] },
            { Key.Key7 , [0x10] },
            { Key.Key8 , [0x11] },
            { Key.Key9 , [0x12] },
            { Key.Key0 , [0x13] },
            { Key.Minus , [0x14] },
            { Key.Equal , [0x15, 0x7D] },
            { Key.Backspace , [0x16] },
            { Key.Tab , [0x17] },
            { Key.Q , [0x18] },
            { Key.W , [0x19] },
            { Key.E , [0x1A] },
            { Key.R , [0x1B] },
            { Key.T , [0x1C] },
            { Key.Y , [0x1D] },
            { Key.U , [0x1E] },
            { Key.I , [0x1F] },
            { Key.O , [0x20] },
            { Key.P , [0x21] },
            { Key.Bracketleft , [0x22] },
            { Key.Bracketright , [0x23] },
            { Key.Enter , [0x24, 0x6D] },
            { Key.Ctrl , [0x25, 0x69] },
            { Key.A , [0x26] },
            { Key.S , [0x27] },
            { Key.D , [0x28] },
            { Key.F , [0x29] },
            { Key.G , [0x2A] },
            { Key.H , [0x2B] },
            { Key.J , [0x2C] },
            { Key.K , [0x2D] },
            { Key.L , [0x2E] },
            { Key.Semicolon , [0x2F] },
            { Key.Apostrophe , [0x30] },
            { Key.Quoteleft , [0x31] },
            { Key.Shift , [0x32, 0x3E] },
            { Key.Backslash , [0x33] },
            { Key.Z , [0x34] },
            { Key.X , [0x35] },
            { Key.C , [0x36] },
            { Key.V , [0x37] },
            { Key.B , [0x38] },
            { Key.N , [0x39] },
            { Key.M , [0x3A] },
            { Key.Comma , [0x3B, 0x67, 0x81] },
            { Key.Period , [0x3C] },
            { Key.Slash , [0x3D] },
            { Key.KpMultiply , [0x3F] },
            { Key.Alt , [0x40, 0x6C] },
            { Key.Space , [0x41] },
            { Key.Capslock , [0x42] },
            { Key.F1 , [0x43] },
            { Key.F2 , [0x44] },
            { Key.F3 , [0x45] },
            { Key.F4 , [0x46] },
            { Key.F5 , [0x47] },
            { Key.F6 , [0x48] },
            { Key.F7 , [0x49] },
            { Key.F8 , [0x4A] },
            { Key.F9 , [0x4B] },
            { Key.F10 , [0x4C] },
            { Key.Numlock , [0x4D] },
            { Key.Scrolllock , [0x4E] },
            { Key.Kp7 , [0x4F] },
            { Key.Kp8 , [0x50] },
            { Key.Kp9 , [0x51] },
            { Key.KpSubtract , [0x52] },
            { Key.Kp4 , [0x53] },
            { Key.Kp5 , [0x54] },
            { Key.Kp6 , [0x55] },
            { Key.KpAdd , [0x56] },
            { Key.Kp1 , [0x57] },
            { Key.Kp2 , [0x58] },
            { Key.Kp3 , [0x59] },
            { Key.Kp0 , [0x5A] },
            { Key.KpPeriod , [0x5B] },
            { Key.Section , [0x5E] },
            { Key.F11 , [0x5F] },
            { Key.F12 , [0x60] },
            { Key.KpEnter , [0x68] },
            { Key.KpDivide , [0x6A] },
            { Key.Print , [0x6B] },
            { Key.Home , [0x6E] },
            { Key.Up , [0x6F] },
            { Key.Pageup , [0x70] },
            { Key.Left , [0x71] },
            { Key.Right , [0x72] },
            { Key.End , [0x73] },
            { Key.Down , [0x74] },
            { Key.Pagedown , [0x75] },
            { Key.Insert , [0x76] },
            { Key.Delete , [0x77] },
            { Key.Volumemute , [0x79] },
            { Key.Volumedown , [0x7A] },
            { Key.Volumeup , [0x7B] },
            { Key.Pause , [0x7F] },
            { Key.Launch0 , [0x80] },
            { Key.Yen , [0x84] },
            { Key.Meta , [0x85, 0x86] },
            { Key.Menu , [0x87] },
            { Key.Back , [0xA6] },
            { Key.Forward , [0xA7] },
            { Key.Refresh , [0xB5] },
            { Key.F13 , [0xBF] },
            { Key.F14 , [0xC0] },
            { Key.F15 , [0xC1] },
            { Key.F16 , [0xC2] },
            { Key.F17 , [0xC3] },
            { Key.F18 , [0xC4] },
            { Key.F19 , [0xC5] },
            { Key.F20 , [0xC6] },
            { Key.F21 , [0xC7] },
            { Key.F22 , [0xC8] },
            { Key.F23 , [0xC9] },
            { Key.F24 , [0xCA] },
            { Key.F25 , [0xCB] },
            { Key.F26 , [0xCC] },
            { Key.F27 , [0xCD] },
            { Key.F28 , [0xCE] },
            { Key.F29 , [0xCF] },
            { Key.F30 , [0xD0] },
            { Key.F31 , [0xD1] },
            { Key.F32 , [0xD2] },
            { Key.F33 , [0xD3] },
            { Key.F34 , [0xD4] },
            { Key.F35 , [0xD5] }
        }
    );
    #endregion



    /*
    // Functionalities for mapping between
    // logical (X Key Symbols or Key Codes) and physical keys (X Key Codes), and their string names.
    #region Key Code Translations
    /// <summary>
    /// Helper method to convert an X Key Code (representing a physical button on the keyboard)
    /// to an X Key Symbol (representing the actual character that programs use).
    /// <para/>
    /// This method is used to get a list of X Key Symbols
    /// that are currently pressed on the keyboard.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the keyboard of interest.
    /// </param>
    /// <param name="PhysicalKey">
    /// The X Key Code number representing a physical button on the keyboard.
    /// </param>
    /// <param name="Index">
    /// I have no idea what this is supposed to be, so I usually just set it to 0.
    /// </param>
    /// <returns>
    /// The X Key Symbol code for the actual character represented by the given phsyical button.
    /// </returns>
    [DllImport("libX11.so.6", EntryPoint = "XKeycodeToKeysym", SetLastError = true)]
    public static extern KeySym PhysicalKeyToKeyCode(DisplayHandle DisplayPtr, ScanCode PhysicalKey, int Index);

    /// <summary>
    /// Helper method to convert an X Key Symbol (representing the actual character that programs use)
    /// to an X Key Code (representing a physical button on the keyboard).
    /// <para/>
    /// This method is used to check if the physical button
    /// representing a specific symbol (as interpreted by the Display)
    /// is being pressed on the keyboard.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the keyboard of interest.
    /// </param>
    /// <param name="KeyCode">
    /// The X Key Symbol code for the physical button to check.
    /// </param>
    /// <returns>
    /// The X Key Code for the physical button corresponding to the given X Key Symbol.
    /// </returns>
    [DllImport("libX11.so.6", EntryPoint = "XKeysymToKeycode", SetLastError = true)]
    public static extern ScanCode KeyCodeToPhysicalKey(DisplayHandle DisplayPtr, KeySym KeyCode);


    /// <summary>
    /// Helper method to get the X Key Symbol code
    /// for the character (as interpreted by the Display) with the given name.
    /// <para/>
    /// This method is used for debugging purposes only.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the keyboard of interest.
    /// </param>
    /// <param name="KeyCodeName">
    /// The name of the given character.
    /// </param>
    /// <returns>
    /// The X Key Symbol code for the character with the given name.
    /// </returns>
    [DllImport("libX11.so.6", EntryPoint = "XStringToKeysym", SetLastError = true)]
    public static extern KeySym StringToKeyCode(DisplayHandle DisplayPtr, string KeyCodeName);

    /// <summary>
    /// Helper method to get the name of
    /// a given X Key Symbol character (as interpreted by the Display).
    /// <para/>
    /// This method is used for debugging purposes only.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the keyboard of interest.
    /// </param>
    /// <param name="KeyCode">
    /// The X Key Symbol code of the given character.
    /// </param>
    /// <returns>
    /// The name for the character that the Display interprets the given X Key Symbol as.
    /// </returns>
    [DllImport("libX11.so.6", EntryPoint = "XKeysymToString", SetLastError = true)]
    public static extern string KeyCodeToString(DisplayHandle DisplayPtr, KeySym KeyCode);
    #endregion
    */


    // Functionalities for querying
    // the relative position, overlapping windows, and button mask of a pointer.
    #region Pointer State Queries
    /// <summary>
    /// Helper method to get the state of the pointer in a given Display.
    /// </summary>
    /// <param name="DisplayPtr">
    /// Handle to the Display controlling the pointer of interest.
    /// </param>
    /// <param name="RefWindow">
    /// The Window to use as the origin
    /// from where the relative position of the pointer is obtained.
    /// </param>
    /// <param name="RootMouseOverlappedWindow">
    /// Memory to store the handle for the Root Window (or <c>0</c> if no such Windows exist)
    /// that the logical position of the pointer is currently overlapping with.
    /// </param>
    /// <param name="ChildMouseOverlappedWindow">
    /// Memory to store the handle for the Child Window (or <c>0</c> if no such Windows exist)
    /// that the logical position of the pointer is currently overlapping with.
    /// </param>
    /// <param name="RootRelativeMouseX">
    /// Memory to store the x-coordinate of the position of the pointer
    /// relative to <paramref name="RootMouseOverlappedWindow"/>.
    /// </param>
    /// <param name="RootRelativeMouseY">
    /// Memory to store the y-coordinate of the position of the pointer
    /// relative to <paramref name="RootMouseOverlappedWindow"/>.
    /// </param>
    /// <param name="RefRelativeMouseX">
    /// Memory to store the x-coordinate of the position of the pointer
    /// relative to <paramref name="RefWindow"/>.
    /// </param>
    /// <param name="RefRelativeMouseY">
    /// Memory to store the y-coordinate of the position of the pointer
    /// relative to <paramref name="RefWindow"/>.
    /// </param>
    /// <param name="PointerButtonMask">
    /// The X Button Mask for all the buttons presssed when this method is called.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="RefWindow"/>
    /// is on the same screen as the pointer;
    /// <c>false</c> otherwise.
    /// </returns>
    [DllImport("libX11.so.6", EntryPoint = "XQueryPointer", SetLastError = true)]
    public static extern bool QueryPointerState(
        DisplayHandle DisplayPtr, WindowHandle RefWindow,
        out WindowHandle RootMouseOverlappedWindow, out WindowHandle ChildMouseOverlappedWindow,
        out int RootRelativeMouseX, out int RootRelativeMouseY,
        out int RefRelativeMouseX, out int RefRelativeMouseY,
        out ButtonMask PointerButtonMask
    );

    
    // Match the X Button Mask values (for pointers) with the corresponding Godot Mouse Button Mask.
    // This was done using my own mouse, and may be inaccurate.

    /// <summary>
    /// A Dictionary mapping a <c>Godot.MouseButtonMas</c> to an X Button Mask
    /// that corresponds to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<MouseButtonMask, uint> GodotMouseToXButtonMask = new
    (
        new Dictionary<MouseButtonMask, uint>()
        {
            { MouseButtonMask.Left, 0x100 },        // X Button1Mask
            { MouseButtonMask.Right, 0x400 },       // X Button3Mask
            { MouseButtonMask.Middle, 0x200 },      // X Button2Mask
            { MouseButtonMask.MbXbutton1, 0x800 },  // X Button4Mask
            { MouseButtonMask.MbXbutton2, 0x1000 }  // X Button5Mask
        }
    );
    #endregion



    // Functionalities for taking a snapshot of the state of the keyboard and buttons.
    #region Snapshot
    /// <summary>
    /// A cache of the logical state (which buttons are pressed)
    /// of the physical keyboard in the Display as a 256-bit bit field.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the keyboard
    /// taken during the last call to <c>UpdateKeyStateCache</c>.
    /// </summary>
    private byte[] PhysicalKeyStateCache = new byte[32];

    /// <summary>
    /// Take a snapshot of the logical state of the physical keyboard in the Display
    /// and update the <c>PhysicalKeyStateCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateKeyStateCache(Window Target)
    {
        QueryPhysicialKeyPresses(
            (DisplayHandle)DisplayServer.WindowGetNativeHandle(
                DisplayServer.HandleType.DisplayHandle, Target.GetWindowId()
            ),
            PhysicalKeyStateCache
        );
    }


    /// <summary>
    /// A cache of the X Button Mask for all the buttons presssed in the Display.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the X Button Mask
    /// taken during the last call to <c>UpdateButtonMaskCache</c>.
    /// </summary>
    private ButtonMask ButtonMaskCache = 0;


    /// <summary>
    /// Take a snapshot of the X Button Mask in the Display
    /// and update the <c>ButtonMaskCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the button mask from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateButtonMaskCache(Window Target)
    {
        // Stub fields to hold return values from the function that we will not use/expose.
        WindowHandle RootMouseFocusedWindow = 0, ChildMouseFocusedWindow = 0;
        int RootRelativeMouseX = 0, RootRelativeMouseY = 0;
        int RefRelativeMouseX = 0, RefRelativeMouseY = 0;

        QueryPointerState(
            (DisplayHandle) DisplayServer.WindowGetNativeHandle(
                DisplayServer.HandleType.DisplayHandle, Target.GetWindowId()
            ),
            (WindowHandle) DisplayServer.WindowGetNativeHandle(
                DisplayServer.HandleType.WindowHandle, Target.GetWindowId()
            ),
            out RootMouseFocusedWindow, out ChildMouseFocusedWindow,
            out RootRelativeMouseX, out RootRelativeMouseY,
            out RefRelativeMouseX, out RefRelativeMouseY,
            out ButtonMaskCache
        );
    }


    /// <summary>
    /// Take a snapshot of the logical state
    /// of all Physical Keys and X Button Masks in the Display,
    /// and update the <c>PhysicalKeyStateCache</c> and <c>ButtonMaskCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard and button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateInputStateCache(Window Target)
    {
        UpdateKeyStateCache(Target);
        UpdateButtonMaskCache(Target);
    }
    #endregion



    // Functionalities for querying information
    // from a snapshot of the keyboard and buttons.
    #region Snapshot Query
    /// <summary>
    /// Check if a given <c>Godot.Key</c> is being pressed (held down)
    /// using the snapshot saved in the <c>PhysicalKeyStateCache</c>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.Key</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsKeyPressed(Key GodotKeyCode)
    {
        ScanCode[] XKeyCodes;
        // Check every X Key Code that may correspond to the given Godot Key,
        // since multiple X Key Codes may represent the same Godot Key.
        if (GodotToXKeySym.TryGetValue(GodotKeyCode, out XKeyCodes))
        {
            foreach (var XKeyCode in XKeyCodes)
            {
                // The corresponding byte index is floor(n / 8), which can be done by shifting right 3 bits.
                // The bit index within the byte can be found by taking the 3 least significant bits of n.
                // To check if the bit is on, we shift 1 by the bit index to get the check bit mask.
                if ((PhysicalKeyStateCache[XKeyCode >>> 3] & (1 << (XKeyCode & 7))) != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Get a list of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the <c>PhysicalKeyStateCache</c>.
    /// </summary>
    /// <returns>
    /// An <c>Array[Key]</c> of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the <c>PhysicalKeyStateCache</c>.
    /// </returns>
    public Godot.Collections.Array<Key> GetKeysPressed()
    {
        Godot.Collections.Array<Key> GodotKeysPressed = new();

        ScanCode XKeyCode = 0;
        Key GodotKey;

        // Check every bit (each corresponding to an X Key Code)
        // to see if the corresponding physical button is pressed.
        foreach (var KeyStatesGroup in PhysicalKeyStateCache)
        {
            // Store the X Key Code for the next KeyStatesGroup.
            ScanCode NextKeyCode = (ScanCode)(XKeyCode + 8);

            // Check every bit in the current KeyStatesGroup to see which physical button is pressed.
            var CheckBit = KeyStatesGroup;
            while (CheckBit != 0)
            {
                // If it is, convert it to the corresponding Godot.Key, and note it down.
                if ((CheckBit & 1) != 0)
                {
                    if (XKeyCodeToGodot.TryGetValue(XKeyCode, out GodotKey))
                    {
                        GodotKeysPressed.Add(GodotKey);
                    }
                }
                CheckBit >>= 1;
                ++XKeyCode;
            }
            XKeyCode = NextKeyCode;
        }

        return GodotKeysPressed;
    }



    /// <summary>
    /// Check if the mouse button corresponding to
    /// a given <c>Godot.MouseButtonMask</c> is being pressed (held down)
    /// using the snapshot saved in the <c>ButtonMaskCache</c>.
    /// <para/>
    /// <b>Note:</b>
    /// This method does not work when <paramref name="ButtonMask"/>
    /// is a combination of 2 or more <c>Godot.MouseButtonMask</c>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.MouseButtonMask</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsMouseButtonPressed(MouseButtonMask ButtonMask)
    {
        uint XMouseMask;
        if (GodotMouseToXButtonMask.TryGetValue(ButtonMask, out XMouseMask))
        {
            return (ButtonMaskCache & XMouseMask) > 0;
        }
        return false;
    }

    /// <summary>
    /// Get the <c>Godot.MouseButtonMask</c> being pressed (held down)
    /// in the snapshot saved in the <c>ButtonMaskCache</c>.
    /// </summary>
    /// <returns>
    /// The <c>Godot.MouseButtonMask</c>
    /// corresponding to the Godot Mouse Buttons being pressed (held down)
    /// in the snapshot saved in the <c>ButtonMaskCache</c>.
    /// </returns>
    public MouseButtonMask GetMouseButtonMask()
    {
        MouseButtonMask GodotMouseButtonMask = 0;

        // Check every Godot Mouse Mask to see
        // if the ButtonMaskCache contains the corresponding X Button Mask.
        foreach (KeyValuePair<MouseButtonMask, uint> GodotMouseXButtonPair in GodotMouseToXButtonMask)
        {
            // Note down the activated masks.
            if ((GodotMouseXButtonPair.Value & ButtonMaskCache) != 0)
            {
                GodotMouseButtonMask |= GodotMouseXButtonPair.Key;
            }
        }
        return GodotMouseButtonMask;
    }
    #endregion
#endregion





    #elif GODOT_MACOS
#region MACOS_IMPLEMENTATION
    // Import the relevant methods and constants from the MaciOS C# repository:
    // https://github.com/dotnet/macios/blob/70046dc7dbe2cbdfa3a571daa72c26edcd1cf6fc/src/ObjCRuntime/Constants.cs#L30
    // https://github.com/dotnet/macios/blob/70046dc7dbe2cbdfa3a571daa72c26edcd1cf6fc/src/CoreGraphics/CGEventSource.cs
    private const string CoreGraphicsDLL = (
        "/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreGraphics.framework/CoreGraphics"
    );



    // Functionalities for querying whether or not a key or button is pressed. 
    #region Key and Button State Queries
    /// <summary>
    /// Helper method to get the logical state
    /// (whether or not a button is pressed)
    /// of of a keyboard button.
    /// </summary>
    /// <param name="StateID">
    /// Specifies the scope to get the state of the key in, using 1 of 3 values below:
    /// <list type="bullet">
    /// <item>
    /// -1 (Private):
    /// Only key presses and releases performed while this application has focused
    /// are used to determine the state of a key.
    /// </item>
    /// <item>
    ///  0 (Combined Session):
    /// Only key presses and releases performed in the user session this application is in
    /// are used to determine the state of a key.
    /// </item>
    /// <item>
    ///  1 (HID System):
    /// All key presses and releases performed within the system
    /// are used to determine the state of a key.
    /// </item> 
    /// </list>
    /// </param>
    /// <param name="KeyCode">
    /// The MacOS Virtual Key code of the key to check.
    /// </param>
    /// <returns>
    /// A <c>true</c> if the key is being pressed; <c>false</c> otherwise.
    /// </returns>
    [DllImport(CoreGraphicsDLL, EntryPoint = "CGEventSourceKeyState", SetLastError = true)]
    extern static bool GetKeyState(EventSourceStateID StateID, ScanCode KeyCode);

    /// <summary>
    /// Helper method to get the logical state
    /// (whether or not a button is pressed)
    /// of of a mouse button.
    /// </summary>
    /// <param name="StateID">
    /// Specifies the scope to get the state of the button in, using 1 of 3 values below:
    /// <list type="bullet">
    /// <item>
    /// -1 (Private):
    /// Only button presses and releases performed while this application has focused
    /// are used to determine the state of a button.
    /// </item>
    /// <item>
    ///  0 (Combined Session):
    /// Only button presses and releases performed by the user using this application
    /// are used to determine the state of a button.
    /// </item>
    /// <item>
    ///  1 (HID System):
    /// All button presses and releases performed within the system
    /// are used to determine the state of a button.
    /// </item> 
    /// </list>
    /// </param>
    /// <param name="ButtonCode">
    /// Specifies the mouse button to check, using either 1 of 3 values below:
    /// <list type="bullet">
    /// <item>0 (Left Mouse Button)</item>
    /// <item>1 (Right Mouse Button)</item>
    /// <item>2 (Middle Mouse Button)</item>
    /// </list>
    /// </param>
    /// <returns>
    /// A <c>true</c> if the key is being pressed; <c>false</c> otherwise.
    /// </returns>
    [DllImport(CoreGraphicsDLL, EntryPoint = "CGEventSourceButtonState", SetLastError = true)]
    extern static bool GetButtonState(EventSourceStateID StateID, MouseButtonCode ButtonCode);


    /// <summary>
    /// A Dictionary mapping a MacOS Virtual Key to a list of <c>Godot.Key</c>s
    /// corresponding to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<ScanCode, Key[]> MacVKeyToGodot = new
    (
        new Dictionary<ScanCode, Key[]>()
        {
            { 0x001b , [Key.Escape] },
            { 0x0009 , [Key.Tab] },
            { 0x007f , [Key.Backtab, Key.Delete] },
            { 0x0008 , [Key.Backspace] },
            { 0x000d , [Key.Enter] },
            { 0xF727 , [Key.Insert] },
            { 0xF730 , [Key.Pause] },
            { 0xF72E , [Key.Print] },
            { 0xF731 , [Key.Sysreq] },
            { 0xF739 , [Key.Clear, Key.Numlock] },
            { 0x2196 , [Key.Home] },
            { 0x2198 , [Key.End] },
            { 0x001c , [Key.Left] },
            { 0x001e , [Key.Up] },
            { 0x001d , [Key.Right] },
            { 0x001f , [Key.Down] },
            { 0x21de , [Key.Pageup] },
            { 0x21df , [Key.Pagedown] },
            { 0xF72F , [Key.Scrolllock] },
            { 0xF704 , [Key.F1] },
            { 0xF705 , [Key.F2] },
            { 0xF706 , [Key.F3] },
            { 0xF707 , [Key.F4] },
            { 0xF708 , [Key.F5] },
            { 0xF709 , [Key.F6] },
            { 0xF70A , [Key.F7] },
            { 0xF70B , [Key.F8] },
            { 0xF70C , [Key.F9] },
            { 0xF70D , [Key.F10] },
            { 0xF70E , [Key.F11] },
            { 0xF70F , [Key.F12] },
            { 0xF710 , [Key.F13] },
            { 0xF711 , [Key.F14] },
            { 0xF712 , [Key.F15] },
            { 0xF713 , [Key.F16] },
            { 0xF714 , [Key.F17] },
            { 0xF715 , [Key.F18] },
            { 0xF716 , [Key.F19] },
            { 0xF717 , [Key.F20] },
            { 0xF718 , [Key.F21] },
            { 0xF719 , [Key.F22] },
            { 0xF71A , [Key.F23] },
            { 0xF71B , [Key.F24] },
            { 0xF71C , [Key.F25] },
            { 0xF71D , [Key.F26] },
            { 0xF71E , [Key.F27] },
            { 0xF71F , [Key.F28] },
            { 0xF720 , [Key.F29] },
            { 0xF721 , [Key.F30] },
            { 0xF722 , [Key.F31] },
            { 0xF723 , [Key.F32] },
            { 0xF724 , [Key.F33] },
            { 0xF725 , [Key.F34] },
            { 0xF726 , [Key.F35] },
            { 0xF735 , [Key.Menu] },
            { 0xF746 , [Key.Help] },
            { 0xF734 , [Key.Stop] },
            { 0xF736 , [Key.Launch0] },
            { 0x0020 , [Key.Space] },
            { '!' , [Key.Exclam] },
            { '\"' , [Key.Quotedbl] },
            { '#' , [Key.Numbersign] },
            { '$' , [Key.Dollar] },
            { '%' , [Key.Percent] },
            { '&' , [Key.Ampersand] },
            { '\'' , [Key.Apostrophe] },
            { '(' , [Key.Parenleft] },
            { ')' , [Key.Parenright] },
            { '*' , [Key.Asterisk] },
            { '+' , [Key.Plus] },
            { ',' , [Key.Comma] },
            { '-' , [Key.Minus] },
            { '.' , [Key.Period] },
            { '/' , [Key.Slash] },
            { '0' , [Key.Key0] },
            { '1' , [Key.Key1] },
            { '2' , [Key.Key2] },
            { '3' , [Key.Key3] },
            { '4' , [Key.Key4] },
            { '5' , [Key.Key5] },
            { '6' , [Key.Key6] },
            { '7' , [Key.Key7] },
            { '8' , [Key.Key8] },
            { '9' , [Key.Key9] },
            { ':' , [Key.Colon] },
            { ';' , [Key.Semicolon] },
            { '<' , [Key.Less] },
            { '=' , [Key.Equal] },
            { '>' , [Key.Greater] },
            { '?' , [Key.Question] },
            { '@' , [Key.At] },
            { 'a' , [Key.A] },
            { 'b' , [Key.B] },
            { 'c' , [Key.C] },
            { 'd' , [Key.D] },
            { 'e' , [Key.E] },
            { 'f' , [Key.F] },
            { 'g' , [Key.G] },
            { 'h' , [Key.H] },
            { 'i' , [Key.I] },
            { 'j' , [Key.J] },
            { 'k' , [Key.K] },
            { 'l' , [Key.L] },
            { 'm' , [Key.M] },
            { 'n' , [Key.N] },
            { 'o' , [Key.O] },
            { 'p' , [Key.P] },
            { 'q' , [Key.Q] },
            { 'r' , [Key.R] },
            { 's' , [Key.S] },
            { 't' , [Key.T] },
            { 'u' , [Key.U] },
            { 'v' , [Key.V] },
            { 'w' , [Key.W] },
            { 'x' , [Key.X] },
            { 'y' , [Key.Y] },
            { 'z' , [Key.Z] },
            { '[' , [Key.Bracketleft] },
            { '\\' , [Key.Backslash] },
            { ']' , [Key.Bracketright] },
            { '^' , [Key.Asciicircum] },
            { '_' , [Key.Underscore] },
            { '`' , [Key.Quoteleft] },
            { '{' , [Key.Braceleft] },
            { '|' , [Key.Bar] },
            { '}' , [Key.Braceright] },
            { '~' , [Key.Asciitilde] }
        }
    );

    /// <summary>
    /// A Dictionary mapping a <c>Godot.Key</c> to a corresponding MacOS Virtual Key
    /// (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<Key, ScanCode> GodotToMacVKey = new
    (
        new Dictionary<Key, ScanCode>()
        {
            { Key.Escape , 0x001b },
            { Key.Tab , 0x0009 },
            { Key.Backtab , 0x007f },
            { Key.Backspace , 0x0008 },
            { Key.Enter , 0x000d },
            { Key.Insert , 0xF727 },
            { Key.Delete , 0x007f },
            { Key.Pause , 0xF730 },
            { Key.Print , 0xF72E },
            { Key.Sysreq , 0xF731 },
            { Key.Clear , 0xF739 },
            { Key.Home , 0x2196 },
            { Key.End , 0x2198 },
            { Key.Left , 0x001c },
            { Key.Up , 0x001e },
            { Key.Right , 0x001d },
            { Key.Down , 0x001f },
            { Key.Pageup , 0x21de },
            { Key.Pagedown , 0x21df },
            { Key.Numlock , 0xF739 },
            { Key.Scrolllock , 0xF72F },
            { Key.F1 , 0xF704 },
            { Key.F2 , 0xF705 },
            { Key.F3 , 0xF706 },
            { Key.F4 , 0xF707 },
            { Key.F5 , 0xF708 },
            { Key.F6 , 0xF709 },
            { Key.F7 , 0xF70A },
            { Key.F8 , 0xF70B },
            { Key.F9 , 0xF70C },
            { Key.F10 , 0xF70D },
            { Key.F11 , 0xF70E },
            { Key.F12 , 0xF70F },
            { Key.F13 , 0xF710 },
            { Key.F14 , 0xF711 },
            { Key.F15 , 0xF712 },
            { Key.F16 , 0xF713 },
            { Key.F17 , 0xF714 },
            { Key.F18 , 0xF715 },
            { Key.F19 , 0xF716 },
            { Key.F20 , 0xF717 },
            { Key.F21 , 0xF718 },
            { Key.F22 , 0xF719 },
            { Key.F23 , 0xF71A },
            { Key.F24 , 0xF71B },
            { Key.F25 , 0xF71C },
            { Key.F26 , 0xF71D },
            { Key.F27 , 0xF71E },
            { Key.F28 , 0xF71F },
            { Key.F29 , 0xF720 },
            { Key.F30 , 0xF721 },
            { Key.F31 , 0xF722 },
            { Key.F32 , 0xF723 },
            { Key.F33 , 0xF724 },
            { Key.F34 , 0xF725 },
            { Key.F35 , 0xF726 },
            { Key.Menu , 0xF735 },
            { Key.Help , 0xF746 },
            { Key.Stop , 0xF734 },
            { Key.Launch0 , 0xF736 },
            { Key.Space , 0x0020 },
            { Key.Exclam , '!' },
            { Key.Quotedbl , '\"' },
            { Key.Numbersign , '#' },
            { Key.Dollar , '$' },
            { Key.Percent , '%' },
            { Key.Ampersand , '&' },
            { Key.Apostrophe , '\'' },
            { Key.Parenleft , '(' },
            { Key.Parenright , ')' },
            { Key.Asterisk , '*' },
            { Key.Plus , '+' },
            { Key.Comma, ',' },
            { Key.Minus , '-' },
            { Key.Period , '.' },
            { Key.Slash , '/' },
            { Key.Key0 , '0' },
            { Key.Key1 , '1' },
            { Key.Key2 , '2' },
            { Key.Key3 , '3' },
            { Key.Key4 , '4' },
            { Key.Key5 , '5' },
            { Key.Key6 , '6' },
            { Key.Key7 , '7' },
            { Key.Key8 , '8' },
            { Key.Key9 , '9' },
            { Key.Colon , ':' },
            { Key.Semicolon , ';' },
            { Key.Less , '<' },
            { Key.Equal , '=' },
            { Key.Greater , '>' },
            { Key.Question , '?' },
            { Key.At , '@' },
            { Key.A , 'a' },
            { Key.B , 'b' },
            { Key.C , 'c' },
            { Key.D , 'd' },
            { Key.E , 'e' },
            { Key.F , 'f' },
            { Key.G , 'g' },
            { Key.H , 'h' },
            { Key.I , 'i' },
            { Key.J , 'j' },
            { Key.K , 'k' },
            { Key.L , 'l' },
            { Key.M , 'm' },
            { Key.N , 'n' },
            { Key.O , 'o' },
            { Key.P , 'p' },
            { Key.Q , 'q' },
            { Key.R , 'r' },
            { Key.S , 's' },
            { Key.T , 't' },
            { Key.U , 'u' },
            { Key.V , 'v' },
            { Key.W , 'w' },
            { Key.X , 'x' },
            { Key.Y , 'y' },
            { Key.Z , 'z' },
            { Key.Bracketleft , '[' },
            { Key.Backslash , '\\' },
            { Key.Bracketright , ']' },
            { Key.Asciicircum , '^' },
            { Key.Underscore , '_' },
            { Key.Quoteleft , '`' },
            { Key.Braceleft , '{' },
            { Key.Bar , '|' },
            { Key.Braceright , '}' },
            { Key.Asciitilde , '~' }
        }
    );


    /// <summary>
    /// A Dictionary mapping a <c>Godot.MouseButtonMas</c> to a MacOS Mouse Button
    /// that corresponds to it (according to the Engine's source code).
    /// </summary>
    public static readonly ReadOnlyDictionary<MouseButtonMask, MouseButtonCode> GodotMouseToMacButton = new
    (
        new Dictionary<MouseButtonMask, MouseButtonCode>()
        {
            { MouseButtonMask.Left, 0 },
            { MouseButtonMask.Right, 1 },
            { MouseButtonMask.Middle, 2 }
        }
    );
    #endregion



    // Functionalities for taking a snapshot of the state of the keyboard and buttons.
    #region Snapshot
    /// <summary>
    /// A cache of the set of <c>Godot.Key</c>s pressed.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the keyboard
    /// taken during the last call to <c>UpdateKeyStateCache</c>.
    /// </summary>
    private HashSet<Key> GodotKeyStateCache = new();

    /// <summary>
    /// Take a snapshot of the logical state
    /// of the physical keyboard in the current user session
    /// and update the <c>KeyStateCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On MacOS, this has no effects.
    /// </param>
    public void UpdateKeyStateCache(Window Target)
    {
        GodotKeyStateCache.Clear();
        foreach (var GodotMacVKeyPair in MacVKeyToGodot)
        {
            if (GetKeyState(0, GodotMacVKeyPair.Key))
            {
                GodotKeyStateCache.Concat(GodotMacVKeyPair.Value);
            }
        }
    }


    /// <summary>
    /// A cache of the <c>Godot.MouseButtonMask</c> for all mouse buttons presssed.
    /// <para/>
    /// <b>Note:</b>
    /// This cache is a snapshot of the Godot Mouse Button Mask
    /// taken during the last call to <c>UpdateButtonMaskCache</c>.
    /// </summary>
    private MouseButtonMask GodotMouseMaskCache = 0;

    /// <summary>
    /// Take a snapshot of the Buttons being pressed
    /// in the current user session
    /// and update the <c>GodotMouseMaskCache</c> with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On MacOS, this has no effects.
    /// </param>
    public void UpdateButtonMaskCache(Window Target)
    {
        GodotMouseMaskCache = 0;
        foreach (var GodotMouseMacButtonPair in GodotMouseToMacButton)
        {
            if (GetButtonState(0, GodotMouseMacButtonPair.Value))
            {
                GodotMouseMaskCache |= GodotMouseMacButtonPair.Key;
            }
        }
    }


    /// <summary>
    /// Take a snapshot of the logical state of all input devices
    /// and update the cache with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard and button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// <para/>
    /// On MacOS, this does nothing.
    /// </param>
    public void UpdateInputStateCache(Window Target)
    {
        UpdateKeyStateCache(Target);
    }
    #endregion



    // Functionalities for querying information
    // from a snapshot of the keyboard and buttons.
    #region Snapshot Queries
    /// <summary>
    /// Check if a given <c>Godot.Key</c> is being pressed (held down)
    /// using the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.Key</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsKeyPressed(Key GodotKeyCode)
    {
        return GodotKeyStateCache.Contains(GodotKeyCode);
    }

    /// <summary>
    /// Get a list of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// An <c>Array[Key]</c> of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </returns>
    public Godot.Collections.Array<Key> GetKeysPressed()
    {
        return [.. GodotKeyStateCache];
    }



    /// <summary>
    /// Check if any of the mouse buttons corresponding to
    /// a given <c>Godot.MouseButtonMask</c> is being pressed (held down)
    /// using the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// <c>true</c> if any button within the given <c>Godot.MouseButtonMask</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsMouseButtonPressed(MouseButtonMask ButtonMask)
    {
        return (GodotMouseMaskCache & ButtonMask) != 0;
    }

    /// <summary>
    /// Get the <c>Godot.MouseButtonMask</c> being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// The <c>Godot.MouseButtonMask</c>
    /// corresponding to the Godot Mouse Buttons being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </returns>
    public MouseButtonMask GetMouseButtonMask()
    {
        return GodotMouseMaskCache;
    }
    #endregion
#endregion





    #else
#region UNIMPLEMENTABLE_STUBS
    /// <summary>
    /// Take a snapshot of the logical state of the physical keyboard
    /// and update the keyboard input state cache with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateKeyStateCache(Window Target) {}


    /// <summary>
    /// Take a snapshot of the Buttons being pressed
    /// and update the mouse input state cache with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateButtonMaskCache(Window Target) {}


    /// <summary>
    /// Take a snapshot of the logical state of all input devices
    /// and update the input state cache with it.
    /// </summary>
    /// <param name="Target">
    /// The window to capture the physical keyboard and button masks from.
    /// <para/>
    /// On some OS such as Linux, Windows may reside in different Display Servers
    /// and detect different user inputs.
    /// </param>
    public void UpdateInputStateCache(Window Target) {}



    /// <summary>
    /// Check if a given <c>Godot.Key</c> is being pressed (held down)
    /// using the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.Key</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsKeyPressed(Key GodotKeyCode) { return false; }

    /// <summary>
    /// Get a list of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// An <c>Array[Key]</c> of <c>Godot.Key</c> is being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </returns>
    public Godot.Collections.Array<Key> GetKeysPressed() { return new Godot.Collections.Array<Key>(); }



    /// <summary>
    /// Check if the mouse button corresponding to
    /// a given <c>Godot.MouseButtonMask</c> is being pressed (held down)
    /// using the snapshot saved in the cache.
    /// <para/>
    /// <b>Note:</b>
    /// This method does not work when <paramref name="ButtonMask"/>
    /// is a combination of 2 or more <c>Godot.MouseButtonMask</c>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <c>Godot.MouseButtonMask</c> is being pressed;
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsMouseButtonPressed(MouseButtonMask ButtonMask) { return false; }

    /// <summary>
    /// Get the <c>Godot.MouseButtonMask</c> being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </summary>
    /// <returns>
    /// The <c>Godot.MouseButtonMask</c>
    /// corresponding to the Godot Mouse Buttons being pressed (held down)
    /// in the snapshot saved in the cache.
    /// </returns>
    public MouseButtonMask GetMouseButtonMask(){ return 0; }
#endregion
    #endif
}
