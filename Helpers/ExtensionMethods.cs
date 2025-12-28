namespace Pronia.Helpers
{
    public static  class ExtensionMethods
    {
        public static bool CheckType( this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckSize(this IFormFile file, int mb)
        {
            return (file.Length / 1024) <= mb * 1024 * 1024;
        }

        public static string SaveFile(this IFormFile file, string path)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string mainImagePath = Path.Combine(path, uniqueFileName);
            using FileStream stream = new FileStream(mainImagePath, FileMode.Create);
            file.CopyTo(stream);
            return uniqueFileName;

        }
    }
}
