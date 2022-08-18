namespace PriceHunter.Common.Validation
{
    public static class MimeValidation
    {
        public static class MimeTypes
        {
            public readonly static string Jpeg = "jpeg";
            public readonly static string Jpg = "jpg";
            public readonly static string Png = "png";
        }
        /// <summary>
        /// jpg | jpeg
        /// </summary>
        private static readonly byte[] Jpg = { 255, 216, 255 };

        /// <summary>
        /// png
        /// </summary>
        private static readonly byte[] Png = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };

        private static readonly Dictionary<string, string> MimeTypesDictionary = new()
        {
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"png", "image/png"}
        };

        private static string GetMimeType(byte[] file, string fileName)
        {
            string mime = "application/octet-stream";

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return mime;
            }

            if (file.Take(3).SequenceEqual(Jpg))
            {
                mime = "image/jpeg";
            }
            else if (file.Take(16).SequenceEqual(Png))
            {
                mime = "image/png";
            }

            return mime;
        }

        public static bool IsValidMime(byte[] file, string fileName, params string[] allowedExtensions)
        {
            var mime = GetMimeType(file, fileName);
            return MimeTypesDictionary.ContainsValue(mime) && MimeTypesDictionary.Any(mimeItem => mimeItem.Value == mime && allowedExtensions.Contains(mimeItem.Key));
        }
    }
}