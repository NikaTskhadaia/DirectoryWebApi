﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace PersonDirectory.Domain.Models
{
    public partial class PhoneNumber
    {
        public int Id { get; set; }
        public PhoneNumberType NumberType { get; set; }
        public string Number { get; set; }
    }
}