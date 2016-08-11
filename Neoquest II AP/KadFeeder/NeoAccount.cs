using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoFunctions
{
    public class Account
    {
        const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0";

        private static Regex usernameRegex = new Regex(@"^[a-z0-9_]{3,20}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex proxyRegex = new Regex(@"^((?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)):([0-9]{2,6})$", RegexOptions.Compiled);
        private static Regex cookieRegex = new Regex(@"^Set-Cookie: (neologin=[a-z0-9_]{3,20}%2B[a-z0-9]{40})", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private string username, proxy;
        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                if (usernameRegex.IsMatch(value))
                    this.username = value;
                else
                    throw new ArgumentException(value + " is not a valid username!");
            }
        }
        public string Password { get; private set; }
        public string NeoCookie { get; private set; }

        public string Proxy
        {
            get
            {
                return this.proxy;
            }
            set
            {
                if (proxyRegex.IsMatch(value))
                    this.proxy = value;
                else if (value.Length > 0)
                    throw new ArgumentException("Invalid proxy format. Use the IP:Port format.");
                else
                    this.proxy = string.Empty;
            }
        }
        private int Port
        {
            get
            {
                if (this.Proxy.Length > 0)
                {
                    Match proxyMatch = proxyRegex.Match(proxy);
                    int port;
                    if (!proxyMatch.Success || !int.TryParse(proxyMatch.Groups[2].Value, out port))
                        throw new ArgumentException("Invalid proxy format. Use IP:Port format.");
                    return port;
                }
                else
                    return 80;
            }
        }

        public Account(string user, string pass, string proxy = "", string cookie = "")
        {
            this.Username = user;
            this.Password = pass;
            this.Proxy = proxy;
            this.NeoCookie = cookie;
        }

        public override string ToString()
        {
            return this.Username + ":" + this.Password;
        }

        public bool Login()
        {
            string data = string.Format("destination=%252Findex.phtml&username={0}&password={1}",
                this.Username, this.Password);
            string html = this.Post("/login.phtml", data, "http://www.neopets.com/");
            Match cookieMatch = cookieRegex.Match(html);
            this.NeoCookie = cookieMatch.Groups[1].Value;
            return cookieMatch.Success;
        }

        public string Request(string type, string page, string data = "", string referer = "")
        {
            using (TcpClient tcp = new TcpClient("www.neopets.com", this.Port))
            {
                byte[] headers = Encoding.Default.GetBytes(this.MakeHeaders(type, page, referer, data));
                using (NetworkStream ns = tcp.GetStream())
                {
                    ns.Write(headers, 0, headers.Length);
                    using (StreamReader sr = new StreamReader(ns, Encoding.Default))
                    {
                        string html = sr.ReadToEnd();
                        if (Regex.IsMatch(html, "^Content-Encoding: .*?gzip", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                        {
                            int breakPos = html.IndexOf("\r\n\r\n");
                            string head = html.Substring(0, breakPos);
                            string body = html.Substring(breakPos + 4);
                            body = DecompressGZip(body);
                            return head + "\r\n\r\n" + body;
                        }
                        else
                            return html;
                    }
                }
            }
        }

        public string Get(string page, string referer = "")
        {
            return this.Request("GET", page, referer: referer);
        }

        public string Post(string page, string data, string referer = "")
        {

            return this.Request("POST", page, data, referer);
        }

        private string MakeHeaders(string type, string page, string referer, string data)
        {
            string ret = string.Format("{0} {1} HTTP/1.1\nHost: {2}\nUser-Agent: {3}\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\nAccept-Language: en-us,en;q=0.5\nAccept-Encoding: gzip, deflate\nConnection: close",
                type, page, this.Proxy.Length > 0 ? this.Proxy.Split(new char[] { ':' })[0] : "www.neopets.com", UserAgent);
            if (referer.Length > 0)
                ret += "\nReferer: " + referer;
            if (this.NeoCookie.Length > 0)
                ret += "\nCookie: " + this.NeoCookie;
            if (type.IndexOf("post", StringComparison.OrdinalIgnoreCase) > -1)
                ret += string.Format("\nContent-Type: application/x-www-form-urlencoded\nContent-Length: {0}\n\n{1}",
                    data.Length, data);
            return ret + "\r\n\r\n";
        }

        private static string DecompressGZip(string compressed)
        {
            using (MemoryStream memStream = new MemoryStream(Encoding.Default.GetBytes(compressed)))
            {
                using (GZipStream decompressStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    byte[] endBytes = new byte[4];
                    int intPosition = (int)memStream.Length - 4;
                    memStream.Position = intPosition;
                    memStream.Read(endBytes, 0, 4);
                    memStream.Position = 0;
                    byte[] buffer = new byte[BitConverter.ToInt32(endBytes, 0) + 100];
                    int intOffset = 0;
                    while (true)
                    {
                        int intO = decompressStream.Read(buffer, intOffset, 100);
                        if (intO == 0) break;
                        intOffset += intO;
                    }
                    return Encoding.Default.GetString(buffer);
                }
            }
        }
    }
}