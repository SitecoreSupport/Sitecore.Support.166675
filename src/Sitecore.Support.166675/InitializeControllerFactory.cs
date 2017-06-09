using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Pipelines;
using System.Web.Mvc;
using Sitecore.Support.Mvc.Controllers;

namespace Sitecore.Support.Mvc.Pipelines.Loader
{
    public class InitializeControllerFactory
    {
        public virtual void Process(PipelineArgs args)
        {
            this.SetControllerFactory(args);
        }

        protected virtual void SetControllerFactory(PipelineArgs args)
        {
            var controllerFactory = new SitecoreControllerFactoryExtended(ControllerBuilder.Current.GetControllerFactory());
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}