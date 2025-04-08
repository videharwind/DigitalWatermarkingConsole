using System;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Reflection.Metadata;

public class program
{
    public static string BitStuffing(string text)
    {
        string bits = "100111110111010111011110111111110";
        string output = "";
        int cnt = 0;
        int j = 0;
        for (int i = 0; i < bits.Length; i++)
        {
            if (bits[i] == '1')
            {
                cnt++;
                output += '1';
                if (cnt == 5)
                {
                    output += '0';
                    cnt = 0;
                }
            }
            else if (bits[i] == '0')
            {
                cnt = 0;
                output += '0';
            }
        }
        output = "01111110" + output + "01111110";
        return output;
    }
    
    public static string TextToBinary(string text)
    {
        StringBuilder binarytext = new StringBuilder();
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        foreach(byte b in bytes)
        {
            binarytext.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        return binarytext.ToString();
    }
    public static string BinaryToText(string binary)
    {
        var bytes = new byte[binary.Length / 8];

        for (int i = 0; i < bytes.Length; i++)
        {
            string byteString = binary.Substring(i * 8, 8);
            bytes[i] = Convert.ToByte(byteString, 2);
        }

        return Encoding.UTF8.GetString(bytes);
    }
    public static void EmbedToWatermark(Bitmap img,string watermark)
    {
        string BinaryWatermark = TextToBinary(watermark) + "1111111111111110";
        int width = img.Width;
        int height = img.Height;
        int pos = 0;
        for(int h =0;h<height;h++)
        {
            for(int w =0;w<width;w++)
            {
                Color pixel = img.GetPixel(w, h);
                int alpha = pixel.A;
                int red = pixel.R;
                int blue = pixel.B;
                int green = pixel.G;
                if(pos < BinaryWatermark.Length)
                {
                    blue = (blue & 0b11111110) | (BinaryWatermark[pos] - '0');
                    pos++;
                }
                Color NewPixel = Color.FromArgb(alpha, red, green, blue);
                img.SetPixel(w, h, NewPixel);
            }
        }
    }
    public static string ExtractWatermark(Bitmap img)
    {
        StringBuilder extractedBits = new StringBuilder();
        int width = img.Width;
        int height = img.Height;

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                Color pixel = img.GetPixel(w, h);
                int blue = pixel.B;

                int lsb = blue & 1;
                extractedBits.Append(lsb);

                if (extractedBits.Length >= 16 && extractedBits.ToString().EndsWith("1111111111111110"))
                {
                    extractedBits.Length -= 16;
                    return BinaryToText(extractedBits.ToString());
                }
            }
        }

        return BinaryToText(extractedBits.ToString());
    }

    public static void Main()
    {
        string inputpath = @"C:\Users\videh\Pictures\Screenshots\Screenshot (149).png";
        string outputpath = @"C:\Users\videh\Downloads\Watermarkoutput.png";

        Bitmap img;
        try
        {
            img = new Bitmap(inputpath);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
        string watermark = "helloworld";
        EmbedToWatermark(img, watermark);

        try
        {
            img.Save(outputpath);
            Console.WriteLine("Watermarked image saved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving watermarked image: " + ex.Message);
            return;
        }

        Bitmap watermarkedImg;
        try
        {
            watermarkedImg = new Bitmap(outputpath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading watermarked image: " + ex.Message);
            return;
        }

        string extractedMessage = ExtractWatermark(watermarkedImg);
        Console.WriteLine("Extracted Message: " + extractedMessage);
       
    }
}