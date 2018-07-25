#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "include/BS_API.h"
#include "include/BS_Errno.h"


char* fechaTimeDate(uint32_t);
void callbackForLog();

int main(int argc, char* argv[])
{

	/*1. Initializing the SDK */
    void* context = NULL;
    context = BS2_AllocateContext();
	uint32_t i = 0;

    if(context != NULL)
    {
        int result = BS2_Initialize(context);
        if(result == BS_SDK_SUCCESS)
        {
			/*2. Connecting the device with variables (Se obtienen en: Menú de la facestation -> Información del dispositivo) */
			const char* deviceAddress = "192.168.226.149";
			uint16_t devicePort = 51211;
			uint32_t deviceId = 0;
			printf("1. Initializing the SDK: %p\n--DeviceAddress: %s\n--DevicePort: %u\n--DeviceId: %u\n", context, deviceAddress, devicePort, deviceId);
		    puts("antes");
			result = BS2_ConnectDeviceViaIP(context, deviceAddress, devicePort, &deviceId);
		    if(result == BS_SDK_SUCCESS)
		    {
				puts("arriba");
		        printf("2. Connecting the device ID: %d\n", deviceId);
				
				/*3. Verifying the device function */
				BS2SimpleDeviceInfo deviceInfo;
				result = BS2_GetDeviceInfo(context, deviceId, &deviceInfo);
				if(result == BS_SDK_SUCCESS)
				{
					printf("3. Verifying device functions: \n --ID: %u \n --IPv4: %u \n --Connection mode: %u \n --MaxNumOfUsers: %u \n --Type: %#X \n --FaceSupported: %u \n --PinSupported: %u \n --CardSupported: %u \n --userNameSupported: %u \n --userPhotoSupported: %u \n --fingerSupported: %u \n", 
						deviceInfo.id, deviceInfo.ipv4Address, 
						deviceInfo.connectionMode ,
						deviceInfo.maxNumOfUser, 
						deviceInfo.type, 
						deviceInfo.faceSupported, 
						deviceInfo.pinSupported, 
						deviceInfo.cardSupported, 
						deviceInfo.userNameSupported, 
						deviceInfo.userNameSupported, 
						deviceInfo.fingerSupported);
					/* 4. Getting device configuration */
					// BS2FaceConfig not supported in this SDK version
					if(deviceInfo.faceSupported)
					{
						puts("Faces supported");
					}
					if(deviceInfo.pinSupported)
					{
						puts("Pin supported");
					}
					if(deviceInfo.cardSupported)
					{
						puts("Card supported");
					}
				}
				/* 5. Enroll a new User */
				
				/* 6. Managing the log */
				BS2Event* logs = NULL;
				uint32_t numLogs = 0;
				printf("DEVICEID: %u \n", deviceId);
				result = BS2_GetLog(context, deviceId, 0,0, &logs, &numLogs);
				if(result == BS_SDK_SUCCESS)
				{
					uint8_t logsFromDevice = 0;
					printf("NumLogs: %u \n", numLogs);
					for(i = 0; i < numLogs; i++)
					{
						BS2Event aux = logs[i];
						if(542340166 == aux.deviceID || 0 != aux.id){
							logsFromDevice++;
							printf("--UserId: %u --ID: %u --Code: %u -> 0x%X  --Subcode: %u -> 0x%X --Date: %u --DeviceID: %u \n", 
							(uint32_t)atoi(logs[i].userID), 
							aux.id, 
							aux.mainCode, 
							aux.mainCode,
							aux.subCode,
							aux.subCode,
							aux.dateTime,
							aux.deviceID);
						}
					}
					printf("NumLogs from Device: %u \n", logsFromDevice);
					BS2_ReleaseObject(logs);
				}
				/*OnLogReceived ptrLogReceived = &callbackForLog;
				result = BS2_StartMonitoringLog(context, deviceId, callbackForLog);
				if(result == BS_SDK_SUCCESS)
				{
					puts("Start monitoring log");
				}*/
				/* 6.1 Filtered log
				char* uid = NULL;
				uint32_t endTime = time(NULL);
				uint32_t startTime = endTime - 60*60; //last hour

				result = BS2_GetFilteredLog(context, deviceId, NULL, 0, startTime, endTime, 0, &logs, &numLogs);
				if(result == BS_SDK_SUCCESS)
				{
					printf("NumLogs: %u \n", numLogs);
					for(i = 0 ; i < numLogs ; i++)
					{
						printf("Log --UserId: %u --ID: %u --Code: 0x%x  --Subcode: 0x%x --Date: %u\n", 
							(uint32_t)atoi(logs[i].userID), 
							logs[i].id , logs[i].mainCode, 
							logs[i].subCode, 
							logs[i].dateTime);
					}
				}
				BS2_ReleaseObject(logs);

				/* 7. User information 
				IsAcceptableUserID isAcceptableUserID = NULL;
				char* uidsObjs = NULL;
				uint32_t numUsers = 0;

				int result = BS2_GetUserList(context, deviceId, &uidsObjs, &numUsers, isAcceptableUserID);
				if(result == BS_SDK_SUCCESS)
				{
					printf("NumUsers: %u \n", numUsers);
					for (i = 0; i < numUsers; i++)
					{
						printf("UsuerID: %u\n", uidsObjs[i]);
					}
				}
				BS2_ReleaseObject(uidsObjs);*/
		    }
		    else
		    {
				puts("aqui");
		        printf("2. Failed to connect to device. (error code : 0x%x)\n", result);
		    }
			puts("despues");
		}
	}
    else
    {
        printf("1. Cannot initialize FaceStation context \n");
    }
	puts("saliendo ifs anidados");
    if(context != NULL)
    {
    	BS2_ReleaseContext(context);
    }
	puts("fin");
    return 0;
}

void callbackForLog(){
	puts("Callback");
}