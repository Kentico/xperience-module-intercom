# Kentico Xperience Intercom modules
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico)

|  | Package |
| ------------- |:-------------:|
| Administration | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Intercom.Admin.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Intercom.Admin.KX13/0.0.1-preview) |
| Live site | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Intercom.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Intercom.KX13/0.0.1-preview) |

[Intercom integration](https://www.intercom.com/) for [Kentico Xperience](https://xperience.io/)

This repository contains the source code for modules that integrate Kentico Xperience with Intercom chat service.

## Description

This project consists of two modules:
* Administration module - adds the *Intercom* application to the application menu in the Xperience administration interface.
* Live site module - contains HtmlHelper that simplifies putting Intercom scripts on your site pages.

## Requirements and prerequisites

* *Kentico Xperience 13* installed. Both ASP.NET Core and ASP.NET MVC 5 development models are supported. However, the demo site in this repository is an ASP.NET Core project.
* *Xperience Enterprise* license edition for your site, as the integration uses on-line marketing features (e.g. Contacts).
* The *Enable on-line marketing* setting needs to be selected in the *Settings* application.
* You need to have a Intercom.com account.
   - You can create your own free developer account at Intercom.com.

## Setting up the environment
### Administration package installation
1. Open the solution with your administration project (*~/WebApp.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run *Install-Package Kentico.Xperience.Intercom.Admin.KX13 -Version 0.0.1-preview*
1. Rebuild the CMSApp project.

### Live site package installation
1. Open the solution with your live site project (e.g., *~/DancingGoatCore.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run *Install-Package Kentico.Xperience.Intercom.KX13 -Version 0.0.1-preview*.
1. Rebuild your live site project.

### Configuring your intercom App ID

### Adding Intercom scripts to your live site

### Using webhooks

## Get involved

Check out the [contributing](CONTRIBUTING.md) page to see how to file issues, start discussions, and begin contributing.

## Questions & Support

See the [Kentico home repository](https://github.com/Kentico/Home/blob/master/README.md) for more information about the product(s) and general advice on submitting questions.

