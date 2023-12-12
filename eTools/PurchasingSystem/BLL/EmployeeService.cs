#nullable disable
using PurchasingSystem.DAL;
using PurchasingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.BLL
{
    public class EmployeeService
    {
        #region Fields
        private readonly PurchasingContext _purchasingContext;
        #endregion
        internal EmployeeService(PurchasingContext purchasingContext)
        {
            _purchasingContext = purchasingContext;
        }

        public EmployeeView GetEmployee(int employeeID)
        {
            // get an employee's details
            // parameter validation
            if (employeeID <= 0)
            {
                throw new ArgumentNullException("Please provide a valid employee ID.");
            }

            return _purchasingContext.Employees
            .Where(x => x.EmployeeId == employeeID && !x.RemoveFromViewFlag)
            .Select(x => new EmployeeView
            {
                EmployeeID = x.EmployeeId,
                FullName = x.FirstName + ' ' + x.LastName
            }).FirstOrDefault();
        }
    }
}
