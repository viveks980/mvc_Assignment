using MvcAssignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAssignment.Interface
{
    interface signUpInterface
    {
        ResponseModel adduser(RegisterModel objmodel);
        ResponseModel loginUser(RegisterModel objmodel);
        List<ProductModel> getProductDetails();
        ProductModel addProduct(int ProductId);
        List<RegisterModel> getUserName();
        ResponseModel addorder(OrderModel objmodel);
        List<ContryModel> getCountryList();
        public List<StateModel> getStateList(int id);
        public List<CityModel> getCityList(int id);
        public List<RegisterModel> getEmail(int id);
        List<getOrder> getOrderDetails();
        ResponseModel deleteOrder(int OrderId);
    }
}
