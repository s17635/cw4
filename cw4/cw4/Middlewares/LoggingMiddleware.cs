using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw4.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            string filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent( System.Reflection.Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName).FullName;
            filePath = Path.Combine(filePath, "Middlewares");
            filePath = Path.Combine(filePath, "requestsLog.txt");

            if(!File.Exists(filePath))
            {
                var myFile = File.Create(filePath);
                myFile.Close();
            }
            
            var bodyStream = string.Empty;
            using(var reader = new StreamReader(context.Request.Body,Encoding.UTF8, true,1024,true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }

            using (var streamWriter = File.AppendText(filePath))
            {
                string method = context.Request.Method;
                string route = context.Request.Path;
                string query = context.Request.QueryString.ToString();
                
                streamWriter.WriteLine($"\nNew request {DateTime.Now}");
                streamWriter.WriteLine($"HTTP {method} {route}");
                streamWriter.WriteLine($"Query: {(query == ""? "null" : query)}");
                streamWriter.WriteLine($"Body: {(bodyStream==""?"null":("\n"+bodyStream))}");
            }


            context.Request.Body.Seek(0, SeekOrigin.Begin);

            await _next(context);
        }
    }
}
