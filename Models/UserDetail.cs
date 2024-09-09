using System;
using System.ComponentModel.DataAnnotations;
using UserManagement.Models;

namespace UserManagement.Models
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordDetails { get; set; }

        public bool IsActivated { get; set; }
        //public virtual PasswordDetail PasswordDetail { get; set; }
    }
}
