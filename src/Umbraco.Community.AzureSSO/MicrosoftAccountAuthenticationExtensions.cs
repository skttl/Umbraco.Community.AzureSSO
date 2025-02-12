using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Umbraco.Community.AzureSSO;
using Umbraco.Community.AzureSSO.Settings;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.DependencyInjection
{
	public static class MicrosoftAccountAuthenticationExtensions
	{
		public static IUmbracoBuilder AddMicrosoftAccountAuthentication(this IUmbracoBuilder builder)
		{
			var azureSsoConfiguration = new AzureSSOConfiguration();
			builder.Config.Bind("AzureSSO", azureSsoConfiguration);

			builder.Services.AddSingleton<AzureSsoSettings>(conf => new AzureSsoSettings(azureSsoConfiguration));
			builder.Services.ConfigureOptions<MicrosoftAccountBackOfficeExternalLoginProviderOptions>();
			
			var initialScopes = new string[] { };
			builder.AddBackOfficeExternalLogins(logins =>
			{
				logins.AddBackOfficeLogin(
					backOfficeAuthenticationBuilder =>
					{
						backOfficeAuthenticationBuilder.AddMicrosoftIdentityWebApp(options =>
								{
									builder.Config.Bind("AzureSSO:Credentials", options);
									options.SignInScheme = backOfficeAuthenticationBuilder.SchemeForBackOffice(MicrosoftAccountBackOfficeExternalLoginProviderOptions.SchemeName);
									options.Events = new OpenIdConnectEvents();
								},
								options => { builder.Config.Bind("AzureSSO:Credentials", options); },
								displayName: azureSsoConfiguration.DisplayName ?? "Azure Active Directory",
								openIdConnectScheme: backOfficeAuthenticationBuilder.SchemeForBackOffice(MicrosoftAccountBackOfficeExternalLoginProviderOptions.SchemeName) ?? String.Empty)
							.EnableTokenAcquisitionToCallDownstreamApi(options => builder.Config.Bind("AzureSSO:Credentials", options), initialScopes)
							.AddInMemoryTokenCaches();


					});
			});
			return builder;
		}
		
	}
}
