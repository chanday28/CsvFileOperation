using Microsoft.VisualStudio.TestTools.UnitTesting;
using SievoAssignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SievoAssignment.Tests
{
    [TestClass()]
    public class ProgramTests
    {

        [TestMethod()]
        public void GetFullFilePathValidTest()
        {
            Environment.CurrentDirectory = "C:/Chanda/Personal/Project/Assignment/Assignment/bin/Debug/";
            string[] filepath = { "file", "./File/ExampleDate.tsv" };
            Assert.IsNotNull(Program.GetFullFilePath(filepath));
        }

        [TestMethod()]
        public void GetFullFilePathWithoutExtensionTest()
        {
            Environment.CurrentDirectory = "C:/Chanda/Personal/Project/Assignment/Assignment/bin/Debug/";
            string[] filepath = { "file", "./File/ExampleData" };
            Assert.IsTrue(Program.GetFullFilePath(filepath).Contains("Please Enter valid file name with extension"));
        }

        [TestMethod()]
        public void GetFullFilePathValidArguementTest()
        {
            Environment.CurrentDirectory = "C:/Chanda/Personal/Project/Assignment/Assignment/bin/Debug/";
            string[] filepath = { "file" };
            Assert.IsTrue(Program.GetFullFilePath(filepath).Contains("Please enter valid path"));
        }


        [TestMethod()]
        public void SortByStartDateTest()
        {
            IEnumerable<string> mockfiledata = new List<string> { "Project	Description	Start date	Category	Responsible	Savings amount	Currency	Complexity", "2	Harmonize Lactobacillus acidophilus sourcing	2014-01-01 00:00:00.000	Dairy	Daisy Milks	NULL	NULL	Simple", "3	Substitute Crème fraîche with evaporated milk in ice-cream products	2013-01-01 00:00:00.000	Dairy	Daisy Milks	141415.942696	EUR	Moderate" };

            Assert.AreEqual(Program.SortByStartDate(mockfiledata), String.Empty);
        }

        [TestMethod()]
        public void GetDataByProjectIdTest()
        {
            string[] filepath = { "project", "2" };
            IEnumerable<string> mockfiledata = new List<string> { "Project	Description	Start date	Category	Responsible	Savings amount	Currency	Complexity", "2	Harmonize Lactobacillus acidophilus sourcing	2014-01-01 00:00:00.000	Dairy	Daisy Milks	NULL	NULL	Simple", "3	Substitute Crème fraîche with evaporated milk in ice-cream products	2013-01-01 00:00:00.000	Dairy	Daisy Milks	141415.942696	EUR	Moderate" };

            Assert.AreEqual(Program.GetDataByProjectId(filepath, mockfiledata), String.Empty);
        }

        [TestMethod()]
        public void GetDataByProjectIdNoRecordTest()
        {
            string[] filepath = { "project" };
            IEnumerable<string> mockfiledata = new List<string> { "Project	Description	Start date	Category	Responsible	Savings amount	Currency	Complexity", "2	Harmonize Lactobacillus acidophilus sourcing	2014-01-01 00:00:00.000	Dairy	Daisy Milks	NULL	NULL	Simple" };

            Assert.IsTrue(Program.GetDataByProjectId(filepath, mockfiledata).Contains("Please enter Project Id"));
        }

    }
}