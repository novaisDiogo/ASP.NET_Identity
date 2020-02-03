using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDoZero.Models
{
    public class ApplicationUser : IdentityUser //Adicionar novos campos a tabela AspNetUsers
    {
        public string City { get; set; }
    }
}
//Incluir Propriedade e criar uma nova migration e Update-DataBase
//Incluir na sua viewModel, view e depois editar controller
//Editar RegisterViewModel Controller Account register
//Editar EditUserViewModel Controller Administration EditUser