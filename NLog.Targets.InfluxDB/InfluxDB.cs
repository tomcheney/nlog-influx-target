using NLog.Config;

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace NLog.Targets.InfluxDB
{
    [Target("InfluxDB")]
    public sealed class InfluxDB : Target
    {
        public InfluxDB()
        {
            //set defaults
            this.Host = "";
            this.Bucket = "";
            this.Org = "";
            this.Token = "";
        }

        [RequiredParameter]
        public string Host { get; set; }

        [RequiredParameter]
        public string Bucket { get; set; }

        [RequiredParameter]
        public string Token { get; set; }

        [RequiredParameter]
        public string Org { get; set; }

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
            base.InitializeTarget();
            m_influxClient = InfluxDBClientFactory.Create(Host, Token);
            m_writeApi = m_influxClient.GetWriteApi();

        }
        protected override void Write(LogEventInfo logEvent)
        {
            var point = PointData.Measurement("NLog")
                .Tag("LoggerName", logEvent.LoggerName)
                .Tag("Level", logEvent.Level.ToString())
                .Field("Message", logEvent.Message)
                .Timestamp(logEvent.TimeStamp.ToUniversalTime(), WritePrecision.Ns);
            m_writeApi.WritePoint(point, Bucket, Org);
        }
    }
}
