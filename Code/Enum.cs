using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI
{
    public sealed class ActivityCategory
    {
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string Created = "Created";
        public const string Modified = "Modified";
        public const string Deleted = "Deleted";
    }

    public sealed class PermissionCategory
    {
        public const string FullControl = "FullControl";
        public const string Read = "Read";
        public const string Write = "Write";
        public const string Delete = "Delete";
    }

    public sealed class AppRole
    {
        public const int SuperAdmin = 1;
        public const int Admin = 2;
        public const int Initiator = 3;
        public const int Reviewer = 4;
        public const int Approver = 5;
        public const int User = 6;
    }

    public sealed class ControlType
    {
        public const int TextBox = 1;
        public const int Dropdown = 2;
        public const int TwoLevelDropDown = 3;
        public const int ThreeLevelDropdown = 4;
        public const int FileUpload = 5;
        public const int DateControl = 6;
    }
}