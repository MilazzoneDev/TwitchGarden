using UnityEngine;
using System.Collections;
using System.Globalization;

public class ColorPicker {

	public static Color pickColor(string colorChoice)
	{
		string arg = colorChoice.ToLower();
		Color returnColor = Color.clear;
		//check for an RGB value
		if(arg.Length == 6)
		{
			int r, g, b;
			bool rb,gb,bb;
			var provider = new CultureInfo("en-US");
			rb = int.TryParse(arg.Substring(0,2),System.Globalization.NumberStyles.HexNumber, provider, out r);
			gb = int.TryParse(arg.Substring(2,2),System.Globalization.NumberStyles.HexNumber, provider, out g);
			bb = int.TryParse(arg.Substring(4,2),System.Globalization.NumberStyles.HexNumber, provider, out b);
			if(rb && gb && bb)
			{
				float rf = r/255.0f;
				float gf = g/255.0f;
				float bf = b/255.0f;
				returnColor = new Color(rf,gf,bf);
			}
		}

		//default colors
		if(arg[0].Equals('b'))
		{
			if(arg.Equals("blue"))
			{
				returnColor = Color.blue;
			}
			else if(arg.Equals("black"))
			{
				returnColor = Color.black;
			}
		}
		else if(arg[0].Equals('g'))
		{
			if(arg.Equals("green"))
			{
				returnColor = Color.green;
			}
		}
		else if(arg[0].Equals('k'))
		{
			if(arg.Equals("kitsu"))
			{
				//FFA600 or FF9100
				returnColor = new Color(255.0f/255.0f,145.0f/255.0f,0.0f/255.0f);
			}
		}
		else if(arg[0].Equals('l'))
		{
			if(arg.Equals("lightblue"))
			{
				returnColor = new Color(135.0f/255.0f,206.0f/255.0f,250.0f/255.0f);
			}
			else if(arg.Equals("lightpink"))
			{
				returnColor = new Color(255.0f/255.0f, 182.0f/255.0f, 193.0f/255.0f);
			}
		}
		else if(arg[0].Equals('o'))
		{
			if(arg.Equals("orange"))
			{
				returnColor = new Color(255.0f/255.0f,165.0f/255.0f,0.0f/255.0f);
			}
		}
		else if(arg[0].Equals('p'))
		{
			if(arg.Equals("purple"))
			{
				returnColor = new Color(128.0f/255.0f,0.0f/255.0f,128.0f/128.0f);
			}
			else if(arg.Equals("pink"))
			{
				returnColor = Color.magenta;
			}
		}
		else if(arg[0].Equals('r'))
		{
			if(arg.Equals("red"))
			{
				returnColor = Color.red;
			}
			else if(arg.Equals("redorange"))
			{
				returnColor = new Color(255.0f/255.0f,69.0f/255.0f,0.0f/128.0f);
			}
			else if(arg.Equals("random"))
			{
				returnColor = new Color(Random.value,Random.value,Random.value);
			}
		}
		else if(arg[0].Equals('w'))
		{
			if(arg.Equals("white"))
			{
				returnColor = Color.white;
			}
		}
		else if(arg[0].Equals('y'))
		{
			if(arg.Equals("yellow"))
			{
				returnColor = Color.yellow;
			}
		}

		return returnColor;
	}

}
