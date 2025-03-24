using House.Model.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace House.Model.Attributes
{
    public class ChecksumAttribute : Attribute
    {
        public ChecksumAttribute() : base()
        {
    
        }
    }
}
