using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDoZero.Models
{
    [Table("TesteIdentity")]
    public class TesteIdentity
    {
        [Display(Name = "Codigo")]
        [Column("Id")]
        public int Id { get; set; }
        [Display(Name = "Descrição")]
        [Column("Nome")]
        public string Nome { get; set; }
    }
}
