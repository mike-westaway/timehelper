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
## Preparing to Deploy
## Deploying
## Testing your Deployment

