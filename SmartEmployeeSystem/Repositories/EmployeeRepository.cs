
using Npgsql;
using SmartEmployeeSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace SmartEmployeeSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly NpgsqlConnection _connection;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public List<EmployeeModel> GetEmployees()
        {
            List<EmployeeModel> employeesList = new List<EmployeeModel>();
            try
            {
                _connection.Open();
                string query = @"select e.*, d.department_name 
                                    from employees e
                                    join departments d
                                    on e.department_id = d.department_id";
                var cmd = new NpgsqlCommand(query, _connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employeesList.Add(new EmployeeModel
                    {
                        employee_id = reader.GetInt32(0),
                        user_id = reader.GetInt32(1),
                        department_id = reader.GetInt32(2),
                        employee_code = reader.GetString(3),
                        first_name = reader.GetString(4),
                        last_name = reader.GetString(5),
                        designation = reader.GetString(6),
                        base_salary = reader.GetDecimal(7),
                        date_of_joining = reader.GetDateTime(8),
                        department_name = reader.GetString(9)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return employeesList;
        }

        public EmployeeModel? GetEmployeeModelById(int id)
        {
            EmployeeModel? employee = null;
            try
            {
                _connection.Open();
                string query = @"select e.*, d.department_name 
                                    from employees e
                                    join departments d
                                    on e.department_id = d.department_id
                                    where e.employee_id = @id";
                var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    employee = new EmployeeModel
                    {
                        employee_id = reader.GetInt32(0),
                        user_id = reader.GetInt32(1),
                        department_id = reader.GetInt32(2),
                        employee_code = reader.GetString(3),
                        first_name = reader.GetString(4),
                        last_name = reader.GetString(5),
                        designation = reader.GetString(6),
                        base_salary = reader.GetDecimal(7),
                        date_of_joining = reader.GetDateTime(8),
                        department_name = reader.GetString(9)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return employee;
        }

        public EmployeeModel? GetEmployeeByUserId(int userId)
        {
            EmployeeModel? employee = null;
            try
            {
                _connection.Open();
                string query = @"select e.*, d.department_name
                                from employees e
                                join departments d
                                on e.department_id = d.department_id
                                where e.user_id = @uID";
                var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@uID", userId);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    employee = new EmployeeModel
                    {
                        employee_id = reader.GetInt32(0),
                        user_id = reader.GetInt32(1),
                        department_id = reader.GetInt32(2),
                        employee_code = reader.GetString(3),
                        first_name = reader.GetString(4),
                        last_name = reader.GetString(5),
                        designation = reader.GetString(6),
                        base_salary = reader.GetDecimal(7),
                        date_of_joining = reader.GetDateTime(8),
                        department_name = reader.GetString(9)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return employee;
        }
        public void AddEmployee(EmployeeModel employee)
        {
            try
            {
                _connection.Open();

                var hasher = new PasswordHasher<string>();
                string defaultPassword = "EMP@" + employee.employee_code;
                string hashedPassword = hasher.HashPassword(string.Empty, defaultPassword);

                string userQuery = @"INSERT INTO users 
                            (username, password_hash, email, role, is_active, created_at)
                            VALUES (@u, @p, @e, 'Employee', true, @c)
                            RETURNING user_id";
                var userCmd = new NpgsqlCommand(userQuery, _connection);
                userCmd.Parameters.AddWithValue("@u", employee.username ?? string.Empty);
                userCmd.Parameters.AddWithValue("@p", hashedPassword);
                userCmd.Parameters.AddWithValue("@e", employee.email ?? string.Empty);
                userCmd.Parameters.AddWithValue("@c", DateTime.Now);

                object? newUserIdObj = userCmd.ExecuteScalar();
                if (newUserIdObj is null)
                {
                    throw new InvalidOperationException("Failed to create user record.");
                }
                int newUserId = Convert.ToInt32(newUserIdObj);

                string empQuery = @"INSERT INTO employees 
                           (user_id, department_id, employee_code,
                            first_name, last_name, designation,
                            base_salary, date_of_joining)
                           VALUES (@u, @d, @ec, @fn, @ln, @des, @bs, @doj)";
                var empCmd = new NpgsqlCommand(empQuery, _connection);
                empCmd.Parameters.AddWithValue("@u", newUserId);
                empCmd.Parameters.AddWithValue("@d", employee.department_id);
                empCmd.Parameters.AddWithValue("@ec", employee.employee_code ?? string.Empty);
                empCmd.Parameters.AddWithValue("@fn", employee.first_name ?? string.Empty);
                empCmd.Parameters.AddWithValue("@ln", employee.last_name ?? string.Empty);
                empCmd.Parameters.AddWithValue("@des", employee.designation ?? string.Empty);
                empCmd.Parameters.AddWithValue("@bs", employee.base_salary);
                empCmd.Parameters.AddWithValue("@doj", employee.date_of_joining);
                empCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void UpdateEmployee(EmployeeModel employee)
        {
            try
            {
                _connection.Open();
                string query = @"update employees 
                                    set department_id = @d,
                                    employee_code   = @ec,
                                    first_name      = @fn,
                                    last_name       = @ln,
                                    designation     = @des,
                                    base_salary     = @bs,
                                    date_of_joining = @doj
                                    where employee_id = @id";

                var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@d", employee.department_id);
                cmd.Parameters.AddWithValue("@ec", employee.employee_code ?? string.Empty);
                cmd.Parameters.AddWithValue("@fn", employee.first_name ?? string.Empty);
                cmd.Parameters.AddWithValue("@ln", employee.last_name ?? string.Empty);
                cmd.Parameters.AddWithValue("@des", employee.designation ?? string.Empty);
                cmd.Parameters.AddWithValue("@bs", employee.base_salary);
                cmd.Parameters.AddWithValue("@doj", employee.date_of_joining);
                cmd.Parameters.AddWithValue("@id", employee.employee_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
        public void DeleteEmployee(int id)
        {
            try
            {
                _connection.Open();
                string query = @"DELETE FROM employees 
                                WHERE employee_id = @id";
                var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}