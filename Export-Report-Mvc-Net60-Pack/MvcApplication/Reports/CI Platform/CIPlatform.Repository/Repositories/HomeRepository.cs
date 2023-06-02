//using CIPlatform.Entities.Data;
//using CIPlatform.Entities.Models;
//using CIPlatform.Repository.Interface;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CIPlatform.Repository.Repositories
//{
//    public class HomeRepository : IHomeRepository
//    {
//        public readonly CIPlatformDbContext _CIPlatformDbContext;


//        public HomeRepository(CIPlatformDbContext cIPlatformDbContext)
//        {
//            _CIPlatformDbContext = cIPlatformDbContext; 
//        }

//        public List<User> GetuserData()
//        {
//            List<User> users = _CIPlatformDbContext.Users.ToList();
//            return users;
//        }

//        public List<User> GetUserData()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
