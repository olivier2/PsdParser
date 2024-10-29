namespace PsdParser
{
    public class LayerRecordAndImage
    {
        public LayerRecord Record { get; private set; }
        public LayerImage Image { get; private set; }
        public LayerRecordAndImage(LayerRecord record, LayerImage image)
        {
            Record = record;
            Image = image;
        }
        public void Deconstruct(out LayerRecord record, out LayerImage image) => (record, image) = (Record, Image);
    }
}