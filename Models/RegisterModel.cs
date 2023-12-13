using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAssignment.Models
{
    public class RegisterModel
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string decryptedPassword { get; set; }
        public string encryptedPassword { get; set; }
    }
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        

    }
    public class OrderModel
    {
        public int OrderId { get; set; }
        public int productName { get; set; }
        public DateTime OrderDate { get; set; }
        public Boolean OrderStatus { get; set; }
        public int name { get; set; }
        public int address { get; set; }
        public string userId { get; set; }

    }
    public class ContryModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

    }
    public class StateModel
    {
        public int StateId { get; set; }
        public string StateName { get; set; }

    }
    public class CityModel
    {
        public int CityId { get; set; }
        public string CityName { get; set; }

    }
    public class getOrder
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public Boolean OrderStatus { get; set; }
        public string name { get; set; }
        public string address { get; set; }

    }
}
