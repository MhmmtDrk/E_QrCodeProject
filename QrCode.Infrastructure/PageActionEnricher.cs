using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Infrastructure
{
    public class PageActionEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var frame = new StackFrame(1, true);
            var method = frame.GetMethod();

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Page", method.DeclaringType?.Name));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Action", method.Name));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("LineNumber", frame.GetFileLineNumber()));
        }
    }
}
