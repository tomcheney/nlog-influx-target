# nlog-influx-target
NLog logging target for InfluxDB v2.

![Nuget](https://img.shields.io/nuget/dt/NLog.Targets.InfluxDB)
[![GitHub issues](https://img.shields.io/github/issues/tomcheney/nlog-influx-target)](https://github.com/tomcheney/nlog-influx-target/issues)
![Nuget](https://img.shields.io/nuget/v/NLog.Targets.InfluxDB)
[![GitHub forks](https://img.shields.io/github/forks/tomcheney/nlog-influx-target)](https://github.com/hayrullahcansu/nlog-kafka-target/network)
[![GitHub stars](https://img.shields.io/github/stars/tomcheney/nlog-influx-target)](https://github.com/hayrullahcansu/nlog-kafka-target/stargazers)


## Supported frameworks 
```
.NET Core (NLog.Extensions.Logging package)
.NET Standard 2.x+ - NLog 4.5
```

## Getting Started
### Step 1: Install NLog.Targets.InfluxDB package from [nuget.org](https://www.nuget.org/packages/NLog.Targets.InfluxDB/)
```
Install via Package-Manager   Install-Package NLog.Targets.InfluxDB
Install via .NET CLI          dotnet add package NLog.Targets.InfluxDB
```
### Step 2: Configure nlog sections

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">
  <extensions>
		<add assembly="NLog.Targets.InfluxDB"/>
	</extensions>

	<targets>
		<target xsi:type="InfluxDB"
				name="influx"
				Host="https://yourinfluxhost.com"
				Bucket="MyBucket"
				Token="inFluXApi=K3y"
				Org="Organisation"/>
	</targets>
	<rules>
		<logger name="*" minlevel="Trace" writeTo="influx" />
	</rules>
</nlog>
```
| Param Name | Variable Type | Requirement | Description                         |
|------------|---------------|-------------|-------------------------------------|
| Host       | `:string`     |    yes   | InfluxDB Host                       |
| Bucket      | `:string`     |    yes   | InfluxDB Bucket         |
| Token     | `:string`     |      yes     | InfluxDB API Token |
| Org    | `:string`     |    yes   | InfluxDB Organisation  |



## Example Project
There is an example WPF application project `NLog.Targets.InfluxDB.Demo` in the repo which demonstrates usage.
