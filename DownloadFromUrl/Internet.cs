using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace twr.common
{
    /// <summary>
    /// internet.
    /// </summary>
    public static class Internet
    {
        #region Properties

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public static HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// enable the tls protocols.
        /// </summary>
        public static void EnableTlsProtocols()
        {
//            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Downloads value from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The response value.</returns>
        public static byte[] DownloadUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                Internet.EnableTlsProtocols();

                // Create a web request to the URL
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;

                // Get the web response
                var response = (HttpWebResponse)request.GetResponse();
                StatusCode = response.StatusCode;

                byte[] fileContent = null;

                // Make sure the response is valid
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Open the response stream
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                fileContent = Encoding.Default.GetBytes(reader.ReadToEnd());
                            }
                        }
                    }
                }

                response.Close();

                return fileContent;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Downloads value from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="value">The response value.</param>
        /// <returns>True if the download was successful; otherwise false.</returns>
        public static bool DownloadUrl(string url, out string value)
        {
            value = string.Empty;

            var fileContent = DownloadUrl(url);
            if (fileContent == null)
                return false;

            try
            {
                foreach (var b in fileContent)
                {
                    value += (char)b;
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return false;
            }
        }

        /// <summary>
        /// download the file.
        /// </summary>
        /// <param name="url">the url.</param>
        /// <param name="destinationFile">the destination file.</param>
        /// <param name="throwException">the throw exception.</param>
        /// <param name="timeoutInSeconds">the timeout in seconds.</param>
        /// <returns>true if the download file, otherwise false.</returns>
        public static bool DownloadFile(string url, string destinationFile, bool throwException = false, int timeoutInSeconds = 0)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(destinationFile))
                return false;

            try
            {
                Internet.EnableTlsProtocols();

                // Create a web request to the URL
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeoutInSeconds * 1000;

                // Get the web response
                var response = (HttpWebResponse)request.GetResponse();

                // Make sure the response is valid
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    // Open the response stream
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            return false;

                        // Open the destination file
                        using (var fileStream = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            // Create a 4K buffer to chunk the file
                            var bytes = new byte[4096];
                            int bytesRead;
                            // Read the chunk of the web response into the buffer
                            while (0 < (bytesRead = responseStream.Read(bytes, 0, bytes.Length)))
                            {
                                // Write the chunk from the buffer to the file
                                fileStream.Write(bytes, 0, bytesRead);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                //MessageBox.Show("Error saving file from URL:\r\n" + err.Message);
                Trace.WriteLine(exception.Message);
                if (throwException)
                    throw;

                return false;
            }

            #endregion
        }
    }
}