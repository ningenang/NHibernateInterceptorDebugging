# NhibernateInterceptorDebugging

Requirements:
* Visual Studio
* .NET 4.5
* SQL Server 2008+, with System CLR Types for SQL Server installed
    * [System CLR Types for SQL Server 2008](https://www.microsoft.com/en-us/download/details.aspx?id=26728)
    * [System CLR Types for SQL Server 2012](https://www.microsoft.com/en-us/download/details.aspx?id=29065)
    * [System CLR Types for SQL Server 20016](https://www.microsoft.com/en-us/download/details.aspx?id=52676)

To run the application:
1. Create the required database by running *DAL/Scripts/CreateDatabase.sql*
    * **Note**: Make sure the paths at the start of the script point to a suitable location.
2. Open *LoggedInPersonIDInterceptor.sln*.
3. Right-click the Solution | Properties | Common Properties | Startup Project.
4. Select *Multiple startup projects*. Set *Action* to *Start* for the projects *WcfService* and *ConsoleApp*.
5. Start the application.