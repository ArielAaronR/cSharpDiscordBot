using System;
using System.Collections.Generic;

namespace DiscordBot.Models
{
    public class DojoDachi
    {
        public static Random rand = new Random();
        public int Happiness { get; set; }
        public int Fullness { get; set; }
        public int Energy { get; set; }
        public int Meal { get; set; }
        public DojoDachi()
        {
            Happiness = 20;
            Fullness = 20;
            Energy = 50;
            Meal = 3;

        }
        public string Feed()
        {
            if (Meal == 0)
            {
                return "Stop bro";
            }
            Meal -= 1;
            if (rand.Next(1, 5) != 1)
            {
                Fullness += rand.Next(5, 11);
                return "Yums";
            }
            else
            {
                return "not hungry";
            }

        }
        public string Play()
        {
            if (Energy == 0)
            {
                return "Stop bro I'm tired";
            }
            Energy -= 5;
            if (rand.Next(1, 5) != 1)
            {
                Happiness += rand.Next(5, 11);

                return "Energized";
            }
            else
            {
                return "Nah Fam";
            }
        }
        public string Work()
        {
            if (Energy == 0)
            {
                return "Stop bro I'm tired";
            }

            Energy -= 5;
            Meal += rand.Next(1, 4);

            return "Same bro";
        }
        public string Sleep()
        {
            if (Energy == 0)
            {
                return "Stop bro I'm tired";
            }
            Energy += 15;
            Fullness -= 5;
            Happiness -= 5;
            return "Go to sleep";
        }
        public bool CheckWin()
        {
            if (Energy >= 100 && Fullness >= 100 && Happiness >= 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckLoss()
        {
            if (Fullness <= 0 || Happiness <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    } //Class Ends Here
}

