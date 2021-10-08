namespace MultitaskCopyLib
{
    public class Pack
    {
        private static int _id = 0;
        public int Id => _id;
        public byte[] Content { get; }

        public Pack(byte[] content)
        {
            _id++;
            Content = content;
        }

        public Pack(byte[] content, int id)
        {
            _id = id;
            Content = content;
        }
    }
}