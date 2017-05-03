using System;
using System.Configuration;

namespace WebAPIpgw.Utility
{
    public class Util
    {
        public static int GetDelay(string whichDelay)
        {
            return random.Next(Configuration.GetDelayMin(whichDelay),
                                 Configuration.GetDelayMax(whichDelay));
        }
      
        static System.Random random = new System.Random();
    }

    public static class Configuration
    {
       private static int _delayMin = int.MaxValue;
       private static int _delayMax = int.MaxValue;

        public static int getKeyValInt(string key)
        {
            int j;
            bool b = Int32.TryParse(ConfigurationManager.AppSettings[key], out j);
            if (b)
                return j;
            else
                return 0;
        }

        public static int GetDelayMin(string whichDelay)
        {
            _delayMin = getKeyValInt(whichDelay + "DelayMin");
            return _delayMin;
        }

        public static int GetDelayMax(string whichDelay)
        {
            _delayMax = getKeyValInt(whichDelay + "DelayMax");
            return _delayMax;
        }

        public static string getKeyVal(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }


    }
}