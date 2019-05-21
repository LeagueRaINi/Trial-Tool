using System;
using System.Runtime.InteropServices;

namespace AdobeReset.Interop
{
    public static class User32
    {
        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowCompositionAttribute(
            IntPtr                             hwnd,
            ref WindowCompositionAttributeData windowCompositionAttributeData
        );

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        #endregion

        #region Enums

        public enum WindowCompositionAttribute
        {
            Undefined,
            NCRenderingEnabled,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRTLLayout,
            ForceIconicRepresentation,
            ExtendedFrameBounds,
            HasIconicBitmap,
            ThemeAttributes,
            NCRenderingExiled,
            NCAdornmentInfo,
            ExcludedFromLivePreview,
            VideoOverlayActive,
            ForceActiveWindowAppearance,
            DisallowPeek,
            Cloak,
            Cloaked,
            AccentPolicy,
            FreezeRepresentation,
            EverUncloaded,
            VisualOwner,
            Last
        }

        public enum AccentState
        {
            Disabled,
            EnableGradient,
            EnableTransparentGradient,
            EnableBlurBehind,
            InvalidState
        }

        #endregion
    }
}
