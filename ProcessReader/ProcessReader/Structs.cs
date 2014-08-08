using System;
using System.Runtime.InteropServices;

namespace ProcessReader.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct LinkedList
    {
        public IntPtr ListStart;
        public uint Length;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct ListStart
    {
        public IntPtr First;
        public IntPtr Last;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct MeasuredValue
    {
        public int Timestamp;
        public float Value1;
        public float Value2;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct ListElement
    {
        public IntPtr Next;
        public IntPtr Previous;
        public IntPtr Content;
    }

}