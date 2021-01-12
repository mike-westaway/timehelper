# Resources Deployed

A sucessful deployment will result in a single resource group in your Azure Subscription containing all the necessary resources.  This resource group will be called:

<application_alias>timehelper-rg

Each resource is briefly described in the following table (application_aliastimehelper is shortened to app in this table):

| Type | Purpose | Name |
| --------------- | --------------- | --------------- |
| Storage Account | Holds dummy data used for testing in container timehelper-dummy-data.  Also contains a logs container with detailed output from the installation process | app+random |
| App Service | App Service to host Api Application | app-api |
| App Service | App Service to host Web Application | app-web |
| App Service | App Service to host Client Application | app-client |
| Azure SQL Database Server | Hosts rules database | app-db-server|
| Azure SQL Database | Contains tables forstoring associating projects with calendar entries | app-db |
| Application Insights | Monitoring and daignostics for web and api applications | app-web |
| App Service Plan | App Service Pricing and Scaling for the three app services | app-plan |

** Insert screenshot here **
