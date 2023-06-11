﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  
using System.Text.Json.Serialization;

namespace ClinicaServer.Models
{
    public partial class Paciente
    {
        public Paciente()
        {
            Atendimento = new HashSet<Atendimento>();
        }

        public uint Id { get; set; }

       
                              
        //[RegularExpression(@"([0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2})", ErrorMessage ="Formato do CPF Invalido")]
        public ulong Cpf { get; set; }

        [MaxLength(50,ErrorMessage ="Tamanho do Nome Ultrapassou o Limite Maximo")]
        public string Nome { get; set; }

       // [RegularExpression(@"(\\d{2}/\\d{2}/\\d{4})", ErrorMessage ="Formato de Data Invalido")]
        public DateTime Datanascimento { get; set; }
        public ulong Telefone { get; set; }
        public string Urlimagem { get; set; }
        public virtual ICollection<Atendimento> Atendimento { get; set; }
    }
}