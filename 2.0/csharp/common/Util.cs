using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Suprema
{    
    class Util
    {
        public static T AllocateStructure<T>()
        {
            int structSize = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return instance;
        }

        public static T[] AllocateStructureArray<T>(int count)
        {
            T[] result = new T[count];
            int structSize = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(structSize * count);
            IntPtr curBuffer = buffer;

            for (int idx = 0; idx < count; idx++)
            {
                result[idx] = (T)Marshal.PtrToStructure(curBuffer, typeof(T));
                curBuffer = (IntPtr)((long)curBuffer + structSize);                
            }

            Marshal.FreeHGlobal(buffer);
            return result;
        }


        public static T ConvertTo<T>(byte[] src)
        {
            if (src.Length < Marshal.SizeOf(typeof(T)))
            {
                throw new ArgumentException("array size is less than object size", "src");
            }

            IntPtr buffer = Marshal.AllocHGlobal(src.Length);
            Marshal.Copy(src, 0, buffer, src.Length);
            T item = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return item;
        }

        public static byte[] ConvertTo<T>(ref T instance)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(instance, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static bool GetInput(out int input)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Int32.TryParse(inputStr, out input);
            }
            else
            {
                input = 0;
                return false;
            }
        }

        public static int GetInput()
        {
            do
            {
                string inputStr = Console.ReadLine();
                if (inputStr.Length > 0)
                {
                    return Convert.ToInt32(inputStr);
                }
            } while (true);
        }        

        public static byte GetInput(byte defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToByte(inputStr);
            }

            return defaultValue;
        }

        public static UInt16 GetInput(UInt16 defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToUInt16(inputStr);
            }

            return defaultValue;
        }

        public static UInt32 GetInput(UInt32 defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToUInt32(inputStr);
            }

            return defaultValue;
        }

        public static bool GetTimestamp(string formatString, UInt32 defaultValue, out UInt32 timestamp)
        {
            string inputStr = Console.ReadLine();

            if (defaultValue == 0)
            {
                defaultValue = Convert.ToUInt32(Util.ConvertToUnixTimestamp(DateTime.Now));
            }

            timestamp = defaultValue;
            if (inputStr.Length > 0)
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(inputStr, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    Console.WriteLine("Invalid datetime : {0}", inputStr);
                    return false;
                }
                else
                {
                    timestamp = Convert.ToUInt32(Util.ConvertToUnixTimestamp(dateTime));
                }
            }

            return true;
        }

        public static bool IsYes()
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length == 0 || String.Compare(inputStr, "Y", true) == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsNo()
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length == 0 || String.Compare(inputStr, "N", true) == 0)
            {
                return true;
            }

            return false;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static bool LoadBinary(string filePath, out IntPtr binaryData, out UInt32 binaryDataLen)
        {
            bool handled = false;
            FileStream fs = null;
                        
            binaryData = IntPtr.Zero;
            binaryDataLen = 0;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                int fileSize = (int)fs.Length;
                int totalReadCount = 0;
                byte[] readBuffer = new byte[fileSize];               

                while (totalReadCount < fileSize)
                {
                    int readCount = fs.Read(readBuffer, totalReadCount, (fileSize - totalReadCount));
                    if (readCount > 0)
                    {
                        totalReadCount += readCount;
                    }
                    else
                    {
                        Console.WriteLine("I/O error occurred while reading firmware file.");
                        break;
                    }
                }

                if (totalReadCount == fileSize)
                {
                    binaryData = Marshal.AllocHGlobal(fileSize);
                    Marshal.Copy(readBuffer, 0, binaryData, fileSize);
                    binaryDataLen = (UInt32)fileSize;
                    handled = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", filePath, e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return handled;
        }

        public static string getActionMsg(BS2Action action)
        {
            BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)action.type;

            switch (actionType)
            {
                case BS2ActionTypeEnum.NONE:
                    return "Not specified";
                case BS2ActionTypeEnum.RELAY:
                    {
                        BS2RelayAction relay = Util.ConvertTo<BS2RelayAction>(action.actionUnion);
                        return String.Format("RelayAction relayIndex[{0}] signalID[{1}] count[{2}] onDuration[{3}ms] offDuration[{4}ms] delay[{5}ms]",
                                            relay.relayIndex,
                                            relay.signal.signalID,
                                            relay.signal.count,
                                            relay.signal.onDuration,
                                            relay.signal.offDuration,
                                            relay.signal.delay);
                    }
                case BS2ActionTypeEnum.TTL:
                    {
                        BS2OutputPortAction outputPort = Util.ConvertTo<BS2OutputPortAction>(action.actionUnion);
                        return String.Format("OutputPortAction relayIndex[{0}] signalID[{1}] count[{2}] onDuration[{3}ms] offDuration[{4}ms] delay[{5}ms]",
                                            outputPort.portIndex,
                                            outputPort.signal.signalID,
                                            outputPort.signal.count,
                                            outputPort.signal.onDuration,
                                            outputPort.signal.offDuration,
                                            outputPort.signal.delay);
                    }
                case BS2ActionTypeEnum.DISPLAY:
                    {
                        BS2DisplayAction display = Util.ConvertTo<BS2DisplayAction>(action.actionUnion);
                        return String.Format("DisplayAction displayID[{0}] resourceID[{1}] delay[{2}ms]",
                                            display.displayID,
                                            display.resourceID,
                                            display.duration);
                    }
                case BS2ActionTypeEnum.SOUND:
                    {
                        BS2SoundAction sound = Util.ConvertTo<BS2SoundAction>(action.actionUnion);
                        return String.Format("SoundAction soundIndex[{0}] count[{1}]", sound.soundIndex, sound.count);
                    }
                case BS2ActionTypeEnum.LED:
                    {
                        BS2LedAction led = Util.ConvertTo<BS2LedAction>(action.actionUnion);
                        string ledSignalStr = "";
                        for (int idx = 0; idx < BS2Envirionment.BS2_LED_SIGNAL_NUM; ++idx)
                        {
                            ledSignalStr += String.Format("[color[{0}] duration[{1}ms] delay[{2}ms]]",
                                                            (BS2LedColorEnum)led.signal[idx].color,
                                                            led.signal[idx].duration,
                                                            led.signal[idx].delay);

                            if (idx + 1 < BS2Envirionment.BS2_LED_SIGNAL_NUM)
                            {
                                ledSignalStr += ", ";
                            }
                        }

                        return String.Format("LedAction count[{0}] {1}", led.count, ledSignalStr);
                    }
                case BS2ActionTypeEnum.BUZZER:
                    {
                        BS2BuzzerAction buzzer = Util.ConvertTo<BS2BuzzerAction>(action.actionUnion);
                        string buzzerSignalStr = "";
                        for (int idx = 0; idx < BS2Envirionment.BS2_BUZZER_SIGNAL_NUM; ++idx)
                        {
                            buzzerSignalStr += String.Format("[tone[{0}] fadeout[{1}] duration[{2}ms] delay[{3}ms]]",
                                                            (BS2BuzzerToneEnum)buzzer.signal[idx].tone,
                                                            Convert.ToBoolean(buzzer.signal[idx].fadeout),
                                                            buzzer.signal[idx].duration,
                                                            buzzer.signal[idx].delay);

                            if (idx + 1 < BS2Envirionment.BS2_BUZZER_SIGNAL_NUM)
                            {
                                buzzerSignalStr += ", ";
                            }
                        }

                        return String.Format("BuzzerAction count[{0}] {1}", buzzer.count, buzzerSignalStr);
                    }
                default:
                    return "Not implemented yet.";
            }
        }

        public static string GetLogMsg(BS2Event eventLog)
        {
#if false
            return "eventlog : ";
#else
            switch (((BS2EventCodeEnum)eventLog.code & BS2EventCodeEnum.MASK))
            {
                case BS2EventCodeEnum.DOOR_LOCKED:
                case BS2EventCodeEnum.DOOR_UNLOCKED:
                case BS2EventCodeEnum.DOOR_CLOSED:
                case BS2EventCodeEnum.DOOR_OPENED:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN_ALARM:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN_ALARM_CLEAR:
                case BS2EventCodeEnum.DOOR_HELD_OPEN:
                case BS2EventCodeEnum.DOOR_HELD_OPEN_ALARM:
                case BS2EventCodeEnum.DOOR_HELD_OPEN_ALARM_CLEAR:
                case BS2EventCodeEnum.DOOR_APB_ALARM:
                case BS2EventCodeEnum.DOOR_APB_ALARM_CLEAR:
                    return GetDoorIdMsg(eventLog);
                case BS2EventCodeEnum.ZONE_APB_ALARM:
                case BS2EventCodeEnum.ZONE_APB_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_TIMED_APB_ALARM:
                case BS2EventCodeEnum.ZONE_TIMED_APB_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_FIRE_ALARM:
                case BS2EventCodeEnum.ZONE_FIRE_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_VIOLATION:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_START:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_END:
                case BS2EventCodeEnum.ZONE_SCHEDULED_UNLOCK_START:
                case BS2EventCodeEnum.ZONE_SCHEDULED_UNLOCK_END:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_ALARM:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_ALARM_CLEAR:
                    return GetZoneIdMsg(eventLog);
                case BS2EventCodeEnum.SUPERVISED_INPUT_OPEN:
                case BS2EventCodeEnum.SUPERVISED_INPUT_SHORT:
                case BS2EventCodeEnum.DEVICE_INPUT_DETECTED:
                    return GetIOInfoMsg(eventLog);
                case BS2EventCodeEnum.USER_ENROLL_SUCCESS:
                case BS2EventCodeEnum.USER_ENROLL_FAIL:
                case BS2EventCodeEnum.USER_UPDATE_SUCCESS:
                case BS2EventCodeEnum.USER_UPDATE_FAIL:
                case BS2EventCodeEnum.USER_DELETE_SUCCESS:
                case BS2EventCodeEnum.USER_DELETE_FAIL:
                    return GetUserIdMsg(eventLog);
                case BS2EventCodeEnum.VERIFY_SUCCESS:
                case BS2EventCodeEnum.VERIFY_FAIL:
                case BS2EventCodeEnum.VERIFY_DURESS:
                case BS2EventCodeEnum.IDENTIFY_SUCCESS:
                case BS2EventCodeEnum.IDENTIFY_FAIL:
                case BS2EventCodeEnum.IDENTIFY_DURESS:
                    return GetUserIdAndTnaKeyMsg(eventLog);
                default:
                    return GetGeneralMsg(eventLog);
            }
#endif
        }

        private static string GetDoorIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] doorID[{4}] image[{5}]", 
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.doorID,
                                eventLog.image);
        }

        private static string GetZoneIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] zoneID[{4}] image[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.zoneID,
                                eventLog.image);
        }

        private static string GetIOInfoMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] device[{4, 10}] port[{5}] value[{6}] image[{7}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.ioDeviceID,
                                eventDetail.port,
                                eventDetail.value,
                                eventLog.image);
        }

        private static string GetUserIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            string userID = System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0');
            if (userID.Length == 0)
            {
                userID = "unknown";
            }

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] userID[{4}] image[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                userID,
                                eventLog.image);
        }

        private static string GetUserIdAndTnaKeyMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            string userID = System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0');
            if (userID.Length == 0)
            {
                userID = "unknown";
            }

            string subMsg = "";
            if ((BS2EventCodeEnum)eventLog.code != BS2EventCodeEnum.VERIFY_FAIL_CARD)
            {
                BS2TNAKeyEnum tnaKeyEnum = (BS2TNAKeyEnum)eventLog.param;
                if (tnaKeyEnum != BS2TNAKeyEnum.UNSPECIFIED)
                {
                    subMsg = String.Format("userID[{0}] T&A[{1}]", userID, tnaKeyEnum.ToString());
                }
                else
                {
                    subMsg = String.Format("userID[{0}]", userID);
                }
            }
            else
            {
                subMsg = String.Format("cardID[{0}]", userID);
            }

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] {4} image[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                subMsg,
                                eventLog.image);
        }

        private static string GetGeneralMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] image[{4}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventLog.image);
        }
    }
}
