
using System.Text;

namespace NET_9_Business_App_MinimalAPI_Results.Results
{
    public class HtmlResults : IResult
    {
        //create a private, in-class param to hold the html stream obj
        private readonly string html;

        public HtmlResults(string html)//a string obj param with html code 
        {
            this.html = html;//populate local HtmlResults html obj with string obj param
        }
        public async Task ExecuteAsync(HttpContext httpContext)//open an http context channel
        {
            httpContext.Response.ContentType = "text/html";//define content type
            httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(html);//set length of stream

            await httpContext.Response.WriteAsync(html);//write contents into stream (i.e. your display)
        }
    }
}
