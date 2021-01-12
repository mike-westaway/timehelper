![Build Time Helper Infrastructure](https://github.com/nikkh/timehelper/workflows/Build%20Time%20Helper%20Infrastructure/badge.svg) ![TimeHelper Web Site Deployment](https://github.com/nikkh/timehelper/workflows/TimeHelper%20Web%20Site%20Deployment/badge.svg) ![TimeHelper Api Deployment](https://github.com/nikkh/timehelper/workflows/TimeHelper%20Api%20Deployment/badge.svg)

# About TimeHelper

TimeHelper is project that explores the possibility of building a quite sophisticated Time Tracking system based on the users entries in their Microsoft Outlook calendar.  The key components of the project are:

1. Azure Active Directory - which controls user consent and (if consent is given) access to the users calendar data
1. TimeHelper-Api - an api which abstracts the complex authentication and authorization interactions from client applications
1. TimeHelper-Client - a native react.js application that enables users to manage and submit their timesheets
1. TimeHelper-Web - is a utility web site that provides support for testing authenitaction and authorization and simple tests for the api.

Time helper is hosted on Azure, and can be deployed automatically (with a single click) using GitHub actions.

Given the permissions and processes for Azure Active Directory (AAD) configuration many vary significantly acorss organisations, AAD configuration must be completed manually before running the GitHub action that creates the hosting infrastructure and deploys the application components.

## AAD Configuration

You dont need to be a global administrator to complete this configuration, but you will need to be a member of the Application administrator role.

AAD configuration should be carried out very carefully.  If you have problems installing and running TimeHelper, it's most likely because of an error configuring AAD.  There are three elements to the AAD configuration required:

- Configure a service principal to allow GitHub actions to create resources in your Azure Subscription
- Configure an AAD application to represent client applications that will access the api component
- Configure an AAD application to represent the api component

These elements are described in detail in the following sections:

### Service Principal for GitHub Actions
The actions in this repo create all the resources necessary to run the TimeHelper application in a new resource group in one of your subscription.  In order to do that we need Azure credentials with contributor rights to the subscription where you will host. Run the following command in Azure CLI and copy the resultant json output to your clipboard

`az ad sp create-for-rbac --name "myApp" --role contributor --scopes /subscriptions/{subscription-id} --sdk-auth`

Then create GitHub secret called AZURE_CREDENTIALS and paste the json content generated above into the value foeld for the secret. [see here for more details](https://github.com/Azure/login#configure-deployment-credentials)

### Client AAD Application
### Api AAD Application

## Preparing to Deploy
Once you have completed your AAD configuration, then we need to store some parameters relating to tht set-up in your GitHub account.  Sensitive data will be stored in GitHub secrets to ensure they are kept private, and are masked in deployment logs, etc. Please see the [GitHub documentation for more information on managing secrets](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets)

The secrets that need to be configured are described in the following sections:

### AAD_WEB_CLIENT_ID
### AAD_WEB_CLIENT_SECRET
### AAD_API_CLIENT_ID
### AAD_API_CLIENT_SECRET
### AAD_DOMAIN
### AAD_TENANTID
### AZURE_CREDENTIALS
### DB_ADMIN_PASSWORD
### PAT
Does this still get used?
### TIMEHELPER_API_SCOPE


## Deploying
## Testing your Deployment

