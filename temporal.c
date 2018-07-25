/* Log Variables */
				//uint32_t eventId = 0; //Esta variable y la sig. estan comentadas porque no son necesarias para un log filtrado
				//uint32_t amount = 0;
				//Lista donde se guardaran los eventos en el log que se obtenga
				BS2Event* logsObj;
				uint32_t numLog;

				//Variables para extras para la obención de un log filtrado
				char* uid = NULL;
				uint16_t eventCode = 0x1303;
				uint32_t start = 0;
				uint32_t end = 0;
				uint8_t tnakey = 0;


				BS2UserBlob userBlob;
				uint32_t uidCount = -200;

				//Parmetros para la lista de usuarios
				char* uidsObjs = NULL;
				uint32_t numUid = 0;
				IsAcceptableUserID ptrIsAcceptableUserID = NULL;
				uint32_t i = 0;
				
				//printf("estos son los componentes para obtener el log:\ncontexto:\t%p\ndeviceId:\t%u\nevenId:\t%u\namount:\t%u\n\n", context, deviceId, eventId, amount);
				printf("estos son los componentes para obtener el log:\ncontexto:\t%p\ndeviceId:\t%u\nuid:\t\t%s\neventCode:\t%x\nstart:\t\t%u\nend:\t\t%u\ntnakey:\t\t%u\n", context, deviceId, uid, eventCode, start, end, tnakey);
				int resultadoLog = BS2_GetFilteredLog(context, deviceId, uid, eventCode, start, end, tnakey, &logsObj, &numLog);
				//int resultadoLog = BS2_GetLog(context, deviceId, eventId, amount, &logsObj, &numLog);
				printf("Creo que este podrá ser el numero de resultados xD: %d\n", numLog);



				int resultListUser = BS2_GetUserList(context, deviceId, &uidsObjs, &numUid, ptrIsAcceptableUserID);
				if(resultListUser == BS_SDK_SUCCESS)
				{
					printf("Se obtuvo chido la lista y son %d usuarios! y son: \n", numUid);
					for (i = 0; i < numUid; i++)
					{
						printf("UsuerID:\t%s\n", uidsObjs);
					}
				}
				else
				{
					printf("nos dio el siguiente error %d o bien %x\n", resultListUser, resultListUser);
				}

				BS2_ReleaseObject(uidsObjs);

				if(resultadoLog == BS_SDK_SUCCESS)
				{
					printf("El log se consiguió correctamente\n");

					for(i = 0; i < numLog; i++)
					{
						//if(logsObj[i].subCode == 16 || logsObj[i].subCode == 19)
						//{
							uidCount = (uint32_t)atoi(logsObj[i].userID);
							//uidCount = (uint32_t)numUid +1;
							printf("parametros para buscar al usuario: \ncontexto:\t%p\ndeviceID:\t%u\nuserIDchar:\t%s\tuserIDuint:\t%u\nuidCount:\t%u\n", context,deviceId, logsObj[i].userID, atoi(logsObj[i].userID), uidCount);
							int resultinfo = BS2_GetUserInfos(context, deviceId, logsObj[i].userID, uidCount, &userBlob);
							printf("result de la info, si es un numero negativo entonses es error! buscar en BS_Errno.h: %d\n", resultinfo);
							printf("supuesto nombre%s\n\n", userBlob.user_name);
							//printf("id:\t\t%u\ndateTime:\t%s\ndeviceID:\t%u\nuserID:\t\t%s\nname:\t\t%s\nmainCode:\t%x\nsubCode:\t%x\ncode:\t\t%x\n\n", logsObj[i].id, fechaTimeDate(logsObj[i].dateTime), logsObj[i].deviceID, logsObj[i].userID, userBlob.user_name, logsObj[i].mainCode, logsObj[i].subCode, logsObj[i].code);
						//}
					}
				}
				else
				{
					printf("Failed to get log of device. (error code : 0x%x o bien %d)\n", resultadoLog, resultadoLog);
				}


//Método que convierte los segundos en una fecha
char* fechaTimeDate(uint32_t segundos)
{
	time_t fecha = segundos;
	struct tm* fechasa = localtime(&fecha);
	char* fechaChar = NULL;
	sprintf(fechaChar,"%d/%d/%d\t%d:%d:%d", fechasa->tm_mday, fechasa->tm_mon + 1, fechasa->tm_year + 1900, fechasa->tm_hour, fechasa->tm_min, fechasa->tm_sec);
	return fechaChar;
}