#!/bin/bash -e
output_blob=$OUTPUT_LOG_NAME
echo "<h2>TimeHelper Web Site</h2>" >> $output_blob
echo "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
echo    Deploying Time Helper Infrastructure
echo "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
echo ---Global Variables
echo "TIMEHELPER_ALIAS: $TIMEHELPER_ALIAS"
echo "TIMEHELPER_LOCATION: $TIMEHELPER_LOCATION"
echo "DB_ADMIN_USER: $DB_ADMIN_USER"
echo "DB_ADMIN_PASSWORD: $DB_ADMIN_PASSWORD"
echo "TIMEHELPER_API_SCOPE: $TIMEHELPER_API_SCOPE"
az config set extension.use_dynamic_install=yes_without_prompt
echo
# set local variables
appservice_webapp_sku="S1 Standard"
database_edition="Basic"
echo ---Local Variables
echo "App service Sku: $appservice_webapp_sku"
echo "Database Edition: $database_edition"
echo 
# Derive as many variables as possible
applicationName="${TIMEHELPER_ALIAS}"
storageSuffix=$RANDOM
storageAccountName="$applicationName$storageSuffix"
webAppName="${applicationName}-web"
apiAppName="${applicationName}-api"
clientAppName="${applicationName}-client"
hostingPlanName="${applicationName}-plan"
dbServerName="${applicationName}-db-server"
dbName="${applicationName}-db"
resourceGroupName="${applicationName}-rg"
timehelperApiBaseUrl="https://${applicationName}-api.azurewebsites.net/"
timehelperApiDefaultScope="${TIMEHELPER_API_SCOPE}.default"
timehelperApiScope="${TIMEHELPER_API_SCOPE}access_as_user"
swaggerTermsUri="${webAppName}.azurewebsites.net/Home/Terms"
echo ---Derived Variables
echo "Application Name: $applicationName"
echo "Resource Group Name: $resourceGroupName"
echo "Web App Name: $webAppName"
echo "Api App Name: $apiAppName"
echo "Client App Name: $clientAppName"
echo "Hosting Plan: $hostingPlanName"
echo "DB Server Name: $dbServerName"
echo "DB Name: $dbName"
echo "timehelper base url: $timehelperApiBaseUrl"
echo "timehelper api default scope: $timehelperApiDefaultScope"
echo "timehelper api scope: $timehelperApiScope"
echo "swagger terms uri: $swaggerTermsUri"
echo "SWAGGER_CONTACT_URL: $SWAGGER_CONTACT_URL"
echo "SWAGGER_CONTACT_NAME: $SWAGGER_CONTACT_NAME"
echo "SWAGGER_CONTACT_EMAIL: $SWAGGER_CONTACT_EMAIL"
echo

echo "Creating resource group $resourceGroupName in $TIMEHELPER_LOCATION"
az group create -l "$TIMEHELPER_LOCATION" --n "$resourceGroupName" --tags  TimeHelperInstance=$TIMEHELPER_INSTANCE Application=TimeHelper MicroserviceID=$applicationName PendingDelete=$PENDING_DELETE >> $output_blob
echo "<p>Resource Group: $resourceGroupName</p>" >> $output_blob

echo "Creating storage account $storageAccountName in $TIMEHELPER_LOCATION"
az storage account create  --name $storageAccountName  --location $TIMEHELPER_LOCATION  --resource-group $resourceGroupName  --sku Standard_LRS >> $output_blob
storageAccountConnectionString=$(az storage account show-connection-string -g $resourceGroupName -n $storageAccountName -o tsv)
echo "<p>Storage Account: $storageAccountName</p>" >> $output_blob
dummyDataBlobContainerName='timehelper-dummy-data'
expiry=$(date --date="1 month" +%F)
az storage container create -n $dummyDataBlobContainerName --connection-string $storageAccountConnectionString >> $output_blob
dummyDataBlobContainer=$(az storage account show -n $storageAccountName --query primaryEndpoints.blob -o tsv)$dummyDataBlobContainerName
dummyDataSasToken=$(az storage container generate-sas -n $dummyDataBlobContainerName --connection-string $storageAccountConnectionString --expiry $expiry --permissions acdlrw -o tsv)
dummyDataContainerSasUri="$dummyDataBlobContainer$quickstartContainer?$dummyDataSasToken"
azcopy copy keith2@nikkh.net.dummy.json $dummyDataContainerSasUri --recursive=false --from-to LocalBlob
echo "dummyDataBlobContainer=$dummyDataBlobContainer"

echo "Creating Azure Sql Resources in $TIMEHELPER_LOCATION"
echo "<p>Azure Sql Server: $dbServerName</p>" >> $output_blob
az sql server create -n $dbServerName -g $resourceGroupName -l $TIMEHELPER_LOCATION -u $DB_ADMIN_USER -p $DB_ADMIN_PASSWORD >> $output_blob
az sql server firewall-rule create -g $resourceGroupName -s $dbServerName -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 >> $output_blob
echo "<p>Azure Sql Database: $databaseName</p>" >> $output_blob
az sql db create -g $resourceGroupName -s $dbServerName -n $dbName --service-objective S0 >> $output_blob
baseDbConnectionString=$(az sql db show-connection-string -c ado.net -s $dbServerName -n $dbName -o tsv)
dbConnectionStringWithUser="${baseDbConnectionString/<username>/$DB_ADMIN_USER}"
sqlConnectionString="${dbConnectionStringWithUser/<password>/$DB_ADMIN_PASSWORD}"



echo "Creating app service hosting plan $apiAppName in group $resourceGroupName"
echo "<p>Hosting Plan: $hostingPlanName</p>" >> $output_blob
az  appservice plan create -g $resourceGroupName --name $hostingPlanName --location $TIMEHELPER_LOCATION --number-of-workers 1 --sku S1 --is-linux >> $output_blob

echo "Creating app insights component $webAppName in group $resourceGroupName"
echo "<p>Application Insights: $webAppName</p>" >> $output_blob
az monitor app-insights component create --app $webAppName --location $TIMEHELPER_LOCATION --kind web -g $resourceGroupName --application-type web >> $output_blob
aIKey=$(az monitor app-insights component show --app $webAppName -g $resourceGroupName --query instrumentationKey -o tsv)
echo "Application Insights Key: $aIKey"
APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$aIKey;"
APPINSIGHTS_INSTRUMENTATIONKEY=$aIKey
ApplicationInsightsAgent_EXTENSION_VERSION='~2'

echo "Creating app service for web site $webAppName in group $resourceGroupName"
echo "<p>App Service (Web App): $webAppName</p>" >> $output_blob

az webapp create \
  --name $webAppName \
  --plan $hostingPlanName \
  --resource-group $resourceGroupName \
  --runtime  "DOTNETCORE|3.1" >> $output_blob

echo "Updating App Settings for web site $webAppName"
echo "<p>Web App Settings:" >> $output_blob
az webapp config appsettings set -g $resourceGroupName -n $webAppName --settings Api__TimeHelperApiBaseAddress=$timehelperApiBaseUrl ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientSecret=$AAD_CLIENTSECRET APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION TimeHelperApiDefaultScope=timehelperApiDefaultScope TimeHelperApiScope=timehelperApiScope >> output_blob
echo "</p>" >> $output_blob

echo "Creating app service for client application $clientAppName in group $resourceGroupName"
echo "<p>App Service (Web App): $clientAppName</p>" >> $output_blob

az webapp create \
  --name $clientAppName \
  --plan $hostingPlanName \
  --resource-group $resourceGroupName \
  --runtime  "DOTNETCORE|3.1" >> $output_blob

echo "Updating App Settings for web site $clientAppName"
echo "<p>Client App Settings:" >> $output_blob
az webapp config appsettings set -g $resourceGroupName -n $webAppName --settings Api__TimeHelperApiBaseAddress=$timehelperApiBaseUrl ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientSecret=$AAD_CLIENTSECRET APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION TimeHelperApiDefaultScope=timehelperApiDefaultScope TimeHelperApiScope=timehelperApiScope >> output_blob
echo "</p>" >> $output_blob

echo "<p>App Service (Api App): $apiAppName</p>" >> $output_blob

az webapp create \
  --name $apiAppName \
  --plan $hostingPlanName \
  --resource-group $resourceGroupName \
  --runtime  "DOTNETCORE|3.1" >> $output_blob
  
echo "Updating App Settings for api $apiAppName"
echo "<p>Web App Settings:" >> $output_blob
az webapp config appsettings set -g $resourceGroupName -n $apiAppName --settings ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientSecret=$AAD_CLIENTSECRET APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION ContactUri=$SWAGGER_CONTACT_URL ContactName=$SWAGGER_CONTACT_NAME ContactEmail=$SWAGGER_CONTACT_EMAIL TermsUri=$swaggerTermsUri DummyDataBlobContainer=$dummyDataBlobContainer >> output_blob
az webapp config connection-string set -g $resourceGroupName -n $apiAppName -t SQLAzure --settings TimeHelperDataContext=$sqlConnectionString
echo "</p>" >> $output_blob

echo "OUTPUT_LOGGING=$OUTPUT_LOGGING"
if [ "$OUTPUT_LOGGING" = TRUE ]; then
 cat $output_blob
else
 cat $output_blob
fi
