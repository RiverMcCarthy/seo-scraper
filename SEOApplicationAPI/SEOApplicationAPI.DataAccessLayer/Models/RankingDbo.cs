﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEOApplicationAPI.DataAccessLayer.Models
{
    public class RankingDbo
    {
        public int Id { get; set; }

        public string SearchPhrase { get; set; } = null!;

        public string Url { get; set; } = null!;

        public int Rank { get; set; }

        public DateTime SearchedOn { get; set; }
    }
}
