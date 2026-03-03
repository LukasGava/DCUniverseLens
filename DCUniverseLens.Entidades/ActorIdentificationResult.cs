using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCUniverseLens.Entidades
{
        public class ActorIdentificationResult
        {
            public string ActorName { get; set; }
            public float Confidence { get; set; }
            public Dictionary<string, float> TopPredictions { get; set; }
            public string ImageData { get; set; }
            public FaceRectangle FaceRectangle { get; set; }
            public bool IsRecognized { get; set; }
            public Dictionary<string, string> PersonajesNombres { get; set; }

            public string Apodo { get; set; }


        }
    }

