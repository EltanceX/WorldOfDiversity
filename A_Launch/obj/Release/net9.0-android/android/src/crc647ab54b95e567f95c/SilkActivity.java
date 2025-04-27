package crc647ab54b95e567f95c;


public abstract class SilkActivity
	extends org.libsdl.app.SDLActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_loadLibraries:()V:GetLoadLibrariesHandler\n" +
			"n_setOrientationBis:(IIZLjava/lang/String;)V:GetSetOrientationBis_IIZLjava_lang_String_Handler\n" +
			"n_onDestroy:()V:GetOnDestroyHandler\n" +
			"";
		mono.android.Runtime.register ("Silk.NET.Windowing.Sdl.Android.SilkActivity, Silk.NET.Windowing.Sdl", SilkActivity.class, __md_methods);
	}

	public SilkActivity ()
	{
		super ();
		if (getClass () == SilkActivity.class) {
			mono.android.TypeManager.Activate ("Silk.NET.Windowing.Sdl.Android.SilkActivity, Silk.NET.Windowing.Sdl", "", this, new java.lang.Object[] {  });
		}
	}

	public void loadLibraries ()
	{
		n_loadLibraries ();
	}

	private native void n_loadLibraries ();

	public void setOrientationBis (int p0, int p1, boolean p2, java.lang.String p3)
	{
		n_setOrientationBis (p0, p1, p2, p3);
	}

	private native void n_setOrientationBis (int p0, int p1, boolean p2, java.lang.String p3);

	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
