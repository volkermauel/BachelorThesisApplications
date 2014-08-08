using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProcessReader.Structs;

namespace ProcessReader
{
    /// <summary>
    /// This class hides the WinAPI calls to OpenProcess, CloseHandle and ReadProcessMemory and provides an API to access the applications information
    /// </summary>
    internal class Wrapper
    {
        private IntPtr ProcessHandle { get; set; }
        private IntPtr BaseAddr { get; set; }

        /// <summary>
        /// Creates a new instance of the Wrapper-Class and implicitly calls OpenProcess
        /// </summary>
        /// <param name="targetProcess">The Process to read from</param>
        public Wrapper(Process targetProcess)
        {
            ProcessHandle = Imports.OpenProcess(Imports.PROCESS_WM_READ, false, targetProcess.Id);
            BaseAddr = targetProcess.MainModule.BaseAddress;
        }

        /// <summary>
        /// Closes the handle to the process
        /// </summary>
        ~Wrapper()
        {
            Imports.CloseHandle(ProcessHandle);
        }

        /// <summary>
        /// Returns the Loop Counter of the Application
        /// </summary>
        public uint Count
        {
            get
            {
                var buffer = new byte[4];
                int readBytes=0;
                Imports.ReadProcessMemory(ProcessHandle, IntPtr.Add(BaseAddr,Offsets.CountingVariable), buffer, 4, ref readBytes);
                if (readBytes == 4)
                    return BitConverter.ToUInt32(buffer, 0);
                return 0;
            }
        }

        /// <summary>
        /// Reads the lists information and marshals it into structs
        /// </summary>
        public unsafe LinkedList List
        {
            get
            {
                var buffer = new byte[sizeof(LinkedList)];
                int readBytes = 0;
                Imports.ReadProcessMemory(ProcessHandle, IntPtr.Add(BaseAddr, Offsets.LinkedList), buffer, buffer.Length, ref readBytes);
                if (readBytes == buffer.Length)
                    return Helper.ByteArrayToStructure<LinkedList>(buffer);
                return new LinkedList();
                
            }
        }

        /// <summary>
        /// Returns the ListStart containing the First and Last-Pointer of the list
        /// </summary>
        private unsafe ListStart Liststart
        {
            get
            {
                var buffer = new byte[sizeof(ListStart)];
                int readBytes = 0;
                Imports.ReadProcessMemory(ProcessHandle, List.ListStart, buffer, buffer.Length, ref readBytes);
                if (readBytes == buffer.Length)
                    return Helper.ByteArrayToStructure<ListStart>(buffer);
                return new ListStart();
            }
        }

        /// <summary>
        /// Reads a ListElement and returns it. On failure it returns a new (empty) element.
        /// </summary>
        /// <param name="addr">The address to read from</param>
        /// <returns>The read element</returns>
        private unsafe ListElement ReadListElement(IntPtr addr)
        {
            var buffer = new byte[sizeof(ListElement)];
            int readBytes = 0;
            Imports.ReadProcessMemory(ProcessHandle, addr, buffer, buffer.Length, ref readBytes);
            if (readBytes == buffer.Length)
                return Helper.ByteArrayToStructure<ListElement>(buffer);
            return new ListElement();
        }

        /// <summary>
        /// Read the MeasuredValue and returns it. On failure it retuns a new (empty) element.
        /// </summary>
        /// <param name="addr">The address to read frm</param>
        /// <returns>The read element</returns>
        private unsafe MeasuredValue ReadMeasuredValue(IntPtr addr)
        {
            var buffer = new byte[sizeof(MeasuredValue)];
            int readBytes = 0;
            Imports.ReadProcessMemory(ProcessHandle, addr, buffer, buffer.Length, ref readBytes);
            if (readBytes == buffer.Length)
                return Helper.ByteArrayToStructure<MeasuredValue>(buffer);
            return new MeasuredValue();
        }

        /// <summary>
        /// Returns a List<> of all MeasuredValues inside the TargetApplication
        /// </summary>
        public List<MeasuredValue> Values
        {
            get
            {
                var count = (int) Count;
                var result = new List<MeasuredValue>(count);
                var first = ReadListElement(Liststart.First);
                result.Add(ReadMeasuredValue(first.Content));
                for (int i = 1; i < count; i++)
                {
                    first = ReadListElement(first.Next);
                    result.Add(ReadMeasuredValue(first.Content));
                }
                return result;
            }
        }


    }
}
