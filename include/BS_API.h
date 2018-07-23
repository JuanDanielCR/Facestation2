/*
 * BS_API.h
 *
 *  Created on: 2015. 4. 10.
 *      Author: scpark
 */

#ifndef CORE_SRC_BS_API_H_
#define CORE_SRC_BS_API_H_

#include "BSCommon/BS2Types.h"
#include "BSCommon/config/BS2FactoryConfig.h"
#include "BSCommon/config/BS2SystemConfig.h"
#include "BSCommon/config/BS2AuthConfig.h"
#include "BSCommon/config/BS2StatusConfig.h"
#include "BSCommon/config/BS2DisplayConfig.h"
#include "BSCommon/config/BS2IpConfig.h"
#include "BSCommon/config/BS2IpConfigExt.h"
#include "BSCommon/config/BS2TnaExtConfig.h"
#include "BSCommon/config/BS2CardConfig.h"
#include "BSCommon/config/BS2FingerprintConfig.h"
#include "BSCommon/config/BS2Rs485Config.h"
#include "BSCommon/config/BS2WiegandConfig.h"
#include "BSCommon/config/BS2WiegandDeviceConfig.h"
#include "BSCommon/config/BS2InputConfig.h"
#include "BSCommon/config/BS2WlanConfig.h"
#include "BSCommon/config/BS2TriggerActionConfig.h"
#include "BSCommon/config/BS2EventConfig.h"
#include "BSCommon/config/BS2WiegandMultiConfig.h"
#include "BSCommon/data/BS2AccessGroup.h"
#include "BSCommon/data/BS2AccessLevel.h"
#include "BSCommon/data/BS2Action.h"
#include "BSCommon/data/BS2AntiPassbackZone.h"
#include "BSCommon/data/BS2BlackList.h"
#include "BSCommon/data/BS2CSNCard.h"
#include "BSCommon/data/BS2DaySchedule.h"
#include "BSCommon/data/BS2Device.h"
#include "BSCommon/data/BS2Door.h"
#include "BSCommon/data/BS2Event.h"
#include "BSCommon/data/BS2EventExt.h"
#include "BSCommon/data/BS2Face.h"
#include "BSCommon/data/BS2Fingerprint.h"
#include "BSCommon/data/BS2FireAlarmZone.h"
#include "BSCommon/data/BS2ScheduledLockUnlockZone.h"
#include "BSCommon/data/BS2Holiday.h"
#include "BSCommon/data/BS2Resource.h"
#include "BSCommon/data/BS2Rs485Channel.h"
#include "BSCommon/data/BS2Rs485SlaveDeviceSetting.h"
#include "BSCommon/data/BS2Schedule.h"
#include "BSCommon/data/BS2SmartCard.h"
#include "BSCommon/data/BS2TimedAntiPassbackZone.h"
#include "BSCommon/data/BS2Trigger.h"
#include "BSCommon/data/BS2User.h"
#include "BSCommon/data/BS2Zone.h"

#ifdef BS_SDK_V2_DLL
#define BS_API_EXPORT __declspec(dllimport)
#define BS_CALLING_CONVENTION __cdecl
#else
#define BS_API_EXPORT
#define BS_CALLING_CONVENTION
#endif


#ifndef BS2_MAX_NUM_OF_CARD_PER_USER
#define BS2_MAX_NUM_OF_CARD_PER_USER                8
#endif

#ifndef BS2_MAX_NUM_OF_FINGER_PER_USER
#define BS2_MAX_NUM_OF_FINGER_PER_USER              10
#endif

#ifndef BS2_MAX_NUM_OF_FACE_PER_USER
#define BS2_MAX_NUM_OF_FACE_PER_USER                5
#endif

#ifndef BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER
#define BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER        16
#endif

#pragma pack(1)

typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto user_photo;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];
}BS2UserBlob;

typedef struct
{
	uint16_t eventMask;
	BS2_EVENT_ID id;
	BS2EventExtInfo info;                         // valid if eventMask has BS2_EVENT_MASK_INFO
	union
	{
		BS2_USER_ID userID;                       // valid if eventMask has BS2_EVENT_MASK_USER_ID
		uint8_t cardID[BS2_CARD_DATA_SIZE];       // valid if eventMask has BS2_EVENT_MASK_CARD_ID
		BS2_DOOR_ID doorID;                       // valid if eventMask has BS2_EVENT_MASK_DOOR_ID
		BS2_ZONE_ID zoneID;                       // valid if eventMask has BS2_EVENT_MASK_ZONE_ID
		BS2EventExtIoDevice ioDevice;             // valid if eventMask has BS2_EVENT_MASK_IODEVICE
	};
	BS2_TNA_KEY tnaKey;                           // valid if eventMask has BS2_EVENT_MASK_TNA_KEY
	BS2_JOB_CODE jobCode;                         // valid if eventMask has BS2_EVENT_MASK_JOB_CODE
	uint16_t imageSize;                           // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t image[BS2_EVENT_MAX_IMAGE_SIZE];      // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t reserved;
}BS2EventBlob;

typedef struct
{
	BS2_DEVICE_ID id;
	BS2_DEVICE_TYPE type;
	BS2_CONNECTION_MODE connectionMode;
    uint32_t ipv4Address;
    BS2_PORT port;
    uint32_t maxNumOfUser;
	uint8_t userNameSupported;
	uint8_t userPhotoSupported;
	uint8_t pinSupported;
	uint8_t cardSupported;
	uint8_t fingerSupported;
	uint8_t faceSupported;
	uint8_t wlanSupported;
	uint8_t tnaSupported;
	uint8_t triggerActionSupported;
	uint8_t wiegandSupported;
	uint8_t imageLogSupported;
	uint8_t dnsSupported;
	uint8_t jobCodeSupported;
	uint8_t wiegandMultiSupported;
}BS2SimpleDeviceInfo;

typedef struct
{
	uint32_t configMask;
	BS2FactoryConfig factoryConfig;
	BS2SystemConfig systemConfig;
	BS2AuthConfig authConfig;
	BS2StatusConfig statusConfig;
	BS2DisplayConfig displayConfig;
	BS2IpConfig ipConfig;
	BS2IpConfigExt ipConfigExt;
	BS2TNAConfig tnaConfig;
	BS2CardConfig cardConfig;
	BS2FingerprintConfig fingerprintConfig;
	BS2Rs485Config rs485Config;
	BS2WiegandConfig wiegandConfig;
	BS2WiegandDeviceConfig wiegandDeviceConfig;
	BS2InputConfig inputConfig;
	BS2WlanConfig wlanConfig;
	BS2TriggerActionConfig triggerActionConfig;
	BS2EventConfig eventConfig;
	BS2WiegandMultiConfig wiegandMultiConfig;
}BS2Configs;

typedef struct
{
	BS2_RESOURCE_TYPE type;
	uint32_t numResData;
	struct {
		uint8_t index;
		uint32_t dataLen;
		uint8_t* data;
	}resData[128];
}BS2ResourceElement;

typedef struct
{
	uint8_t isSmartCard;
	union
	{
		BS2CSNCard card;
		BS2SmartCardData smartCard;
	};
}BS2Card;

typedef struct
{
	BS2_DEVICE_ID parentDeviceID;
	BS2_DEVICE_ID deviceID;
	BS2_DEVICE_TYPE deviceType;
}BS2DeviceNode;

#pragma pack()

typedef void (*OnDeviceFound)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceAccepted)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceConnected)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceDisconnected)(BS2_DEVICE_ID deviceId);
typedef void (*OnReadyToScan)(BS2_DEVICE_ID deviceId, uint32_t sequence);
typedef void (*OnProgressChanged)(BS2_DEVICE_ID deviceId, uint32_t progressPercentage);
typedef void (*OnLogReceived)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnAlarmFired)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnInputDetected)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnConfigChanged)(BS2_DEVICE_ID deviceId, uint32_t configMask);
typedef void (*OnVerifyUser)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, uint8_t isCard, uint8_t cardType, const uint8_t* data, uint32_t dataLen);
typedef void (*OnIdentifyUser)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, BS2_FINGER_TEMPLATE_FORMAT format, const uint8_t* templateData, uint32_t templateSize);
typedef int (*IsAcceptableUserID)(const char* uid);

typedef uint32_t (*PreferMethod)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetRootCaFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetServerCaFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetServerPrivateKeyFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetPassword)(BS2_DEVICE_ID deviceID);
typedef void (*OnErrorOccured)(BS2_DEVICE_ID deviceID, int errCode);


#ifdef __cplusplus
extern "C"
{
#endif

BS_API_EXPORT void* BS_CALLING_CONVENTION BS2_AllocateContext();
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_Initialize(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceSearchingTimeout(void* context, uint32_t second);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetMaxThreadCount(void* context, uint32_t maxThreadCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IsAutoConnection(void* context, int* enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAutoConnection(void* context, int enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceEventListener(void* context,
                                            OnDeviceFound ptrDeviceFound,
											OnDeviceAccepted ptrDeviceAccepted,
                                            OnDeviceConnected ptrDeviceConnected,
                                            OnDeviceDisconnected ptrDeviceDisconnected);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetNotificationListener(void* context,
											OnAlarmFired ptrAlarmFired,
											OnInputDetected ptrInputDetected,
											OnConfigChanged ptrConfigChanged);
BS_API_EXPORT void BS_CALLING_CONVENTION BS2_ReleaseContext(void* context);
BS_API_EXPORT void BS_CALLING_CONVENTION BS2_ReleaseObject(void* object);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSSLHandler(void* context, PreferMethod ptrPreferMethod, GetRootCaFilePath ptrGetRootCaFilePath, GetServerCaFilePath ptrGetServerCaFilePath, GetServerPrivateKeyFilePath ptrGetServerPrivateKeyFilePath, GetPassword ptrGetPassword, OnErrorOccured ptrOnErrorOccured);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetServerPort(void* context, BS2_PORT serverPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchDevices(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDevices(void* context, BS2_DEVICE_ID** deviceListObj, uint32_t* numDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceInfo(void* context, BS2_DEVICE_ID deviceId, BS2SimpleDeviceInfo* deviceInfo);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ConnectDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ConnectDeviceViaIP(void* context, const char* deviceAddress, BS2_PORT defaultDevicePort, BS2_DEVICE_ID* deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_DisconnectDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceTopology(void* context, BS2_DEVICE_ID deviceId, BS2DeviceNode** networkNodeObj, uint32_t* numNetworkNode);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceTopology(void* context, BS2_DEVICE_ID deviceId, BS2DeviceNode* networkNode, uint32_t numNetworkNode);

// AccessControl api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_GROUP_ID* accessGroupIds, uint32_t accessGroupIdCount, BS2AccessGroup** accessGroupObj, uint32_t* numAccessGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2AccessGroup** accessGroupObj, uint32_t* numAccessGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2AccessGroup* accessGroups, uint32_t accessGroupCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_GROUP_ID* accessGroupIds, uint32_t accessGroupIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessGroup(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_LEVEL_ID* accessLevelIds, uint32_t accessLevelIdCount, BS2AccessLevel** accessLevelObj, uint32_t* numAccessLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2AccessLevel** accessLevelObj, uint32_t* numAccessLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2AccessLevel* accessLevels, uint32_t accessLevelCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_LEVEL_ID* accessLevelIds, uint32_t accessLevelIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessLevel(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2_SCHEDULE_ID* accessSheduleIds, uint32_t accessScheduleIdCount, BS2Schedule** accessScheduleObj, uint32_t* numAccessSchedule);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2Schedule** accessScheduleObj, uint32_t* numAccessSchedule);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2Schedule* accessShedules, uint32_t accessSheduleCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2_SCHEDULE_ID* accessSheduleIds, uint32_t accessScheduleIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessSchedule(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2_HOLIDAY_GROUP_ID* holidayGroupIds, uint32_t holidayGroupIdCount, BS2HolidayGroup** holidayGroupObj, uint32_t* numHolidayGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2HolidayGroup** holidayGroupObj, uint32_t* numHolidayGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2HolidayGroup* holidayGroups, uint32_t holidayGroupCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2_HOLIDAY_GROUP_ID* holidayGroupIds, uint32_t holidayGroupIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllHolidayGroup(void* context, BS2_DEVICE_ID deviceId);

// Blacklist api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount, BS2BlackList** blacklistObj, uint32_t* numBlacklist);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList** blacklistObj, uint32_t* numBlacklist);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllBlackList(void* context, BS2_DEVICE_ID deviceId);

// Card api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanCard(void* context,
													BS2_DEVICE_ID deviceId,
													BS2Card* card,
													OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_WriteCard(void* context, BS2_DEVICE_ID deviceId, BS2SmartCardData* smartCard);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EraseCard(void* context, BS2_DEVICE_ID deviceId);

// Config api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearDatabase(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ResetConfig(void* context, BS2_DEVICE_ID deviceId, uint8_t includingDB);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetConfig(void* context, BS2_DEVICE_ID deviceId, BS2Configs* configs);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetConfig(void* context, BS2_DEVICE_ID deviceId, BS2Configs* configs);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFactoryConfig(void* context, BS2_DEVICE_ID deviceId, BS2FactoryConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSystemConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSystemConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAuthConfig(void* context, BS2_DEVICE_ID deviceId, BS2AuthConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAuthConfig(void* context, BS2_DEVICE_ID deviceId, BS2AuthConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetStatusConfig(void* context, BS2_DEVICE_ID deviceId, BS2StatusConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetStatusConfig(void* context, BS2_DEVICE_ID deviceId, BS2StatusConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDisplayConfig(void* context, BS2_DEVICE_ID deviceId, BS2DisplayConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDisplayConfig(void* context, BS2_DEVICE_ID deviceId, BS2DisplayConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfig(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2IpConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfig(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2IpConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTNAConfig(void* context, BS2_DEVICE_ID deviceId, BS2TNAConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTNAConfig(void* context, BS2_DEVICE_ID deviceId, BS2TNAConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCardConfig(void* context, BS2_DEVICE_ID deviceId, BS2CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetCardConfig(void* context, BS2_DEVICE_ID deviceId, BS2CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFingerprintConfig(void* context, BS2_DEVICE_ID deviceId, BS2FingerprintConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFingerprintConfig(void* context, BS2_DEVICE_ID deviceId, BS2FingerprintConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetRS485Config(void* context, BS2_DEVICE_ID deviceId, BS2Rs485Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetRS485Config(void* context, BS2_DEVICE_ID deviceId, BS2Rs485Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandDeviceConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandDeviceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandDeviceConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandDeviceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetInputConfig(void* context, BS2_DEVICE_ID deviceId, BS2InputConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetInputConfig(void* context, BS2_DEVICE_ID deviceId, BS2InputConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWlanConfig(void* context, BS2_DEVICE_ID deviceId, BS2WlanConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWlanConfig(void* context, BS2_DEVICE_ID deviceId, BS2WlanConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTriggerActionConfig(void* context, BS2_DEVICE_ID deviceId, BS2TriggerActionConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTriggerActionConfig(void* context, BS2_DEVICE_ID deviceId, BS2TriggerActionConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetEventConfig(void* context, BS2_DEVICE_ID deviceId, BS2EventConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetEventConfig(void* context, BS2_DEVICE_ID deviceId, BS2EventConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandMultiConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandMultiConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandMultiConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandMultiConfig* config);

// Door api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount, BS2Door** doorObj, uint32_t* numDoor);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDoor(void* context, BS2_DEVICE_ID deviceId, BS2Door** doorObj, uint32_t* numDoor);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDoorStatus(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount, BS2DoorStatus** doorStatusObj, uint32_t* numDoorStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDoorStatus(void* context, BS2_DEVICE_ID deviceId, BS2DoorStatus** doorStatusObj, uint32_t* numDoorStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDoor(void* context, BS2_DEVICE_ID deviceId, BS2Door* doors, uint32_t doorCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDoorAlarm(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ALARM_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllDoor(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ReleaseDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_LockDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UnlockDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);

// Fingerprint api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLastFingerprintImage(void* context, BS2_DEVICE_ID deviceId, uint8_t** imageObj, uint32_t* imageWidth, uint32_t* imageHeight);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFingerprint(void* context,
															BS2_DEVICE_ID deviceId,
															BS2Fingerprint* finger,
															uint32_t templateIndex,
															uint32_t quality,
															uint8_t templateFormat, //BS2_FINGER_TEMPLATE_FORMAT
															OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFingerprintImage(void* context,
															BS2_DEVICE_ID deviceId,
															uint32_t templateIndex,
															uint32_t quality,
															uint8_t templateFormat, //BS2_FINGER_TEMPLATE_FORMAT
															unsigned char* imageData,
															OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyFingerprint(void* context, BS2_DEVICE_ID deviceId, BS2Fingerprint* finger);

// Log api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLog(void* context,
													BS2_DEVICE_ID deviceId,
													BS2_EVENT_ID eventId,
													uint32_t amount,
													BS2Event** logsObj,
													uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLog(void* context,
															BS2_DEVICE_ID deviceId,
															char* uid,
															BS2_EVENT_CODE eventCode,
															BS2_TIMESTAMP start,
															BS2_TIMESTAMP end,
															uint8_t tnakey,
															BS2Event** logsObj,
															uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetImageLog(void* context, BS2_DEVICE_ID deviceId, BS2_EVENT_ID eventId, uint8_t** imageObj, uint32_t* imageSize);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearLog(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_StartMonitoringLog(void* context, BS2_DEVICE_ID deviceId, OnLogReceived ptrLogReceived);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_StopMonitoringLog(void* context, BS2_DEVICE_ID deviceId);

// Misc api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_FactoryReset(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RebootDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_LockDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UnlockDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceTime(void* context, BS2_DEVICE_ID deviceId, BS2_TIMESTAMP* gmtTime);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceTime(void* context, BS2_DEVICE_ID deviceId, BS2_TIMESTAMP gmtTime);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UpgradeFirmware(void* context, BS2_DEVICE_ID deviceId, uint8_t* firmwareData, uint32_t firmwareDataLen, uint8_t keepVerifyingSlaveDevice, OnProgressChanged ptrProgressChanged);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UpdateResource(void* context, BS2_DEVICE_ID deviceId, BS2ResourceElement* resourceElement, uint8_t keepVerifyingSlaveDevice, OnProgressChanged ptrProgressChanged);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetKeepAliveTimeout(void* context, long ms);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_MakePinCode(void* context, char* plaintext, unsigned char* ciphertext);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ComputeCRC16CCITT(unsigned char* data, uint32_t dataLen, uint16_t* crc);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCardModel(char* modelName, BS2_CARD_MODEL* cardModel);
BS_API_EXPORT const char* BS_CALLING_CONVENTION BS2_Version();

// Slave Control api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSlaveDevice(void* context, BS2_DEVICE_ID deviceId, BS2Rs485SlaveDevice** slaveDeviceObj, uint32_t* slaveDeviceCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSlaveDevice(void* context, BS2_DEVICE_ID deviceId, BS2Rs485SlaveDevice* slaveDevices, uint32_t slaveDeviceCount);

// Server Matching api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetServerMatchingHandler(void* context, OnVerifyUser ptrVerifyUser, OnIdentifyUser ptrIdentifyUser);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUser(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IdentifyUser(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlob* userBlob);

// User api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatabaseInfo(void* context, BS2_DEVICE_ID deviceId, uint32_t* numUsers, uint32_t* numCards, uint32_t* numFingers, uint32_t* numFaces, IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserList(void* context,
														BS2_DEVICE_ID deviceId,
														char** uidsObj,
														uint32_t* numUid,
														IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfos(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatas(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrolUser(void* context, BS2_DEVICE_ID deviceId, BS2UserBlob* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveUser(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllUser(void* context, BS2_DEVICE_ID deviceId);

// Wiegand api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID** wiegandDeviceObj, uint32_t* numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID** wiegandDeviceObj, uint32_t* numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_AddWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID* wiegandDevice, uint32_t numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID* wiegandDevice, uint32_t numWiegandDevice);

// Zone api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2AntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2AntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2AntiPassbackZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAntiPassbackZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAllAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2TimedAntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2TimedAntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2TimedAntiPassbackZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTimedAntiPassbackZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAllTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2FireAlarmZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2FireAlarmZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFireAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllFireAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2FireAlarmZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFireAlarmZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllFireAlarmZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ScheduledLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2ScheduledLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetScheduledLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllScheduledLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2ScheduledLockUnlockZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetScheduledLockUnlockZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId);

#ifdef __cplusplus
}
#endif


#endif /* CORE_SRC_BS_API_H_ */
