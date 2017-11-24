namespace PmlChecker.PmlChecker
{
    public class SourceCode
    {
        private string _fileName;

        public SourceCode(string fileName)
        {
            _fileName = fileName;
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
    }
}