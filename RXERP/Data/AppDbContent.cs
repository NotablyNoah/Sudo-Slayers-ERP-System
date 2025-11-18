using Microsoft.EntityFrameworkCore;
using RXERP.Data;
using RXERP.Pages;

namespace RXERP.Data // namespace declaration to match file path
{
    /// <summary>
    /// Database context class for the application.
    /// </summary>
    public class AppDbContent : DbContext
    {
        /// <summary>
        /// Constructor that accepts DbContextOptions.
        /// </summary>
        /// <param name="options"></param>
        public AppDbContent(DbContextOptions<AppDbContent> options) : base(options)
        {

        }

        // DbSet properties for your entities can be added here
        // the second usercredential refers to the UserCredential class in UserCredentials.cs
        public DbSet<UserCredentials> UserCredentials { get; set; }
        //public DbSet<Employee> Employees { get; set; }    
        //for FUTURE tableset, will need another Employee.cs page in the data folder

        public DbSet<VendorData> VendorData { get; set; }


        public DbSet<EmployeeData> EmployeeData { get; set; }

        //Financial Data Tables
        public DbSet<FinancialDataInvoice> FinanceDataInvoices { get; set; }
        public DbSet<FinancialDataInvoiceLine> FinanceDataInvoiceLines { get; set; }
        public DbSet<FinancialDataJournal> FinancialDataJournals { get; set; }
        public DbSet<FinancialDataJournalLine> FinancialDataJournalLines { get; set; }
        public DbSet<FinancialDataPayment> FinancialDataPayments { get; set; }
    }
}
