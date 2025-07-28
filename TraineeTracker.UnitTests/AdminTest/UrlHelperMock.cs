using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace TraineeTracker.UnitTests.AdminTest {
    public class UrlHelperMock : IUrlHelper {
        public ActionContext ActionContext { get; } = new ActionContext {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        };
        public string? Action(UrlActionContext actionContext) => "https://dummy-link";
        public string? Content(string? contentPath) => "https://dummy-link";
        public bool IsLocalUrl(string? url) => false;
        public string? Link(string? routeName, object? values) => "https://dummy-link";
        public string? RouteUrl(UrlRouteContext routeContext) => "https://dummy-link";
        public VirtualPathData? GetVirtualPath(RouteContext context) {
            return new VirtualPathData(Mock.Of<IRouter>(), "dummy-link");
        }
    }
}