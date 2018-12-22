// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace QuickstartIdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection(options => options.ApplicationDiscriminator = "00000").SetApplicationName("00000");

            services.AddMvc();

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(Option =>
            {
                Option.PublicOrigin = "http://localhost:54660/IdentityServer/";
                Option.IssuerUri = "http://localhost:54660/IdentityServer/";
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            //CspOptions = new CspOptions
            //{
            //    Enabled = false,
            //}
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "Cookies";
            //    options.DefaultChallengeScheme = "oidc";
            //}).AddCookie("Cookies")
            //.AddOpenIdConnect("oidc", options =>
            //{
            //    options.SignInScheme = "Cookies";

            //    options.Authority = "http://localhost:54660/IdentityServer";
            //    options.RequireHttpsMetadata = false;

            //    options.ClientId = "mvc";
            //    options.ClientSecret = "secret";
            //    options.ResponseType = "code id_token";

            //    options.SaveTokens = true;
            //    options.GetClaimsFromUserInfoEndpoint = true;

            //    options.Scope.Add("api1");
            //    options.Scope.Add("offline_access");
            //});
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "ZfsoftCookies";
            //    options.DefaultChallengeScheme = "oidc";
            //    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //    .AddCookie("ZfsoftCookies", options =>
            //    {
            //        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            //        options.SlidingExpiration = true;
            //    })
            //    .AddOpenIdConnect("oidc", options =>
            //    {
            //        options.SignInScheme = "ZfsoftCookies";

            //        options.Authority = "http://localhost:50876/";
            //        options.RequireHttpsMetadata = false;

            //        options.ClientId = "mvc";
            //        options.ClientSecret = "secret";
            //        options.ResponseType = "code id_token";

            //        options.SaveTokens = true;
            //        options.GetClaimsFromUserInfoEndpoint = true;

            //        options.Scope.Add("api1");
            //        //options.Scope.Add("role");
            //        options.Scope.Add("offline_access");
            //    });
            //services.AddAuthentication()
            //.AddGoogle("Google", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
            //    options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
            //})
            //.AddOpenIdConnect("oidc", "OpenID Connect", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            //    options.Authority = "https://demo.identityserver.io/";
            //    options.ClientId = "implicit";

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseAuthentication();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}