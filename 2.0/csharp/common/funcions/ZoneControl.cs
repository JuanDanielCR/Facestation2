using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class ZoneControl : FunctionModule
    {
        delegate int ClearZoneStatusDelegate(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);
        delegate int ClearAllZoneStatusDelegate(IntPtr context, UInt32 deviceId, UInt32 zoneID);
        delegate int GetZoneDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);
        delegate int GetZoneStatusDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);
        delegate int GetALLZoneDelegate(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);
        delegate int SetZoneAlarmDelegate(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);
        delegate int RemoveZoneDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);
        delegate int RemoveAllZoneDelegate(IntPtr context, UInt32 deviceId);
        delegate void PrintDelegate<T>(IntPtr context, T item);

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (isMasterDevice)
            {
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get anti-passback zone", getAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get anti-passback zone status", getAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set anti-passback zone", setAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set anti-passback zone alarm", setAPBZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove anti-passback zone", removeAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Clear anti-passback zone status", clearAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get timed anti-passback zone", getTimedAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get timed anti-passback zone status", getTimedAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set timed anti-passback zone", setTimedAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set timed anti-passback zone alarm", setTimedAPBZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove timed anti-passback zone", removeTimedAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Clear timed anti-passback zone status", clearTimedAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get fire alarm zone", getFireAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get fire alarm zone status", getFireAlarmZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set fire alarm zone", setFireAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set fire alarm zone alarm", setFireAlarmZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove fire alarm zone", removeFireAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get scheduled lock/unlock zone", getScheduledLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get scheduled lock/unlock zone status", getScheduledLockUnlockZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set scheduled lock/unlock zone", setScheduledLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set scheduled lock/unlock zone alarm", setScheduledLockUnlockZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove scheduled lock/unlock zone", removeScheduledLockUnlockZone));
            }

            return functionList;
        }

        void getAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2AntiPassbackZone>(sdkContext, deviceID, "anti-passback", API.BS2_GetAllAntiPassbackZone, API.BS2_GetAntiPassbackZone, print);            
        }

        void getAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "anti-passback", API.BS2_GetAntiPassbackZoneStatus);
        }

        void setAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many anti-passback zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2AntiPassbackZone> apbList = new List<BS2AntiPassbackZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2AntiPassbackZone zone = Util.AllocateStructure<BS2AntiPassbackZone>();

                Console.WriteLine("Enter a value for anti-passback zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Envirionment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Which type is this anti-passback zone? [{0} : {1}, {2} : {3}(default)", (byte)BS2APBZoneTypeEnum.HARD, BS2APBZoneTypeEnum.HARD, (byte)BS2APBZoneTypeEnum.SOFT, BS2APBZoneTypeEnum.SOFT);
                Console.Write("  >>>> ");
                zone.type = Util.GetInput((byte)BS2APBZoneTypeEnum.SOFT);

                Console.WriteLine("  Do you want to activate this anti-passback zone? [Y/n]");
                Console.Write("  >>>> ");
                if(Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                Console.WriteLine("  How many seconds should this anti-passback zone be reset after? [0(default): No Reset]");
                Console.Write("  >>>> ");
                zone.resetDuration = Util.GetInput((UInt32)0);

                for (int loop = 0; loop < BS2Envirionment.BS2_MAX_APB_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

                Console.WriteLine("  How many alarms for this anti-passback zone do you want to set? [0(default)-{0}]", BS2Envirionment.BS2_MAX_APB_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming relay control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.RELAY;                    

                    Console.WriteLine("  Enter the relay port on this device.[0(default)]");
                    Console.Write("  >>>> ");
                    relay.relayIndex = Util.GetInput(0);
                                
                    Console.WriteLine("  How many play the relay action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.count = Util.GetInput((UInt16)1);

                    Console.WriteLine("  Enter the active duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.onDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the deactive duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.offDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.delay = Util.GetInput((UInt16)100);     
                           
                    byte[] inputActionArray = Util.ConvertTo<BS2RelayAction>(ref relay);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numReaders = 0;
                Console.WriteLine("  Enter the ID of the readers which are used to enter into this anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string readerID in readerIDs)
                {
                    if (readerID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(readerID, out item))
                        {
                            if (zone.numReaders + 1 >= BS2Envirionment.BS2_MAX_READERS_PER_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Envirionment.BS2_MAX_READERS_PER_APB_ZONE);
                                break;
                            }

                            zone.readers[zone.numReaders].deviceID = item;
                            zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.ENTRY;
                            zone.numReaders++;
                        }
                    }
                }

                if (zone.numReaders + 1 < BS2Envirionment.BS2_MAX_READERS_PER_APB_ZONE)
                {
                    Console.WriteLine("  Enter the ID of the readers which are used to exit into this anti-passback zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    readerIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string readerID in readerIDs)
                    {
                        if (readerID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(readerID, out item))
                            {
                                if (zone.numReaders + 1 >= BS2Envirionment.BS2_MAX_READERS_PER_APB_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of reader should less than {0}.", BS2Envirionment.BS2_MAX_READERS_PER_APB_ZONE);
                                    break;
                                }

                                zone.readers[zone.numReaders].deviceID = item;
                                zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.EXIT;
                                zone.numReaders++;
                            }
                        }
                    }
                }

                zone.numBypassGroups = 0;
                Console.WriteLine("  Enter the ID of the access group which can bypass this anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            if (zone.numBypassGroups + 1 >= BS2Envirionment.BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of bypass access group should less than {0}.", BS2Envirionment.BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE);
                                break;
                            }

                            zone.bypassGroupIDs[zone.numBypassGroups] = item;
                            zone.numBypassGroups++;
                        }
                    }
                }

                apbList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2AntiPassbackZone));
            IntPtr apbListObj = Marshal.AllocHGlobal(structSize * apbList.Count);
            IntPtr curApbListObj = apbListObj;
            foreach (BS2AntiPassbackZone item in apbList)
            {
                Marshal.StructureToPtr(item, curApbListObj, false);
                curApbListObj = (IntPtr)((long)curApbListObj + structSize);
            }

            Console.WriteLine("Trying to set anti-passback zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAntiPassbackZone(sdkContext, deviceID, apbListObj, (UInt32)apbList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(apbListObj);
        }

        void setAPBZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "anti-passback", API.BS2_SetAntiPassbackZoneAlarm);
        }

        void removeAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "anti-passback", API.BS2_RemoveAllAntiPassbackZone, API.BS2_RemoveAntiPassbackZone);
        }

        void clearAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            clearZoneStatus(sdkContext, deviceID, "anti-passback", API.BS2_ClearAllAntiPassbackZoneStatus, API.BS2_ClearAntiPassbackZoneStatus);
        }

        void getTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2TimedAntiPassbackZone>(sdkContext, deviceID, "timed anti-passback", API.BS2_GetAllTimedAntiPassbackZone, API.BS2_GetTimedAntiPassbackZone, print);            
        }

        void getTimedAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "timed anti-passback", API.BS2_GetTimedAntiPassbackZoneStatus);
        }

        void setTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many timed anti-passback zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2TimedAntiPassbackZone> apbList = new List<BS2TimedAntiPassbackZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2TimedAntiPassbackZone zone = Util.AllocateStructure<BS2TimedAntiPassbackZone>();

                Console.WriteLine("Enter a value for timed anti-passback zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Envirionment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Which type is this timed anti-passback zone? [{0} : {1}, {2} : {3}(default)", (byte)BS2APBZoneTypeEnum.HARD, BS2APBZoneTypeEnum.HARD, (byte)BS2APBZoneTypeEnum.SOFT, BS2APBZoneTypeEnum.SOFT);
                Console.Write("  >>>> ");
                zone.type = Util.GetInput((byte)BS2APBZoneTypeEnum.SOFT);

                Console.WriteLine("  Do you want to activate this timed anti-passback zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                Console.WriteLine("  How many seconds should this timed anti-passback zone be reset after? [0(default): No Reset]");
                Console.Write("  >>>> ");
                zone.resetDuration = Util.GetInput((UInt32)0);

                for (int loop = 0; loop < BS2Envirionment.BS2_MAX_APB_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

                Console.WriteLine("  How many alarms for this timed anti-passback zone do you want to set? [0(default)-{0}]", BS2Envirionment.BS2_MAX_TIMED_APB_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2OutputPortAction ttl = Util.AllocateStructure<BS2OutputPortAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming ttl control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.TTL;

                    Console.WriteLine("  Enter the ttl port on this device.[0(default)]");
                    Console.Write("  >>>> ");
                    ttl.portIndex = Util.GetInput(0);

                    Console.WriteLine("  How many play the relay action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    ttl.signal.count = Util.GetInput((UInt16)1);

                    Console.WriteLine("  Enter the active duration.[100(default)]");
                    Console.Write("  >>>> ");
                    ttl.signal.onDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the deactive duration.[100(default)]");
                    Console.Write("  >>>> ");
                    ttl.signal.offDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    ttl.signal.delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray = Util.ConvertTo<BS2OutputPortAction>(ref ttl);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numReaders = 0;
                Console.WriteLine("  Enter the ID of the readers which are used to enter into this timed anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string readerID in readerIDs)
                {
                    if (readerID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(readerID, out item))
                        {
                            if (zone.numReaders + 1 >= BS2Envirionment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Envirionment.BS2_MAX_READERS_PER_TIMED_APB_ZONE);
                                break;
                            }

                            zone.readers[zone.numReaders].deviceID = item;
                            zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.ENTRY;
                            zone.numReaders++;
                        }
                    }
                }

                if (zone.numReaders + 1 < BS2Envirionment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                {
                    Console.WriteLine("  Enter the ID of the readers which are used to exit into this timed anti-passback zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    readerIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string readerID in readerIDs)
                    {
                        if (readerID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(readerID, out item))
                            {
                                if (zone.numReaders + 1 >= BS2Envirionment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Envirionment.BS2_MAX_READERS_PER_TIMED_APB_ZONE);
                                    break;
                                }

                                zone.readers[zone.numReaders].deviceID = item;
                                zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.EXIT;
                                zone.numReaders++;
                            }
                        }
                    }
                }

                zone.numBypassGroups = 0;
                Console.WriteLine("  Enter the ID of the access group which can bypass this timed anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            if (zone.numBypassGroups + 1 >= BS2Envirionment.BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of bypass access group should less than {0}.", BS2Envirionment.BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE);
                                break;
                            }

                            zone.bypassGroupIDs[zone.numBypassGroups] = item;
                            zone.numBypassGroups++;
                        }
                    }
                }

                apbList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2TimedAntiPassbackZone));
            IntPtr apbListObj = Marshal.AllocHGlobal(structSize * apbList.Count);
            IntPtr curApbListObj = apbListObj;
            foreach (BS2TimedAntiPassbackZone item in apbList)
            {
                Marshal.StructureToPtr(item, curApbListObj, false);
                curApbListObj = (IntPtr)((long)curApbListObj + structSize);
            }

            Console.WriteLine("Trying to set timed anti-passback zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetTimedAntiPassbackZone(sdkContext, deviceID, apbListObj, (UInt32)apbList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(apbListObj);
        }

        void setTimedAPBZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "timed anti-passback", API.BS2_SetTimedAntiPassbackZoneAlarm);
        }

        void removeTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "timed anti-passback", API.BS2_RemoveAllTimedAntiPassbackZone, API.BS2_RemoveTimedAntiPassbackZone);
        }

        void clearTimedAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            clearZoneStatus(sdkContext, deviceID, "timed anti-passback", API.BS2_ClearAllTimedAntiPassbackZoneStatus, API.BS2_ClearTimedAntiPassbackZoneStatus);
        }

        void getFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2FireAlarmZone>(sdkContext, deviceID, "fire alarm", API.BS2_GetAllFireAlarmZone, API.BS2_GetFireAlarmZone, print);            
        }

        void getFireAlarmZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "fire alarm", API.BS2_GetFireAlarmZoneStatus);
        }

        void setFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many fire alarm zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2FireAlarmZone> fazList = new List<BS2FireAlarmZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2FireAlarmZone zone = Util.AllocateStructure<BS2FireAlarmZone>();

                Console.WriteLine("Enter a value for fire alarm zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Envirionment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Do you want to activate this fire alarm zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                for (int loop = 0; loop < BS2Envirionment.BS2_MAX_FIRE_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

                Console.WriteLine("  How many alarms for this fire alarm zone do you want to set? [0(default)-{0}]", BS2Envirionment.BS2_MAX_FIRE_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2SoundAction sound = Util.AllocateStructure<BS2SoundAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming sound control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.SOUND;

                    Console.WriteLine("  How many play the sound action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    sound.count = Util.GetInput((byte)1);

                    Console.WriteLine("  Enter the sound index.[{0}(default) : {1}, {2} : {3}, {4} : {5}]", (UInt16)BS2SoundIndexEnum.WELCOME, BS2SoundIndexEnum.WELCOME, (UInt16)BS2SoundIndexEnum.AUTH_SUCCESS, BS2SoundIndexEnum.AUTH_SUCCESS, (UInt16)BS2SoundIndexEnum.AUTH_FAIL, BS2SoundIndexEnum.AUTH_FAIL);
                    Console.Write("  >>>> ");
                    sound.soundIndex = Util.GetInput((UInt16)BS2SoundIndexEnum.WELCOME);

                    byte[] inputActionArray = Util.ConvertTo<BS2SoundAction>(ref sound);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                Console.WriteLine("  How many fire sensors for this fire alarm zone do you want to set? [1(default)-{0}]", BS2Envirionment.BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE);
                Console.Write("  >>>> ");
                zone.numSensors = Util.GetInput((byte)1);

                for (byte loop = 0; loop < zone.numSensors; ++loop)
                {
                    Console.WriteLine("  Enter the ID of the fire sensor for this fire alarm zone:");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the port of the fire sensor for this fire alarm zone: [0(default)]");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].port = (byte)Util.GetInput((byte)0);

                    Console.WriteLine("  Enter the switch type of the fire sensor for this fire alarm zone: [{0}(default) : {1}, {2} : {3}]", (byte)BS2SwitchTypeEnum.NORMAL_CLOSE, BS2SwitchTypeEnum.NORMAL_CLOSE, (byte)BS2SwitchTypeEnum.NORMAL_OPEN, BS2SwitchTypeEnum.NORMAL_OPEN);
                    Console.Write("  >>>> ");
                    zone.sensor[loop].switchType = (byte)Util.GetInput((byte)0);

                    Console.WriteLine("  Enter the duration of the fire sensor for this fire alarm zone: [{100}(default)]");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].duration = (UInt16)Util.GetInput((UInt16)100);
                }

                zone.numDoors = 0;
                Console.WriteLine("  Enter the ID of the door which belong to this fire alarm zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string doorID in doorIDs)
                {
                    if (doorID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(doorID, out item))
                        {
                            if (zone.numDoors + 1 >= BS2Envirionment.BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of doorID should less than {0}.", BS2Envirionment.BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE);
                                break;
                            }

                            zone.doorIDs[zone.numDoors] = item;
                            zone.numDoors++;
                        }
                    }
                }

                fazList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2FireAlarmZone));
            IntPtr fazListObj = Marshal.AllocHGlobal(structSize * fazList.Count);
            IntPtr curFazListObj = fazListObj;
            foreach (BS2FireAlarmZone item in fazList)
            {
                Marshal.StructureToPtr(item, curFazListObj, false);
                curFazListObj = (IntPtr)((long)curFazListObj + structSize);
            }

            Console.WriteLine("Trying to set fire alarm zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetFireAlarmZone(sdkContext, deviceID, fazListObj, (UInt32)fazList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(fazListObj);
        }

        void setFireAlarmZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "fire alarm", API.BS2_SetFireAlarmZoneAlarm);
        }

        void removeFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "fire alarm", API.BS2_RemoveAllFireAlarmZone, API.BS2_RemoveFireAlarmZone);
        }

        void getScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2ScheduledLockUnlockZone>(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_GetAllScheduledLockUnlockZone, API.BS2_GetScheduledLockUnlockZone, print);            
        }

        void getScheduledLockUnlockZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_GetScheduledLockUnlockZoneStatus);
        }

        void setScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many scheduled lock/unlock zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2ScheduledLockUnlockZone> slulList = new List<BS2ScheduledLockUnlockZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2ScheduledLockUnlockZone zone = Util.AllocateStructure<BS2ScheduledLockUnlockZone>();

                Console.WriteLine("Enter a value for scheduled lock/unlock zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Envirionment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Envirionment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Enter the ID of access schedule to lock this scheduled lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.ALWAYS, BS2ScheduleIDEnum.ALWAYS);
                Console.Write("  >>>> ");
                zone.lockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.ALWAYS);

                Console.WriteLine("  Enter the ID of access schedule to unlock this scheduled lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.NEVER, BS2ScheduleIDEnum.NEVER);
                Console.Write("  >>>> ");
                zone.unlockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.NEVER);                

                Console.WriteLine("  Do you want to activate this scheduled lock/unlock zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                for (int loop = 0; loop < BS2Envirionment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

                Console.WriteLine("  How many alarms for this scheduled lock/unlock do you want to set? [0(default)-{0}]", BS2Envirionment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2BuzzerAction buzzer = Util.AllocateStructure<BS2BuzzerAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming buzzer control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.BUZZER;

                    buzzer.count = 1;
                    Console.WriteLine("  Enter the type of buzzer tone.[{0} : {1}, {2} : {3}, {4} : {5}, {6}(default) : {7}]", 
                                    (byte)BS2BuzzerToneEnum.OFF,
                                    BS2BuzzerToneEnum.OFF,
                                    (byte)BS2BuzzerToneEnum.LOW,
                                    BS2BuzzerToneEnum.LOW,
                                    (byte)BS2BuzzerToneEnum.MIDDLE,
                                    BS2BuzzerToneEnum.MIDDLE,
                                    (byte)BS2BuzzerToneEnum.HIGH,
                                    BS2BuzzerToneEnum.HIGH);
                    Console.Write("  >>>> ");
                    buzzer.signal[0].tone = Util.GetInput((byte)BS2BuzzerToneEnum.HIGH);

                    Console.WriteLine("  Do you want to set the fade out effect for this scheduled lock/unlock zone? [y/N]");
                    Console.Write("  >>>> ");
                    if (Util.IsNo())
                    {
                        buzzer.signal[0].fadeout = 0;
                    }
                    else
                    {
                        buzzer.signal[0].fadeout = 1;
                    }

                    Console.WriteLine("  Enter the duration of buzzer for this scheduled lock/unlock zone: [{100}(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].duration = (UInt16)Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray = Util.ConvertTo<BS2BuzzerAction>(ref buzzer);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numDoors = 0;
                Console.WriteLine("  Enter the ID of the door which belong to this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string doorID in doorIDs)
                {
                    if (doorID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(doorID, out item))
                        {
                            if (zone.numDoors + 1 >= BS2Envirionment.BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of doorID should less than {0}.", BS2Envirionment.BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                break;
                            }

                            zone.doorIDs[zone.numDoors] = item;
                            zone.numDoors++;
                        }
                    }
                }

                zone.bidirectionalLock = 0;
                zone.numBypassGroups = 0;
                if (zone.lockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Should it's door be locked bi-directionally when this scheduled lock/unlock zone is on the lock schedule? [Y/n]");
                    Console.Write("  >>>> ");
                    if (Util.IsYes())
                    {
                        zone.bidirectionalLock = 1;
                    }

                    Console.WriteLine("  Enter the ID of the access group which can bypass this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numDoors + 1 >= BS2Envirionment.BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Envirionment.BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.bypassGroupIDs[zone.numBypassGroups] = item;
                                zone.numBypassGroups++;
                            }
                        }
                    }
                }

                zone.numUnlockGroups = 0;
                if (zone.unlockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Enter the ID of the access group which can unlock this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numDoors + 1 >= BS2Envirionment.BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Envirionment.BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.unlockGroupIDs[zone.numUnlockGroups] = item;
                                zone.numUnlockGroups++;
                            }
                        }
                    }
                }

                slulList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2ScheduledLockUnlockZone));
            IntPtr slulListObj = Marshal.AllocHGlobal(structSize * slulList.Count);
            IntPtr curSlulListObj = slulListObj;
            foreach (BS2ScheduledLockUnlockZone item in slulList)
            {
                Marshal.StructureToPtr(item, curSlulListObj, false);
                curSlulListObj = (IntPtr)((long)curSlulListObj + structSize);
            }

            Console.WriteLine("Trying to set scheduled lock/unlock zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetScheduledLockUnlockZone(sdkContext, deviceID, slulListObj, (UInt32)slulList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(slulListObj);
        }

        void setScheduledLockUnlockZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_SetScheduledLockUnlockZoneAlarm);
        }

        void removeScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_RemoveAllScheduledLockUnlockZone, API.BS2_RemoveScheduledLockUnlockZone);
        }

        void getZone<T>(IntPtr sdkContext, UInt32 deviceID, string zoneType, GetALLZoneDelegate getALLZoneDelegate, GetZoneDelegate getZoneDelegate, PrintDelegate<T> printDelegate)
        {
            IntPtr zoneObj = IntPtr.Zero;
            UInt32 numZone = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all {0} zones? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all {0} zones from device.", zoneType);
                result = (BS2ErrorCode)getALLZoneDelegate(sdkContext, deviceID, out zoneObj, out numZone);             
            }
            else
            {
                Console.WriteLine("Enter the ID of the zone which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> zoneIDList = new List<UInt32>();

                foreach (string zoneID in zoneIDs)
                {
                    if (zoneID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(zoneID, out item))
                        {
                            zoneIDList.Add(item);
                        }
                    }
                }

                if (zoneIDList.Count > 0)
                {
                    IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                    IntPtr curZoneIDObj = zoneIDObj;
                    foreach (UInt32 item in zoneIDList)
                    {
                        Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                        curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                    }

                    Console.WriteLine("Trying to get {0} zones from device.", zoneType);
                    result = (BS2ErrorCode)getZoneDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count, out zoneObj, out numZone);             

                    Marshal.FreeHGlobal(zoneIDObj);
                }
                else
                {
                    Console.WriteLine("Invalid parameter");
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numZone > 0)
            {
                IntPtr curZoneObj = zoneObj;
                int structSize = Marshal.SizeOf(typeof(T));

                for (int idx = 0; idx < numZone; ++idx)
                {
                    T item = (T)Marshal.PtrToStructure(curZoneObj, typeof(T));
                    printDelegate(sdkContext, item);
                    curZoneObj = (IntPtr)((long)curZoneObj + structSize);
                }

                API.BS2_ReleaseObject(zoneObj);
            }
            else
            {
                Console.WriteLine(">>> There is no {0} zone in the device.", zoneType);
            }
        }

        void getZoneStatus(IntPtr sdkContext, UInt32 deviceID, string zoneType, GetZoneStatusDelegate getZoneStatusDelegate)
        {
            Console.WriteLine("Enter the ID of the zone which you want to get its status: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> zoneIDList = new List<UInt32>();

            foreach (string zoneID in zoneIDs)
            {
                if (zoneID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(zoneID, out item))
                    {
                        zoneIDList.Add(item);
                    }
                }
            }

            if (zoneIDList.Count > 0)
            {
                IntPtr zoneStatusObj = IntPtr.Zero;
                UInt32 numZoneStatus = 0;
                IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                IntPtr curZoneIDObj = zoneIDObj;
                foreach (UInt32 item in zoneIDList)
                {
                    Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                    curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                }

                Console.WriteLine("Trying to get the status of the {0} zone from device.", zoneType);
                BS2ErrorCode result = (BS2ErrorCode)getZoneStatusDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count, out zoneStatusObj, out numZoneStatus);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else if (numZoneStatus > 0)
                {
                    IntPtr curZoneStatusObj = zoneStatusObj;
                    int structSize = Marshal.SizeOf(typeof(BS2ZoneStatus));

                    for (int idx = 0; idx < numZoneStatus; ++idx)
                    {
                        BS2ZoneStatus item = (BS2ZoneStatus)Marshal.PtrToStructure(curZoneStatusObj, typeof(BS2ZoneStatus));
                        print(sdkContext, item);
                        curZoneStatusObj = (IntPtr)((long)curZoneStatusObj + structSize);
                    }

                    API.BS2_ReleaseObject(zoneStatusObj);
                }
                else
                {
                    Console.WriteLine(">>> There is no such {0} zone in the device.", zoneType);                    
                }

                Marshal.FreeHGlobal(zoneIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void setZoneAlarm(IntPtr sdkContext, UInt32 deviceID, string zoneType, SetZoneAlarmDelegate setZoneAlarmDelegate)
        {
            byte alarmed = 1;
            Console.WriteLine("Do you want to release the {0} zone alarm? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                alarmed = 0;
            }

            Console.WriteLine("Enter the ID of the zone which you want to set: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> zoneIDList = new List<UInt32>();

            foreach (string zoneID in zoneIDs)
            {
                if (zoneID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(zoneID, out item))
                    {
                        zoneIDList.Add(item);
                    }
                }
            }

            if (zoneIDList.Count > 0)
            {
                IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                IntPtr curZoneIDObj = zoneIDObj;
                foreach (UInt32 item in zoneIDList)
                {
                    Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                    curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                }

                Console.WriteLine("Trying to set the alarm of the {0} zone from device.", zoneType);
                BS2ErrorCode result = (BS2ErrorCode)setZoneAlarmDelegate(sdkContext, deviceID, alarmed, zoneIDObj, (UInt32)zoneIDList.Count);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(zoneIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void removeZone(IntPtr sdkContext, UInt32 deviceID, string zoneType, RemoveAllZoneDelegate removeAllZoneDelegate, RemoveZoneDelegate removeZoneDelegate)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all {0} zones? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all {0} zones from device.", zoneType);
                result = (BS2ErrorCode)removeAllZoneDelegate(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the zone which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> zoneIDList = new List<UInt32>();

                foreach (string zoneID in zoneIDs)
                {
                    if (zoneID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(zoneID, out item))
                        {
                            zoneIDList.Add(item);
                        }
                    }
                }

                if (zoneIDList.Count > 0)
                {
                    IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                    IntPtr curZoneIDObj = zoneIDObj;
                    foreach (UInt32 item in zoneIDList)
                    {
                        Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                        curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove {0} zones from device.", zoneType);
                    result = (BS2ErrorCode)removeZoneDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count);

                    Marshal.FreeHGlobal(zoneIDObj);
                }
                else
                {
                    Console.WriteLine("Invalid parameter");
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void clearZoneStatus(IntPtr sdkContext, UInt32 deviceID, string zoneType, ClearAllZoneStatusDelegate clearAllZoneStatusDelegate, ClearZoneStatusDelegate clearZoneStatusDelegate)
        {
            Console.WriteLine("Enter the ID of the zone which you want to clear.");
            Console.Write(">>>> ");
            UInt32 zoneID = (UInt32)Util.GetInput();

            if (zoneID == 0)
            {
                Console.WriteLine("The zone id should be greater than 0.");
                return;
            }

            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to clear all users? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to clear all user.");
                result = (BS2ErrorCode)clearAllZoneStatusDelegate(sdkContext, deviceID, zoneID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the user which you want to clear: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] userIDs = Console.ReadLine().Split(delimiterChars);
                List<string> userIDList = new List<string>();

                foreach (string userID in userIDs)
                {
                    if (userID.Length == 0)
                    {
                        Console.WriteLine("The user id can not be empty.");
                        return;
                    }
                    else if (userID.Length > BS2Envirionment.BS2_USER_ID_SIZE)
                    {
                        Console.WriteLine("The user id should less than {0} words.", BS2Envirionment.BS2_USER_ID_SIZE);
                        return;
                    }
                    else
                    {
                        userIDList.Add(userID);
                    }
                }

                if (userIDList.Count > 0)
                {
                    byte[] targetUid = new byte[BS2Envirionment.BS2_USER_ID_SIZE];
                    IntPtr userIDObj = Marshal.AllocHGlobal(BS2Envirionment.BS2_USER_ID_SIZE * userIDList.Count);
                    IntPtr curUserIDObj = userIDObj;
                    foreach (string item in userIDList)
                    {
                        byte[] uid = Encoding.UTF8.GetBytes(item);
                        Array.Clear(targetUid, 0, BS2Envirionment.BS2_USER_ID_SIZE);
                        Array.Copy(uid, 0, targetUid, 0, uid.Length);

                        Marshal.Copy(targetUid, 0, curUserIDObj, BS2Envirionment.BS2_USER_ID_SIZE);
                        curUserIDObj = (IntPtr)((long)curUserIDObj + BS2Envirionment.BS2_USER_ID_SIZE);
                    }

                    Console.WriteLine("Trying to clear user.");
                    result = (BS2ErrorCode)clearZoneStatusDelegate(sdkContext, deviceID, zoneID, userIDObj, (UInt32)userIDList.Count);

                    Marshal.FreeHGlobal(userIDObj);
                }
                else
                {
                    Console.WriteLine("Invalid parameter");
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void print(IntPtr sdkContext, BS2AntiPassbackZone zone)
        {
            Console.WriteLine(">>>> APB Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--type[{0}]", (BS2APBZoneTypeEnum)zone.type);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--resetDuration[{0}]", zone.resetDuration);
            Console.WriteLine("     |--alarm");

            for (int idx = 0; idx < BS2Envirionment.BS2_MAX_APB_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--readers");
            for (byte idx = 0; idx < zone.numReaders; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}] type[{1}]", zone.readers[idx].deviceID, (BS2APBZoneReaderTypeEnum)zone.readers[idx].type);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--access group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2TimedAntiPassbackZone zone)
        {
            Console.WriteLine(">>>> Timed APB Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--type[{0}]", (BS2APBZoneTypeEnum)zone.type);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--resetDuration[{0}]", zone.resetDuration);
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Envirionment.BS2_MAX_TIMED_APB_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--readers");
            for (byte idx = 0; idx < zone.numReaders; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}] type[{1}]", zone.readers[idx].deviceID, (BS2APBZoneReaderTypeEnum)zone.readers[idx].type);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--access group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2FireAlarmZone zone)
        {
            Console.WriteLine(">>>> Fire Alarm Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--sensor");
            for (byte idx = 0; idx < zone.numSensors; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}] port[{1}] switchType[{2}]", zone.sensor[idx].deviceID, zone.sensor[idx].port, (BS2SwitchTypeEnum)zone.sensor[idx].switchType);
            }
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Envirionment.BS2_MAX_FIRE_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }            
            Console.WriteLine("     |--doorIDs");
            for (byte idx = 0; idx < zone.numDoors; ++idx)
            {
                Console.WriteLine("     |  |--door ID[{0}]", zone.doorIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2ScheduledLockUnlockZone zone)
        {
            Console.WriteLine(">>>> Scheduled Lock/Unlock Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--lockScheduleID[{0}]", zone.lockScheduleID);
            Console.WriteLine("     |--unlockScheduleID[{0}]", zone.unlockScheduleID);
            Console.WriteLine("     |--bidirectionalLock[{0}]", Convert.ToBoolean(zone.bidirectionalLock));
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Envirionment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--doorIDs");
            for (byte idx = 0; idx < zone.numDoors; ++idx)
            {
                Console.WriteLine("     |  |--door ID[{0}]", zone.doorIDs[idx]);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--bypass group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
            Console.WriteLine("     |--unlockGroupIDs");
            for (byte idx = 0; idx < zone.numUnlockGroups; ++idx)
            {
                Console.WriteLine("     |  |--unlock group ID[{0}]", zone.unlockGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2ZoneStatus zoneStatus)
        {
            Console.WriteLine(">>>> Zone ID[{0, 10}] status[{1}] disabled[{2}]", zoneStatus.id, (BS2ZoneStatusEnum)zoneStatus.status, Convert.ToBoolean(zoneStatus.disabled));            
        }
    }
}
