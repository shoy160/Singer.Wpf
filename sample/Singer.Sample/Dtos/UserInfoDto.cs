using System;

namespace Singer.Sample.Dtos
{
    public class UserInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Permission { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
