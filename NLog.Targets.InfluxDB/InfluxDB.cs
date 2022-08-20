using NLog.Config;

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Net;
using NLog.Layouts;

namespace NLog.Targets.InfluxDB
{
    [Target("InfluxDB")]
    public sealed class InfluxDB : Target
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

        InfluxDBClient m_influxClient;
        WriteApi m_writeApi;
        string m_application = "";
        string m_version = "";
        string m_hostName = Dns.GetHostName();
        string m_guid = Guid.NewGuid().ToString();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_writeApi.Flush();
            m_writeApi.Dispose();
            m_influxClient.Dispose();
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            var host = RenderLogEvent(Host, LogEventInfo.CreateNullEvent());
            var token = RenderLogEvent(Token, LogEventInfo.CreateNullEvent());
            m_influxClient = InfluxDBClientFactory.Create(host, token);
            m_writeApi = m_influxClient.GetWriteApi();

            var name = System.Reflection.Assembly.GetEntryAssembly().GetName();

            m_application = name.Name;
            m_version = name.Version.ToString();
        }
        protected override void Write(LogEventInfo logEvent)
        {
            var point = PointData.Measurement("NLog")
                .Tag("LoggerName", logEvent.LoggerName)
                .Tag("Level", logEvent.Level.ToString())
                .Tag("Application", m_application)
                .Tag("Version", m_version)
                .Tag("GUID", m_guid)
                .Tag("Host", m_hostName)
                .Field("Message", logEvent.Message)
                .Field("level", logEvent.Level.ToString().ToLower())
                .Timestamp(logEvent.TimeStamp.ToUniversalTime(), WritePrecision.Ns);
            
            var bucket = RenderLogEvent(Bucket, logEvent);
            var organization = RenderLogEvent(Org, logEvent);
            m_writeApi.WritePoint(point, bucket, organization);
        }
    }
}
