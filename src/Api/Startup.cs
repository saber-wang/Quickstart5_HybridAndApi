// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection(options => options.ApplicationDiscriminator = "00000").SetApplicationName("00000");

            services.AddMvc();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies")
               .AddOpenIdConnect("oidc", options =>
               {
                   options.SignInScheme = "Cookies";

                   options.Authority = "http://localhost:54660/IdentityServer";
                   options.RequireHttpsMetadata = false;

                   options.ClientId = "mvc";
                   options.ClientSecret = "secret";
                   options.ResponseType = "code id_token";

                   options.SaveTokens = true;
                   options.GetClaimsFromUserInfoEndpoint = true;

                   options.Scope.Add("api1");
                   options.Scope.Add("offline_access");
               })
                    .AddIdentityServerAuthentication("Bearer", options =>
                     {
                         options.Authority = "http://localhost:54660/IdentityServer";
                         options.RequireHttpsMetadata = false;
                         options.ApiSecret = "secret123";
                         options.ApiName = "api1";
                         options.SupportedTokens= SupportedTokens.Both;
                     });

            services.AddAuthorization(option =>
            {
                //默认 只写 [Authorize]，表示使用oidc进行认证
                option.DefaultPolicy = new AuthorizationPolicyBuilder("oidc").RequireAuthenticatedUser().Build();
                //ApiController使用这个  [Authorize(Policy = "ApiPolicy")]，使用jwt认证方案
                option.AddPolicy("ApiPolicy", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                context.Request.Host = HostString.FromUriComponent(new Uri("http://localhost:54660/"));
                await next.Invoke();
            });
            //var options = new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
            //    ForwardLimit = 1
            //};
            //options.KnownNetworks.Clear();
            //options.KnownProxies.Clear();
            //app.UseForwardedHeaders(options);
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "MyApi/{controller=MAccount}/{action=Login}/{id?}");

            });
        }
    }
}