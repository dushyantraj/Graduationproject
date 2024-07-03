using System;
using System.Collections.Generic;

namespace CafeteriaClient
{
    public static class MenuFactory
    {
        private static readonly Dictionary<string, Func<IMenu>> MenuCreators = new Dictionary<string, Func<IMenu>>
        {
            { Roles.Admin, () => new Menus.AdminMenu() },
            { Roles.Chef, () => new Menus.ChefMenu() },
            { Roles.Employee, () => new Menus.EmployeeMenu() }
        };

        public static IMenu GetMenu(string role)
        {
            if (MenuCreators.TryGetValue(role, out var creator))
            {
                return creator();
            }
            return null; // Handle unknown roles as needed
        }
    }
}
