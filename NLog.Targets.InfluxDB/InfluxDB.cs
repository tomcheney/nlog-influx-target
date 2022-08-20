using NLog.Config;

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Net;
using NLog.Layouts;
using System.Collections.Generic;

namespace NLog.Targets.InfluxDB
{
    [Target("InfluxDB")]
    public class InfluxDB : Target
    {
        public InfluxDB()
        {
            //set defaults
            Host = "";
            Bucket = "";
            Org = "";
            Token = "";
        }

        [RequiredParameter]
        public Layout Host { get; set; }

        [RequiredParameter]
        public Layout Bucket { get; set; }

        [RequiredParameter]
        public Layout Token { get; set; }

        [RequiredParameter]
        public Layout Org { get; set; }

        [DefaultParameter]
        public Layout Measurement { get; set; } = "NLog";

        [ArrayParameter(typeof(TargetPropertyWithContext), "tag")]
        public virtual IList<TargetPropertyWithContext> Tags { get; } = new List<TargetPropertyWithContext>();

        [ArrayParameter(typeof(TargetPropertyWithContext), "field")]
        public virtual IList<TargetPropertyWithContext> Fields { get; } = new List<TargetPropertyWithContext>();

        InfluxDBClient m_influxClient;
        WriteApi m_writeApi;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_writeApi.Flush();
            m_writeApi.Dispose();
            m_influxClient.Dispose();
        }

        protected override void InitializeTarget()
        {
            var host = RenderLogEvent(Host, LogEventInfo.CreateNullEvent());
            var token = RenderLogEvent(Token, LogEventInfo.CreateNullEvent());
            m_influxClient = InfluxDBClientFactory.Create(host, token);
            m_writeApi = m_influxClient.GetWriteApi();


            if (Tags.Count == 0)
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly()?.GetName();
                var applicationName = entryAssembly?.Name;
                var applicationVersion = entryAssembly?.Version.ToString();
                var instanceGuid = Guid.NewGuid().ToString();
                var hostName = Dns.GetHostName();

                Tags.Add(new TargetPropertyWithContext("LoggerName", "${logger}"));
                Tags.Add(new TargetPropertyWithContext("Level", "${level}"));
                Tags.Add(new TargetPropertyWithContext("Application", applicationName));
                Tags.Add(new TargetPropertyWithContext("Version", applicationVersion));
                Tags.Add(new TargetPropertyWithContext("GUID", instanceGuid));
                Tags.Add(new TargetPropertyWithContext("Host", hostName));
            }

            if (Fields.Count == 0)
            {
                Fields.Add(new TargetPropertyWithContext("Message", "${message:raw=true}"));
            }

            base.InitializeTarget();

        }
        protected override void Write(LogEventInfo logEvent)
        {
            var measurement = RenderLogEvent(Measurement, logEvent);
            var point = PointData.Measurement(measurement);
            for (int i = 0; i < Tags.Count; ++i)
            {
                var tag = Tags[i];
                var tagValue = RenderLogEvent(tag.Layout, logEvent);
                if (string.IsNullOrEmpty(tagValue))
                    continue;

                point = point.Tag(tag.Name, tagValue);
            }

            for (int i = 0; i < Fields.Count; ++i)
            {
                var field = Fields[i];
                var fieldValue = RenderLogEvent(field.Layout, logEvent);
                if (string.IsNullOrEmpty(fieldValue))
                    continue;

                point = point.Field(field.Name, fieldValue);
            }

            var bucket = RenderLogEvent(Bucket, logEvent);
            var organization = RenderLogEvent(Org, logEvent);
            m_writeApi.WritePoint(point, bucket, organization);
        }
    }
}
