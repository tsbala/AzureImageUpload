namespace ImageResizerLib.DTO
{
    public class ResizeParams
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Location { get; set; }
    }

    public class Resize160X160 : ResizeParams
    {
        public Resize160X160(ImageDetails imageDetails)
        {
            Identifier = imageDetails.Identifier;
            Name = imageDetails.Name;
            Height = 160;
            Width = 160;
            Location = imageDetails.Location;
        }
    }

    public class Resize400X400 : ResizeParams
    {
        public Resize400X400(ImageDetails imageDetails)
        {
            Identifier = imageDetails.Identifier;
            Name = imageDetails.Name;
            Height = 400;
            Width = 400;
            Location = imageDetails.Location;
        }
    }
}
