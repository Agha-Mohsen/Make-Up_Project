﻿namespace _0_Framework.Infrastructure
{
    public static class Roles
    {
        public const string Administrator = "1";
        public const string SystemUser = "2";
        public const string ContentUploader = "3";

        public static string GetRoleBy(long id)
        {
            return id switch
            {
                1 => "مدیر سیستم",
                3 => "محتوا گذار",
                _ => ""
            };
        }
    }
}