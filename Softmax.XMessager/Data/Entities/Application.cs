﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XMessager.Data.Entities
{
    public class Application
    {
        [Key]
        public string ApplicationId { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Client Client { get; set; }
    }
}