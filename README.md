![Build Time Helper Infrastructure](https://github.com/nikkh/timehelper/workflows/Build%20Time%20Helper%20Infrastructure/badge.svg) ![TimeHelper Web Site Deployment](https://github.com/nikkh/timehelper/workflows/TimeHelper%20Web%20Site%20Deployment/badge.svg) ![TimeHelper Api Deployment](https://github.com/nikkh/timehelper/workflows/TimeHelper%20Api%20Deployment/badge.svg)

# About TimeHelper

TimeHelper is project that explores the possibility of building a quite sophisticated Time Tracking system based on the users entries in their Microsoft Outlook calendar.  The key components of the project are:

1. Azure Active Directory - which controls user consent and (if consent is given) access to the users calendar data
1. TimeHelper-Api - an api which abstracts the complex authentication and authorization interactions from client applications
1. TimeHelper-Client - a native react.js application that enables users to manage and submit their timesheets
1. TimeHelper-Web - is a utility web site that provides support for testing authenitaction and authorization and simple tests for the api.

> Feel free to just read this site, but if you want to deploy, the best thing its to [fork this repo](https://docs.github.com/en/free-pro-team@latest/github/getting-started-with-github/fork-a-repo). Once forked, go to your copy.  Click on actions.  If you are asked click on “I Understand my workflows, go ahead and enable them”. You may need to correct a few things (such as the badges above). 

Time helper is hosted on Azure, and can be deployed automatically (with a single click) using GitHub actions.

Given the permissions and processes for Azure Active Directory (AAD) configuration many vary significantly acorss organisations, AAD configuration must be completed manually before running the GitHub action that creates the hosting infrastructure and deploys the application components.

## AAD Configuration

You dont need to be a global administrator to complete this configuration, but you will need to be a member of the Application administrator role.

AAD configuration should be carried out very carefully.  If you have problems installing and running TimeHelper, it's most likely because of an error configuring AAD.  There are three elements to the AAD configuration required:

- Configure a service principal to allow GitHub actions to create resources in your Azure Subscription
- Configure an AAD application to represent client applications that will access the api component
- Configure an AAD application to represent the api component

These elements are described in detail in the [Configure AAD Applications](docs/configure-aad-applications.md).

## Preparing to Deploy

Once you have completed your AAD configuration, then we need to store some parameters relating to tht set-up in your GitHub account.  Sensitive data will be stored in GitHub secrets to ensure they are kept private, and are masked in deployment logs, etc. Please see the [GitHub documentation for more information on managing secrets](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets).  We noted the data items we need to store in secrets as we did our AAD configuration: you'll need those notes now.

Configure GitHub secrets as described in the [Configure GitHub Secrets](docs/configure-github-secrets.md).


## Deploying
It's now time to deploy your application.  This is the fun part - you'll get to go and get a coffee!.

Click on actions.  You should see something similar to this:

![GitHub actions](/docs/images/github-actions.png)

1. Click on 'Build TimeHelper Infrastructure'
1. Towards the right of the screen choose 'Run Workflow'
1. On the pop-up screen that appears configure your deployment:

![run workflows](/docs/images/run-workflow.png)

Fill out the parameters as follows:

- location can be any valid azure location (e.g. uksouth, ukwest, northeurope)
- instance name is just a tag that makes it easier to find your resources, you can choose anything you like
- pending delete wont do anything for you - set it to false
- The most important value is 'Application alias'.  This is used to prefix Azure resource names (including storage accounts) so choose a prefix which is around 5 characters alphanumeric (something like nick1).

> Click 'Run Workflow'

You can follow the deployment progress by clicking on the workflow name. When the job as completed, (hopefully icons are all green) then click on 'Job Summary'.  

![job summary](/docs/images/job-summary.png)

Make a note of the two reply urls in the log for the job summary and [add the newly generated reply urls to AAD Applications](docs/add-reply-urls.md).

## What was deployed?

If your application deployed sucessfully, then a number of Azure resources have been created.  [Learn more about these resources](docs/resources-deployed.md).

## Testing your application

It's time to test your application.  There are three main elements to test.  

| Component | Description | Url Format | What you should see |
| ----------- | ----------- | ----------- | ----------- |
| Api | The Api that accesses calendar data via the graph api | https://<application_alias>-api.azurewebsites.net | Swagger Api documentation |
| Web | Test Harness to test authentication and view calendar data | https://<application_alias>-web.azurewebsites.net | Bootstrap format web page |
| Client | The client that lets you manage your timesheets based on your calendar | https://<application_alias>-client.azurewebsites.net | Rich Internet Application |

The actual urls to use are recorded in the job summary (see above). If each of the above urls load a page without error then your application is correctly deployed.  The first time you logon you will be asked to provide consent for the application to access your calendar entries on you behalf:

![consent screenshot](/docs/images/consent.png)

