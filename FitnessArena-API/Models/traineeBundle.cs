﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FitnessArena_API.Models
{
    [Table("traineeBundle")]
    public partial class traineeBundle
    {
        [Key]
        public int bundleId { get; set; }
        [Key]
        public int traineeId { get; set; }
        [Column(TypeName = "date")]
        public DateTime? subscriptionDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? expiryDate { get; set; }

        [ForeignKey("bundleId")]
        [JsonIgnore]
        [InverseProperty("traineeBundles")]
        public virtual bundle bundle { get; set; }
        [ForeignKey("traineeId")]
        [InverseProperty("traineeBundles")]
        [JsonIgnore]
        public virtual user trainee { get; set; }
    }
}