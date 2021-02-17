using SievoAssignment.Model;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SievoAssignment
{
   public class Program
    {
        /// <summary>
        /// Main method is the main function exceute console application
        /// </summary>
        /// <param name="args">Arguement is pass to support comman line arguement like file,sortNyStartDate,filter by project ID </param>
       public static void Main(string[] args)
        {
            try
            {
                bool showMenu = true;

                IEnumerable<string> filedata = File.ReadAllLines(ReadFilePath());
                if (filedata != null && args!=null )
                {
                    var result = args[0].ToLower().Contains("file") ? GetFullFilePath(args) : args[0].ToLower().Contains("sort") ? SortByStartDate(filedata) : args[0].ToLower().Contains("project") ? GetDataByProjectId(args, filedata) : "";
                    Console.WriteLine(result);
                }
                filedata = null;// To avoid File connection leakage

                while (showMenu)
                {
                    //Arguement 2 is only to check if the call is from Visual editor or command prompt
                    showMenu = DisplayMenu(args.Length > 2 && !String.IsNullOrEmpty(args[2]));//arguement to check if the call is from command prompt or editor
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Display menu for user input
        /// </summary>
        /// <param name="cmdCheck">Arguement  is only to check if the call is from Visual editor or command prompt</param>
        /// <returns>Boolean value to break the switch case</returns>
        public static bool DisplayMenu(bool cmdCheck)
        {
            Console.WriteLine("Data Management System");
            Console.WriteLine();
            Console.WriteLine("Choose an option");
            Console.WriteLine("1. Create new data");
            Console.WriteLine("2. View Data");
            Console.WriteLine("3. Command Execution");
            Console.WriteLine("4. Exit");
            Console.Write("\r\nSelect an option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    CreateNew();
                    return true;
                case "2":
                    Console.WriteLine(File.ReadAllText(ReadFilePath()));
                    return true;
                case "3":
                    return CommandLineArguement(cmdCheck);
                case "4":
                    return false;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\r\nSelect valid option\n");
                    Console.ResetColor();
                    return true;
            }
        }

        /// <summary>
        /// This method is called to get full path with file name
        /// </summary>
        /// <param name="filepath">Contain file name</param>
        /// <returns>Full Path of file name</returns>
        public static string GetFullFilePath(string[] filepath)
        {
            string fileFulllpath = string.Empty;
            if (filepath.Length > 1 && filepath[1] != null)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                fileFulllpath = Directory.GetFiles(currentDirectory, filepath[1], SearchOption.AllDirectories).FirstOrDefault();
            }
            else
                fileFulllpath = "Please enter valid path";
            return !string.IsNullOrEmpty(fileFulllpath) ? fileFulllpath : "Please Enter valid file name with extension";
        }

        /// <summary>
        /// Sort the record by Start Date
        /// </summary>
        /// <param name="filedata">Contain record from tsv file</param>
        /// <returns>Record sort by date</returns>
        public static string SortByStartDate(IEnumerable<string> filedata)
        {

            List<string> header = ReadFileHeader(filedata);

            List<DataReadModel> obj = filedata
              .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"))// skip empty lines if any
              .Select(line => line.Split('\t'))
              .Skip(1)
              .Select(line => new DataReadModel
              {
                  Project = Convert.ToInt32(line[0]),
                  Description = line[1],
                  StartDate = line[2],
                  Category = line[3],
                  Responsible = line[4],
                  SavingsAmount = line[5],
                  Currency = line[6],
                  Complexity = line[7]
              }).OrderBy(o => o.StartDate).ToList();

            return WriteFileToConsole(header, obj);
        }

        /// <summary>
        /// Filter tsv file data using Project Id
        /// </summary>
        /// <param name="arguement"> project id</param>
        /// <param name="filedata">tsv file record</param>
        /// <returns>Filter record with Project Id</returns>

        public static string GetDataByProjectId(string[] arguement, IEnumerable<string> filedata)
        {
            string result = string.Empty;
            if (arguement.Length > 1 && arguement[1] != null)
            {
                List<string> header = ReadFileHeader(filedata);// skip empty lines if any

                List<DataReadModel> obj = filedata
                   .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"))// skip empty lines if any
                   .Select(line => line.Split('\t'))
                   .Skip(1)
                   .Select(line => new DataReadModel
                   {
                       Project = Convert.ToInt32(line[0]),
                       Description = line[1],
                       StartDate = line[2],
                       Category = line[3],
                       Responsible = line[4],
                       SavingsAmount = line[5],
                       Currency = line[6],
                       Complexity = line[7]
                   }).Where(x => x.Project == Convert.ToInt32(arguement[1])).ToList();

                result = WriteFileToConsole(header, obj);
            }
            else
                result = "Please enter Project Id";

            return result;
        }

        /// <summary>
        /// Append new record to File
        /// </summary>

        public static void CreateNew()
        {
            string filePath = ReadFilePath();

            List<DataReadModel> dataReadModels = ReadDatatsvFile(filePath, "#");

            DataReadModel d = new DataReadModel();
            Console.WriteLine("Would you like to enter additional data?");

            Console.WriteLine("Please enter the Project ID: ");
            d.Project = ValidateProjectId();

            Console.WriteLine("Please enter the Description: ");
            d.Description = ValidateRequiredField(Console.ReadLine(), "Description");

            Console.WriteLine("Please enter the Start date (yyyy-MM-dd HH:mm:ss.fff): ");
            d.StartDate = ValidateStartDate();

            Console.WriteLine("Please enter the Category: ");
            d.Category = ValidateRequiredField(Console.ReadLine(), "Category");

            Console.WriteLine("Please enter the Responsible: ");
            d.Responsible = ValidateRequiredField(Console.ReadLine(), "Responsible");

            //Saving Amount Validation
            Console.WriteLine("Please enter the Savings amount: ");
            d.SavingsAmount = ValidateSavingsAmount();

            Console.WriteLine("Please enter the Currency: ");
            d.Currency = Console.ReadLine();
            if (String.IsNullOrEmpty(d.Currency))
                d.Currency = null;

            Console.WriteLine("Please enter the Complexity: ");
            d.Complexity = ValidateComplexity();

            CultureInfo cultureInfo = new CultureInfo("en-EN");
            CsvConfiguration config = new CsvConfiguration(cultureInfo);
            config.Delimiter = "\t";
            dataReadModels.Add(d);
          
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(dataReadModels);
            }
           
            Console.WriteLine("Record Inserted Successfully \n");
        }

        /// <summary>
        /// Read File Path from app Config
        /// </summary>
        /// <returns>Return File path</returns>
        private static string ReadFilePath()
        {
            return ConfigurationManager.AppSettings["filePath"];
        }

        /// <summary>
        /// Another way to read tsv file 
        /// </summary>
        /// <param name="file">file path</param>
        /// <param name="delimiter">to ignore comment lines</param>
        /// <returns>List of record from tsv file</returns>
        private static List<DataReadModel> ReadDatatsvFile(string file, string delimiter)
        {

            List<DataReadModel> records = new List<DataReadModel>();
            CultureInfo cultureInfo = new CultureInfo("en-EN");
            CsvConfiguration config = new CsvConfiguration(cultureInfo);
            config.Delimiter = "\t";
            config.ShouldSkipRecord = (x) => x[0].StartsWith(delimiter);
            config.IgnoreBlankLines = true;
            //FileStream fs = new FileStream(file, FileMode.Open);
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<DataReadModel>().ToList();
            }
            //reader.Close();
            //fs.Close();

            return records;

        }

        /// <summary>
        /// Read header of tsv file
        /// </summary>
        /// <param name="filedata">tsv record</param>
        /// <returns>Header of tsv file</returns>
        private static List<string> ReadFileHeader(IEnumerable<string> filedata)
        {
            return filedata
                  .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#")).Select(line => line.Split('\t')).First().ToList();
        }

        /// <summary>
        /// Print data on console
        /// </summary>
        /// <param name="header">Header of tsv file</param>
        /// <param name="data">record to print</param>
        /// <returns>empty if success or error message</returns>
        private static string WriteFileToConsole(List<string> header, List<DataReadModel> data)
        {

            if (data != null && header != null)
            {
                header.ToList().ForEach(i => Console.Write("{0}\t", i));
                data.ForEach(i => Console.WriteLine("{0}\t {1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", i.Project, i.Description, i.StartDate, i.Category, i.Responsible, i.SavingsAmount, i.Currency, i.Complexity));
                return string.Empty;
            }
            else
                return "No record found";
        }

        /// <summary>
        /// Open command prompt to exceute command line arguement
        /// </summary>
        /// <param name="cmdCheck">Arguement  is only to check if the call is from Visual editor or command prompt</param>
        /// <returns>Check Command prompt thread and return Boolean</returns>
        private static bool CommandLineArguement(bool cmdCheck)
        {

            if (cmdCheck)
            {
                if (Process.GetProcessesByName("cmd").Count() > 1)
                {
                    Console.WriteLine("Allready running");
                    return true;
                }
                ProcessStartInfo pi = new ProcessStartInfo("cmd.exe");
                pi.CreateNoWindow = true;
                pi.WorkingDirectory = Directory.GetCurrentDirectory();

                Process p = new Process();
                p.StartInfo = pi;
                p.Start();
                return true;
            }
            else
                return false;

        }

        #region Validation

        private static int ValidateProjectId()
        {
            int projectId;
            bool projectConverted = false;

            do
            {
                string project = Console.ReadLine();
                projectConverted = Int32.TryParse(project, out projectId);

                if (!projectConverted)
                    Console.WriteLine("Please enter integer");
            }
            while (!projectConverted);
            return projectId;
        }

        private static string  ValidateStartDate()
        {
            DateTime startDate;
            bool dateConverted = false;

            do
            {
                string StartDate = Console.ReadLine();
                dateConverted = DateTime.TryParseExact(StartDate, "yyyy-MM-dd HH:mm:ss.fff", null, DateTimeStyles.None, out startDate);

                if (!dateConverted)
                    Console.WriteLine("Invalid date");
            }
            while (!dateConverted);
            return startDate.ToString();
        }

        private static string ValidateSavingsAmount()
        {
            decimal amt;
            bool amtConverted = false;
            do
            {
                string savingsAmount = Console.ReadLine();
                if (String.IsNullOrEmpty(savingsAmount))
                {
                    savingsAmount = null;
                    return savingsAmount;
                }
                else
                {
                    amtConverted = Decimal.TryParse(savingsAmount, out amt);
                    if (!amtConverted)
                        Console.WriteLine("Invalid Saving Amount");
                }
            }
            while (!amtConverted);
            return amt.ToString();
        }

        private static string ValidateComplexity()
        {
            //Complexity Vaidation
            string complexity;
           
            List<string> itemValues = System.Enum.GetNames(typeof(EnumComplexity)).ToList(); //Maintained Enum to add and modify Complexity
            foreach (var i in itemValues)
            {
                Console.WriteLine(i);
            }
            bool complexityAvailable = false;
            do
            {
                 complexity = Console.ReadLine();
                complexityAvailable = Enum.IsDefined(typeof(EnumComplexity), complexity.ToUpper());
                if (!complexityAvailable)
                    Console.WriteLine("Invalid Complexity");
            }
            while (!complexityAvailable);
            return complexity;
        }

        private static string ValidateRequiredField(string value,string message)
        {
           //string readValue= Console.ReadLine();

            while (string.IsNullOrEmpty(value))
            {
                Console.WriteLine(message +" is required");
                value = Console.ReadLine();
            }
            return value;

        }
        #endregion
    }
}
