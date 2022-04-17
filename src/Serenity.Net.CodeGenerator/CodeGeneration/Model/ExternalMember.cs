﻿using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ExternalMember
    {
        public List<ExternalAttribute> Attributes { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool? IsStatic { get; set; }
    }
}