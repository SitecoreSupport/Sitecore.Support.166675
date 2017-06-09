using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Helpers;

namespace Sitecore.Support.Mvc.Controllers
{
    public class SitecoreControllerFactoryExtended : SitecoreControllerFactory
    {
        public SitecoreControllerFactoryExtended(IControllerFactory innerFactory) : base(innerFactory)
        {
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Assert.ArgumentNotNull(requestContext, "requestContext");
            Assert.ArgumentNotNull(controllerName, "controllerName");
            try
            {
                HttpContextBase requiredService = ServiceLocator.ServiceProvider.GetRequiredService<HttpContextBase>();
                ISitecoreServiceLocatorScope scope = requiredService.Items[typeof(ISitecoreServiceLocatorScope)] as ISitecoreServiceLocatorScope;

                if (controllerName.EqualsText(this.SitecoreControllerName))
                {
                    return this.CreateSitecoreController(requestContext, controllerName);
                }
                DefaultControllerFactory factory = this.InnerFactory as DefaultControllerFactory;
                if (factory != null)
                {
                    //Get controller type
                    MethodInfo dynMethod = factory.GetType().GetMethod("GetControllerType", BindingFlags.NonPublic | BindingFlags.Instance);

                    Type serviceType = (Type)dynMethod.Invoke(factory, new object[] { requestContext, controllerName });

                    if (serviceType != null)
                    {
                        if (scope != null)
                        {
                            var controller = (scope.ServiceProvider.GetService(serviceType) as IController) ?? TypeHelper.CreateObject<IController>(serviceType, new object[0]);
                            return (controller ?? this.InnerFactory.CreateController(requestContext, controllerName));
                        }
                    }
                }
                return this.InnerFactory.CreateController(requestContext, controllerName);

            }
            catch (Exception exception)
            {
                if (!MvcSettings.DetailedErrorOnMissingController)
                {
                    throw;
                }
                throw this.CreateControllerCreationException(controllerName, exception);
            }
        }
    }
}