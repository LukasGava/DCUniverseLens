using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCUniverseLens.Entidades
{
    public class MultipleActorsResult
    {
        public string OriginalImageData { get; set; }
        public List<ActorIdentificationResult> DetectedActors { get; set; }
    }
}
