using UnityEngine;
using System.Collections;

public class RGBandHSVconverter{


	/* Converts from HSVA to RGBA
	 * 
	 * H - Hue. Any value from 0 to 360
	 * S - Saturation. Any value from 0 to 1
	 * V - Value/Brightness. Any value from 0 to 1
	 * A - Alpha. Any value from 0 to 1
	 */
	public static Color HSVtoRGB(float H, float S, float V){
		S = Mathf.Clamp (S, 0, 1);
		V = Mathf.Clamp (V, 0, 1);
		Color ret = new Color ();
		float C = V*S;
		float X = C * (1 - Mathf.Abs ((H / 60) % 2 - 1));
		float m = V - C;
		switch (((int)H / 60)%6) {
		case 0:
			ret.r = C;
			ret.g = X;
			ret.b = 0;
			break;
		case 1:
			ret.r = X;
			ret.g = C;
			ret.b = 0;
			break;
		case 2:
			ret.r = 0;
			ret.g = C;
			ret.b = X;
			break;
		case 3:
			ret.r = 0;
			ret.g = X;
			ret.b = C;
			break;
		case 4:
			ret.r = X;
			ret.g = 0;
			ret.b = C;
			break;
		case 5:
			ret.r = C;
			ret.g = 0;
			ret.b = X;
			break;
		}
		ret.r += m;
		ret.g += m;
		ret.b += m;
		return ret;
	}

	public static Vector3 RGBtoHSV( float r, float g, float b )
	{
		float h;
		float s; 
		float v;
		float min; 
		float max; 
		float delta;
		min = Mathf.Min( r, g, b);
		max = Mathf.Max( r, g, b );
		v = max;				// v
		delta = max - min;
		if( max != 0 )
			s = delta / max;		// s
		else {
			// r = g = b = 0		// s = 0, v is undefined
			s = 0;
			h = -1;

			return new Vector3(h,s,v);
		}
		if( r == max )
			h = ( g - b ) / delta;		// between yellow & magenta
		else if( g == max )
			h = 2 + ( b - r ) / delta;	// between cyan & yellow
		else
			h = 4 + ( r - g ) / delta;	// between magenta & cyan
		h *= 60;				// degrees
		if( h < 0 )
			h += 360;

		return new Vector3(h,s,v);
	}


}
