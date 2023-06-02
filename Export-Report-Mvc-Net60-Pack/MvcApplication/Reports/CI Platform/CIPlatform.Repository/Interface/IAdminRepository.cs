using CIPlatform.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Repository.Interface
{
    public interface IAdminRepository
    {
        AdminViewModel getAdminDetails();
        AdminViewModel getSearchAdmin(string? search, int pg);
        void addViaAdmin(MissionViewModel userData, string type, long Id);
        AdminViewModel getViaAdmin(long Id, string type);
        bool checkSameData(string Email, long sortOrder, long bannerId);
    }
}
