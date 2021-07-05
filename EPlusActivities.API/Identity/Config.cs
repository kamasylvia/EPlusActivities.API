// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace EPlusActivities.API.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResource(
                            name: "test.profile.name",
                            displayName: "Test profile name",
                            userClaims: new [] {"role", "policy"}
                        )
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("eplus.test.scope"),
                new ApiScope("scope1"),
                new ApiScope("client.test.scope"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("eplus.api","E+ 小程序签到抽奖活动 API")
                {
                    // !!!重要
                    Scopes = { "eplus.test.scope"}
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "eplus.test.scope" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "client.test" }
                },

                // interactive client using sms
                new Client
                {
                    ClientId="sms.client",
                    ClientName="SMS Credentials Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    AccessTokenLifetime = 60*60*24,
                    SlidingRefreshTokenLifetime =  2592000,
                    AllowedGrantTypes = {OidcConstants.AuthenticationMethods.ConfirmationBySms},
                    // AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        "eplus.test.scope",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                    },

                }
            };
    }
}
