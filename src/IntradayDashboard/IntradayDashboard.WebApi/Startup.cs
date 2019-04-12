using System;
using System.Net;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Redis;
using IntradayDashboard.Core.Repositories.Database;
using IntradayDashboard.Infrastructure;
using IntradayDashboard.Infrastructure.Repositories.Database;
using IntradayDashboard.WebApi.Helpers;
using IntradayDashboard.WebApi.Hubs;
using IntradayDashboard.WebApi.Services;
using IntradayDashboard.WebApi.Services.Interfaces;
using IntradayDashboard.Core.Model.Entities;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using IntradayDashboard.Infrastructure.Services;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Infrastructure.Data.Uow;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using IntradayDashboard.WebApi.Model.MQModels.Offer;
using Microsoft.AspNetCore.Hosting;
using IntradayDashboard.WebApi.Model.MQModels.Consumption;
using GreenPipes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using IntradayDashboard.Infrastructure.Seed;
using IntradayDashboard.Infrastructure.Repositories.CacheManager;
using Hangfire.Logging;
using Hangfire.Logging.LogProviders;
using LogLevel = Hangfire.Logging.LogLevel;
using Hangfire.Console;
using Hangfire.Heartbeat.Server;
using Hangfire.Heartbeat;

namespace IntradayDashboard.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

        }
        public IConfiguration Configuration { get; }
        public Autofac.IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region Core default Configs
                services.AddMvc(
                        options => {
                            options.EnableEndpointRouting = false; // Test
                        }
                    )
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            #endregion


            #region Identity Config - Sessions
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 4;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                });
                
                builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
                builder.AddEntityFrameworkStores<IntradayDbContext>();
                builder.AddRoleValidator<RoleValidator<Role>>();
                builder.AddRoleManager<RoleManager<Role>>();
                builder.AddSignInManager<SignInManager<User>>();

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    // SignalR Authentication part.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                ((path.StartsWithSegments("/api/auth/login"))))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                    options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                    options.AddPolicy("VipOnly", policy => policy.RequireRole("VIP"));
                });

                services.AddSession(o =>   
                {
                    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    o.Cookie.Name = "Engie.ApiPortal.Session";
                    o.Cookie.HttpOnly = true;
                });

            #endregion
            

            #region Database - Repo - AutoMapper - IOC Configurations

                services.AddDbContext<IntradayDbContext>(x => {
                x.UseSqlServer(Configuration.GetConnectionString("MSSQLConnection"));
                });

                services.Configure<DbSettings>(Configuration);
                Mapper.Reset();
                services.AddAutoMapper();
                services.AddAutofac();
                services.AddScoped<IUrlHelper>(factory =>
                {
                    var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                    return new UrlHelper(actionContext);
                });

            #region Deleted Configs
            // services.AddDbContext<IntradayDbContext>(

            //     x => {
            //             x.UseSqlServer(Configuration.GetConnectionString("MSSQLConnection"),
            //             sqlServerOptionsAction : sqlOptions => {
            //                 sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                 sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //         }
            //     );
            //             x.ConfigureWarnings( 
            //             warnings => {
            //                 warnings.Ignore(CoreEventId.IncludeIgnoredWarning);
            //                 warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
            //             }
            //         );
            //     }

            // );



            #endregion

            #endregion

            #region Cors Configs


                services.AddCors(o => o.AddPolicy("CorsPolicy", buildercam =>
                {
                    buildercam
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //.AllowCredentials()
                        .AllowAnyOrigin();
                }));


            #endregion

            #region Swagger Configs

            services.AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Title = "Engie Api Test Portal",
                        Version = "v1",
                        Description = "This tool provides testing environment for Engie Api Portal",
                        TermsOfService = "Tersm Of Service"
                    });
                });


            #endregion



            #region Hangfire, Hangfire-redisdb - SignalR Configs
                
            services.AddSignalR();
            LogProvider.SetCurrentLogProvider(new ColouredConsoleLogProvider(LogLevel.Info));

            services.AddHangfire(configuration =>
                {
                    var redisServerConStr = Configuration.GetSection("Hangfire.Redis:ConnectionString").Value;
                    var hangfireRedisDbConStr = Configuration.GetSection("Hangfire.Redis:DatabaseId").Value;
                    var redisServer = ConnectionMultiplexer.Connect(redisServerConStr);
                    var hangFireRedisDb = (int)Convert.ToInt32(hangfireRedisDbConStr);

                    configuration.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1));
                    configuration.UseConsole();
                    configuration.UseColouredConsoleLogProvider();
                    configuration.UseRedisStorage(redisServer, new RedisStorageOptions()
                    {
                        Db = hangFireRedisDb,
                        InvisibilityTimeout = TimeSpan.FromHours(1)
                        
                    });
                    
                });


            #endregion







            #region Services Declerations 
                services.AddTransient<Seed>();
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
                services.AddScoped<DbContext, IntradayDbContext>();
                services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Can not impelement abstract repo. Cause reflection doesn't work
                services.AddTransient<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IOfferService, OfferService>();
                services.AddScoped<IConsumptionService, ConsumptionService>();
                services.AddScoped<IOfferApiService, OfferApiService>();
                services.AddScoped<ITenantService, TenantService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IAuthHelper, AuthHelper>();
                services.AddScoped<ICacheManager, CacheManager>();

                //services.AddTransient(typeof(IMQService<>), typeof(MQService<>)); // Deleted. 
                services.AddTransient(typeof(IBackGroundJobsService<>), typeof(BackGroundJobsHelper<>));  // **Hangfire- MassTransit Test
            #endregion
            
            #region MassTransit-RabbitMq MQ System in usage
                services.AddScoped<IOfferMQService, OfferMQService>();
                services.AddScoped<IConsumptionMQService, ConsumptionMQService>();
                services.AddScoped<OfferConsumer>();
                services.AddScoped<ConsumptionConsumer>();
                services.AddMassTransit(x =>
                {
                    x.AddConsumers(typeof(OfferConsumer), typeof(ConsumptionConsumer));

                    #region MassTransit

                    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host("localhost", "/", h => { });

                        cfg.UseExtensionsLogging(provider.GetService<ILoggerFactory>());

                        cfg.ReceiveEndpoint(host, "IntradayDashboard-MQ-EndPoint", e =>
                        {
                            e.PrefetchCount = 16;
                            e.UseMessageRetry(r => r.Interval(2,100));

                            e.Consumer<OfferConsumer>(provider); // ** Defining consumers.
                            e.Consumer<ConsumptionConsumer>(provider);

                            EndpointConvention.Map<OfferMessage>(e.InputAddress);  // ** Defining EndPoints
                            EndpointConvention.Map<ConsumptionMessage>(e.InputAddress);
                        });
                    }));

                    #endregion

                    

                    x.AddRequestClient<OfferMessage>();
                    x.AddRequestClient<ConsumptionMessage>();

                });

                services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

                services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<OfferMessage>()); // Defining the types we send to MQ
                services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<ConsumptionMessage>());
                services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BusService>();
            #endregion
            
            #region Mass-Transit Configuration
                // var serviceBusBuilder = new ContainerBuilder();    
            // var rabbitMqUrl = Configuration.GetSection("ServiceBus.Server:ServiceBusConnection").Value;
            // serviceBusBuilder.Register(context =>
            // {
            //     return Bus.Factory.CreateUsingRabbitMq(rmq =>
            //     {
            //         var host = rmq.Host(new Uri(rabbitMqUrl), h =>
            //         {
            //             h.Username("guest");
            //             h.Password("guest");
            //         }
            //         );
            //         rmq.ExchangeType = ExchangeType.Fanout;
            //         rmq.ReceiveEndpoint(host, "IntradayDashboard", ep =>
            //         {
            //             var message = new MessageModel() { Content = "" };
            //             ep.Handler<MessageModel>(ctx =>
            //             {
            //                 return Console.Out.WriteLineAsync($"Received: {ctx.Message.Content}");
            //             });
            //             ep.Consumer<SendMessageConsumer>();
            //             //ep.Observer<MessageModelObserver>(); // * TEst mas Transit Consumer
            //         });
            //     });
            // }).
            //  As<IBusControl>()
            // .As<IBus>()
            // .As<IPublishEndpoint>()
            // .SingleInstance();
        
            // serviceBusBuilder.Populate(services);
            // ApplicationContainer = serviceBusBuilder.Build();
            // var serviceProvider = new AutofacServiceProvider(ApplicationContainer);

            #endregion
            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifeTime, ILoggerFactory loggerFactory, Seed seeder)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                    var context = serviceScope.ServiceProvider.GetRequiredService<IntradayDbContext>();
                    context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                var isSeedEnabled = Convert.ToBoolean(Configuration.GetSection("Seed:Enabled").Value);

                if (isSeedEnabled)
                {
                    seeder.SeedUsers();
                }

            }
            else
            {
                app.UseExceptionHandler(builder =>
               {
                   builder.Run(async context =>
                   {
                       context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                       var error = context.Features.Get<IExceptionHandlerFeature>();
                       if (error != null)
                       {
                           context.Response.AddApplicationError(error.Error.Message);
                           await context.Response.WriteAsync(error.Error.Message);
                       }
                   });
               });
                app.UseHsts();
            }

            #region Swagger Test Ui Config.
            
                app.UseSwagger()
                .UseSwaggerUI(sw =>
                {
                    sw.SwaggerEndpoint($"/swagger/v1/swagger.json", "IntradayDashboard.WebApi");
                });
                
            #endregion

            #region SignalR Declarations
                app.UseSignalR(routes =>
                {
                    routes.MapHub<BackgroundJobHub>("/signalr/jobs");
                    routes.MapHub<LoginHub>("/api/auth/login/*");
                });

            #endregion

            #region DefaultDotnet Confs
                app.UseAuthentication();
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseSession();
                app.UseHttpsRedirection();
                app.UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller}/{action}");

                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Fallback", action = "Index" }
                    );
                });
                app.UseCors("CorsPolicy");
            #endregion



            #region Hangfire Background Job

                var options = new BackgroundJobServerOptions
                {
                    ServerName = String.Format(
                        "{0}.{1}",
                        Environment.MachineName,
                        Guid.NewGuid().ToString()),
                    WorkerCount = Environment.ProcessorCount * 5,
                    HeartbeatInterval = TimeSpan.FromSeconds(1),
                    //TaskScheduler = TaskScheduler.Current,
                    //Activator = JobActivator.Current,
                    Queues = new[] { "offer_queue", "consumption_queue", "default" }

                };
                app.UseHangfireServer(options, additionalProcesses: new[] { new SystemMonitor(checkInterval: TimeSpan.FromSeconds(1)) });
                //app.UseHangfireServer(options);   
                app.UseHangfireDashboard();
                
                    
            #endregion
            
            #region StartService buss
            // var bus = ApplicationContainer.Resolve<IBusControl>();
            // var busHandle = TaskUtil.Await(() => bus.StartAsync());
            // appLifeTime.ApplicationStopping.Register(() => busHandle.Stop());
            #endregion
            
        }
    }
}
