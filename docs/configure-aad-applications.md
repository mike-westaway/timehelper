# Configure AAD Applications

USe the azure portal or CLI to carry out the following AAD configuration:

## Service Principal for GitHub Actions
The actions in this repo create all the resources necessary to run the TimeHelper application in a new resource group in one of your subscription.  In order to do that we need Azure credentials with contributor rights to the subscription where you will host. Run the following command in Azure CLI and copy the resultant json output to your clipboard

`az ad sp create-for-rbac --name "myApp" --role contributor --scopes /subscriptions/{subscription-id} --sdk-auth`

Then create GitHub secret called AZURE_CREDENTIALS and paste the json content generated above into the value field for the secret. [see here for more details](https://github.com/Azure/login#configure-deployment-credentials)

## Api AAD Application
The Api AAD Application represents the identity of the api applications in the TimeHelper solution.  This application holds delegated permisions to access the [Microsoft Graph Api](https://docs.microsoft.com/en-us/graph/use-the-api).  This means that in order for the Api application to read graph information, such as calendar entries, from the Graph Api, the user needs to consent to the Api application accessing the information on their behalf.

The application also exposes permissions.  The client AAD Application (below) will be authorised to use the Api AAD Application and therefore only authenticated and authorized client applications can use this Api.

For instructions see [Configuring the Api AAD Application](configure_api_app.md).

## Client AAD Application
The Client AAD Application represents the identity of the client applications in the TimeHelper solution.  Configuration in these applications points to this AAD application which is then used to represent the client applications in scenarios for authentication, consent and authorization.

The client application holds valid parameters for authentication.  For example, when the user logs into a client application, the authentication occurs at Azure AD, but this application holds a list of valid 'reply urls' (where the resultant token is returned).  If a url is not in the valid list, authentication will fail.  This prevents unauthorized or spoof applications form obtaining a token.

The client application has permissions to use the Api Application, so clients need to assume the identity of this application before that can access the Api (and consequently the Microsoft Graph to read (for example) calendar information.

For instructions see [Configuring the Client AAD Application](configure_client_app.md).
