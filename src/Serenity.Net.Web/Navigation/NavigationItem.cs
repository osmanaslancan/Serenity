﻿namespace Serenity.Navigation
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string FullPath { get; set; }
        public string IconClass { get; set; }
        public string ItemClass { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public NavigationItem Parent { get; set; }
        public List<NavigationItem> Children { get; private set; }
        public bool IsSection { get; set; }

        public NavigationItem()
        {
            Children = new List<NavigationItem>();
        }
    }
}