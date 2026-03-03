using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCUniverseLens.Entidades.EF
{
    public partial class Actor
    {
        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
