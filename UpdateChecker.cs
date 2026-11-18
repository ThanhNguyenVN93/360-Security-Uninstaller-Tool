using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace frm360security
{
    internal class ReleaseInfo
    {
        public string TagName;      // e.g. "v1.1.0"
        public string Body;         // release notes (markdown)
        public string HtmlUrl;      // browser URL for the release page
        public Version RemoteVersion;
    }

    internal static class UpdateChecker
    {
        private const string ApiUrl =
            "https://api.github.com/repos/ThanhNguyenVN93/360-Security-Uninstaller-Tool/releases/latest";

        public static event Action<ReleaseInfo> UpdateAvailable;
        public static event Action CheckFailed;

        public static void CheckAsync()
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("User-Agent", "360-Uninstaller-Tool-UpdateCheck");
            wc.DownloadStringCompleted += OnDownloadCompleted;
            try { wc.DownloadStringAsync(new Uri(ApiUrl)); }
            catch { if (CheckFailed != null) CheckFailed(); }
        }

        private static void OnDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled || e.Error != null) { if (CheckFailed != null) CheckFailed(); return; }

                string json = e.Result;
                string tag = Extract(json, "tag_name");
                string htmlUrl = Extract(json, "html_url");
                string body = Extract(json, "body");

                if (string.IsNullOrEmpty(tag)) return;

                // tag is "v1.2.3" — strip leading 'v'
                string vStr = tag.TrimStart('v');
                Version remote;
                try { remote = new Version(vStr); }
                catch { return; }

                Version local = Assembly.GetExecutingAssembly().GetName().Version;
                if (remote > local && UpdateAvailable != null)
                {
                    UpdateAvailable(new ReleaseInfo
                    {
                        TagName = tag,
                        Body = body ?? string.Empty,
                        HtmlUrl = htmlUrl ?? string.Empty,
                        RemoteVersion = remote
                    });
                }
            }
            catch { }
        }

        // Simple JSON field extractor — no dependency on JSON libs
        private static string Extract(string json, string key)
        {
            // Match: "key":"value"  (string values only)
            Match m = Regex.Match(json,
                "\"" + Regex.Escape(key) + "\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
            if (!m.Success) return null;
            return m.Groups[1].Value
                .Replace("\\n", "\n")
                .Replace("\\r", "")
                .Replace("\\t", "\t")
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\");
        }
    }
}
