/* Log Variables */
//uint32_t eventId = 0; //Esta variable y la sig. estan comentadas porque no son necesarias para un log filtrado
//uint32_t amount = 0;
BS2Event* logsObj;
uint32_t numLog;

//Variables para extras para la obención de un log filtrado
	                    
	uint32_t start = 0;
	uint32_t end = 0;

	BS2UserBlob userBlob;
	uint32_t uidCount = -200;

	int resultadoLog = BS2_GetFilteredLog(context, deviceId, uid, eventCode, start, end, tnakey, &logsObj, &numLog);
	if(resultadoLog == BS_SDK_SUCCESS)
	{
		printf("El log se consiguió correctamente\n");
    	for(i = 0; i < numLog; i++)
		{
			//if(logsObj[i].subCode == 16 || logsObj[i].subCode == 19)
			//{
		    	uidCount = (uint32_t)atoi(logsObj[i].userID);
	    	    //uidCount = (uint32_t)numUid +1;
				
				
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