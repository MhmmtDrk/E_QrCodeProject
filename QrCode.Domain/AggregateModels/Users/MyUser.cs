using Microsoft.AspNetCore.Identity;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Users
{
    public  class ApplicationUser :IdentityUser
    {
        public string? Name { get; set; }

        

        //    //    private User(Guid id) : base(id)
        //    //    {

        //    //    }
        //    //    private User(Guid id, string userName, string email, string password, ):base(id)
        //    //    {
        //    //        UserName = userName;
        //    //        Email = email;
        //    //        Password = password;
        //    //        //CreatedDate = createdDate;
        //    //    }

        //    //    public string UserName { get;private set; }
        //    //    public string Email { get; private set; }
        //    //    public string Password { get; private set; }
        //    //   // public DateTime CreatedDate { get; private set; }
        //    //    public static User CreateUser(string userName,string email,string password)
        //    //    {

        //    //        User user = new(
        //    //            id: Guid.NewGuid(),
        //    //            userName: new(userName),
        //    //            email: new(email),
        //    //            password: new(password)
        //    //            );
        //    //        return user;
        //    //    }
        //    //}
        //    //public sealed record UserName
        //    //{
        //    //    public string Value { get; init; }
        //    //    public UserName(string value)
        //    //    {
        //    //        if (string.IsNullOrEmpty(value))
        //    //        {
        //    //            throw new ArgumentException("Name is required");
        //    //        }
        //    //        if (value.Length<6)
        //    //        {
        //    //            throw new ArgumentException("En az 6 karakter girilmelidir");
        //    //        }

        //    //        Value = value;
        //    //    }
        //    //}
        //    //public sealed record Email
        //    //{        
        //    //    public string Value { get; init; }
        //    //    public Email(string value)
        //    //    {
        //    //        if(string.IsNullOrEmpty(value))
        //    //        {
        //    //            throw new ArgumentException("Email is required");
        //    //        }
        //    //        if (!value.Contains("@"))
        //    //        {
        //    //            throw new ArgumentException("Email format is wrong");
        //    //        }
        //    //        if (!value.Contains("."))
        //    //        {
        //    //            throw new ArgumentException("Email format is wrong");
        //    //        }
        //    //        Value = value;
        //    //    }
        //    //}
        //    //public sealed record Password
        //    //{
        //    //    public string Value { get; init; }
        //    //    public Password(string value)
        //    //    {
        //    //        if (string.IsNullOrEmpty(value))
        //    //        {
        //    //            throw new ArgumentException("Password is required");
        //    //        }
        //    //        if (value.Length < 6)
        //    //        {
        //    //            throw new ArgumentException("En az 6 karakter girilmelidir");
        //    //        }

        //    //        Value = value;
        //    //    }
    }
}
