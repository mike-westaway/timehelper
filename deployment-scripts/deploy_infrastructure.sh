#!/bin/bash -e
output_blob=$OUTPUT_LOG_NAME
echo "<h2>TimeHelper WebSite</h2>" >> $output_blob
echo "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
echo         Deploying Time Helper Web
echo "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
echo ---Global Variables
echo "TIMEHELPER_ALIAS: $TIMEHELPER_ALIAS"
echo "TIMEHELPER_LOCATION: $TIMEHELPER_LOCATION"
echo "DB_ADMIN_USER: $DB_ADMIN_USER"
echo "DB_ADMIN_PASSWORD: $DB_ADMIN_PASSWORD" 
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
webAppName="${applicationName}-web"
hostingPlanName="${applicationName}-plan"
dbServerName="${applicationName}-db-server"
dbName="${applicationName}-db"
resourceGroupName="${applicationName}-rg"
timehelperApiBaseUrl="https://${applicationName}-api.azurewebsites.net/"


echo ---Derived Variables
echo "Application Name: $applicationName"
echo "Resource Group Name: $resourceGroupName"
echo "Web App Name: $webAppName"
echo "Hosting Plan: $hostingPlanName"
echo "DB Server Name: $dbServerName"
echo "DB Name: $dbName"
echo "time helper base url: $timehelperApiBaseUrl"
echo

echo "Creating resource group $resourceGroupName in $TIMEHELPER_LOCATION"
az group create -l "$TIMEHELPER_LOCATION" --n "$resourceGroupName" --tags  HeraclesInstance=$TIMEHELPER_INSTANCE Application=TimeHelper MicrososerviceName=timerhelper-web MicroserviceID=$applicationName PendingDelete=$PENDING_DELETE >> $output_blob
echo "<p>Resource Group: $resourceGroupName</p>" >> $output_blob

echo "Creating Azure Sql Resources in $TIMEHELPER_LOCATION"
echo "<p>Azure Sql Server: $dbServerName</p>" >> $output_blob
az sql server create -n $dbServerName -g $resourceGroupName -l $TIMEHELPER_LOCATION -u $DB_ADMIN_USER -p $DB_ADMIN_PASSWORD >> $output_blob
az sql server firewall-rule create -g $resourceGroupName -s $dbServerName -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 >> $output_blob
echo "<p>Azure Sql Database: $databaseName</p>" >> $output_blob
az sql db create -g $resourceGroupName -s $dbServerName -n $dbName --service-objective S0 >> $output_blob

echo "Creating app service $webAppName in group $resourceGroupName"
echo "<p>App Service (Web App): $webAppName</p>" >> $output_blob
az  appservice plan create -g $resourceGroupName --name $hostingPlanName --location $TIMEHELPER_LOCATION --number-of-workers 1 --sku S1 --is-linux
az webapp create \
  --name $webAppName \
  --plan $hostingPlanName \
  --resource-group $resourceGroupName \
  --runtime  "DOTNETCORE|3.1" >> $output_blob

# application insights info
aIKey=$(az monitor app-insights component show --app $webAppName -g $resourceGroupName --query instrumentationKey -o tsv)
# Attempt to get App Insights configured without the need for the portal
APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$aIKey;"
APPINSIGHTS_INSTRUMENTATIONKEY=$aIKey
ApplicationInsightsAgent_EXTENSION_VERSION='~2'

echo "Updating App Settings for $webAppName"
echo "<p>Web App Settings:" >> $output_blob
az webapp config appsettings set -g $resourceGroupName -n $webAppName --settings Api__TimeHelper-Api-BaseAddress=$timehelperApiBaseUrl ASPNETCORE_ENVIRONMENT=Development AzureAD__Domain=$AAD_DOMAIN AzureAD__TenantId=$AAD_TENANTID AzureAD__ClientId=$AAD_CLIENTID APPLICATIONINSIGHTS_CONNECTION_STRING=$APPLICATIONINSIGHTS_CONNECTION_STRING APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_INSTRUMENTATIONKEY ApplicationInsightsAgent_EXTENSION_VERSION=$ApplicationInsightsAgent_EXTENSION_VERSION >> $output_blob
echo "</p>" >> $output_blob
if [ "$OUTPUT_LOGGING" = TRUE ]; then
 cat $output_blob
fi
