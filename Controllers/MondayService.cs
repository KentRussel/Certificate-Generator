using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;
using Monday_Tradeskola.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Application = Microsoft.Office.Interop.Word.Application;
using Document = Microsoft.Office.Interop.Word.Document;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using RestSharp;

namespace Monday_Tradeskola.Controllers
{
    public class MondayService
    {
        private MondayClient _mondayClient;

        public MondayService(MondayClient client)
        {
            _mondayClient = client;
        }
        private string GetResponseData(string query)
        {
            StringContent content = new StringContent(query, Encoding.UTF8, "application/json");
            using (var response = _mondayClient.PostAsync("", content))
            {
                if (!response.Result.IsSuccessStatusCode)
                {
                    var responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    throw new Newtonsoft.Json.JsonException($"The response from {_mondayClient.BaseAddress} was {response.Result.StatusCode}. Content: {responseContent}");
                }
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }
        private dynamic ParseGraphQLResponse(string responseString, string collectionKey = "")
        {
            JObject responseObject = JObject.Parse(responseString);

            if (responseObject["errors"] != null)
            {
                throw new Newtonsoft.Json.JsonException($"The request was successful but contained errors: " + $"{JsonConvert.DeserializeObject<dynamic>(responseObject["errors"].ToString())}");
            }
            if (responseObject["data"] == null)
            {
                throw new Newtonsoft.Json.JsonException("The request was successful but contained no data");
            }

            dynamic data = null;

            if (string.IsNullOrEmpty(collectionKey))
            {
                data = JsonConvert.DeserializeObject<dynamic>(responseObject["data"].ToString());
            }
            else if (responseObject["data"][collectionKey] != null)
            {
                data = JsonConvert.DeserializeObject<dynamic>(responseObject["data"][collectionKey].ToString());
            }
            else
            {
                throw new Newtonsoft.Json.JsonException($"The collection key '{collectionKey}' was not found in the data object");
            }

            return data;
        }
        public List<Item> GetItemDetails(string pulseID)
        {
            string query = $@"{{ ""query"": ""{{ items (ids: {pulseID}) {{ name column_values (ids:[ email, single_select, date4, rating, long_text, mirror21, mirror1, item_id, files]) {{ id text title value }} }} }}"" }}";

            string response = GetResponseData(query);   

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(json);

            return itemDetails;
        }
        public List<Item> GetItemDetails(string pulseID, string colName)
        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + pulseID + ") { name column_values (ids:[\"" + colName + "\"]) { id text title value } } }" + "\" }";

            string response = GetResponseData(query);

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(json);

            return itemDetails;
        }

        public bool GeneratePDF(List<Item> itemDetails)
        {
            string trainingTopic = itemDetails[0].ItemColumnValues[1].Text;
            string name = itemDetails[0].Name;
            string trainingDate = itemDetails[0].ItemColumnValues[2].Text;
            string itemId = itemDetails[0].ItemColumnValues[7].Text;
            string divisionHead = itemDetails[0].ItemColumnValues[5].Text;
            string pic = itemDetails[0].ItemColumnValues[6].Text;
            string email = itemDetails[0].ItemColumnValues[0].Text.Trim();
            string files = itemDetails[0].ItemColumnValues[8].Text.Trim();

            const string emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailRegex);
            if (!regex.IsMatch(email))
            {
                throw new System.FormatException("The email address is not in the correct format.");
            }

            string filePath = @"C:\Users\pragmat1k\Downloads\Certificates\Internal_Certificate_Template.docx";
            string reportPath = @"C:\Users\pragmat1k\Downloads\Certificates\" + "SurveyCertificate_" + name.Replace(" ", "_") + ".docx";

            // Check if the original Word document exists at the specified path
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found at path: {filePath}");
                return false; // Or handle this error as you see fit
            }

            System.Threading.Thread.Sleep(1000);
            System.IO.File.Copy(filePath, reportPath, true);

            using (var doc = WordprocessingDocument.Open(reportPath, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart;

                foreach (var placeholder in mainPart.Document.Descendants<Text>().ToList())
                {
                    string newText = placeholder.Text;
                    if (newText.Contains("{Name}"))
                    {
                        newText = newText.Replace("{Name}", name);
                    }
                    else if (newText.Contains("{Training}"))
                    {
                        newText = newText.Replace("{Training}", trainingTopic);
                    }
                    else if (newText.Contains("{Date of Training}"))
                    {
                        newText = newText.Replace("{Date of Training}", trainingDate);
                    }
                    else if (newText.Contains("(DivisionHeadPlaceholder)"))
                    {
                        newText = newText.Replace("(DivisionHeadPlaceholder)", divisionHead);
                    }
                    else if (newText.Contains("(PicPlaceholder)"))
                    {
                        newText = newText.Replace("(PicPlaceholder)", pic);
                    }
                    else if (newText.Contains("(ItemIdPlaceholder)"))
                    {
                        newText = newText.Replace("(ItemIdPlaceholder)", itemId);
                    }
                    if (newText != placeholder.Text)
                    {
                        placeholder.Text = newText;
                    }
                }
            }

            Application app = new Application();

            // Check if the Word report was successfully created at the specified path
            if (!File.Exists(reportPath))
            {
                Console.WriteLine($"File not found at path: {reportPath}");
                return false; // Or handle this error as you see fit
            }

            Document wordDoc = app.Documents.Open(reportPath);

            string pdfPath = @"C:\Users\pragmat1k\Downloads\Certificates\" + "SurveyCertificate_" + name.Replace(" ", "_") + ".pdf";
            wordDoc.SaveAs2(pdfPath, WdSaveFormat.wdFormatPDF);

            wordDoc.Close();

            app.Quit();

            // Check if the PDF was successfully created at the specified path
            if (!File.Exists(pdfPath))
            {
                Console.WriteLine($"File not found at path: {pdfPath}");
                return false; // Or handle this error as you see fit
            }

            try
            {
                string from = "";
                string subject = trainingTopic + ":" + " Training Certification";
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = "";
                string smtpPass = "";

                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))

                {
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, email, subject, "Attached file is your certificate."))
                    {
                        mail.Attachments.Add(new Attachment(pdfPath));
                        client.Send(mail);
                    }
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SmtpException occurred while sending email: {ex.Message}");
                Console.WriteLine($"Status code: {ex.StatusCode}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while sending email: {ex.Message}");
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.monday.com/v2/file");
                    request.Headers.Add("Authorization", "");
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent($"mutation add_file($file: File!) {{add_file_to_column (item_id: {itemId}, column_id:\"files\" file: $file) {{id}}}}"), "query");
                    content.Add(new StringContent("{\"image\":\"variables.file\"}"), "map");
                    content.Add(new StreamContent(File.OpenRead(pdfPath)), "image", Path.GetFileName(pdfPath));
                    request.Content = content;
                    var response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }

            return true;
        }

    }

}