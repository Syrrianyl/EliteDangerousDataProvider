﻿using EddiDataDefinitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EddiEvents
{
    public class TradePromotionEvent : Event
    {
        public const string NAME = "Trade promotion";
        public const string DESCRIPTION = "Triggered when your trade rank increases";
        public const string SAMPLE = "{\"timestamp\":\"2016-06-10T14:32:03Z\",\"event\":\"Promotion\",\"Trade\":5}";
        public static Dictionary<string, string> VARIABLES = new Dictionary<string, string>();

        static TradePromotionEvent()
        {
            VARIABLES.Add("rating", "The commander's new trade rating");
            VARIABLES.Add("LocalRating", "The translation of the trade rating data into the chosen language");
        }

        [JsonProperty("rating")]
        public string rating{ get; private set; }

        public string LocalRating { get; }

        public TradePromotionEvent(DateTime timestamp, TradeRating rating) : base(timestamp, NAME)
        {
            this.rating = rating.name;
			this.LocalRating = rating.LocalName;
        }
    }
}
