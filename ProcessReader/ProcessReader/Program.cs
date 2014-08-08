using System;
using System.Diagnostics;
using System.Linq;

namespace ProcessReader
{
    class Program
    {
        /// <summary>
        /// The mainfunction. It searches for the targetapplication, creates a wrapper object 
        /// and uses that to access the internal information of the target application
        /// </summary>
        static void Main()
        {
            Process targetProcess = Process.GetProcessesByName("ExampleClient").First(); //find the application to read from
            if (targetProcess == null)
            {
                //if it is not found, write a message and abort further execution
                throw new Exception("Targetapplication not found. Aborting");
            }
            var w = new Wrapper(targetProcess); //create a wrapper-object to access the internal information of the target application

            foreach (var l in w.Values)
            {
                //write all the measured values to the console so we can see that reading worked successfully
                Console.WriteLine("Timestamp:{0}\tValue1:{1}\tValue2:{2}", l.Timestamp, l.Value1, l.Value2);
            }
            w = null;//free the handle
            Console.WriteLine("To close the Application press Enter");
            Console.ReadLine();


        }
    }
}
