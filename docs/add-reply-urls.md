# Add Reply Urls to AAD Client Application

Once your application is deployed you need to make a tweak to the AAD Client Application we defined earlier.

1. [App Registrations](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps)
1. Select your client application (TimeHelperClient?)
1. Click on 'Authentication'
1. Click '+ Add a platfrom'
1. Choose 'Web'
1. Where it says 'Enter the redirect uri of the application' enter the 'Web App Reply Url' from the Job Summary that you noted earlier.
1. Select 'ID Tokens'
1. Click 'Configure'
1. Click '+ Add a platform'
1. Click 'Single-page application'
1. Where it says 'Enter the redirect uri of the application' enter the 'Client App Reply Url' from the Job Summary that you noted earlier.
1. Click 'Configure'

![TimeHelper client authentication configuration](docs/images/timehelperclient-authentication.png)

Congratulations! You should now be ready to run your application!
