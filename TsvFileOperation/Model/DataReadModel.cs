using CsvHelper.Configuration.Attributes;


namespace TsvFileOperation.Model
{
    public class DataReadModel
    {

        #region Entity
        private int m_Project;
        public int Project
        {
            get { return m_Project; }
            set { m_Project = value; }
        }

        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private string m_StartDate;

        [Name("Start date")]
        public string StartDate
        {
            get { return m_StartDate; }
            set { m_StartDate = value; }
        }

        private string m_Category;
        public string Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }

        private string m_Resposible;
        public string Responsible
        {
            get { return m_Resposible; }
            set { m_Resposible = value; }
        }

        [Name("Savings amount")]
        public string SavingsAmount { get; set; }

        public string Currency { get; set; }

        private string m_Complexity;

        public string Complexity
        {
            get { return m_Complexity; }
            set { m_Complexity = value; }
        }

        #endregion

    }

    enum EnumComplexity
    {
        SIMPLE,
        MODERATE,
        HAZARDOUS
    }

}

