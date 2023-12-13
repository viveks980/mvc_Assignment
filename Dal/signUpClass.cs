using MvcAssignment.Interface;
using MvcAssignment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;

namespace MvcAssignment.Dal 
{
    public class signUpClass : signUpInterface
    {
        private readonly string connection = Startup.StaticConfiguration["customData:Connectionstring"];
        public ResponseModel adduser(RegisterModel objmodel)

        {

            ResponseModel result = new ResponseModel();

            RegisterModel User = new RegisterModel();
            MailMessage msg = new MailMessage();

            SmtpClient smtp = new SmtpClient();

            using (SqlConnection con = new SqlConnection(connection))

            {

                try

                {

                    con.Open();




                    using (SqlCommand checkUsernameCmd = new SqlCommand("SELECT COUNT(*) FROM SignUpTable WHERE userid = @userid", con))

                    {

                        checkUsernameCmd.Parameters.AddWithValue("@userid", objmodel.userId);

                        int existingUserCount = (int)checkUsernameCmd.ExecuteScalar();

                        if (existingUserCount > 0)

                        {

                            result.status = false;

                            result.message = "Userid already exists. Please choose a different userid.";

                        }

                        else

                        {


                            using (SqlCommand cmd = new SqlCommand("SignUpUser", con))

                            {

                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@userId", objmodel.userId);
                                cmd.Parameters.AddWithValue("@name", objmodel.name);
                                cmd.Parameters.AddWithValue("@address", objmodel.address);

                                string encryptedPassword = EncryptPassword(objmodel.decryptedPassword);

                                cmd.Parameters.AddWithValue("@encryptedPassword", encryptedPassword);
                                cmd.Parameters.AddWithValue("@decryptedPassword", objmodel.decryptedPassword);
                                var id = cmd.ExecuteNonQuery();


                                if (id > 0)
                                {
                                    result.status = true;
                                    msg.From = new MailAddress("vivek.swami@cylsys.com");
                                    msg.To.Add(objmodel.userId);
                                    msg.Subject = "Testing Mail For Register User..!!";
                                    msg.Body = "Dear " + objmodel.name + " Your Registration is successfull for Online Shopping..!!";
                                    msg.IsBodyHtml = true;
                                    smtp.Host = "smtp-mail.outlook.com";
                                    smtp.Port = 587;
                                    smtp.EnableSsl = true;
                                    smtp.UseDefaultCredentials = false;
                                    smtp.Credentials = new NetworkCredential("vivek.swami@cylsys.com", "Cylsys@2");
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    smtp.Send(msg);
                                    result.message = "Data Saved Successfully";
                                }
                                else
                                {
                                    result.status = false;
                                   result.message = "Please Check...Something Went wrong...!!!";
                               }
                           }
                        }
                    }
                }

                catch (Exception ex)

                {
                   result.status = false;
                    Helper.WriteLog("The error is:" + ex);
                  
                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

            }

            return result;



        }
        private string EncryptPassword(string password)

        {

            string encryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            byte[] clearBytes = Encoding.Unicode.GetBytes(password);

            using (Aes encryptor = Aes.Create())

            {

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {

            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76

        });

                encryptor.Key = pdb.GetBytes(32);

                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())

                {

                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))

                    {

                        cs.Write(clearBytes, 0, clearBytes.Length);

                        cs.Close();

                    }

                    return Convert.ToBase64String(ms.ToArray());

                }

            }

        }
        public ResponseModel loginUser(RegisterModel objmodel)

        {

            ResponseModel result = new ResponseModel();

            using (SqlConnection con = new SqlConnection(connection))

            {

                try

                {

                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("Validate_Login", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@userId", objmodel.userId);

                        cmd.Parameters.AddWithValue("@decryptedPassword", objmodel.decryptedPassword);

                        var reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            result.status = true;

                            result.message = "Login successful.";

                        }

                        else

                        {

                            result.status = false;

                            result.message = "Invalid email or password.";

                        }

                    }

                }

                catch (Exception ex)

                {

                    result.status = false;

                    result.message = "An error occurred. Please try again later.";

                    Helper.WriteLog("The error is:" + ex);

                   

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

            }

            return result;

        }
        public List<ProductModel> getProductDetails()

        {

            List<ProductModel> res = new List<ProductModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("GetProductList", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                ProductModel u = new ProductModel();

                                u.ProductId = (int)reader["ProductId"];

                                u.ProductName = (string)reader["ProductName"];

                               // u.ProductPrice = (string)reader["ProductPrice"];

                                u.ProductPrice = reader["ProductPrice"]?.ToString()?.Trim() != "" ? decimal.Parse(reader["ProductPrice"].ToString()) : 0m;
                                res.Add(u);
                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);
                    Helper.WriteLog("The error is:" + ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public ProductModel addProduct(int ProcuctId)

        {

            ResponseModel res = new ResponseModel();

            ProductModel User = new ProductModel();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("get_ProductById", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ProductId", ProcuctId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                User.ProductId = string.IsNullOrEmpty(reader["ProductId"].ToString()) ? 0 : Convert.ToInt32(reader["ProductId"]);


                                User.ProductName = string.IsNullOrWhiteSpace(reader["ProductName"].ToString()) ? "" : reader["ProductName"].ToString();

                               // User.ProductPrice = string.IsNullOrWhiteSpace(reader["Role_designation"].ToString()) ? "" : reader["Role_designation"].ToString();
                                User.ProductPrice = decimal.TryParse(reader["ProductPrice"].ToString(), out decimal parsedPrice) ? parsedPrice : 0.0m;
                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    res.status = false;

                    res.message = "Errorr!!!";

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }


            }

            return User;

        }
        public List<RegisterModel> getUserName()

        {

            List<RegisterModel> res = new List<RegisterModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("GetNameList", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        SqlDataReader reader = cmd.ExecuteReader();


                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                RegisterModel u = new RegisterModel();

                                u.id = string.IsNullOrEmpty(reader["id"].ToString()) ? 0 : Convert.ToInt32(reader["id"]);

                                u.name = string.IsNullOrWhiteSpace(reader["name"].ToString()) ? "" : reader["name"].ToString();
                                

                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public ResponseModel addorder(OrderModel objmodel)

        {
            ResponseModel result = new ResponseModel();
            OrderModel User = new OrderModel();
            MailMessage msg = new MailMessage();

            SmtpClient smtp = new SmtpClient();
            using (SqlConnection con = new SqlConnection(connection))
            {
                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("save_Order", con))

                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.AddWithValue("@ProductName", objmodel.productName);
                        cmd.Parameters.AddWithValue("@name", objmodel.name);
                        cmd.Parameters.AddWithValue("@address", objmodel.address);
                        cmd.Parameters.AddWithValue("@OrderStatus", objmodel.OrderStatus);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        var id = cmd.ExecuteNonQuery();
                        if (id > 0)
                        {
                            result.status = true;
                            result.message = "Order Confirm Successfully";
                        }
                        else
                        {
                            result.status = false;
                            result.message = "Please Check...Something Went wrong...!!!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.status = false;
                    Helper.WriteLog("The error is:" + ex);
                    Console.WriteLine("Error is:" + ex);
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return result;
        }

        public List<ContryModel> getCountryList()

        {

            List<ContryModel> res = new List<ContryModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("select  CountryId ,CountryName from   Countries", con))

                    {

                        cmd.CommandType = CommandType.Text;


                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                ContryModel u = new ContryModel();

                                u.CountryId = string.IsNullOrEmpty(reader["CountryId"].ToString()) ? 0 : Convert.ToInt32(reader["CountryId"]);

                                u.CountryName = string.IsNullOrWhiteSpace(reader["CountryName"].ToString()) ? "" : reader["CountryName"].ToString();

                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public List<StateModel> getStateList(int id)

        {

            List<StateModel> res = new List<StateModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("GetStatesByCountry", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CountryId", id);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                StateModel u = new StateModel();

                                u.StateId = string.IsNullOrEmpty(reader["StateId"].ToString()) ? 0 : Convert.ToInt32(reader["StateId"]);

                                u.StateName = string.IsNullOrWhiteSpace(reader["StateName"].ToString()) ? "" : reader["StateName"].ToString();

                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public List<CityModel> getCityList(int id)

        {

            List<CityModel> res = new List<CityModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("GetcityByState", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StateId", id);


                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                CityModel u = new CityModel();

                                u.CityId = string.IsNullOrEmpty(reader["CityId"].ToString()) ? 0 : Convert.ToInt32(reader["CityId"]);

                                u.CityName = string.IsNullOrWhiteSpace(reader["CityName"].ToString()) ? "" : reader["CityName"].ToString();

                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public List<RegisterModel> getEmail(int id)

        {

            List<RegisterModel> res = new List<RegisterModel>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("GetEmail", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader reader = cmd.ExecuteReader();


                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {

                                RegisterModel u = new RegisterModel();

                                u.id = string.IsNullOrEmpty(reader["id"].ToString()) ? 0 : Convert.ToInt32(reader["id"]);

                                u.userId = string.IsNullOrWhiteSpace(reader["userId"].ToString()) ? "" : reader["userId"].ToString();


                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public List<getOrder> getOrderDetails()

        {

            List<getOrder> res = new List<getOrder>();

            using (SqlConnection con = new SqlConnection(connection))

            {

                con.Open();

                try

                {

                    using (SqlCommand cmd = new SqlCommand("getOrderData", con))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)

                        {

                            while (reader.Read())

                            {
                                getOrder u = new getOrder();
                                u.OrderId = (int)reader["OrderId"];
                                u.OrderDate = (DateTime)reader["OrderDate"];
                                u.OrderStatus = (Boolean)reader["OrderStatus"];
                                u.name = (string)reader["name"];
                                u.address = (string)reader["CityName"];
                                u.ProductName = (string)reader["ProductName"];
                                res.Add(u);


                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex);

                }

                finally

                {

                    con.Close();

                    con.Dispose();

                }

                return res;

            }


        }
        public ResponseModel deleteOrder(int OrderId)

        {
            ResponseModel result = new ResponseModel();

            using (SqlConnection con = new SqlConnection(connection))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("deleteOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", OrderId);

                        var Id = cmd.ExecuteNonQuery();

                        if (Id > 0)
                        {
                            result.status = true;
                            result.message = "Order Cancelled Successfully..!!";
                          


                        }
                        else
                        {
                            result.status = false;
                            result.message = "Please Check...Something Went wrong...!!!";

                        }
                    }

                }
                catch (Exception ex)
                {
                    result.status = false;
                    result.message = "Exception Occure..!!";

                }
                finally
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return result;

        }
        
    }
}
