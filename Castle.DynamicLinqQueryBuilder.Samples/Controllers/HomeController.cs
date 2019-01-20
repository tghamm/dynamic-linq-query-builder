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

        //Return the default definitions for the class, and the list of People
        public ActionResult Index()
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            var definitions = typeof(PersonRecord).GetDefaultColumnDefinitionsForType(false);
            var people = PersonBuilder.GetPeople();
            
            //Augment the definitions to show advanced scenarios not
            //handled by GetDefaultColumnDefinitionsForType(...)

            //Let's tweak the generated definition of FirstName to make it
            //a select element in jQuery QueryBuilder UI populated by
            //the possible values from our dataset
            var firstName = definitions.First(p => p.Field.ToLower() == "firstname");
            firstName.Values = people.Select(p => p.FirstName).Distinct().ToList();
            firstName.Input = "select";

            ViewBag.FilterDefinition =
               JsonConvert.SerializeObject(definitions, jsonSerializerSettings);
            ViewBag.Model = people;
            return View();
        }

        //Take the POSTed FilterRule, build query, and return results
        [HttpPost]
        public JsonResult Index(FilterRule obj)
        {
            var people = PersonBuilder.GetPeople().BuildQuery(obj).ToList();
            return Json(people);
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