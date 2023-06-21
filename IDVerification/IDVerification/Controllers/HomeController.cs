using CsvHelper;
//using DocumentFormat.OpenXml.Spreadsheet;
using IDVerification.Data;
using IDVerification.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using Spire.Xls;
using System.Data;
using System.Globalization;

namespace IDVerification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDVDbContext _IDVDbContext;
        public HomeController(IDVDbContext iDVDbContext)
        {
            _IDVDbContext = iDVDbContext;
        }
        [HttpGet]
        [Route("getString")]
        public string getString(string? value)
        {
            if (value != null)
            {
                return (value);
            }
            return ("Enter String");
        }

        [HttpGet]
        [Route("/idv/single/idv-result")]
        public async Task<IActionResult> GetIDVResult(string FirstName, string LastName)
        {
            string filePath = "";
            string jsonString = "";
            IDVResult iDVResult = null;
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                return BadRequest("Firstname and Lastname are required.");
            }
            FirstName = FirstName.ToLower();
            LastName = LastName.ToLower();
            if (FirstName == "mock" && LastName == "pass")
            {
                var a = Directory.GetCurrentDirectory();
                //filePath = @"D:/Tatvasoft/RO Task/IDVerification/IDVerification/Documents/pass.json";
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/pass.json");
            }
            else if (FirstName == "mock" && LastName == "fail")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/fail.json");
            }
            else if (FirstName == "mock" && LastName == "partial")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/partial.json");
            }
            else
            {
                return BadRequest("Enter valid Firstname and Lastname");
            }

            jsonString = System.IO.File.ReadAllText(filePath);
            iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);

            return Created("", iDVResult);
        }

        [HttpPost]
        [Route("AddExcel(xls/xlsx/csv)ToDb")]
        public IActionResult AddExcelToDb(IFormFile excelFile)
        {
            //List<UserTable> resultList = new List<UserTable>();
            //string connectionString = "server=192.168.1.148;port=3306;user=root;password=Tatva@123;database=rodatabasetask;";
            //using (MySqlConnection connection = new MySqlConnection(connectionString))
            //{
            //    connection.Open();

            //    using (MySqlCommand command = new MySqlCommand("spgetUserData", connection))
            //    {
            //        command.CommandType = CommandType.StoredProcedure;

            //        using (MySqlDataReader reader = command.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                // Retrieve data from the reader and populate your object
            //                UserTable userData = new UserTable
            //                {
            //                    UserId = reader.GetInt32("Id"),
            //                    FirstName = reader.GetString("Name"),
            //                };

            //                resultList.Add(userData);
            //            }
            //        }
            //    }
            //}

            if (Path.GetExtension(excelFile.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                // Convert XLS to XLSX format
                excelFile = ConvertXlsToXlsx(excelFile);
            }
            if (Path.GetExtension(excelFile.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                // Convert CSV to XLSX format
                excelFile = ConvertCsvToXlsx(excelFile);
            }
            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents/FATF_Jurisdiction_Ratings.xlsx"); 
            //string fileName = Path.GetFileName(filePath);
            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            //// Create an instance of FormFile with the file contents and other details
            //IFormFile excelFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, null, fileName);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                stream.Position = 0;
                try
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            return BadRequest("No worksheet found in the Excel file.");

                        var rowCount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;
                        var csvContent = string.Empty;

                        for (int row = 1; row <= rowCount; row++)
                        {
                            var csvRow = string.Empty;
                            for (int col = 1; col <= colCount; col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
                                if (cellValue.Contains(","))
                                {
                                    cellValue = cellValue.Replace(",", "-");
                                }
                                csvRow += $"{cellValue.Trim()},";
                            }
                            csvContent += $"{csvRow}\n";
                        }

                        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                        var csvStream = new MemoryStream(csvBytes);

                        using (var reader = new StreamReader(csvStream))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            var records = csv.GetRecords<FatfJurisdictionRating>().ToList();

                            foreach (var record in records)
                            {
                                _IDVDbContext.FatfJurisdictionRatings.Add(record);
                            }
                            _IDVDbContext.SaveChanges();
                            return Ok(records);
                        }
                    }
                }
                catch (InvalidDataException e)
                {
                    return BadRequest("The selected file is not a valid Excel file.");
                }
                catch (Exception e)
                {
                    return BadRequest("Input is not appropriate!!! "+e);
                }
            }
        }


        private IFormFile ConvertXlsToXlsx(IFormFile xlsFile)
        {
            // Save the XLS file to a temporary location
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                xlsFile.CopyTo(fileStream);
            }

            // Convert XLS to XLSX using Spire.Xls
            var xlsxFilePath = Path.ChangeExtension(tempFilePath, ".xlsx");
            var workbook = new Workbook();
            workbook.LoadFromFile(tempFilePath, ExcelVersion.Version97to2003);
            workbook.SaveToFile(xlsxFilePath, ExcelVersion.Version2016);

            // Create a new IFormFile from the converted XLSX file
            var xlsxFileBytes = System.IO.File.ReadAllBytes(xlsxFilePath);
            var xlsxFileName = Path.GetFileNameWithoutExtension(xlsFile.FileName) + ".xlsx";
            var xlsxFile = new FormFile(new MemoryStream(xlsxFileBytes), 0, xlsxFileBytes.Length, null, xlsxFileName);

            // Delete the temporary files
            System.IO.File.Delete(tempFilePath);
            System.IO.File.Delete(xlsxFilePath);

            return xlsxFile;
        }

        private IFormFile ConvertCsvToXlsx(IFormFile csvFile)
        {
            // Read the CSV file content
            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                var csvContent = reader.ReadToEnd();

                // Create a new XLSX file path
                var xlsxFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Convert CSV to XLSX using EPPlus
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    var rows = csvContent.Split('\n');

                    for (int i = 0; i < rows.Length; i++)
                    {
                        var cells = rows[i].Split(',');
                        for (int j = 0; j < cells.Length; j++)
                        {
                            worksheet.Cells[i + 1, j + 1].Value = cells[j];
                        }
                    }

                    package.SaveAs(new FileInfo(xlsxFilePath));
                }

                // Create a new IFormFile from the converted XLSX file
                var xlsxFileBytes = System.IO.File.ReadAllBytes(xlsxFilePath);
                var xlsxFileName = Path.GetFileNameWithoutExtension(csvFile.FileName) + ".xlsx";
                var xlsxFile = new FormFile(new MemoryStream(xlsxFileBytes), 0, xlsxFileBytes.Length, null, xlsxFileName);

                // Delete the temporary XLSX file
                System.IO.File.Delete(xlsxFilePath);

                return xlsxFile;
            }
        }
    }
}
