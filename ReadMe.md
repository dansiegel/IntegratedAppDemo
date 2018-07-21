# Mobile App Development Lab

This mobile app development lab will help walk you through several fundamentals for Mobile App Development. While it is easy to get fixated on the fact that you will need to develop a mobile application, no serious application is a standalone product. As you go through this lab you will set up a variety of services on Azure. These services will help you to authenticate your users, store your data, react to events triggered by a user, notify users of changes. After setting up all of the various components and being able to deploy the code from your local machine, this lab will guide you through the next phase of App Development by walking you through setting up a CI builds on VSTS for you Azure Functions and Azure App Service with a CD release pipeline that will deploy these changes automatically. Finally we will setup builds in App Center allowing you to distribute your app internally through App Center, or externally to the App Store, Google Play or Intune. Though not expressly covered, external releases to the App Store also allow you to distribute to external testers through Test Flight, or to Alpha and Beta channels through Google Play.

## About the App

The app itself is very basic. It has just three Views:

- SplashScreen (where we send the user to Authenticate)
- MainPage (where we list the TodoItems in the database)
- TodoItemDetail (where we add/edit our TodoItems)

The app follows an MVVM Design pattern with Prism and Xamarin.Forms, and uses Azure Active Directory B2C for Authentication. The app also makes use of App Center for Analytics, Crash logging, and Push Notifications.

## Technologies used

- Xamarin Forms for Cross Platform Application Development
- Azure Active Directory for user Authentication
- EntityFramework Core for data storage.
  - Seperate DbContext's used for Function App and Api
- Azure Web App for mobile API
- Azure Functions app for handling Push Notifications with App Center
- Azure Service Bus for triggering User device tracking in Azure Function
- App Center for Push Notifications, Crashes, and Analytics
- Prism Library for a solid MVVM design pattern
- Mobile.BuildTools to protect the application secrets in the Mobile App

## Configuration

There is a lot of moving pieces here, so get comfortable. You'll be operating across multiple browser tabs, and be bouncing between a Browser, Text Editor, and the IDE as you get this all setup. This requires that you have an account on Azure and in App Center. App Center is completely free, and you can sign up for a Free Trial or sign up for Dev Essentials for a recurring monthly credit of $25 (USD) for Azure. Azure also has free monthly credits available for developers with an active Visual Studio license as well as for MVP's.

You'll also want to be sure to have a Visual Studio account so you have access to your own instance of VSTS. This is completely free and will give you 4 hours of free build time every month (for up to 5 users that do not have paid Visual Studio subscriptions).

To get started log into the [Azure Portal](https://portal.azure.com).

### Resource Groups

Azure has a concept of Resource Groups. This allows you to group and manage various resources you may have on your Azure Subscription. They also work fantastic for deleting all of the resources you've created at the end of a lab or demo. When you create the Service Bus in the next step you'll need to add it to a Resource Group. Be sure to create a new one, and reuse the same resource group as you continue through the lab. You can place it in any region you want, I generally suggest picking one near you. Note that some resources may be able to be placed into a different Region than the Resource Group, when possible change the region to match. It's not critical, and you can certainly distribute your various services across regions.

### Service Bus

From the Azure Portal, click on Create Resource and search for Service Bus. It will show up that it's for `Internet of Things`. Create a new Service Bus, adding the namespace. Once you've created the Service Bus you will want to add a new Queue named `knownuser`.

### Azure Active Directory B2C

Create another new Resource and search for Azure Active Directory B2C. Note that this a multi-step process as you will need to create the new tenant, wait for it to be created and then link it to your subscription. Once you have linked the tenant to your subscription you can navigate to the B2C configuration page.

#### Applications

We will be setting up 2 applications, one for our API, and one for our Mobile app. Be sure to make note of both of the Application Id's. You can read more about how to set this up in this [blog post](https://dansiegel.net/post/2018/07/18/azure-active-directory-b2c-for-xamarin-applications).

#### Policies

For this demo app we will only be using a Sign Up Sign In policy. This will will allow the mobile app to make a single call with the MSAL library and allow users to authenticate either by Signing up for the first time, or Signing back in with an existing account.

After navigating to the `Sign-up or sign-in policies` pane, click on `Add`. Give it the name `susi`, this will generate a full policy name `b2c_1_susi`. Under the Identity Providers select Email Signup. Under the `Sign Up Attributes` select the Email adddress, Given Name, and Surname. You can select others if you wish but the sample doesn't actually make use of them. In the Application Claims you will again want to select the Email, Given Name, and Surname, as well as the User's Object Id. Click Create to create the new policy.

#### Identity Providers

While not explicitly utilitized by this example, you can also set up any number of Identity Providers including [Microsoft Accounts](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-custom-setup-msa-idp), [Google](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-custom-setup-goog-idp), [Twitter](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-custom-setup-twitter-idp), etc. If you have set them up you can add them in the Identity Providers section of the SUSI policy created in the previous step.

### Azure SQL Database

Click the Create Resource and search for SQL database. You can give it any name you want. If you have an existing SQL Server instance you can use that one, or create a new one. Be sure to leave the Source set to `Blank Database`. By default it will create the database using the `Standard S0` pricing tier, this is far more than we need for our POC app. You can change this to a `Basic` instance.

Be sure to make note of the User Name and Password as the Azure Portal will not expose this as part of the connection string. After you have created the database copy the connection string for ADO.NET and replace the placeholders with your User Name and Password. You will need this connection string later. Keep in mind that you can create a second database if you want to isolate the table with the User Id and Device Id's tracked by the Functions App from the TodoItems tracked by the API.

### App Service

From Visual Studio publish the DemoMobileApi project. This publish will allow you to automatically provision a new App Service or select from an existing one.

You will need to add a connection string in the App Service named `DefaultConnection` with the connection string from the SQL Database.

Next update the App Service Applicaton Settings to include the following:

- AzureAdB2C:ClientId - with the Client Id from the B2C application for the Web Api
- AzureAdB2C:Domain - with the tenant name `{tenantName}.onmicrosoft.com`
- ServiceBus:ConnectionString - with the connection string from the Service Bus we setup in the first step

### Azure Functions App

From Visual Studio 2017 publish the AppCenterPushNotificationRelay project. The publish will allow you to automatically provision a new Function App and Storage account for the Functions App. Once published you will need to add several configuration settings:

- iOSAppName: from App Center
- AndroidAppName: from App Center
- OwnerName: from App Center
- AppCenterToken: from the API Token step in the App Center configuration

Next add a Connection String named `DefaultConnection` with the connection string from your Azure SqlServer. You can either use the same database or a seperate database for the Functions App and the Api in the App Service.

### App Center

This sample includes both an iOS and Android app. It's important to remember that while we will be using App Center to handle sending Push Notifications, App Center is merly an abstraction layer that makes sending Push Notifications easier. App Center uses the native API's under the hood to actually send Push Notifications to both Android and iOS. This means that you will need to do some setup in Firebase for Android, and in the Apple developer portal for the iOS app. For this step it will help to have several browser tabs open including App Center.

#### App Info

There is some basic app info you will need to share for each app with the Functions App. This includes the Owner and App Names. This assumes that the Owner is the same for both apps in App Center. After you have created the app, look at the URL in the browser, you will see something like the following:

`https://appcenter.ms/users/{ownerName}/apps/{appName}`

Make note of the both the Owner Name and the App Name for each platform. You will append the Xamarin.Forms.Device.RuntimePlatform (iOS/Android) to the setting name with the value of what the App Name is in the URL.

#### API Access

The Azure Functions App requires API access to App Center. In order to give it the proper access you will need to click on your username in the lower left hand corner of the App Center portal, and select `Account Settings`. Navigate to the Api Keys section and generate a new Api Token.

#### Android

Add a new Xamarin.Android appplication in App Center. Be sure to note the App Secret for your new App Center app. You will need this later for the Xamarin Application. You can follow the full docs from App Center for configuring the Notifications [here](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-android).

You will need to go to the [Firebase Console](https://console.firebase.google.com/) and create a new application. After you have created the application, you can click on the settings icon next to Project Overview and select Project Settings. Finally click the Cloud Messaging tab. You will need to make note of the Application Id for the Xamarin Android application later. Copy the Server Key and paste it into the Final configuration page of the Push Notification in App Center. With that your Android Application is ready to recieve Push Notifications from App Center. 

#### iOS

Add a new Xamarin.iOS application in App Center. Be sure to note the App Secret for you nre App Center app. you will need this later for the Xamarin Applicaiton. You can follow the full docs from App Center for configuring the Notifications [here](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-ios).

Keek in mind that after you have created the key in the App Developer portal and downloaded the key, you will need to copy the middle line that actually contains the key. App Center will not be able to parse the key if you include the `BEGIN PRIVATE KEY` and `END PRIVATE KEY`.

### Xamarin Application

The vast majority of the code changes will be completely in the cross platform code. Because we are using the Mobile.BuildTools, most of our application setup will be accomplished by adding a json file named `secrets.json` to our core mobile app project as follows (be sure to replace the placeholders with the values from your setup):

```json
{
  "Tenant": "{tenantName}.onmicrosoft.com",
  "ClientId": "{B2C Mobile App Client Id}",
  "AppCenter_Android_Secret": "{App Center Android Secret}",
  "AppCenter_iOS_Secret": "{App Center iOS Secret}",
  "AppServiceEndpoint": "{the url of your App Service}"
}
```

NOTE: Because we are using the Mobile.BuildTools the Info.plist and AndroidManifest.xml are excluded from Source Control. This ensures that the Azure Active Directory B2C Client Id is not exposed in source control, and allows this to be defined as part of a CI build. This means that you can use separate tenants for development and production without adding any crazy build scripts to your build definition. Be sure to copy and rename the templates from the build directory. The Info.plist will go in the root directory of the iOS project and the AndroidManifest.xml will go in the Properties folder of the Android project.

Both the iOS and Android app have an ID of `com.company.integratedtodoclient`, be sure to change this to something unique that you can setup for your development. 

#### Android Setup

In order for Push notifications to be setup in the Android app you will need to get the SenderId from your Firebase application.

```cs
Push.SetSenderId("{Your Sender ID}");
```

Because we are using the Mobile.BuildTools to protect this from being exposed in Source Control, you simply need to add a json file to your Android application named `secrets.json` and add the folowing (with your Sender Id):

```json
{
  "SenderId": "{Your Sender ID}"
}
```

Open the AndroidManifest.xml that you copied. You will see a token `$$AADClientId$$`, be sure to replace this with the Client Id for the Mobile Application in your B2C tenant.

#### iOS Setup

In the App Center setup we had to do some configuration to add the Application and setup a Provisioning Profile in the Apple Developer Portal. You should have the provisioning profile and certificate on your Mac.

Open the Info.plist that you copied. You will see a token `$$AADClientId$$`, be sure to replace this with the Client Id for the Mobile Application in your B2C tenant.

## Next Steps

We live in a world where setting up a CI/CD pipeline is essential. By this time I will assume that you have already forked this repository.

### VSTS

For our Azure Functions and API we will want to setup a Build and Release. We could set up a separate Build for each project, and a separte Release or we can setup a single definition that handles both. Since both of these are in a single Repository the following guidelines will be to build and release both as part of a single definition.

If you do not currently have a VSTS account, you can create VSTS account for free. Be sure to sign in and create a Project. If you have the new Navigation enabled you can disable everything besides the Build and Release, otherwise just ignore everything else (unless you're also using it for Source Control). For the purposes of this guide, I'll assume you have the code on GitHub.

#### VSTS Build

Be sure to enable YAML Build Configurations for your account. If you have not done this previously you will need to click your account icon in the upper right hand corner, and Select Preview Features. Click the drop down and select `For this organization`. Turn on `Build YAML Definitions.

Navigate to the Builds area and click on New. Select GitHub as the Source and find the repo containing this code. It should ask you to Authorize using OAuth. This will give VSTS access to your GitHub account. You can alternatively authenticate with a Token from GitHub.

Be sure to select `Configuration as code` -> `YAML`. Make sure the Agent Queue is set to Hosted VS2017 and set the YAML path to `vsts.yaml`. The YAML file will automatically set the Build to trigger on any update to the master branch, and includes all of the build variables needed to set the Build Configuration and Platform. It will also publish the artifacts for both projects to an Artifact named WebDeploy.

#### VSTS Release

After building your build configuration, click Create Release in the build results. This will automatically create a new Release Definition and add the artifacts from our build.

### App Center CI/CD

The Mobile.BuildTools makes building our apps much easier while protecting our application configuration and secrets from source control. To get started go to the build section of your App in App Center and link it to GitHub (or VSTS if you pushed the code there), and find the repository. In the earlier steps we added a `secrets.json` to our core project, and one to our Android project. In the following configuration we will use the values we have locally to create several build Environment Variables. We will also create some Environment Variables for the Manifest Tokens in our Templates in the build folder.

NOTE: When Distributing your app you should always ensure that each build has a unique build version. This ensures that you can distribute each new build to the App Store, and that when using App Center Distribution your users will be notified that a new version is available. App Center does provide an option to automatically version your app. Because we are using the Mobile.BuildTools we do not need to enable this. In fact enabling the option will break your build as at the time that the App Center task executes the AndroidManifest.xml and Info.plist do not yet exist in your source. The Mobile.BuildTools can automatically take care of this versioning, and the option is already enabled for you in the sample code.

#### Android Build Configuration

So far you probably haven't signed your builds. In App Center we cannot distribute the application unless we sign the app. You can create a self signed cert for your Android App from either Visual Studio or Visual Studio for Mac. Note that this will ask you for a single password, while App Center will ask for two different passwords. You will simply provide the same password twice. Because we created a second `secrets.json` in the Android application we will need to specify that the SenderId is an Android Secret (DroidSecret_) so that the second json file will be created in our Android Project.

```
Secret_Tenant: {tenantName}.onmicrosoft.com
Secret_ClientId: {B2C Mobile App Client Id}
Secret_AppCenter_Android_Secret: {App Center Android Secret}
Secret_AppCenter_iOS_Secret: {can be empty as it isn't used for this build}
Secret_AppServiceEndpoint: {the url of your App Service}
DroidSecret_SenderId: {Your Firebase Sender ID}
Manifest_AADClientId: {B2C Mobile App Client Id}
```

#### iOS Build Configuration

In order to distribute apps to devices we must also sign our iOS app. This is a little more involved than for Android as we have to get the Certificate from the Apple Developer portal. Since we already did this configuration we should have this available on our Mac. You will need to upload the Provisioning Profile which is fairly easy. Next we'll need to find the signing certificate we want to use and export it to a p12 certificate. When we do this it will require that we password protect the certificate. After you update the certificate to App Center you will need to specify the password. Next we will add Environment Variables. This will be largely the same as the Android configuration except we don't need to specify an iOS specific secret.

```
Secret_Tenant: {tenantName}.onmicrosoft.com
Secret_ClientId: {B2C Mobile App Client Id}
Secret_AppCenter_Android_Secret: {can be empty as it isn't used for this build}
Secret_AppCenter_iOS_Secret: {App Center iOS Secret}
Secret_AppServiceEndpoint: {the url of your App Service}
Manifest_AADClientId: {B2C Mobile App Client Id}
```

## Using the PushRelay Function

The Web API sends a Queue Message via the Azure Service bus, when the UserProfiles/Me endpoint is called. This registers the User's current device with the Functions database. This ensures that if the same user logs in from multiple devices that all of the users devices will recieve a Push notification when an item is updated.

The Function App also contains functions that allow for sending Notifications based on Audience, or to all applications regardless of whether or not the Functions App is currently tracking the user. The HttpTrigger in the Notification Relay `PushRelay` works very similar to the App Center API, except it allows you to send the same notification to multiple platforms at the same time.

The Push Relay is built on the same models that the App Center API uses. This means that you can send a single NotificationContent that can be sent to App Center. The similarity ends there. Rather than defining a single NotificationTarget, the NotificationRequest in the Models uses a Dictionary to define 1-* platforms with a corresponding NotificationTarget.

### Understanding the NotificationTarget

The `NotificationTarget` is what allows us to define whether we want to send a request to All Devices, selected Device Ids, or a selcted audience such as all users in a Specific Country. The App Center API does not require that a `NotificationTarget` be included in the request (i.e. it can be null). A `NotificationTarget` is ONLY required to send a Push Notification to specific devices or specific audiences. When using a `NotificationTarget` you can specify a single DeviceId, mulitple DeviceId's, a single Audience, or multiple Audiences. This is due to the fact that it always expects an enumberable of either Audiences or DeviceId's. You cannot however specify both Audiences and Devices in a single target. This is due to the fact that the Target Type must be either for Devices or Audiences.

NOTE: The Device ID is an App Center thing. It is generated by the App Center SDK. It is not affected by the user updating the app, however it will change if the user uninstalls the app, and then reinstalls the app later. This sample also has no pruning, so for production use I would suggest tracking the latest Date the DeviceId was published and add a Function that executes on some timer and removes Device Id's we haven't seen in the past {however much time}.