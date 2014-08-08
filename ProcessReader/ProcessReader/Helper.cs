using System.Runtime.InteropServices;

namespace ProcessReader
{
    /// <summary>
    /// This class provides a helper-function that marshals a byte-array into a struct
    /// Sourcecode from http://stackoverflow.com/questions/14465722/which-marshalling-method-is-better
    /// </summary>
    class Helper
    {
        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return result;
        }
    }
}
