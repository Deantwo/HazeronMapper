﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HazeronMapper
{
    static class Readscan
    {
        public static void readscan(string scanfile, Galaxy galaxy)
        {
            string systemName = null;
            string sectorName = null;
            string systemLoc = null;
            string sectorLoc = null;
            bool readingWH = false;
            int whlinecount = 0;
            int whcount = 0;

            string whname = null;
            bool whpolarity = false;
            string linkstosystemName = null;
            string linktosystemLoc = null;
            string linkstosectorName = null;
            string linktosectorLoc = null;
            //SectorObj sector = new SectorObj(sectorName, galaxy);
            SectorObj sector = null;
            SystemObj sys = null;

            //debugging
            int linecount = 0;
            int sectorscount = 0;
            int systemscount = 0;
         
            //using (StreamReader reader = new StreamReader(scanfile))
            //{
                //string line;
                //while ((line = reader.ReadLine()) != null)
            List<string> lines = new List<string>(Regex.Split(scanfile, "\r\n|\r|\n"));
            foreach (string line in lines)
            {
                linecount++;
                if (line != "")
                {
                    if (!readingWH)
                    {
                        if (line.Contains("Wormholes"))
                        {
                            readingWH = true;
                        }

                        else if (line.Contains("Sector ("))
                        {
                            sectorName = line;
                            int index = line.IndexOf("(");
                            sectorLoc = line.Substring(index);
                        }
                        else if (line.Contains("System") && (!line.Contains("System Survey of ")))
                        {
                                
                            int index = line.IndexOf("(");
                            systemName = line.Remove(index);
                            systemLoc = line.Substring(index);
                        }
                    }
                    else
                    {
                        whlinecount++;
                        if (sector == null)
                        {
                            sector = new SectorObj(sectorLoc, sectorName, galaxy);
                            sectorscount++;
                            sys = new SystemObj(systemLoc, systemName, galaxy.sectors_dictionary[sectorLoc]);
                            systemscount++;
                        }
                        if (whlinecount == 1)
                        {
                            whname = line;
                        }
                        else if (line.Contains("Positive Wormhole"))
                        {
                            whpolarity = true;
                        }
                        else if (line.Contains("Negative Wormhole"))
                        {
                            whpolarity = false;
                        }
                        else if (whlinecount == 3)
                        {
                            int index = line.IndexOf("(");
                            linkstosystemName = line.Remove(index);
                            linktosystemLoc = line.Substring(index);
                        }
                        else if (whlinecount == 4)
                        {
                            linkstosectorName = line;
                            int index = line.IndexOf("(");
                            linktosectorLoc = line.Substring(index);
                        }
                        else if (whlinecount == 5)
                        {
                            WormHoleObj wh = new WormHoleObj(whname, whpolarity, galaxy.sectors_dictionary[sectorLoc].systems_dictionary[sys.location], galaxy);
                            //WormHoleObj wh = new WormHoleObj(whname, whpolarity, sys, galaxy);
                            SectorObj whlinksec = new SectorObj(linktosectorLoc, linkstosectorName, galaxy);
                            sectorscount++;
                            SystemObj whlinksys = new SystemObj(linktosystemLoc, linkstosystemName, galaxy.sectors_dictionary[linktosectorLoc]);
                            systemscount++;
                            wh.makelink(whlinksys);
                            whlinecount = 0;
                            whcount++;
                        }
                    }
                    if (line == "Primary")
                    {
                        break;
                    }
                }
            }
            
        }
    }
}
