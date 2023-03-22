using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataRooms.UI.Models;

namespace DataRooms.UI.Areas.Company.Model
{
    public class CustomCompanyModel : PagingModel
    {
        public IPagedList<DataRooms.UI.Models.Company> PagedCompanies { get; set; }
        public DataRooms.UI.Models.Company CompanyDetails { get; set; }
        public DataRooms.UI.Models.ADInfo ADDetails { get; set; }
    }
}