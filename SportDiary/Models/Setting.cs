using System;

namespace SportDiary.Models
{
    public class Setting
    {
        public string Name { get; set; }
        public Type ContentType { get; set; }
        public string Icon { get; set; }

        public Setting() { }
    }
}
