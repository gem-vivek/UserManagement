// Models/PasswordDetail.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    public class PasswordDetail
    {

        [Key,ForeignKey("UserDetail")]
        public int UserID { get; set; }  // Foreign Key
        public string Password { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        public static implicit operator string(PasswordDetail v)
        {
            throw new NotImplementedException();
        }
    }

}
