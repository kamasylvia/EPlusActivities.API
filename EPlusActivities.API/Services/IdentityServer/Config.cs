// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace EPlusActivities.API.Services.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("eplus.test.scope"),
                new ApiScope("scope1"),
                new ApiScope("client.test.scope")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("eplus.api", "E+ 小程序签到抽奖活动 API")
                {
                    // !!!重要
                    Scopes = { "eplus.test.scope" }
                }
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
                new Client
                {
                    ClientId = "password",
                    ClientName = "Resource Owner Password Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("Pa$$w0rd".Sha256()) },
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        "eplus.test.scope",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    }
                },
                // mvc, hybrid flow
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName = "Hybrid mode",
                    ClientSecrets = { new Secret("hybrid secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RedirectUris =
                    {
                        "http://localhost:8080/signin-oidc" // 登陆后到跳转界面
                    },
                    PostLogoutRedirectUris =
                    {
                        "http://localhost:8080/signout-callback-oidc" // 登出后到跳转界面
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        "eplus.test.scope",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                },
                // interactive client using sms
                new Client
                {
                    ClientId = "sms.client",
                    ClientName = "SMS Credentials Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    AccessTokenLifetime = 60 * 60 * 24,
                    SlidingRefreshTokenLifetime = 2592000,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedGrantTypes = { OidcConstants.AuthenticationMethods.ConfirmationBySms },
                    // AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        "eplus.test.scope",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    }
                }
            };
    }
}
