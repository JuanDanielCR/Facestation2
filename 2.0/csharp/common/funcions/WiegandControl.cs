using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class WiegandControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Search wiegand device", searchWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get wiegand device", getWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Add wiegand device", addWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove wiegand device", removeWiegandDevice));

            return functionList;
        }

        public void searchWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr wiegandDeviceObj = IntPtr.Zero;
            UInt32 numWiegandDevice = 0;

            Console.WriteLine("Trying to search the wiegand devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SearchWiegandDevices(sdkContext, deviceID, out wiegandDeviceObj, out numWiegandDevice);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numWiegandDevice > 0)
            {
                for (int idx = 0; idx < numWiegandDevice; ++idx)
                {
                    UInt32 wiegandDeviceID = Convert.ToUInt32(Marshal.ReadInt32(wiegandDeviceObj, (int)idx * sizeof(UInt32)));
                    Console.WriteLine(">>>> WiegandDevice id[{0, 10}]", wiegandDeviceID);
                }

                API.BS2_ReleaseObject(wiegandDeviceObj);
            }
            else
            {
                Console.WriteLine(">>> There is no wiegand device in the device.");
            }
        }

        public void getWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr wiegandDeviceObj = IntPtr.Zero;
            UInt32 numWiegandDevice = 0;

            Console.WriteLine("Trying to get the wiegand devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetWiegandDevices(sdkContext, deviceID, out wiegandDeviceObj, out numWiegandDevice);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numWiegandDevice > 0)
            {
                for (int idx = 0; idx < numWiegandDevice; ++idx)
                {
                    UInt32 wiegandDeviceID = Convert.ToUInt32(Marshal.ReadInt32(wiegandDeviceObj, (int)idx * sizeof(UInt32)));
                    Console.WriteLine(">>>> WiegandDevice id[{0, 10}]", wiegandDeviceID);
                }

                API.BS2_ReleaseObject(wiegandDeviceObj);
            }
            else
            {
                Console.WriteLine(">>> There is no wiegand device in the device.");
            }
        }

        public void addWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the wiegand device which you want to add: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] wiegandDeviceIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> wiegandDeviceIDList = new List<UInt32>();

            foreach (string holidayGroupID in wiegandDeviceIDs)
            {
                if (holidayGroupID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(holidayGroupID, out item))
                    {
                        wiegandDeviceIDList.Add(item);
                    }
                }
            }

            if (wiegandDeviceIDList.Count > 0)
            {
                IntPtr wiegandDeviceIDObj = Marshal.AllocHGlobal(sizeof(UInt32) * wiegandDeviceIDList.Count);
                for (int idx = 0; idx < wiegandDeviceIDList.Count; ++idx)
                {
                    Marshal.WriteInt32(wiegandDeviceIDObj, idx * sizeof(UInt32), (int)wiegandDeviceIDList[idx]);
                }

                Console.WriteLine("Trying to add the wiegand devices.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_AddWiegandDevices(sdkContext, deviceID, wiegandDeviceIDObj, (UInt32)wiegandDeviceIDList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(wiegandDeviceIDObj);
            }
        }

        public void removeWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the wiegand device which you want to remove: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] wiegandDeviceIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> wiegandDeviceIDList = new List<UInt32>();

            foreach (string holidayGroupID in wiegandDeviceIDs)
            {
                if (holidayGroupID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(holidayGroupID, out item))
                    {
                        wiegandDeviceIDList.Add(item);
                    }
                }
            }

            if (wiegandDeviceIDList.Count > 0)
            {
                IntPtr wiegandDeviceIDObj = Marshal.AllocHGlobal(sizeof(UInt32) * wiegandDeviceIDList.Count);
                for (int idx = 0; idx < wiegandDeviceIDList.Count; ++idx)
                {
                    Marshal.WriteInt32(wiegandDeviceIDObj, idx * sizeof(UInt32), (int)wiegandDeviceIDList[idx]);
                }

                Console.WriteLine("Trying to remove the wiegand devices.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_RemoveWiegandDevices(sdkContext, deviceID, wiegandDeviceIDObj, (UInt32)wiegandDeviceIDList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(wiegandDeviceIDObj);
            }
        }
    }
}
