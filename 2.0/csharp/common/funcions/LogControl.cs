using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Data.SQLite;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Suprema
{
    public class LogEld
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class LogControl : FunctionModule
    {
        static HttpClient client = new HttpClient(); // Connection with an ELD Servlet
        private API.OnLogReceived cbOnLogReceived = null; //To prevent garbage collection
        
        static LogControl()
        {
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get log", getLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Filtering log", getFilteredLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Clear log", clearLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Monitoring log", monitoringLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get image log", getImageLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Synchronization between local and device log", syncLocalAndDeviceLog));
            return functionList;
        }

        void getLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            const UInt32 defaultLogPageSize = 1024;
            Type structureType = typeof(BS2Event);
            int structSize = Marshal.SizeOf(structureType);
            bool getAllLog = false;
            UInt32 lastEventId = 0;
            UInt32 amount;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;
            cbOnLogReceived = new API.OnLogReceived(NormalLogReceived);

            Console.WriteLine("What is the ID of the last log which you have? [0: None]");
            Console.Write(">>>> ");
            lastEventId = Util.GetInput((UInt32)0);
            Console.WriteLine("How many logs do you want to get? [0: All]");
            Console.Write(">>>> ");
            amount = Util.GetInput((UInt32)0);

            if (amount == 0)
            {
                getAllLog = true;
                amount = defaultLogPageSize;
            }

            do
            {
                outEventLogObjs = IntPtr.Zero;
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    break;
                }
                
                if (outNumEventLogs > 0)
                {                    
                    IntPtr curEventLogObjs = outEventLogObjs;
                    for (UInt32 idx = 0; idx < outNumEventLogs; idx++)
                    {
                        BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, structureType);                        
                        Console.WriteLine(Util.GetLogMsg(eventLog));
                        curEventLogObjs += structSize;
                        lastEventId = eventLog.id;
                    }

                    API.BS2_ReleaseObject(outEventLogObjs);                    
                }

                if (outNumEventLogs < defaultLogPageSize)
                {
                    break;
                }
            }
            while (getAllLog);
        }

        void getFilteredLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Type structureType = typeof(BS2Event);
            int structSize = Marshal.SizeOf(structureType);
            IntPtr uid = IntPtr.Zero;
            UInt16 eventCode = 0;
            UInt32 start = 0;
            UInt32 end = 0;
            byte tnaKey = 0;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            Console.WriteLine("Which event do you want to get? [0: All]");
            Console.Write(">>>> ");
            eventCode = Util.GetInput((UInt16)0);

            Console.WriteLine("When do you want to get the log from? [yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out start))
            {
                return;
            }

            Console.WriteLine("When do you want to get the log to? [yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out end))
            {
                return;
            }

            Console.WriteLine("Which tnaKey do you want to get? [0: All(default), 1-16]");
            Console.Write(">>>> ");
            tnaKey = Util.GetInput(0);

            if (tnaKey > BS2Envirionment.BS2_MAX_TNA_KEY)
            {
                Console.WriteLine("Invalid tnaKey : {0}", tnaKey);
                return;
            }

            Console.WriteLine("Which user do you want to the log for? [userID]");
            Console.Write(">>>> ");
            string userIDStr = Console.ReadLine();
            if (userIDStr.Length > 0)
            {
                byte[] uidArray = Encoding.ASCII.GetBytes(userIDStr);
                byte[] outUidArray = new byte[BS2Envirionment.BS2_USER_ID_SIZE];

                uid = Marshal.AllocHGlobal(BS2Envirionment.BS2_USER_ID_SIZE);
                for (int idx = 0; idx < BS2Envirionment.BS2_USER_ID_SIZE; idx++)
                {
                    outUidArray[idx] = 0;
                }

                Array.Copy(uidArray, outUidArray, uidArray.Length);
                Marshal.Copy(outUidArray, 0, uid, BS2Envirionment.BS2_USER_ID_SIZE);
            }

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetFilteredLog(sdkContext, deviceID, uid, eventCode, start, end, tnaKey, out outEventLogObjs, out outNumEventLogs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (outNumEventLogs > 0)
            {
                IntPtr curEventLogObjs = outEventLogObjs;
                for (int idx = 0; idx < outNumEventLogs; idx++)
                {
                    BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, structureType);
                    Console.WriteLine(Util.GetLogMsg(eventLog));
                    curEventLogObjs = (IntPtr)((long)curEventLogObjs + structSize);
                }

                API.BS2_ReleaseObject(outEventLogObjs);
            }
            else
            {
                Console.WriteLine("There are no matching logs.");
            }

            if (uid != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(uid);
            }
        }

        void clearLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to clear log.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ClearLog(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void monitoringLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            cbOnLogReceived = new API.OnLogReceived(RealtimeLogReceived);
            Console.WriteLine("Trying to activate log monitoring.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_StartMonitoringLog(sdkContext, deviceID, cbOnLogReceived);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Console.WriteLine("Press ESC to stop log monitoring.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Trying to deactivate log monitoring.");
            result = (BS2ErrorCode)API.BS2_StopMonitoringLog(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            cbOnLogReceived = null;
        }

        void getImageLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SimpleDeviceInfo deviceInfo;
            int structSize = Marshal.SizeOf(typeof(BS2Event));
            UInt16 imageLogEventCode = (UInt16)BS2EventCodeEnum.DEVICE_TCP_CONNECTED;
            BS2EventConfig eventConfig = Util.AllocateStructure<BS2EventConfig>();
            eventConfig.numImageEventFilter = 1;
            eventConfig.imageEventFilter[0].mainEventCode = (byte)(imageLogEventCode >> 8);
            eventConfig.imageEventFilter[0].scheduleID = (UInt32)BS2ScheduleIDEnum.ALWAYS;

            Console.WriteLine("Trying to get the device[{0}] information.", deviceID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                return;
            }

            Console.WriteLine("Trying to activate image log.");
            result = (BS2ErrorCode)API.BS2_SetEventConfig(sdkContext, deviceID, ref eventConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to clear log for quick test.");
            result = (BS2ErrorCode)API.BS2_ClearLog(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to disconnect device[{0}] for quick test.", deviceID);
            result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Thread.Sleep(500); //waiting for socket close

            Console.WriteLine("Trying to connect device[{0}].", deviceID);
            result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, new IPAddress(BitConverter.GetBytes(deviceInfo.ipv4Address)).ToString(), deviceInfo.port, out deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            result = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, 0, 1024, out outEventLogObjs, out outNumEventLogs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            if (outNumEventLogs > 0)
            {
                IntPtr curEventLogObjs = outEventLogObjs;
                for (int idx = 0; idx < outNumEventLogs; idx++)
                {
                    BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, typeof(BS2Event));
                    if (Convert.ToBoolean(eventLog.image))
                    {
                        Console.WriteLine("Trying to get image log[{0}].", eventLog.id);

                        IntPtr imageObj = IntPtr.Zero;
                        UInt32 imageSize = 0;

                        result = (BS2ErrorCode)API.BS2_GetImageLog(sdkContext, deviceID, eventLog.id, out imageObj, out imageSize);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                        else
                        {
                            int written = 0;
                            FileStream file = new FileStream(String.Format("{0}.jpg", eventLog.id), FileMode.Create, FileAccess.Write);

                            Console.WriteLine("Trying to save image log[{0}].", eventLog.id);
                            WriteFile(file.Handle, imageObj, (int)imageSize, out written, IntPtr.Zero);
                            file.Close();

                            if (written != imageSize)
                            {
                                Console.WriteLine("Got error({0}).", result);
                            }
                            else
                            {
                                Console.WriteLine("Successfully saved the image log[{0}].", eventLog.id);
                                Process.Start(file.Name);
                            }
                        }
                        break;
                    }

                    curEventLogObjs = (IntPtr)((long)curEventLogObjs + structSize);
                }

                API.BS2_ReleaseObject(outEventLogObjs);
            }

            eventConfig.numImageEventFilter = 0;

            Console.WriteLine("Trying to deactivate image log.");
            result = (BS2ErrorCode)API.BS2_SetEventConfig(sdkContext, deviceID, ref eventConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        void syncLocalAndDeviceLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 lastEventId = 0;
            string dbPath = "Data Source=log.db";

            SQLiteConnection connection = new SQLiteConnection(dbPath);
            connection.Open();

            SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS log (id int PRIMARY KEY, dateTime int, deviceID int, code int, msg text)", connection);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT * FROM log ORDER BY id DESC LIMIT 1";
            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                lastEventId = Convert.ToUInt32(rdr["id"]);
            }
            rdr.Close();

            Console.WriteLine("lastEventId[{0}]", lastEventId);

            const UInt32 logPageSize = 1024;
            int structSize = Marshal.SizeOf(typeof(BS2Event));
            bool runnging = false;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            cmd.CommandText = "INSERT INTO log (Id, dateTime, deviceID, code, msg) VALUES (@IdParam, @dateTimeParam, @deviceIDParam, @codeParam, @msgParam)";

            do
            {
                Console.WriteLine("Get logs from device[{0}] lastEventId[{1}].", deviceID, lastEventId);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, lastEventId, logPageSize, out outEventLogObjs, out outNumEventLogs);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    break;
                }

                if (outNumEventLogs > 0)
                {
                    IntPtr curEventLogObjs = outEventLogObjs;

                    SQLiteTransaction transaction = connection.BeginTransaction();
                    for (int idx = 0; idx < outNumEventLogs; idx++)
                    {
                        BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, typeof(BS2Event));
                        Console.WriteLine(">>> Insert log[{0}] into database.", eventLog.id);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@IdParam", eventLog.id);
                        cmd.Parameters.AddWithValue("@dateTimeParam", eventLog.dateTime);
                        cmd.Parameters.AddWithValue("@deviceIDParam", eventLog.deviceID);
                        cmd.Parameters.AddWithValue("@codeParam", eventLog.code);
                        cmd.Parameters.AddWithValue("@msgParam", Util.GetLogMsg(eventLog));
                        cmd.ExecuteNonQuery();
                        curEventLogObjs = (IntPtr)((long)curEventLogObjs + structSize);
                        lastEventId = eventLog.id;
                    }

                    transaction.Commit();

                    API.BS2_ReleaseObject(outEventLogObjs);
                }

                if (outNumEventLogs < logPageSize)
                {
                    runnging = false;
                }
            }
            while (runnging);

            connection.Close();
        }

        private void NormalLogReceived(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                Console.WriteLine(Util.GetLogMsg(eventLog));
            }
        }

        private void RealtimeLogReceived(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                Console.WriteLine(Util.GetLogMsg(eventLog));
                Console.WriteLine("Constructor: "+client.BaseAddress);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);
    }
}