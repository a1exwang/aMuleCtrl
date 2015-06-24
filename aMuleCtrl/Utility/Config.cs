using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMuleCtrl
{
    class Config
    {
        public static Config GetInstance()
        {
            return TheInstance;
        }
        public static void Initialize(String[] p)
        {
            TheInstance = new Config(p);
        }
        private static Config TheInstance;
        public Config(String[] param)
        {
            mParams = param;
        }

        public Boolean IsSilent()
        {
            return isSilent;
        }
        public Boolean WillSaveFile()
        {
            return willSaveFile;
        }
        public Boolean WillSaveToDb()
        {
            return willSaveToDb;
        }
        public String GetSaveFile()
        {
            return saveFileName;
        }
        public String GetDatabaseFile()
        {
            return dbFile;
        }
        public String GetCurrentDesignation()
        {
            return currentDesignation;
        }
        public void SetCurrentDesignation(String cd)
        {
            currentDesignation = cd;
        }

        public void AddLastCommand(String op, String data)
        {
            lastCommands.Insert(0, new string[] { op, data });
        }
        public String[] GetLastCommand()
        {
            if (lastCommands.Count > 0)
                return lastCommands[0];
            else
                return null;
        }
        public List<String[]> GetLastCommands()
        {
            return lastCommands;
        }
        public void SetSearchType(String t)
        {
            searchType = t;
        }
        public String GetSearchType()
        {
            return searchType;
        }
        public void SetSearchDataType(String type)
        {
            searchDataType = type;
        }
        public String GetSearchDataType()
        {
            return searchDataType;
        }
        public int GetMagnetMaxCount()
        {
            return magnetMax;
        }

        private void ParseParams()
        {
            if (mParams.Contains("-s") || mParams.Contains("--silent"))
            {
                isSilent = true;
            }
        }

        Boolean isSilent = false;
        Boolean willSaveFile = false;
        String saveFileName = "list.txt";
        Boolean willSaveToDb = true;
        String dbFile = "links.sqlite";
        String currentDesignation = "";
        String searchType = "kad";
        String searchDataType = "designation";
        int magnetMax = 20;

        List<String[]> lastCommands = new List<string[]>();

        String[] mParams;
    }
}
