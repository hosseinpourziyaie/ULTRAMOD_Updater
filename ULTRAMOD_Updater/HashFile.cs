/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : HashFile.cs Code
** - Description : HashList structure
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ULTRAMOD_Updater
{
    class HashFile
    {
        private string filename;
        private string md5hash;
        private bool lzma;
        private string link;

        public string fileName
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.filename = value;
            }
        }

        public string md5Hash
        {
            get
            {
                return this.md5hash;
            }
            set
            {
                this.md5hash = value;
            }
        }

        public bool LZMA
        {
            get
            {
                return this.lzma;
            }
            set
            {
                this.lzma = value;
            }
        }

        public string Link
        {
            get
            {
                return this.link;
            }
            set
            {
                this.link = value;
            }
        }

    }
}

