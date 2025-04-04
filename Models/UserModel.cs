
﻿namespace INF27523_TP1_ML_SS.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public NameModel Name { get; set; } = null!;
    }

    public class NameModel
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
    }

}