using System;
using System.Runtime.InteropServices;

namespace OnigRegex
{
    internal unsafe class OnigInterop
    {
        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr onigwrap_create(string pattern, int len, int ignoreCase, int multiline);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void onigwrap_region_free(IntPtr region);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void onigwrap_free(IntPtr regex);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int onigwrap_index_in(IntPtr regex, string text, int offset, int length);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr onigwrap_search(IntPtr regex, string text, int offset, int length);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int onigwrap_num_regs(IntPtr region);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int onigwrap_pos(IntPtr region, int nth);

        [DllImport("libonigwrap", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int onigwrap_len(IntPtr region, int nth);
    }
}
