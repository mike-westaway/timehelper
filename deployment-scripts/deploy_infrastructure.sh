#!/bin/bash -e
output_blob=$OUTPUT_LOG_NAME
echo '<!DOCTYPE html><html><head> </head><body>' >> $output_blob
echo '<h1>Deployment Log</h1>' >> $output_blob
resourcesLink="https://portal.azure.com/#blade/HubsExtension/BrowseResourcesWithTag/tagName/TimeHelperInstance/tagValue/$TIMEHELPER_INSTANCE"
echo '<a href="'$resourcesLink'">Click here to access your TimeHelper resources in Azure</a>' >> $output_blob
echo '<p></p>' >>$output_blob
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

echo "Creating resource group $resourceGroupName in $TIMEHELPER_LOCATION"
az group create -l "$TIMEHELPER_LOCATION" --n "$resourceGroupName" --tags  TimeHelperInstance=$TIMEHELPER_INSTANCE Application=TimeHelper MicroserviceID=$applicationName PendingDelete=$PENDING_DELETE >> $output_blob
echo "<p>Resource Group: $resourceGroupName</p>" >> $output_blob

echo "Creating storage account $storageAccountName in $TIMEHELPER_LOCATION"
echo "<p>Storage Account: $storageAccountName</p>"

az storage account create  --name $storageAccountName  --location $TIMEHELPER_LOCATION  --resource-group $resourceGroupName  --sku Standard_LRS >> $output_blob
storageConnectionString=$(az storage account show-connection-string -n $storageAccountName -g $resourceGroupName --query connectionString -o tsv)
echo "storageConnectionString=$storageConnectionString" >> $output_blob
export AZURE_STORAGE_CONNECTION_STRING="$storageConnectionString" 
echo "AZURE_STORAGE_CONNECTION_STRING=$AZURE_STORAGE_CONNECTION_STRING" >> $output_blob
dummyDataContainerName='timehelper-dummy-data'
echo "dummyDataContainerName=$dummyDataContainerName"
az storage container create -n $dummyDataContainerName --public-access off >> $output_blob
az storage blob upload -c $dummyDataContainerName -f './keith2@nikkh.net.dummy.json' -n 'keith2@nikkh.net.dummy.json' >> $output_blob
echo "Uploaded 'keith2@nikkh.net.dummy.json' to container $dummyDataContainerName in storage account $storageAccountName"

echo "Creating Azure Sql Resources in $TIMEHELPER_LOCATION"
echo "<p>Azure Sql Server: $dbServerName</p>" >> $output_blob
az sql server create -n $dbServerName -g $resourceGroupName -l $TIMEHELPER_LOCATION -u $DB_ADMIN_USER -p $DB_ADMIN_PASSWORD >> $output_blob
az sql server firewall-rule create -g $resourceGroupName -s $dbServerName -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 >> $output_blob
echo "<p>Azure Sql Database: $databaseName</p>" >> $output_blob
az sql db create -g $resourceGroupName -s $dbServerName -n $dbName --service-objective S0 >> $output_blob
baseDbConnectionString=$(az sql db show-connection-string -c ado.net -s $dbServerName -n $dbName -o tsv)
dbConnectionStringWithUser="${baseDbConnectionString/<username>/$DB_ADMIN_USER}"
sqlConnectionString="${dbConnectionStringWithUser/<password>/$DB_ADMIN_PASSWORD}"

echo "Creating app service hosting plan $hostingPlanName in group $resourceGroupName"
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
az webapp config appsettings set -g $resourceGroupName -n $clientAppName --settings Api__TimeHelperApiBaseAddress=$timehelperApiBaseUrl ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientSecret=$AAD_CLIENTSECRET APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION TimeHelperApiDefaultScope=timehelperApiDefaultScope TimeHelperApiScope=timehelperApiScope >> output_blob
echo "</p>" >> $output_blob

echo "<p>App Service (Api App): $apiAppName</p>" >> $output_blob

az webapp create \
  --name $apiAppName \
  --plan $hostingPlanName \
  --resource-group $resourceGroupName \
  --runtime  "DOTNETCORE|3.1" >> $output_blob
echo "<p>we need to change the --runtime option to node or similar</p>"
az webapp config appsettings set -g $resourceGroupName -n $apiAppName --settings ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID AzureAD__ClientSecret=$AAD_CLIENTSECRET APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION ContactUri=$SWAGGER_CONTACT_URL "ContactName=$SWAGGER_CONTACT_NAME" "ContactEmail=$SWAGGER_CONTACT_EMAIL" TermsUri=$swaggerTermsUri DummyDataBlobContainer=$dummyDataBlobContainer >> output_blob
az webapp config connection-string set -g $resourceGroupName -n $apiAppName -t SQLAzure --settings "TimeHelperDataContext=$sqlConnectionString" >> output_blob
echo "</p>" >> $output_blob

echo '</body></html>' >> $output_blob

# Upload the deployment log to the zodiac storage account 
az storage container create -n "logs" --public-access off
az storage blob upload -c "logs" -f $output_blob -n$output_blob
echo "Uploaded $output_blob to logs container in storage account $storageAccountName"
