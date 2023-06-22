using CsvHelper;
//using DocumentFormat.OpenXml.Spreadsheet;
using IDVerification.Data;
using IDVerification.Models;
using IDVerification.VIewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    return BadRequest("Input is not appropriate!!! " + e);
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

        [HttpPost]
        [Route("/mission")]
        public IActionResult getMissions(long missionId)
        {
            var mission = (from m in _IDVDbContext.Missions
                           where m.MissionId == missionId
                           join city in _IDVDbContext.Cities
                           on m.CityId equals city.CityId
                           join country in _IDVDbContext.Countries
                           on city.CountryId equals country.CountryId
                           select new
                           {
                               Id = m.MissionId,
                               m.Title,
                               m.ShortDescription,
                               m.Description,
                               m.StartDate,
                               m.EndDate,
                               m.MissionType,
                               m.Status,
                               m.OrganizationName,
                               m.OrganizationDetail,
                               m.Availability,
                               m.CreatedAt,
                               m.UpdatedAt,
                               m.DeletedAt,
                               m.SeatLeft,
                               m.TotalSeats,
                               m.Deadline,
                               Invites = m.MissionInvites.Select(x => new { Id = x.MissionInviteId, x.FromUserId, x.ToUserId }),
                               Ratings = m.MissionRatings.Select(x => new { Id = x.MissionRatingId, x.MissionId, x.Rating }),
                               Address = city.Name + ',' + country.Name + '(' + country.Iso + ')',
                               Theme = new { Id = m.Theme.MissionThemeId, Title = m.Theme.Title }
                           }).AsQueryable().FirstOrDefault();
            //var json = JsonConvert.SerializeObject(mission);
            //var iDVResult = JsonConvert.DeserializeObject<IDVResult>(jsonString);

            return Ok(mission);
        }

        [HttpPost]
        [Route("/spGetMissions")]
        public IActionResult spGetMissions(long missionId)
        {
            MissionViewModel mvm = new MissionViewModel();

            using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;user=root;password=Tatva@123;database=rodatabasetask;"))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand("spGetMissions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("MissionId", missionId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            {
                                mvm.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
                                mvm.Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address"));
                                mvm.Title = reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title"));
                                mvm.ShortDescription = reader.IsDBNull(reader.GetOrdinal("short_description")) ? string.Empty : reader.GetString(reader.GetOrdinal("short_description"));
                                mvm.Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description"));
                                mvm.StartDate = reader.IsDBNull(reader.GetOrdinal("start_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_date"));
                                mvm.EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("end_date"));
                                mvm.MissionType = reader.IsDBNull(reader.GetOrdinal("mission_type")) ? string.Empty : reader.GetString(reader.GetOrdinal("mission_type"));
                                mvm.Status = reader.IsDBNull(reader.GetOrdinal("status")) ? string.Empty : reader.GetString(reader.GetOrdinal("status"));
                                mvm.OrganizationName = reader.IsDBNull(reader.GetOrdinal("organization_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("organization_name"));
                                mvm.OrganizationDetail = reader.IsDBNull(reader.GetOrdinal("organization_detail")) ? string.Empty : reader.GetString(reader.GetOrdinal("organization_detail"));
                                mvm.Availability = reader.IsDBNull(reader.GetOrdinal("availability")) ? string.Empty : reader.GetString(reader.GetOrdinal("availability"));
                                mvm.CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("created_at"));
                                mvm.UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("updated_at"));
                                mvm.DeletedAt = reader.IsDBNull(reader.GetOrdinal("deleted_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("deleted_at"));
                                mvm.SeatLeft = reader.IsDBNull(reader.GetOrdinal("seat_left")) ? 0 : reader.GetInt32(reader.GetOrdinal("seat_left"));
                                mvm.TotalSeats = reader.IsDBNull(reader.GetOrdinal("total_seats")) ? 0 : reader.GetInt32(reader.GetOrdinal("total_seats"));
                                mvm.Deadline = reader.IsDBNull(reader.GetOrdinal("deadline")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("deadline"));
                                //mvm.Deadline = reader.IsDBNull(reader.GetOrdinal("deadline")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("deadline"));

                            }
                        }
                        // Retrieve mission theme data
                        reader.NextResult();
                        if (reader.Read())
                        {
                            mvm.Theme = new VIewModels.MissionTheme()
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : Convert.ToInt32(reader["Id"]),
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? string.Empty : reader["Title"].ToString()
                            };
                        }

                        // Retrieve mission invite data
                        mvm.Invites = new List<VIewModels.MissionInvite>();
                        reader.NextResult();
                        while (reader.Read())
                        {
                            mvm.Invites.Add(new VIewModels.MissionInvite
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : Convert.ToInt32(reader["Id"]),
                                FromUserId = reader.IsDBNull(reader.GetOrdinal("from_user_id")) ? 0 : Convert.ToInt32(reader["from_user_id"]),
                                ToUserId = reader.IsDBNull(reader.GetOrdinal("to_user_id")) ? 0 : Convert.ToInt32(reader["to_user_id"])
                            });
                        }

                        // Retrieve mission rating data
                        mvm.Ratings = new List<VIewModels.MissionRating>();
                        reader.NextResult();
                        while (reader.Read())
                        {
                            mvm.Ratings.Add(new VIewModels.MissionRating
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : Convert.ToInt32(reader["Id"]),
                                MissionId = reader.IsDBNull(reader.GetOrdinal("mission_id")) ? 0 : Convert.ToInt32(reader["mission_id"]),
                                Rating = reader.IsDBNull(reader.GetOrdinal("RATING")) ? 0 : Convert.ToInt32(reader["RATING"])
                            });
                        }

                        connection.Close();
                    }
                }
            }
            return Ok(mvm);
        }
    }
}
