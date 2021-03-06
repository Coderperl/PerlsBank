using BankStartWeb.Data;
using BankStartWeb.Infrastructure.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{   
    [Authorize(Roles = "Admin,Cashier")]
    public class CustomersModel : PageModel
    {
        public class CustomersViewModel
        {
            public int Id { get; set; }
            public string Givenname { get; set; }
            public string Surname { get; set; }
            public string Streetaddress { get; set; }
            public string City { get; set; }
            public string NationalId { get; set; }
            public string EmailAddress { get; set; }
            
        }

        private readonly ApplicationDbContext context;
        public List<CustomersViewModel> Customers = new List<CustomersViewModel>();
        public string Searchterm { get; set; }
        public int PageNo { get; set; }
        public string SortOrder { get; set; }
        public string SortCol { get; set; }
        public int TotalPageCount { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        public CustomersModel(ApplicationDbContext _context)
        {
            context = _context;
        }
        public void OnGet(string searchterm, string col="id", string order ="asc", int pageno = 1)
        {

            Searchterm = searchterm;
            PageNo = pageno;
            SortOrder = order;
            SortCol = col;
            
            var cust = context.Customers.AsQueryable();
            
            if (!string.IsNullOrEmpty(Searchterm))
                cust = cust.Where(word => word.Givenname.Contains(Searchterm)
                || word.Surname.Contains(Searchterm)
                || word.City.Contains(Searchterm));

            cust = cust.OrderBy(col, order == "asc" ? ExtensionMethods.QuerySortOrder.Asc
                : ExtensionMethods.QuerySortOrder.Desc);

            var pagedResult = cust.GetPaged(PageNo, 20);
            TotalPageCount = pagedResult.PageCount;

            Customers = pagedResult.Results.Select(c => new CustomersViewModel
            {
                Id = c.Id,
                NationalId = c.NationalId,
                Givenname = c.Givenname,
                Surname = c.Surname,
                EmailAddress = c.EmailAddress,
                Streetaddress = c.Streetaddress,
                City = c.City,
                
            }).ToList();
        }

    }
}
