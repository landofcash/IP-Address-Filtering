using System;
using System.Web.Mvc;

namespace IPAddressFiltering
{

    /// <summary>
    /// 
    /// </summary>
    public class HttpIPAddressRejectedResult : HttpStatusCodeResult
    {
        public int SubStatusCode { get; private set; }

        public HttpIPAddressRejectedResult(string statusDescription) : base(403, statusDescription)
        {
            SubStatusCode = 6;
        }
        
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.HttpContext.Response.StatusCode = StatusCode;
            context.HttpContext.Response.SubStatusCode = SubStatusCode;
            if (StatusDescription != null)
            {
                context.HttpContext.Response.StatusDescription = StatusDescription;
            }
        }
    }
}
