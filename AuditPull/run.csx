#r "O365ETL.dll"

using System;


public static void Run(TimerInfo myTimer, TraceWriter log)
{
    string connstring =  System.Configuration.ConfigurationManager.ConnectionStrings["AuditDb"].ConnectionString;
    string schema = System.Configuration.ConfigurationManager.ConnectionStrings["Schema"].ConnectionString;
    string clientSecret = System.Configuration.ConfigurationManager.ConnectionStrings["ClientSecret"].ConnectionString;
    string tenant = System.Configuration.ConfigurationManager.ConnectionStrings["Tenant"].ConnectionString;
    string clientId = System.Configuration.ConfigurationManager.ConnectionStrings["ClientId"].ConnectionString;
    string recordsToProcess =  System.Configuration.ConfigurationManager.ConnectionStrings["recordsToProcess"].ConnectionString;
    recordsToProcess = String.IsNullOrEmpty(recordsToProcess) ? "20" : recordsToProcess;
	int daysToRetrieve;
	daysToRetrieve = 7;
	
    for (int i = 0; i < daysToRetrieve; i++)
	{
		DateTime dateToProcess = DateTime.UtcNow.AddDays(-1*i);
		try
		{
			
			var result =
				O365ETL.GetOfficeData.Process(clientId, clientSecret, tenant, dateToProcess, connstring, schema, recordsToProcess).Result;

		}
		catch (Exception ex)
		{
			throw(ex);
		}
	}
	
	O365ETL.SQLOperations.CreateSP(schema, connstring);
	O365ETL.SQLOperations.RunStoredProc(connstring, schema + ".uspMoveStaging");
}
