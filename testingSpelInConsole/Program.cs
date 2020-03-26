using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testproject;

namespace testingSpelInConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Arrange  (zowel wit als zwart kunnen niet meer)
            Spel spel = new Spel();
            spel.Bord[0, 0] = Kleur.Wit;
            spel.Bord[0, 1] = Kleur.Wit;
            spel.Bord[0, 2] = Kleur.Wit;
            spel.Bord[0, 3] = Kleur.Wit;
            spel.Bord[0, 4] = Kleur.Wit;
            spel.Bord[0, 5] = Kleur.Wit;
            spel.Bord[0, 6] = Kleur.Wit;
            spel.Bord[0, 7] = Kleur.Wit;
            spel.Bord[1, 0] = Kleur.Wit;
            spel.Bord[1, 1] = Kleur.Wit;
            spel.Bord[1, 2] = Kleur.Wit;
            spel.Bord[1, 3] = Kleur.Wit;
            spel.Bord[1, 4] = Kleur.Wit;
            spel.Bord[1, 5] = Kleur.Wit;
            spel.Bord[1, 6] = Kleur.Wit;
            spel.Bord[1, 7] = Kleur.Wit;
            spel.Bord[2, 0] = Kleur.Wit;
            spel.Bord[2, 1] = Kleur.Wit;
            spel.Bord[2, 2] = Kleur.Wit;
            spel.Bord[2, 3] = Kleur.Wit;
            spel.Bord[2, 4] = Kleur.Wit;
            spel.Bord[2, 5] = Kleur.Wit;
            spel.Bord[2, 6] = Kleur.Wit;
            spel.Bord[2, 7] = Kleur.Wit;
            spel.Bord[3, 0] = Kleur.Wit;
            spel.Bord[3, 1] = Kleur.Wit;
            spel.Bord[3, 2] = Kleur.Wit;
            spel.Bord[3, 3] = Kleur.Wit;
            spel.Bord[3, 4] = Kleur.Wit;
            spel.Bord[3, 5] = Kleur.Wit;
            spel.Bord[3, 6] = Kleur.Wit;
            spel.Bord[3, 7] = Kleur.Zwart;
            spel.Bord[4, 0] = Kleur.Wit;
            spel.Bord[4, 1] = Kleur.Wit;
            spel.Bord[4, 2] = Kleur.Wit;
            spel.Bord[4, 3] = Kleur.Wit;
            spel.Bord[4, 4] = Kleur.Wit;
            spel.Bord[4, 5] = Kleur.Wit;
            spel.Bord[4, 6] = Kleur.Zwart;
            spel.Bord[4, 7] = Kleur.Zwart;
            spel.Bord[5, 0] = Kleur.Wit;
            spel.Bord[5, 1] = Kleur.Wit;
            spel.Bord[5, 2] = Kleur.Wit;
            spel.Bord[5, 3] = Kleur.Wit;
            spel.Bord[5, 4] = Kleur.Wit;
            spel.Bord[5, 5] = Kleur.Wit;
            spel.Bord[5, 6] = Kleur.Zwart;
            spel.Bord[5, 7] = Kleur.Zwart;
            spel.Bord[6, 0] = Kleur.Wit;
            spel.Bord[6, 1] = Kleur.Wit;
            spel.Bord[6, 2] = Kleur.Wit;
            spel.Bord[6, 3] = Kleur.Wit;
            spel.Bord[6, 4] = Kleur.Wit;
            spel.Bord[6, 5] = Kleur.Wit;
            spel.Bord[6, 6] = Kleur.Wit;
            spel.Bord[6, 7] = Kleur.Zwart;
            spel.Bord[7, 0] = Kleur.Wit;
            spel.Bord[7, 1] = Kleur.Wit;
            spel.Bord[7, 2] = Kleur.Wit;
            spel.Bord[7, 3] = Kleur.Wit;
            spel.Bord[7, 4] = Kleur.Wit;
            spel.Bord[7, 5] = Kleur.Wit;
            spel.Bord[7, 6] = Kleur.Wit;
            spel.Bord[7, 7] = Kleur.Wit;

            //     0 1 2 3 4 5 6 7
            //     v
            // 0   1 1 1 1 1 1 1 1  
            // 1   1 1 1 1 1 1 1 1
            // 2   1 1 1 1 1 1 1 1
            // 3   1 1 1 1 1 1 1 2
            // 4   1 1 1 1 1 1 2 2
            // 5   1 1 1 1 1 1 2 2
            // 6   1 1 1 1 1 1 1 2
            // 7   1 1 1 1 1 1 1 1
            // Act
            spel.AandeBeurt = Kleur.Wit;
            var actual = spel.Afgelopen();
            Console.WriteLine(spel.Afgelopen());
        }
    }
}
