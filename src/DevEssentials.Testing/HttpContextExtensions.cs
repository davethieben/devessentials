namespace DevEssentials.Testing
{
    public static class HttpContextExtensions
    {
        /*
        public static ServiceFactorySetup SetupHttpContext(this ServiceFactorySetup serviceFactory, Action<HttpContextMock>? mockSetup = null)
        {
            HttpContextMock? mock = null;
            var mockDescriptor = serviceFactory.FirstOrDefault(sd => sd.ServiceType == typeof(HttpContextMock));
            if (mockDescriptor == null)
            {
                mock = new HttpContextMock();
                serviceFactory.AddSingleton<HttpContextMock>(mock);
                serviceFactory.AddTransient<HttpContext>(sp => mock.Object);
            }
            else
            {
                mock = mockDescriptor.ImplementationInstance as HttpContextMock
                    ?? throw new ApplicationException("Cannot find HttpContextMock in ServiceFactory");
            }
            mockSetup?.Invoke(mock);

            return serviceFactory;
        }

        public class HttpContextMock : Mock<HttpContext>
        {
            public HttpContextMock()
            {
                Setup(x => x.User).Returns(() => Principal);
                Setup(x => x.Request).Returns(() => RequestMock.Object);
                Setup(x => x.Response).Returns(() => ResponseMock.Object);
                Setup(x => x.Features).Returns(() => Features);
            }

            public User AppUser { get; set; } = new User { Id = 1, FirstName = "Unit", LastName = "Test" };
            public Settings Settings { get; set; } = new Settings { Id = 1 };

            public ClaimsPrincipal Principal
            {
                get
                {
                    var identity = new ClaimsIdentity("authenticated");
                    identity.AddClaim(new Claim(ClaimTypes.Name, AppUser?.UserName ?? "UnitTest"));
                    identity.AddClaim(new Claim(UserClaimTypes.UserId, (AppUser?.Id ?? 1).ToString()));
                    return new ClaimsPrincipal(identity);
                }
            }

            public Mock<HttpRequest> RequestMock { get; } = new();

            public Mock<HttpResponse> ResponseMock { get; } = new();

            public FeatureCollection Features { get; set; } = new();


        }

        public static ServiceFactorySetup AddController(this ServiceFactorySetup services, Type controllerType, User? user = null)
        {
            if (!typeof(Controller).IsAssignableFrom(controllerType))
                throw new ArgumentException($"Type '{controllerType}' is not a controller");

            services.SetupHttpContext(http => http.AppUser = user ?? http.AppUser);
            services.AddTransient(controllerType);

            return services;
        }

        public static TController GetController<TController>(this IServiceProvider serviceProvider)
            where TController : Controller
        {
            var httpContext = serviceProvider.GetRequiredService<HttpContext>();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());

            var controller = serviceProvider.GetRequiredService<TController>();
            controller.ControllerContext = new ControllerContext(actionContext);
            return controller;
        }

        */
    }
}
