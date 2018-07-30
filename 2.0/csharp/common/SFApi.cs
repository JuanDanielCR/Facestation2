﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace Suprema
{       
    static class API
    {
        public static Dictionary<BS2DeviceTypeEnum, string> productNameDictionary = new Dictionary<BS2DeviceTypeEnum, string>()
        {
            {BS2DeviceTypeEnum.BIOENTRY_PLUS, "BioEntry Plus"},
            {BS2DeviceTypeEnum.BIOENTRY_W, "BioEntry W"},
            {BS2DeviceTypeEnum.BIOLITE_NET, "BioLite Net"},
            {BS2DeviceTypeEnum.XPASS, "Xpass"},
            {BS2DeviceTypeEnum.XPASS_S2, "Xpass S2"},
            {BS2DeviceTypeEnum.SECURE_IO_2, "Secure IO 2"},
            {BS2DeviceTypeEnum.DOOR_MODULE_20, "Door module 20"},
            {BS2DeviceTypeEnum.BIOSTATION_2, "BioStation 2"},
            {BS2DeviceTypeEnum.BIOSTATION_A2, "BioStation A2"},
            {BS2DeviceTypeEnum.FACESTATION_2, "FaceStation 2"},
            {BS2DeviceTypeEnum.IO_DEVICE, "IO device"},
            {BS2DeviceTypeEnum.BIOSTATION_L2, "BioStation L2" },
            {BS2DeviceTypeEnum.BIOENTRY_W2, "BioEntry W2" },
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceFound(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceAccepted(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceConnected(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceDisconnected(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnReadyToScan(UInt32 deviceId, UInt32 sequence);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnProgressChanged(UInt32 deviceId, UInt32 progressPercentage);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnLogReceived(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnAlarmFired(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnInputDetected(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnConfigChanged(UInt32 deviceId, UInt32 configMask);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnVerifyUser(UInt32 deviceId, UInt16 seq, byte isCard, byte cardType, IntPtr data, UInt32 dataLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnIdentifyUser(UInt32 deviceId, UInt16 seq, byte format, IntPtr templateData, UInt32 templateSize);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int IsAcceptableUserID(string uid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UInt32 PreferMethod(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string GetRootCaFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string GetServerCaFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string GetServerPrivateKeyFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string GetPassword(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnErrorOccured(UInt32 deviceID, int errCode);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr BS2_AllocateContext();

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_Initialize(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceSearchingTimeout(IntPtr context, UInt32 second);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetMaxThreadCount(IntPtr context, UInt32 maxThreadCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IsAutoConnection(IntPtr context, ref int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAutoConnection(IntPtr context, int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceEventListener(IntPtr context, OnDeviceFound cbOnDeviceFound, OnDeviceAccepted cbOnDeviceAccepted, OnDeviceConnected cbOnDeviceConnected, OnDeviceDisconnected cbOnDeviceDisconnected);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetNotificationListener(IntPtr context, OnAlarmFired cbOnAlarmFired, OnInputDetected cbOnInputDetected, OnConfigChanged cbOnConfigChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_ReleaseContext(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_ReleaseObject(IntPtr obj);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSSLHandler(IntPtr context, PreferMethod cbPreferMethod, GetRootCaFilePath cbGetRootCaFilePath, GetServerCaFilePath cbGetServerCaFilePath, GetServerPrivateKeyFilePath cbGetServerPrivateKeyFilePath, GetPassword cbGetPassword, OnErrorOccured cbOnErrorOccured); 

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetServerPort(IntPtr context, UInt16 devicePort);                               

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SearchDevices(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDevices(IntPtr context, out IntPtr deviceListObj, out UInt32 numDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceInfo(IntPtr context, UInt32 deviceId, out BS2SimpleDeviceInfo deviceInfo);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ConnectDevice(IntPtr context, UInt32 deviceId);        

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ConnectDeviceViaIP(IntPtr context, string deviceAddress, UInt16 devicePort, out UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_DisconnectDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceTopology(IntPtr context, UInt32 deviceId, out IntPtr networkNodeObj, out UInt32 numNetworkNode);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceTopology(IntPtr context, UInt32 deviceId, IntPtr networkNode, UInt32 numNetworkNode);


        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< AccessControl API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroupIds, UInt32 accessGroupIdCount, out IntPtr accessGroupObj, out UInt32 numAccessGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessGroup(IntPtr context, UInt32 deviceId, out IntPtr accessGroupObj, out UInt32 numAccessGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroups, UInt32 accessGroupCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroupIds, UInt32 accessGroupIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessGroup(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevelIds, UInt32 accessLevelIdCount, out IntPtr accessLevelObj, out UInt32 numAccessLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessLevel(IntPtr context, UInt32 deviceId, out IntPtr accessLevelObj, out UInt32 numAccessLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevels, UInt32 accessLevelCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevelIds, UInt32 accessLevelIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessLevel(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessScheduleIds, UInt32 accessScheduleIdCount, out IntPtr accessScheduleObj, out UInt32 numAccessSchedule);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessSchedule(IntPtr context, UInt32 deviceId, out IntPtr accessScheduleObj, out UInt32 numAccessSchedule);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessSchedules, UInt32 accessScheduleCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessScheduleIds, UInt32 accessScheduleIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessSchedule(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroupIds, UInt32 holidayGroupIdCount, out IntPtr holidayGroupObj, out UInt32 numHolidayGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllHolidayGroup(IntPtr context, UInt32 deviceId, out IntPtr holidayGroupObj, out UInt32 numHolidayGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroups, UInt32 holidayGroupCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroupIds, UInt32 holidayGroupIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllHolidayGroup(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Blacklist API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount, out IntPtr blacklistObj, out UInt32 numBlacklist);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllBlackList(IntPtr context, UInt32 deviceId, out IntPtr blacklistObj, out UInt32 numBlacklist);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllBlackList(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Card API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanCard(IntPtr context, UInt32 deviceId, out BS2Card card, OnReadyToScan cbReadyToScan);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_WriteCard(IntPtr context, UInt32 deviceId, ref BS2SmartCardData smartCard);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EraseCard(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Config API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearDatabase(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ResetConfig(IntPtr context, UInt32 deviceId, [MarshalAs(UnmanagedType.I1)] bool includingDB);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetConfig(IntPtr context, UInt32 deviceId, out BS2Configs configs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetConfig(IntPtr context, UInt32 deviceId, ref BS2Configs configs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFactoryConfig(IntPtr context, UInt32 deviceId, out BS2FactoryConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSystemConfig(IntPtr context, UInt32 deviceId, out BS2SystemConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSystemConfig(IntPtr context, UInt32 deviceId, ref BS2SystemConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAuthConfig(IntPtr context, UInt32 deviceId, out BS2AuthConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAuthConfig(IntPtr context, UInt32 deviceId, ref BS2AuthConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetStatusConfig(IntPtr context, UInt32 deviceId, out BS2StatusConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetStatusConfig(IntPtr context, UInt32 deviceId, ref BS2StatusConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDisplayConfig(IntPtr context, UInt32 deviceId, out BS2DisplayConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDisplayConfig(IntPtr context, UInt32 deviceId, ref BS2DisplayConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfig(IntPtr context, UInt32 deviceId, out BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfigExt(IntPtr context, UInt32 deviceId, out BS2IpConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfigViaUDP(IntPtr context, UInt32 deviceId, out BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfig(IntPtr context, UInt32 deviceId, ref BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfigExt(IntPtr context, UInt32 deviceId, ref BS2IpConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfigViaUDP(IntPtr context, UInt32 deviceId, ref BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTNAConfig(IntPtr context, UInt32 deviceId, out BS2TNAConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTNAConfig(IntPtr context, UInt32 deviceId, ref BS2TNAConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCardConfig(IntPtr context, UInt32 deviceId, out BS2CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCardConfig(IntPtr context, UInt32 deviceId, ref BS2CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFingerprintConfig(IntPtr context, UInt32 deviceId, out BS2FingerprintConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFingerprintConfig(IntPtr context, UInt32 deviceId, ref BS2FingerprintConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetRS485Config(IntPtr context, UInt32 deviceId, out BS2Rs485Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetRS485Config(IntPtr context, UInt32 deviceId, ref BS2Rs485Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandConfig(IntPtr context, UInt32 deviceId, out BS2WiegandConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandDeviceConfig(IntPtr context, UInt32 deviceId, out BS2WiegandDeviceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandDeviceConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandDeviceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetInputConfig(IntPtr context, UInt32 deviceId, out BS2InputConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetInputConfig(IntPtr context, UInt32 deviceId, ref BS2InputConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWlanConfig(IntPtr context, UInt32 deviceId, out BS2WlanConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWlanConfig(IntPtr context, UInt32 deviceId, ref BS2WlanConfig config);        

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTriggerActionConfig(IntPtr context, UInt32 deviceId, out BS2TriggerActionConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTriggerActionConfig(IntPtr context, UInt32 deviceId, ref BS2TriggerActionConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetEventConfig(IntPtr context, UInt32 deviceId, out BS2EventConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetEventConfig(IntPtr context, UInt32 deviceId, ref BS2EventConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandMultiConfig(IntPtr context, UInt32 deviceId, out BS2WiegandMultiConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandMultiConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandMultiConfig config);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Door API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDoor(IntPtr context, UInt32 deviceId, IntPtr doorIds, UInt32 doorIdCount, out IntPtr doorObj, out UInt32 numDoor);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDoor(IntPtr context, UInt32 deviceId, out IntPtr doorObj, out UInt32 numDoor);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDoorStatus(IntPtr context, UInt32 deviceId, IntPtr doorIds, UInt32 doorIdCount, out IntPtr doorStatusObj, out UInt32 numDoorStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDoorStatus(IntPtr context, UInt32 deviceId, out IntPtr doorStatusObj, out UInt32 numDoorStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDoor(IntPtr context, UInt32 deviceId, IntPtr doors, UInt32 doorCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDoorAlarm(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDoor(IntPtr context, UInt32 deviceId, IntPtr doors, UInt32 doorCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllDoor(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ReleaseDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_LockDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UnlockDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Fingerprint API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLastFingerprintImage(IntPtr context, UInt32 deviceId, out IntPtr imageObj, out UInt32 imageWidth, out UInt32 imageHeight);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanFingerprint(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger, UInt32 templateIndex, UInt32 quality, byte templateFormat, OnReadyToScan cbReadyToScan);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyFingerprint(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Log API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLog(IntPtr context, UInt32 deviceId, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFilteredLog(IntPtr context, UInt32 deviceId, IntPtr uid, UInt16 eventCode, UInt32 start, UInt32 end, byte tnakey, out IntPtr logObjs, out UInt32 numLog);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetImageLog(IntPtr context, UInt32 deviceId, UInt32 eventId, out IntPtr imageObj, out UInt32 imageSize);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearLog(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_StartMonitoringLog(IntPtr context, UInt32 deviceId, OnLogReceived cbOnLogReceived);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_StopMonitoringLog(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< MISC API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_FactoryReset(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RebootDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_LockDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UnlockDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceTime(IntPtr context, UInt32 deviceId, out UInt32 gmtTime);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceTime(IntPtr context, UInt32 deviceId, UInt32 gmtTime);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UpgradeFirmware(IntPtr context, UInt32 deviceId, IntPtr firmwareData, UInt32 firmwareDataLen, byte keepVerifyingSlaveDevice, OnProgressChanged cbProgressChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UpdateResource(IntPtr context, UInt32 deviceId, ref BS2ResourceElement resourceElement, byte keepVerifyingSlaveDevice, OnProgressChanged cbProgressChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_SetKeepAliveTimeout(IntPtr context, long ms);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_MakePinCode(IntPtr context, string salt, [In, Out] IntPtr pinCode);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ComputeCRC16CCITT(IntPtr data, UInt32 dataLen, ref UInt16 crc);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCardModel(string modelName, out UInt16 cardModel);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr BS2_Version();

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Slave Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSlaveDevice(IntPtr context, UInt32 deviceId, out IntPtr slaveDeviceObj, out UInt32 slaveDeviceCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSlaveDevice(IntPtr context, UInt32 deviceId, IntPtr slaveDeviceObj, UInt32 slaveDeviceCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Server Matching API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetServerMatchingHandler(IntPtr context, OnVerifyUser cbOnVerifyUser, OnIdentifyUser cbOnIdentifyUser);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyUser(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlob userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IdentifyUser(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlob userBlob);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< User API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserDatabaseInfo(IntPtr context, UInt32 deviceId, out UInt32 numUsers, out UInt32 numCards, out UInt32 numFingers, out UInt32 numFaces, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserList(IntPtr context, UInt32 deviceId, out IntPtr outUidObjs, out UInt32 outNumUids, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserInfos(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserDatas(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs, UInt32 userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrolUser(IntPtr context, UInt32 deviceId, [In, Out] BS2UserBlob[] userBlobs, UInt32 uidCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveUser(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllUser(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Wiegand Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SearchWiegandDevices(IntPtr context, UInt32 deviceId, out IntPtr wiegandDeviceObj, out UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandDevices(IntPtr context, UInt32 deviceId, out IntPtr wiegandDeviceObj, out UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_AddWiegandDevices(IntPtr context, UInt32 deviceId, IntPtr wiegandDevice, UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveWiegandDevices(IntPtr context, UInt32 deviceId, IntPtr wiegandDevice, UInt32 numWiegandDevice);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Zone Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAntiPassbackZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAntiPassbackZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAntiPassbackZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAllAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTimedAntiPassbackZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllTimedAntiPassbackZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAllTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllFireAlarmZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFireAlarmZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllFireAlarmZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFireAlarmZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllFireAlarmZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetScheduledLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllScheduledLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetScheduledLockUnlockZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllScheduledLockUnlockZone(IntPtr context, UInt32 deviceId);
    }
}
