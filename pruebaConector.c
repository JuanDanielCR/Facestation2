#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "include/BS_API.h"
#include "include/BS_Errno.h"
#include "/usr/include/postgresql/libpq-fe.h"

char* fechaTimeDate(uint32_t);

int main(int argc, char* argv[])
{
	/* 	Postgres variables
		PGconn - Conexión con la BD
		PGResult - Resultado de una transacción
		rec_count - Número de tublas obtenidas de una consulta
		row, col - Fila, COlumna a consultar	*/
	PGconn *conn;
 	PGresult *res;
 	int rec_count;
	int row;
	int col;

	/* Conexión con la base de datos mediante la función PQconnectdb, parametro un char* el cual contendrá separado por espacios los sig params:
	bdname=[DATABASE_NAME] host=[Direccion(IP) del host] port=[puerto de comunicación] user=[usuario] password=[contraseña] */
	conn = PQconnectdb("dbname=eld-posgrado-desarrollo host=192.168.226.164 port=5432 user=postgres password=postgres");
	if (PQstatus(conn) == CONNECTION_BAD)
	{
		puts("We were unable to connect to the database");
	} 
	else 
	{
		res = PQexec(conn, "SELECT id_maestria, id_rvoe, nb_maestria, tx_siglas FROM maestria.tma03_maestria;");
		if (PQresultStatus(res) != PGRES_TUPLES_OK)
		{
			puts("We did not get any data!");
		}
		else
		{
			/* Guardamos el numero de tuplas que hubo en el resultado */
			rec_count = PQntuples(res);
			printf("We received %d records.\n", rec_count);
			for (row=0; row<rec_count; row++)
			{
				for (col=0; col<4; col++)
				{
					printf("%s\t", PQgetvalue(res, row, col));
				}
				puts("");
			}
		}
		PQclear(res);
		PQfinish(conn);
	}
	

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
			const char* deviceAddress = "192.168.226.248";
			uint16_t devicePort = 51211;
			uint32_t deviceId = 0;
			printf("1. Initializing the SDK: \t%p\n--DeviceAddress:\t%s\n--DevicePort:\t%u\n--DeviceId:\t%u\n", context, deviceAddress, devicePort, deviceId);

			result = BS2_ConnectDeviceViaIP(context, deviceAddress, devicePort, &deviceId);
		    if(result == BS_SDK_SUCCESS)
		    {
		        printf("2. Connecting the device ID: %d\n", deviceId);
				
				/*3. Verifying the device function */
				BS2SimpleDeviceInfo deviceInfo;
				result = BS2_GetDeviceInfo(context, deviceId, &deviceInfo);
				if(result == BS_SDK_SUCCESS)
				{
					printf("3. Verifying device functions: \n --ID: %u \n --IPv4: %u \n --Connection mode: %u \n --MaxNumOfUsers: %u \n --Type: %u \n --FaceSupported: %u \n --PinSupported: %u \n --CardSupported: %u \n", 
						deviceInfo.id, deviceInfo.ipv4Address, deviceInfo.connectionMode ,deviceInfo.maxNumOfUser, deviceInfo.type, deviceInfo.faceSupported, deviceInfo.pinSupported, deviceInfo.cardSupported);
					/* 4. Getting device configuration */
					if(deviceInfo.faceSupported)
					{

					}
					if(deviceInfo.pinSupported)
					{

					}
					if(deviceInfo.cardSupported)
					{

					}
				}
				/* 5. Enroll a new User */

				/* 6. Managing the log */
				BS2Event* logs = NULL;
				uint32_t numLogs = 0;
				result = BS2_GetLog(context, deviceId, 0,0, &logs, &numLogs);
				if(result == BS_SDK_SUCCESS)
				{
					printf("NumLogs: %u \n", numLogs);
					for(i = 0; i < numLogs; i++)
					{
						puts("Log ");
					}
					BS2_ReleaseObject(logs);
				}

				/* 6.1 Filtered log*/
				char* uid = NULL;
				uint32_t endTime = time(NULL);
				uint32_t startTime = endTime - 60*60; //last hour

				result = BS2_GetFilteredLog(context, deviceId, NULL, 0, startTime, endTime, 0, &logs, &numLogs);
				if(result == BS_SDK_SUCCESS)
				{
					printf("NumLogs: %u \n", numLogs);
					for(i = 0 ; i < numLogs ; i++)
					{
						puts("Logo");
					}
					BS2_ReleaseObject(logs);
				}

				/* 7. User information */
				IsAcceptableUserID isAcceptableUserID = NULL;
				char* uidsObjs = NULL;
				uint32_t numUsers = 0;

				int result = BS2_GetUserList(context, deviceId, &uidsObjs, &numUsers, isAcceptableUserID);
				if(result == BS_SDK_SUCCESS)
				{
					printf("NumUsers: %u \n", numUsers);
					for (i = 0; i < numUsers; i++)
					{
						printf("UsuerID: %s\n", uidsObjs);
					}
				}
				BS2_ReleaseObject(uidsObjs);
		    }
		    else
		    {
		        printf("2. Failed to connect to device. (error code : 0x%x)\n", result);
		    }
		}
	}
    else
    {
        printf("1. Cannot initialize FaceStation context \n");
    }
    if(context != NULL)
    {
    	BS2_ReleaseContext(context);
    }
    return 0;
}