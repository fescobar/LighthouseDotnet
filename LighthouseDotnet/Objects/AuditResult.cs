using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LighthouseDotnet.Objects
{
    /// <summary>
    /// Lighthouse Audit Result
    /// </summary>
    public sealed class AuditResult
    {
        /// <summary>
        /// Json report
        /// </summary>
        public string ReportJson { get; set; }
        /// <summary>
        /// Html report
        /// </summary>
        public string ReportHtml { get; set; }
        /// <summary>
        /// Csv report
        /// </summary>
        public string ReportCsv { get; set; }
        /// <summary>
        /// Performance scope
        /// </summary>
        public decimal? Performance { get; set; }
        /// <summary>
        /// Accessibility scope
        /// </summary>
        public decimal? Accessibility { get; set; }
        /// <summary>
        /// Best practices scope
        /// </summary>
        public decimal? BestPractices { get; set; }
        /// <summary>
        /// SEO scope
        /// </summary>
        public decimal? Seo { get; set; }
        /// <summary>
        /// Progressive Web App scope
        /// </summary>
        public decimal? Pwa { get; set; }


        public decimal? BenchmarkIndex { get; set; }
        public List<Details> Details { get; set; }
        public List<TimingDetails> TimingDetails { get; set; }
        public decimal? TotalDuration { get; set; }

        public Screenshot FinalScreenshot { get; set; }
        public IReadOnlyList<Screenshot> Thumbnails { get; set; }

        public static AuditResult Parse(string json)
        {
            if (String.IsNullOrEmpty(json)) return null;
            dynamic jObj1 = JObject.Parse(json);
            if (jObj1 == null) return null;


            dynamic jObj = JObject.Parse(Convert.ToString(jObj1.json));

            //Console.WriteLine(jObj.json);
            //jObj = jObj.tojson;
            var a = new AuditResult
            {
                ReportJson = jObj1.json,
                ReportHtml = jObj1.html,
                ReportCsv = jObj1.csv,
                Details = new List<Details>(),
                TimingDetails = new List<TimingDetails>(),
                BenchmarkIndex = jObj.environment?.benchmarkIndex,
                Performance = jObj.categories?.performance?.score,
                Accessibility = jObj.categories?.accessibility?.score,
                BestPractices = jObj.categories?["best-practices"]?.score,
                Seo = jObj.categories?.seo?.score,
                Pwa = jObj.categories?.pwa?.score
            };


            var audits = jObj.audits as JObject;
            if (audits != null)
            {
                foreach (var audit in audits)
                {
                    var val = (dynamic)audit.Value;
                    a.Details.Add(new Details()
                    {
                        Name = audit.Key,
                        Score = val.score,
                        NumericValue = val.numericValue
                    });

                    if (audit.Key == "final-screenshot")
                    {
                        var base64 = val.details?.data?.ToString();
                        if (!String.IsNullOrEmpty(base64))
                        {
                            a.FinalScreenshot = new Screenshot(base64);
                        }
                    }
                    else if (audit.Key == "screenshot-thumbnails" && val.details?.items is JArray items && items.HasValues)
                    {
                        var screenShots = new List<Screenshot>();
                        foreach (var item in items)
                        {
                            var base64 = ((dynamic)item).data?.ToString();
                            if (!String.IsNullOrEmpty(base64)) screenShots.Add(new Screenshot(base64));
                        }
                        a.Thumbnails = screenShots;
                    }
                }
            }
            var timingDetails = jObj.timing?.entries as JArray;
            if (timingDetails != null)
            {
                foreach (var timingDetail in timingDetails)
                {
                    var val = (dynamic)timingDetail;
                    a.TimingDetails.Add(new TimingDetails()
                    {
                        Name = val?.name,
                        StartTime = val?.startTime,
                        Duration = val?.duration
                    });
                }
                a.TotalDuration = jObj.timing?.total;
            }

            return a;
        }
    }
}
