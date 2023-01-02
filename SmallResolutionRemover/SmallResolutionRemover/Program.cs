using System.Drawing;
using System.IO;

public class JPEGPicture
{
    private byte[] data;
    private ushort m_width;
    private ushort m_height;

    public byte[] Data { get => data; set => data = value; }
    public ushort Width { get => m_width; set => m_width = value; }
    public ushort Height { get => m_height; set => m_height = value; }

    public void GetJPEGSize()
    {
        ushort height = 0;
        ushort width = 0;
        for (int nIndex = 0; nIndex < Data.Length; nIndex++)
        {
            if (Data[nIndex] == 0xFF)
            {
                nIndex++;
                if (nIndex < Data.Length)
                {
                    /*
                        0xFF, 0xC0,             // SOF0 segement
                        0x00, 0x11,             // length of segment depends on the number of components
                        0x08,                   // bits per pixel
                        0x00, 0x95,             // image height
                        0x00, 0xE3,             // image width
                        0x03,                   // number of components (should be 1 or 3)
                        0x01, 0x22, 0x00,       // 0x01=Y component, 0x22=sampling factor, quantization table number
                        0x02, 0x11, 0x01,       // 0x02=Cb component, ...
                        0x03, 0x11, 0x01        // 0x03=Cr component, ...
                    */
                    if (Data[nIndex] == 0xC2)
                    {
                        Console.WriteLine("0xC0 information:"); // Start Of Frame (baseline DCT)
                        nIndex += 4;
                        if (nIndex < Data.Length - 1)
                        {
                            // 2 bytes for height
                            height = BitConverter.ToUInt16(new byte[2] { Data[++nIndex], Data[nIndex - 1] }, 0);
                            Console.WriteLine("height = " + height);
                        }
                        nIndex++;
                        if (nIndex < Data.Length - 1)
                        {
                            // 2 bytes for width
                            width = BitConverter.ToUInt16(new byte[2] { Data[++nIndex], Data[nIndex - 1] }, 0);
                            Console.WriteLine("width = " + width);
                        }
                    }
                }
            }
        }
        if (height != 0)
            Height = height;
        if (width != 0)
            Width = width;
    }

    public byte[] ImageToByteArray(string ImageName)
    {
        FileStream fs = new FileStream(ImageName, FileMode.Open, FileAccess.Read);
        byte[] ba = new byte[fs.Length];
        fs.Read(ba, 0, Convert.ToInt32(fs.Length));
        fs.Close();
        
        return ba;
    }
}

class Program
{
    static void Main(string[] args)
    {
        //JPEGPicture pic = new JPEGPicture();
        //pic.Data = pic.ImageToByteArray("C:\\Users\\kanniyappans\\Downloads\\Telegram Desktop\\Pictures\\__suhuuu__-20221120-0006.jpg");
        //pic.GetJPEGSize();

        string[] files = Directory.GetFiles("E:\\Suresh\\Media\\Pics\\", "*", SearchOption.TopDirectoryOnly);
        Directory.CreateDirectory("E:\\Suresh\\Media\\Pictures");
        Directory.CreateDirectory("E:\\Suresh\\Media\\Pics\\Small");
        try
        {
            Parallel.For(0, files.Length, i =>
            {
                var img = Image.FromFile(files[i]);
                Console.Write(Path.GetFileName(files[i]) + ": ");
                Console.Write(img.Width + "x" + img.Height);
                if (img.Width > 1000 || img.Height > 1000)
                {
                    img.Dispose();
                    Console.WriteLine(" - Image is big");
                    File.Move(files[i], "E:\\Suresh\\Media\\Pictures\\" + Path.GetFileName(files[i]), true);
                }
                else
                {
                    img.Dispose();
                    Console.WriteLine(" - Image is small");
                    File.Move(files[i], "E:\\Suresh\\Media\\Pics\\Small\\" + Path.GetFileName(files[i]), true);
                }
            });
        }
        catch (System.OutOfMemoryException)
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
        //foreach (var item in files)
        //{
        //    var img = Image.FromFile(item);
        //    Console.Write(Path.GetFileName(item) + ": ");
        //    Console.Write(img.Width + "x" + img.Height);
        //    if (img.Width > 1000 || img.Height > 1000)
        //    {
        //        img.Dispose();
        //        Console.WriteLine(" - Image is big");
        //        File.Move(item, Path.GetDirectoryName(item) + "\\Big\\" + Path.GetFileName(item));
        //    }
        //    else
        //    {
        //        img.Dispose();
        //        Console.WriteLine(" - Image is small");
        //        File.Move(item, Path.GetDirectoryName(item) + "\\Small\\" + Path.GetFileName(item));
        //    }

        //}

    }
}