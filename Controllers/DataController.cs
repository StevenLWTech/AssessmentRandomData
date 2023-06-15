using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace assessment.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly DownloadSettings _downloadSettings;

        public DataController(IOptions<DownloadSettings> downloadSettings)
        {
            _client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _downloadSettings = downloadSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Download, decompress and process data
                await DownloadFileAsync(_downloadSettings.DownloadUrl, _downloadSettings.DownloadPath);
                await DecompressFileAsync(_downloadSettings.DownloadPath, _downloadSettings.DecompressedPath);
                var result = await ProcessData(_downloadSettings.DecompressedPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception and return error response
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task DownloadFileAsync(string url, string downloadPath)
        {
            try
            {
                var fileBytes = await _client.GetByteArrayAsync(url).ConfigureAwait(false);

                await System.IO.File.WriteAllBytesAsync(downloadPath, fileBytes).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while downloading the file", ex);
            }
        }

        private async Task DecompressFileAsync(string compressedFile, string decompressedFile)
        {
            try
            {
                using FileStream originalFileStream = System.IO.File.OpenRead(compressedFile);
                using FileStream decompressedFileStream = System.IO.File.Create(decompressedFile);
                using GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
                await decompressionStream.CopyToAsync(decompressedFileStream).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while decompressing the file", ex);
            }
        }
        private async Task<object> ProcessData(string decompressedPath)
        {
            return await Task.Run(() =>
            {
                int itemCount = 0, floridaCount = 0;
                double highestIncome = 0, sumIncome = 0;
                string fullName = "", personWithHighestIncomeId = "";

                using (StreamReader file = System.IO.File.OpenText(decompressedPath))
                using (JsonTextReader reader = new(file))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            JObject obj = JObject.Load(reader);
                            if (obj != null)
                            {
                                itemCount++;

                                if (obj["address"] is JObject address && address["state_name"]?.ToString() == "Florida")
                                {
                                    if (double.TryParse(obj["yearly_income"]?.ToString(), out double yearlyIncome))
                                    {
                                        sumIncome += yearlyIncome;
                                        floridaCount++;

                                        if (yearlyIncome > highestIncome)
                                        {
                                            highestIncome = yearlyIncome;

                                            if (obj["id"] is JToken idToken)
                                                personWithHighestIncomeId = idToken.ToString() ?? "";

                                            if (obj["full_name"] is JToken nameToken)
                                                fullName = nameToken.ToString() ?? "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var result = new
                {
                    itemCount,
                    floridaCount,
                    personWithHighestIncome = new
                    {
                        Id = personWithHighestIncomeId,
                        Name = fullName,
                        Income = highestIncome
                    },
                    averageIncomeInFlorida = floridaCount > 0 ? sumIncome / floridaCount : 0
                };

                return result;
            });
        }


    }
}
