using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp2021.Models
{
    public enum UserState
    {
        NotLoggedIn,
        RegularUser,
        RegularUserForbidden,
        Manager
    }
}
