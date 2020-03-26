using ScrabbleApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace testproject
{
    public enum Kleur { Geen, Wit, Zwart };
    //Deze enum gebruiken we om alle mogelijke richtingen toe te voegen in een lijst.
    //Die lijst gebruiken we om elke disk van kleur te veranderen als dat mogelijk is.
    //De naam van die lijst is RichtingMogelijk.
    public enum Richting { HorizontaalLinks, HorizontaalRechts, VerticaalOmhoog, VerticaalOmlaag, DiagonaalNoordWest, DiagonaalZuidWest, DiagonaalNoordOost, DiagonaalZuidOost };

    public class Spel : ISpel
    {
        //Properties
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public ICollection<Speler> Spelers { get; set; }
        public Kleur[,] Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        private List<Richting> RichtingMogelijk { get; set; }

        //Constructor
        public Spel()
        {
            //Als eerst moet er een spelbord gemaakt worden.
            Kleur[,] nieuwBord = new Kleur[8, 8];

            //Dit doen we met 2 for loops, zodat we een multidimensionale array krijgen
            //We moeten ook niet vergeten dat in het midden er al disks liggen.
            //Daarvoor zijn de if statements. Die zorgen er voor dat op de standaard plekken al kleuren liggen.
            for (int i = 0; i <= 6; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    if (i == 3 && j == 3 || i == 4 && j == 4)
                    {
                        nieuwBord[i, j] = Kleur.Wit;
                    }
                    else if (i == 3 && j == 4 || i == 4 && j == 3)
                    {
                        nieuwBord[i, j] = Kleur.Zwart;
                    }
                    else
                    {
                        nieuwBord[i, j] = Kleur.Geen;
                    }
                }
            }
            Bord = nieuwBord;
        }

        //Methods
        public bool Pas()
        {
            if (AandeBeurt == Kleur.Wit)
            {
                AandeBeurt = Kleur.Zwart;
                return true;
            }
            else if (AandeBeurt == Kleur.Zwart)
            {
                AandeBeurt = Kleur.Wit;
                return true;
            }

            return false;
        }

        public bool Afgelopen()
        {
            for (int i = 0; i < Bord.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < Bord.GetLength(1) - 1; j++)
                {
                    if (ZetMogelijk(i, j))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Kleur OverwegendeKleur()
        {
            int aantalWitteDisks = 0;
            int aantalZwarteDisks = 0;

            for (int i = 0; i < Bord.GetLength(0); i++)
            {
                for (int j = 0; j < Bord.GetLength(1); j++)
                {
                    if (Bord[i, j] == Kleur.Wit)
                    {
                        aantalWitteDisks++;
                    }
                    else if (Bord[i, j] == Kleur.Zwart)
                    {
                        aantalZwarteDisks++;
                    }
                }
            }

            if (aantalWitteDisks > aantalZwarteDisks)
            {
                return Kleur.Wit;
            }
            else if (aantalWitteDisks < aantalZwarteDisks)
            {
                return Kleur.Zwart;
            }

            return Kleur.Geen;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            RichtingMogelijk = new List<Richting>();

            //Eerst kijken we of de zet niet buiten het speelveld valt
            if (rijZet >= 0 && rijZet <= 7 && kolomZet >= 0 && kolomZet <= 7)
            {
                //Door alle mogelijkheden gaan.
                kanVerticaalOmlaag(rijZet, kolomZet, false);
                kanVerticaalOmhoog(rijZet, kolomZet, false);

                kanHorizontaalRechts(rijZet, kolomZet, false);
                kanHorizontaalLinks(rijZet, kolomZet, false);

                kanDiagonaalZuidWest(rijZet, kolomZet, false);
                kanDiagonaalZuidOost(rijZet, kolomZet, false);

                kanDiagonaalNoordWest(rijZet, kolomZet, false);
                kanDiagonaalNoordOost(rijZet, kolomZet, false);

                //Kijken of er richtingen zijn toegevoegd aan de lijst, zo ja, return true.
                if (RichtingMogelijk.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool DoeZet(int rijZet, int kolomZet)
        {
            //Kijken eerst of de zet mogelijk is.
            if (ZetMogelijk(rijZet, kolomZet))
            {
                //Daarna gaan we door elke mogelijk richting die in RichtingMogelijk lijst is gezet.
                //We gebruiken dezelfde functies als in ZetMogelijk, om het overzichtelijk te houden.
                foreach (Richting richting in RichtingMogelijk)
                {
                    if (richting == Richting.VerticaalOmlaag)
                    {
                        kanVerticaalOmlaag(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.VerticaalOmhoog)
                    {
                        kanVerticaalOmhoog(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.HorizontaalRechts)
                    {
                        kanHorizontaalRechts(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.HorizontaalLinks)
                    {
                        kanHorizontaalLinks(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.DiagonaalZuidWest)
                    {
                        kanDiagonaalZuidWest(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.DiagonaalZuidOost)
                    {
                        kanDiagonaalZuidOost(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.DiagonaalNoordWest)
                    {
                        kanDiagonaalNoordWest(rijZet, kolomZet, true);
                    }
                    else if (richting == Richting.DiagonaalNoordOost)
                    {
                        kanDiagonaalNoordOost(rijZet, kolomZet, true);
                    }
                }

                AandeBeurt = tegenOvergesteldeKleur(AandeBeurt);

                return true;
            }

            return false;
        }

        public Kleur tegenOvergesteldeKleur(Kleur KleurSpeler)
        {
            if (KleurSpeler == Kleur.Zwart)
            {
                return Kleur.Wit;
            }
            else if (KleurSpeler == Kleur.Wit)
            {
                return Kleur.Zwart;
            }
            return Kleur.Geen;
        }

        //Hieronder staan alle richting functies waarmee je een zet kan doen.
        public void kanVerticaalOmlaag(int rijZet, int kolomZet, bool zetDisks)
        {
            //Eerst halen we de kleur van de tegenspeler op, zodat we kunnen controleren in de for loop daarvoor.
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            //In de for loop kijken we eerst of het begin punt niet een kleur heeft.
            //Daarna kijken we of elke disk de tegenspeler zijn kleur is.
            //Bij beide kijken we meteen of getallen niet buiten het speelbord komen.
            for (int i = rijZet; Bord[rijZet, kolomZet] == Kleur.Geen && i < Bord.GetLength(0) - 1 && i != 7 ||
                Bord[i, kolomZet] == tegenSpelerKleur && i < Bord.GetLength(0) - 1 && i != rijZet; i++)
            {
                //In deze kijken we of de volgende plek dezelfde kleur is als de speler
                //en of de huidige zet die van de tegenstander is.
                //Als dat zo is, dan wordt deze richting aan de lijst van mogelijke richtingen toegevoegd.
                if (Bord[i + 1, kolomZet] == AandeBeurt && Bord[i, kolomZet] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.VerticaalOmlaag);
                }

                //In deze functie kijken we of de zetDisks op true staat 
                //en zetten we de disks om naar de huidige spelers zijn kleur.
                //Dit gebeurt staat alleen in de DoeZet functie op true.
                if (zetDisks)
                {
                    Bord[i, kolomZet] = AandeBeurt;
                }
            }
        }

        public void kanVerticaalOmhoog(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            for (int i = rijZet; Bord[rijZet, kolomZet] == Kleur.Geen && i > 0 && i != 0 ||
                Bord[i, kolomZet] == tegenSpelerKleur && i > 0 && i != rijZet; i--)
            {
                if (Bord[i - 1, kolomZet] == AandeBeurt && Bord[i, kolomZet] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.VerticaalOmhoog);
                }

                if (zetDisks)
                {
                    Bord[i, kolomZet] = AandeBeurt;
                }
            }
        }

        public void kanHorizontaalRechts(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            for (int i = kolomZet; Bord[rijZet, kolomZet] == Kleur.Geen && i < Bord.GetLength(1) - 1 && i != 7 ||
                Bord[rijZet, i] == tegenSpelerKleur && i < Bord.GetLength(1) - 1 && i != kolomZet; i++)
            {
                if (Bord[rijZet, i + 1] == AandeBeurt && Bord[rijZet, i] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.HorizontaalRechts);
                }

                if (zetDisks)
                {
                    Bord[rijZet, i] = AandeBeurt;
                }
            }
        }

        public void kanHorizontaalLinks(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            for (int i = kolomZet; Bord[rijZet, kolomZet] == Kleur.Geen && i > 0 && i != 0 ||
                Bord[rijZet, i] == tegenSpelerKleur && i > 0 && i != kolomZet; i--)
            {
                if (Bord[rijZet, i - 1] == AandeBeurt && Bord[rijZet, i] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.HorizontaalLinks);
                }

                if (zetDisks)
                {
                    Bord[rijZet, i] = AandeBeurt;
                }
            }
        }

        public void kanDiagonaalZuidWest(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            int i = rijZet;
            int j = kolomZet;

            //Bij Zuidwest gaat de verticale (i) omhoog en de horizontale (j) omlaag.
            for (; Bord[rijZet, kolomZet] == Kleur.Geen && i < Bord.GetLength(0) - 1 && i != 7 &&
                j > 0 && j != 0 ||
                Bord[i, j] == tegenSpelerKleur && i < Bord.GetLength(0) - 1 && i != rijZet &&
                j > 0 && j != 0 && j != kolomZet;
                i++, j--)
            {
                if (Bord[i + 1, j - 1] == AandeBeurt && Bord[i, j] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.DiagonaalZuidWest);
                }

                if (zetDisks)
                {
                    Bord[i, j] = AandeBeurt;
                }
            }
        }

        public void kanDiagonaalZuidOost(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            int i = rijZet;
            int j = kolomZet;

            //Bij Zuidoost gaat de verticale (i) omhoog en de horizontale (j) omhoog.
            for (; Bord[rijZet, kolomZet] == Kleur.Geen && i < Bord.GetLength(0) - 1 && i != 7 &&
                j < Bord.GetLength(1) - 1 && j != 7 ||
                Bord[i, j] == tegenSpelerKleur && i < Bord.GetLength(0) - 1 && i != rijZet &&
                j < Bord.GetLength(1) - 1 && i != kolomZet;
                i++, j++)
            {
                if (Bord[i + 1, j + 1] == AandeBeurt && Bord[i, j] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.DiagonaalZuidOost);
                }

                if (zetDisks)
                {
                    Bord[i, j] = AandeBeurt;
                }
            }
        }

        public void kanDiagonaalNoordWest(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            int i = rijZet;
            int j = kolomZet;

            //Bij Noordwest gaat de verticale (i) omlaag en de horizontale (j) omlaag.
            for (; Bord[rijZet, kolomZet] == Kleur.Geen && i > 0 && i != 0
                && j > 0 && j != 0 ||
            Bord[i, j] == tegenSpelerKleur && i > 0 && i != rijZet && j > 0 && j != kolomZet; i--, j--)
            {
                if (Bord[i - 1, j - 1] == AandeBeurt && Bord[i, j] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.DiagonaalNoordWest);
                }

                if (zetDisks)
                {
                    Bord[i, j] = AandeBeurt;
                }
            }
        }

        public void kanDiagonaalNoordOost(int rijZet, int kolomZet, bool zetDisks)
        {
            Kleur tegenSpelerKleur = tegenOvergesteldeKleur(AandeBeurt);

            int i = rijZet;
            int j = kolomZet;

            //Bij Noordoost gaat de verticale (i) omlaag en de horizontale (j) omhoog.
            for (; Bord[rijZet, kolomZet] == Kleur.Geen && i > 0 && i != 0 &&
                j < Bord.GetLength(0) - 1 && j != 7 ||
            Bord[i, j] == tegenSpelerKleur && i > 0 && i != rijZet &&
            j < Bord.GetLength(0) - 1 && j != kolomZet; i--, j++)
            {
                if (Bord[i - 1, j + 1] == AandeBeurt && Bord[i, j] == tegenSpelerKleur && zetDisks == false)
                {
                    RichtingMogelijk.Add(Richting.DiagonaalNoordOost);
                }

                if (zetDisks)
                {
                    Bord[i, j] = AandeBeurt;
                }
            }
        }
    }
}