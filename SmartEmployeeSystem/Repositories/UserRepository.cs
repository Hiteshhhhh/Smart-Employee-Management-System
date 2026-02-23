

using Microsoft.AspNetCore.Identity;
using Npgsql;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conn;
        private readonly NpgsqlConnection connection;
        private readonly IHttpContextAccessor access;

        public UserRepository(IConfiguration config, IHttpContextAccessor accessor)
        {
            _conn = config.GetConnectionString("DefaultConnection");
            connection = new NpgsqlConnection(_conn);
            access = accessor;
        }

        public void AddUser(UserModel user)
        {
            try
            {
                var hasher = new PasswordHasher<UserModel>();
                user.password_hash = hasher.HashPassword(user,user.password_hash);
                connection.Open();
                string query = @"insert into users(username,password_hash,email,role,is_active,created_at) values (@u,@p,@e,'Employee',true,@c)";
                var cmd = new NpgsqlCommand(query,connection);
                cmd.Parameters.AddWithValue("@u",user.username);
                cmd.Parameters.AddWithValue("@p",user.password_hash);
                cmd.Parameters.AddWithValue("@e",user.email);
                cmd.Parameters.AddWithValue("@c",DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        public bool IsEmailExist(string email)
        {
            try
            {
                connection.Open();
                string query = @"select * from users where email = @e";
                var cmd = new NpgsqlCommand(query,connection);
                cmd.Parameters.AddWithValue("@e",email);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

       public bool Login(UserModel user)
        {
            try
            {
                connection.Open();
                string query = @"SELECT user_id, username, email, password_hash, role 
                                FROM public.users 
                                WHERE email = @e AND is_active = true";
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@e", user.email);
                var rows = cmd.ExecuteReader();
                if (rows.Read())
                {
                    if (VerifyPassword(rows["password_hash"].ToString(), user.password_hash))
                    {
                        access.HttpContext.Session.SetInt32("userid", rows.GetInt32(0));
                        access.HttpContext.Session.SetString("username", rows.GetString(1));
                        access.HttpContext.Session.SetString("useremail", rows.GetString(2));
                        access.HttpContext.Session.SetString("userrole", rows.GetString(4));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        public bool VerifyPassword(string storedHash, string providedPassword)
        {
            var passwordHasher = new PasswordHasher<UserModel>();
            var result = passwordHasher.VerifyHashedPassword(null, storedHash, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}