using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Castle.DynamicLinqQueryBuilder.Samples.Sample;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Castle.DynamicLinqQueryBuilder.Samples.Controllers
{
    public class HomeController : Controller
    {

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
        public ActionResult Index()
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            ViewBag.FilterDefinition =
               JsonConvert.SerializeObject(typeof (PersonRecord).GetDefaultColumnDefinitionsForType(false), jsonSerializerSettings);
            ViewBag.Model = PersonBuilder.GetPeople();
            return View();
        }

        [HttpPost]
        public JsonResult Index(FilterRule obj)
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var people = PersonBuilder.GetPeople().BuildQuery(obj).ToList();
            //ViewBag.Json = people;
            return Json(people);

            /* 

             ViewBag.FilterDefinition =
                JsonConvert.SerializeObject(typeof(PersonRecord).GetDefaultColumnDefinitionsForType(false), jsonSerializerSettings);
             ViewBag.Model = PersonBuilder.GetPeople();
             return View();*/
            //return "";
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class CustomJsonResult : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }
}