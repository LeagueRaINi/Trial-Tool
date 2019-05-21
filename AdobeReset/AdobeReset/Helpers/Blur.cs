using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AdobeReset.Interop;

namespace AdobeReset.Helpers
{
    public static class Blur
    {
        public static void ApplyBlur(Window window)
        {
            var accent = new User32.AccentPolicy {
                AccentState = User32.AccentState.EnableBlurBehind,
                AccentFlags = 2
            };

            var accentPtr = IntPtr.Zero;
            var accentSize = Marshal.SizeOf(accent);

            try {
                accentPtr = Marshal.AllocHGlobal(accentSize);

                Marshal.StructureToPtr(accent, accentPtr, false);

                var wcaData = new User32.WindowCompositionAttributeData {
                    Attribute = User32.WindowCompositionAttribute.AccentPolicy,
                    SizeOfData = accentSize,
                    Data = accentPtr,
                };

                User32.SetWindowCompositionAttribute(new WindowInteropHelper(window).Handle, ref wcaData);
            }
            finally {
                if (accentPtr != IntPtr.Zero) {
                    Marshal.FreeHGlobal(accentPtr);
                }
            }
        }
    }
}
