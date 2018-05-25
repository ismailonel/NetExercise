using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace NetExercise.Controllers
{
    public class ExerciseController : ApiController
    {
        public IHttpActionResult Post([FromBody]string value)
        {
            try
            {
                var text = HttpContext.Current.Request.Form["text"];
                var xmlText = new StringBuilder();
                xmlText.Append("<text>");

                var sentences = text.Split('.').ToList();
                sentences.ForEach(item =>
                {
                    var sentence = Regex.Split(item, @"[^\w]+").
                        Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s).ToList();

                    if (sentence.Count != 0)
                    {
                        xmlText.Append("<sentence>");
                        sentence.ForEach(word =>
                        {
                            xmlText.Append("<word>");
                            xmlText.Append(word);
                            xmlText.Append("</word>");
                        });
                        xmlText.Append("</sentence>");
                    }
                });
                xmlText.Append("</text>");

                return Ok(FormatXML(xmlText));
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error occured!");
            }

        }

        static string FormatXML(StringBuilder text)
        {
            var element = XElement.Parse(text.ToString());
            var settings = new XmlWriterSettings
            {
                Indent = true
            };
            using (var xmlWriter = XmlWriter.Create(new StringBuilder(), settings))
            {
                element.Save(xmlWriter);
            }

            var xmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
            return xmlDeclaration + "\n" + element.ToString();
        }
    }
}
