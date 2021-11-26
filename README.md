# Kentico Xperience Intercom modules
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico)

|  | Package |
| ------------- |:-------------:|
| Administration | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Intercom.Admin.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Intercom.Admin.KX13) |
| Live site - ASP.NET Core | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Intercom.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Intercom.KX13) |

[Intercom integration](https://www.intercom.com/) for [Kentico Xperience](https://xperience.io/)

This repository contains the source code for modules that integrate Kentico Xperience with the Intercom chat service.

## Description

The Intercom integration modules provide the following features:
* Synchronization of Xperience contact data to contacts in your Intercom organization. Depending on your site's personal data policy, this synchronization can be disabled, or conditionally applied only to contacts who agree with a specific consent.
* Transfer of data submitted through the Intercom chat to corresponding contact attributes in Xperience.
* Logging of custom activities in Xperience when users perform specified actions in the Intercom chat.
* HtmlHelper API that helps developers generate scripts that add the Intercom chat interface on the pages of your live site.

The project consists of two modules:
* **Administration** - Adds the *Intercom* application to the Xperience administration interface. The application is used to set up the integration and allows you to toggle the Intercom functionality on or off.
* **Live site** - Provides an HtmlHelper API to simplify generating of scripts that add the Intercom chat interface to your site's pages. Adds endpoints that recieve requests from Intercom webhooks, which are used to update contacts and log activities.

## Compatibility
* *Kentico Xperience 13.0.0* or newer. 
* Only the **ASP.NET Core** development model for the live site is supported.

## Requirements and prerequisites

* **Xperience Enterprise** license edition for your site's domain, as the integration uses on-line marketing features (contacts and activities).
* Your Xperience administration application must run using the secured **HTTPS** protocol.
* The *Enable on-line marketing* setting needs to be selected in the Xperience *Settings* application.
* You need to have an *Intercom* account, workspace and app created. You can register a free developer account at [https://www.intercom.com/](https://www.intercom.com/). For more information, see the [Intercom Developer Hub Docs](https://developers.intercom.com/building-apps/docs/welcome).
* **Important**: The integration optionally synchronizes contact data from Xperience to your Intercom workspace. As a result, the **personal data** of visitors may be exported to a third party. If necessary, update your website's personal data policy and any related consents.

## Setup (Developers)
The setup of the Intercom integration consists of the following steps:
1. [Install the administration package](#install-the-administration-package)
2. [Install the live site package](#install-the-live-site-package)
3. [Add Intercom webhook endpoints](#add-intercom-webhook-endpoints)
4. [Add Intercom Messenger to your website](#add-intercom-messenger-to-your-website)
5. [Configure Intercom and Xperience](#configure-intercom-and-xperience)

### Install the administration package
1. Open the solution with your Xperience administration project (*~/WebApp.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run `Install-Package Kentico.Xperience.Intercom.Admin.KX13 -Version 1.0.0`
1. Rebuild the CMSApp project.

### Install the live site package
1. Open the solution with your live site project (e.g., *~/DancingGoatCore.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run `Install-Package Kentico.Xperience.Intercom.KX13 -Version 1.0.0`
1. Rebuild your live site project.

### Add Intercom webhook endpoints

To allow Intercom to update the data of contacts and log custom activities in Xperience, your live site application needs to process webhook requests. Add endpoints for these requests to your website:
1. Edit your live site project's startup class (*Startup.cs* by default).
1. Add the following using statement: `using Kentico.Xperience.Intercom;`
1. Within the `ConfigureServices` method, call `services.AddHttpClient();`
1. Register the Intercom endpoints in the `Configure` method:
```
app.UseEndpoints(endpoints =>
{
   endpoints.Kentico().MapRoutes();
   
   // Registers endpoints that process requests from Intercom webhooks 
   endpoints.MapKenticoIntercomRoutes();
   ...
```

Your live site application can now process Intercom webhook requests using the following routes:
* **https://{live_site_url}/kentico.xperience.intercom/updatecontact**
* **https://{live_site_url}/kentico.xperience.intercom/logactivity**

### Add Intercom Messenger to your website

To add Intercom Messenger (chat interface) onto your website's pages, use the `@Html.IntercomScripts()` HtmlHelper provided by the installed live site module. The method generates the required intercom scripts using the configuration provided in the Xperience administration.

Place the scripts before the closing `</body>` tag on every page where you want the Intercom Messenger to appear. For example, a suitable location is your site's main **_Layout.cshtml** view if you want to add Intercom to all or most pages (when using MVC architecture for the site).

```
@using Kentico.Xperience.Intercom

  ...

  @Html.IntercomScripts()
</body>
```

The generated scripts automatically ensure that the names and email address of your contacts in Xperience are synchronized to the corresponding fields of contacts in Intercom. You can configure the synchronization in the Xperience administration according to your site's personal data policy. It can be disabled completely or conditionally enabled only for contacts who agree with a specific consent.

### Configure Intercom and Xperience
Communication between Xperience and your Intercom app requires configuration on both sides.

Open the Xperience administration and navigate to the **Intercom** application. Fill in the settings in the **Setup** section:
* **Enable Intercom** - Toggles the Intercom functionality on or off for your website.
* **Intercom App ID** - The ID of your Intercom app (workspace). See [Where can I find my workspace ID (app ID)?](https://www.intercom.com/help/en/articles/3539-where-can-i-find-my-workspace-id-app-id).
* **Intercom Client ID and Secret** - The Client ID and Client secret values of your Intercom app.
  1. Open the Intercom Developer Hub (access the *Settings* of your Intercom workspace and go to *Developers > Developer Hub*).
  2. Edit your Intercom app.
  3. Navigate to **Configure > Basic information**.
  4. Copy the values to the corresponding settings in Xperience.
<img src="https://user-images.githubusercontent.com/16876168/142841507-339fdb5f-3491-4b9f-9f5f-b170ed50306e.png" height="250">

* **Intercom Identity Verification Secret** - To ensure secure communication, [Enable identity verification](https://www.intercom.com/help/en/articles/183-enable-identity-verification-for-web-and-mobile) in Intercom:
  1. Access the **Settings** of your Intercom workspace.
  2. Go to **Security > Enforce identity on web**.
  3. Enable the **Enforce identity verification** option.
  4. Copy the **Identity verification secret** value to the corresponding setting in Xperience.
* **Send contact attributes to Intercom** - Choose how the integration synchronizes the names and email adress of contacts from Xperience to your Intercom organization (depending on your site's personal data policy). You can leave the synchronization enabled for all contacts (*Always*), disable it completely (*Never*), or enable it only for contacts who agree with a selected consent.

**Save** the settings in the Xperience *Intercom* application.

Next, you need to provide an access token that allows Xperience to obtain data from your Intercom workspace via the API.
1. Open the Intercom Developer Hub (access the *Settings* of your Intercom workspace and go to *Developers > Developer Hub*).
2. Edit your Intercom app.
3. Navigate to **Configure > Authentication**.
4. Click **Edit**.
5. Enable **Use OAuth**.
6. Add the following **Redirect URLs**:

   *{Xperience administration domain}/CMSModules/Kentico.Xperience.Intercom.Admin/Pages/Intercom_AccessTokenDialog.aspx*
   
   The redirect URL must be absolute, including the protocol, domain and virtual directory. If your Xperience administration application is available on multiple domains, add all of the possible options. For example:
   
   `https://adminDomain.com/CMSModules/Kentico.Xperience.Intercom.Admin/Pages/Intercom_AccessTokenDialog.aspx`
7. **Save** the authentication settings.
8. Return to the Xperience *Intercom* application.
9. Click **Get token** next to the **API access** field in the **Intercom data access** section.

<img src="https://user-images.githubusercontent.com/16876168/143205457-9a651aa3-0d1c-4334-9ea1-e74410f09072.png" height="400">

Your marketers can now set up the required contact synchronization and activity logging using the Intercom Series feature and webhooks.

## Intercom Series webhooks (Marketers)

To set up synchronization of contact data from Intercom to Xperience, as well as logging of custom activities in Xperience, you need to use the Intercom Series feature and webhooks.

See [Orchestrate your customer messaging with Series](https://www.intercom.com/help/en/articles/4425207-orchestrate-your-customer-messaging-with-series) for general information about this Intercom feature.

### Update contact data in Xperience
When visitors use the Intercom chat, you may wish to transfer specific pieces of the entered data into your Xperience contacts. To set up when and how the data is transferred, create an Intercom Series according to the following steps:

1. Log in to your Intercom workspace.
2. Navigate to **Outbound > Series**.
3. Create a **New series** or edit an existing one that you use for your Intercom chat.
4. Switch to **Edit** mode.
5. Add blocks to build your chat experience (RULES, BOT, etc.).
![Intercom_Series_Contact_Data](https://user-images.githubusercontent.com/16876168/142988821-58c9db39-09e7-47ac-9cf0-f5a7e8854dd4.png)
6. To transfer data into Xperience contacts, add the **WEBHOOK** block.
7. Set the following when configuring the Webhook block:
	* **HTTP method**: POST
	* **Webhook URL**: {your live site URL}/kentico.xperience.intercom/updatecontact
	* **Webhook header** - add and set the following keys:
		* **Content-Type**: application/json
		* **XperienceAPIKey:** open the *Intercom* application in the Xperience administration and copy the value from **Webhook request security > Your API Key** 
	* **Webhook body** - add keys for all contact attributes that you wish to update in Xperience.
	   * Important: The webhook body must always contain the **ContactGuid** key, with the **User ID** people attribute as the value.
	   * Set the keys according to the following names of Xperience contact fields:
	      * ContactEmail
	      * ContactFirstName
	      * ContactMiddleName
	      * ContactLastName
	      * ContactJobTitle
	      * ContactAddress1
	      * ContactCity
	      * ContactZIP
	      * ContactMobilePhone
	      * ContactBusinessPhone
	      * ContactNotes
	      * ContactCompanyName
	      * Additionally, your developers may implement handling for custom contact fields.
	   * For the key values, select the corresponding people attribute from Intercom from which you want to take the data.				  
![Intercom_Webhook_Contact_Data](https://user-images.githubusercontent.com/16876168/143006883-0d181788-d1fd-48fa-9456-4c265c51ea59.png)
8. Save the webhook.

**Note**: Intercom checks if a user matches a Series block every time they visit your website, and periodically in the background. After you change and save a Series, there may be a delay before the results impact visitors on your website.

When visitors go through the Series in the Intercom chat on your website and submit information, the data of the matching contact in Xperience is now updated correspondingly.

### Log custom activities in Xperience
When visitors use the Intercom chat, you may wish to log activites in Xperience at certain points during the conversation. To set up this logging of activities, create an Intercom Series according to the following steps:

1. Log in to your Intercom workspace.
2. Navigate to **Outbound > Series**.
3. Create a **New series** or edit an existing one that you use for your Intercom chat.
4. Switch to **Edit** mode.
5. Add blocks to build your chat experience (RULES, BOT, etc.).
6. To log Xperience activities, add the **WEBHOOK** block.
7. Set the following when configuring the Webhook block:
   * **HTTP method**: POST
	* **Webhook URL**: {your live site URL}/kentico.xperience.intercom/logactivity
	* **Webhook header** - add and set the following keys:
		* **Content-Type**: application/json
		* **XperienceAPIKey:** open the *Intercom* application in the Xperience administration and copy the value from **Webhook request security > Your API Key** 
	* **Webhook body** - add keys for activity fields that you wish to fill in Xperience.
	   * Important: The webhook body must always contain the following keys:
	      * **ActivityType** key, with the code name of the corresponding Xperience activity as the value. You can find the activity type code names in the Xperience administration in *Contact management > Configuration > Activity types*.
	      * **ContactGuid** key, with the **User ID** people attribute as the value.
	   * Optionally, you can add keys for the **ActivityURL** and **ActivityValue** fields.
	   * For the key values, select the corresponding people attribute from Intercom or enter a value directly.
![Intercom_Webhook_Activity](https://user-images.githubusercontent.com/16876168/143006758-0f817d86-08ff-4434-81af-5704ef9347fc.png)
8. Save the webhook.

**Note**: Intercom checks if a user matches a Series block every time they visit your website, and periodically in the background. After you change and save a Series, there may be a delay before the results impact visitors on your website.

When visitors go through the Series in the Intercom chat on your website, activites are logged in Xperience according to the Series rules. The comment field of the logged activities contains a link to Intercom, where marketers can view the conversation history from the given chat interaction.

## Synchronize data to custom contact fields (Developers)

If your Xperience contacts use [custom fields](https://docs.xperience.io/on-line-marketing-features/configuring-and-customizing-your-on-line-marketing-features/configuring-contacts/adding-custom-fields-to-contacts) and you wish to synchronize Intercom data into these fields, you need to implement additional handling of the data.

1. Open the solution with your **live site project** (e.g., *~/DancingGoatCore.sln*).
1. Prepare a separate assembly (Class Library project).
1. Create a custom module class in the assembly.
1. Override the module's **OnInit** method and assign a handler method to the **IntercomEvents.UpdateContact** global event (available in the *Kentico.Xperience.Intercom* namespace).
1. Load the provided contact data from the event arguments and save the values into the appropriate Xperience contact fields.

For more details, see the example in this repository: [SampleModule.cs](https://github.com/Kentico/xperience-module-intercom/blob/master/src/Kentico.Xperience.Intercom.AspNetCore.SampleModule/SampleModule.cs)

## Get involved

Check out the [contributing](CONTRIBUTING.md) page to see how to file issues, start discussions, and begin contributing.

## Questions & Support

See the [Kentico home repository](https://github.com/Kentico/Home/blob/master/README.md) for more information about the product(s) and general advice on submitting questions.

