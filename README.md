# Umbraco.Community.AzureSSO
Add Azure AD SSO to Umbraco v10-11 sites. This will allow you to automatically create Umbraco user accounts for users in your AD. This will then associate the Umbraco users with groups based on their AD group, and the configuration below.

First you, or an Azure AD administration will need to create an App Registration in the Azure Portal which will be used to authenticate the site against Azure AD. Follow [these instructions to setup the new App Registration](AzureADSetup.md)

To install
`dotnet add package Umbraco.Community.AzureSSO`

In startup.cs under ConfigureServices add:

`.AddMicrosoftAccountAuthentication()`

In the services.AddUmbraco bindings

For example:
```
var builder = services.AddUmbraco(_env, _config)
  .AddBackOffice()
	.AddWebsite()
	.AddComposers()
	.AddMicrosoftAccountAuthentication()
	.AddAzureBlobMediaFileSystem();
```

To configure add the following section to the root of your appsettings.json file and customise as appropriate
```
"AzureSSO": {
    "Credentials": {
        "Instance": "https://login.microsoftonline.com/",
        "Domain": "<domain>",
        "TenantId": "<tenantId>",
        "ClientId": "<clientId>",
        "CallbackPath": "/umbraco-microsoft-signin/",
        "SignedOutCallbackPath ": "/umbraco-microsoft-signout/",
        "ClientSecret": "<clientSecret>"
    },
    "DisplayName": "Azure AD",
    "DenyLocalLogin": true,
    "GroupBindings": {
        "<AD group>": "<umbraco group>",
        "<another AD group>": "<umbraco group>"
    }
},
```

You'll need to configure these settings based on the values in Azure:

| Setting          | Description                                                           |
| ---------------- | -----------------------------------------------------                 |
| Domain           | The value in Primary domain in the Azure Active Directory Overview    |
| TenantId         | The value in Directory (tenant) ID on the app registration Overview   |
| ClientId         | The value in Application (Client) ID on the app registration Overview |
| ClientSecret     | The client secret created for the app registration                    |

You can also customise the configuration by setting these settings:

| Setting          | Description                                           |
| ---------------- | ----------------------------------------------------- |
| DisplayName      | The display name for use on the login button          |
| DenyLocalLogin   | Allow users to login via Umbraco's standard login     |
| GroupBindings    | The bindings for AD group to Umbraco group            |

## Group Bindings

To bind these you'll need to specify the active directory group and then the matching Umbraco group.

For example we use: `"GIBE\Producers" : "editors"` to bind everyone in the `GIBE\Producers` group to the Umbraco editors group. 

Beware these will be reset on each login, so changing groups in umbraco will only take effect until the user next logs in. If a user is removed from an AD group they'll automatically be removed from the matching Umbraco group on next login.



