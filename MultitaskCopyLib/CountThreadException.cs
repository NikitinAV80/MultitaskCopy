using System;

namespace MultitaskCopyLib
{
    public class CountThreadException : Exception
    {
        public CountThreadException() : base("Ошибка в количестве потоков") {}
    }
}