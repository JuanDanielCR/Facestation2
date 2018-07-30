using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class SlaveControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get slave device", getSlaveDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set slave device", setSlaveDevice));

            return functionList;
        }

        public void getSlaveDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveDevice(sdkContext, deviceID, out slaveDeviceObj, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                API.BS2_ReleaseObject(slaveDeviceObj);

                foreach (BS2Rs485SlaveDevice slaveDevice in slaveDeviceList)
                {
                    print(sdkContext, slaveDevice);
                }

                slaveControl(sdkContext, slaveDeviceList);
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        public void setSlaveDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveDevice(sdkContext, deviceID, out slaveDeviceObj, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }                

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice slaveDevice = slaveDeviceList[(int)idx];
                    Console.WriteLine("[{0:000}] ==> SlaveDevice id[{1, 10}] type[{2, 3}] model[{3, 16}] enable[{4}], connected[{5}]",
                                idx,
                                slaveDevice.deviceID,
                                slaveDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType],
                                Convert.ToBoolean(slaveDevice.enableOSDP),
                                Convert.ToBoolean(slaveDevice.connected));
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Enter the index of the slave device which you want to connect: [INDEX_1,INDEX_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] slaveDeviceIndexs = Console.ReadLine().Split(delimiterChars);
                HashSet<UInt32> connectSlaveDevice = new HashSet<UInt32>();

                if (slaveDeviceIndexs.Length == 0)
                {
                    Console.WriteLine("All of the slave device will be disabled.");
                }
                else
                {
                    foreach (string slaveDeviceIndex in slaveDeviceIndexs)
                    {
                        if (slaveDeviceIndex.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(slaveDeviceIndex, out item))
                            {
                                if (item < slaveDeviceCount)
                                {
                                    connectSlaveDevice.Add(slaveDeviceList[(int)item].deviceID);
                                }
                            }
                        }
                    }
                }

                curSlaveDeviceObj = slaveDeviceObj;
                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));

                    if (connectSlaveDevice.Contains(item.deviceID))
                    {
                        if (item.enableOSDP != 1)
                        {
                            item.enableOSDP = 1;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }
                    else
                    {
                        if (item.enableOSDP != 0)
                        {
                            item.enableOSDP = 0;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }

                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                Console.WriteLine("Trying to set the slave devices.");
                result = (BS2ErrorCode)API.BS2_SetSlaveDevice(sdkContext, deviceID, slaveDeviceObj, slaveDeviceCount);

                API.BS2_ReleaseObject(slaveDeviceObj);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    slaveControl(sdkContext, slaveDeviceList);
                }
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        void slaveControl(IntPtr sdkContext, List<BS2Rs485SlaveDevice> slaveDeviceList)
        {
            //TODO implement this section.
        }

        void print(IntPtr sdkContext, BS2Rs485SlaveDevice slaveDevice)
        {
            Console.WriteLine(">>>> SlaveDevice id[{0, 10}] type[{1, 3}] model[{2, 16}] enable[{3}], connected[{4}]", 
                                slaveDevice.deviceID, 
                                slaveDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType],
                                Convert.ToBoolean(slaveDevice.enableOSDP),
                                Convert.ToBoolean(slaveDevice.connected));
        }
    }
}
