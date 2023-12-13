using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcAssignment.Dal;
using MvcAssignment.Interface;
using MvcAssignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAssignment.Controllers
{
    public class UserController : Controller
    {
        signUpInterface ui = new signUpClass();
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult registerUser(RegisterModel objmodel)

        {
            ResponseModel result = new ResponseModel();

            result = ui.adduser(objmodel);

            return Json(result);

        }
        public IActionResult loginUser(RegisterModel objmodel)

        {
            ResponseModel result = new ResponseModel();

            result = ui.loginUser(objmodel);
            if (result.status == true)
            {
                return RedirectToAction("GetProductList", "User");
            }
            TempData["errormg"] = "invalid user & password";
            return View();

            //return Json(result);
        }

        public IActionResult GetProductList()

        {
            List<ProductModel> result = ui.getProductDetails();
            return View(result);
        }
        //public IActionResult addProduct(int ProductId)
        //{
        //   // result = ui.addProduct(ProductId);

        //    var user = ui.getUserName();

        //    ViewBag.UserName = new SelectList(user, "id", "name");

        //    return View(ui.addProduct(ProductId));

        //}

        public IActionResult addProduct(int ProductId,int id)
        {
            var user = ui.getUserName();
            ViewBag.UserName = new SelectList(user, "id", "name");

            var email = ui.getEmail(id);
            ViewBag.roleDes3 = new SelectList(email, "userId", "userId");

            var Country = ui.getCountryList();
            ViewBag.roleDes = new SelectList(Country, "CountryId", "CountryName");

            var State = ui.getStateList(id);
            ViewBag.roleDes1 = new SelectList(State, "StateId", "StateName");

            var City = ui.getCityList(id);
            ViewBag.roleDes2 = new SelectList(City, "CityId", "CityName");

            return View(ui.addProduct(ProductId));

        }
        public JsonResult saveOrder(OrderModel objmodel)

        {
            ResponseModel result = new ResponseModel();

            result = ui.addorder(objmodel);

            return Json(result);

        }
        public IActionResult login()
        {
            return View();
        }
      


        [HttpPost]

        public JsonResult statefilter(int id)

        {

            var State = ui.getStateList(id);

            return Json(State);

        }

        [HttpPost]
        public JsonResult cityfilter(int id)

        {

            var City = ui.getCityList(id);

            return Json(City);

        }
        public IActionResult GetOrderList()

        {
            List<getOrder> result = ui.getOrderDetails();
            return View(result);
        }
        public JsonResult deleteOrder(int OrderId)
        {
            return Json(ui.deleteOrder(OrderId));
        }
    }
}
