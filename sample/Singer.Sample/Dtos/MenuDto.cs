using System;

namespace Singer.Sample.Dtos
{
    [Serializable]
    public class MenuDto
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Permissions { get; set; }

        public bool HasTag { get; set; }
        public string TagWord { get; set; }
        public bool Enabled { get; set; }
    }
}
