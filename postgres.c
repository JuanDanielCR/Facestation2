#include "/usr/include/postgresql/libpq-fe.h"


	/*	Postgres variables
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
	bdname=[DATABASE_NAME] host=[Direccion(IP) del host] port=[puerto de comunicación] user=[usuario] password=[contraseña] 
	/*conn = PQconnectdb("dbname=eld-posgrado-desarrollo host=192.168.226.164 port=5432 user=postgres password=postgres");*/
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


    ////gcc pruebaConector.c -L/usr/include/postgresql/libpq/ -lpq -L/usr/include -lBS_SDK_V2 -lxml2 -o hazz -pthread