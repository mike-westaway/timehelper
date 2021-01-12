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

Then create GitHub secret called AZURE_CREDENTIALS and paste the json content generated above into the value field for the secret. [see here for more details](https://github.com/Azure/login#configure-deployment-credentials)

### Api AAD Application
The Api AAD Application represents the identity of the api applications in the TimeHelper solution.  This application holds delegated permisions to access the [Microsoft Graph Api](https://docs.microsoft.com/en-us/graph/use-the-api).  This means that in order for the Api application to read graph information, such as calendar entries, from the Graph Api, the user needs to consent to the Api application accessing the information on their behalf.

The application also exposes permissions.  The client AAD Application (below) will be authorised to use the Api AAD Application and therefore only authenticated and authorized client applications can use this Api.

For instructions see [Configuring the Api AAD Application](docs/configure_api_app.md).

### Client AAD Application
The Client AAD Application represents the identity of the client applications in the TimeHelper solution.  Configuration in these applications points to this AAD application which is then used to represent the client applications in scenarios for authentication, consent and authorization.

The client application holds valid parameters for authentication.  For example, when the user logs into a client application, the authentication occurs at Azure AD, but this application holds a list of valid 'reply urls' (where the resultant token is returned).  If a url is not in the valid list, authentication will fail.  This prevents unauthorized or spoof applications form obtaining a token.

The client application has permissions to use the Api Application, so clients need to assume the identity of this application before that can access the Api (and consequently the Microsoft Graph to read (for example) calendar information.

For instructions see [Configuring the Client AAD Application](docs/configure_client_app.md).

## Preparing to Deploy

Once you have completed your AAD configuration, then we need to store some parameters relating to tht set-up in your GitHub account.  Sensitive data will be stored in GitHub secrets to ensure they are kept private, and are masked in deployment logs, etc. Please see the [GitHub documentation for more information on managing secrets](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets).

The secrets that need to be configured are described in the following sections:

### AAD_WEB_CLIENT_ID
This is the client (application id) of the Client AAD Application configured above.  This id will be propogated by the GitHub actions into the configuration for the TimeHelper-Web and TimeHelper-Client Azure App services.

### AAD_WEB_CLIENT_SECRET
This is the client secret the Client AAD Application configured above.  This id will be propogated by the GitHub actions into the configuration for the TimeHelper-Web and is the 'password' that allows the Web Application to assume the identity of the Client AAD application (without a secret any application could spoof this and access the Api).

### AAD_API_CLIENT_ID
This is the client (application id) of the Api AAD Application configured above.  This id will be propogated by the GitHub actions into the configuration for the TimeHelper-Api  Azure App service.

### AAD_API_CLIENT_SECRET
This is the client secret the Api AAD Application configured above.  This id will be propogated by the GitHub actions into the configuration for the TimeHelper-Api and is the 'password' that allows the Api Application to assume the identity of the Api AAD application (without a secret any application could spoof this and access the Graph Api, providing it had valid user token and that user had consented to Api application accessing graph data on their behalf).

### AAD_DOMAIN
This is the domain of the AAD tenant that you will be using. it is likely to be your company domain or something like <yourdomain>.onmicrosoft.com. You can specify any valid domain associated with your tenant.  Valid domains can be found by [clicking on the Custom domain names](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Domains) blade in your AAD definition in the Azure portal.

### AAD_TENANTID
This is the tenantid of the AAD tenant that you will be using. This can be found by [on the overview page](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Domains) for your AAD definition in the Azure portal.

### AZURE_CREDENTIALS
This is the full json output generated from the [service principal for github actions](https://github.com/nikkh/timehelper/blob/main/README.md#service-principal-for-github-actions) section above.

### DB_ADMIN_PASSWORD
The deployment created an Azure SQL Database and populates it with sample data.  This secret is used to set the admin password for your SQL DB.  The DB_ADMIN_USER is defined in the [Infrastructure Deployment Workflow file](https://github.com/nikkh/timehelper/blob/main/.github/workflows/infrastructure.yml).  The default value for that is dbadminuser, but can be overriden in the workflow if you wish.

### PAT
This is a Personal Access Token to allow the GitHub actions to access your repo.  This is needed to enable the main infrastructure deployment workflow to kick off deployments of the applications once it has completed normally.  Follow the instructions for [creating a GitHub PAT](https://docs.github.com/en/free-pro-team@latest/github/authenticating-to-github/creating-a-personal-access-token#:~:text=Creating%20a%20token.%201%20Verify%20your%20email%20address%2C,able%20to%20see%20the%20token%20again.%20More%20items) and then store the value of the generated PAT as a secret.

### TIMEHELPER_API_SCOPE


## Deploying
## Testing your Deployment

