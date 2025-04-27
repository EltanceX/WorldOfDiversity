package crc643823cf60aa4af36d;


public class MainActivity_SplashScreenOnExitAnimationListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.window.SplashScreen.OnExitAnimationListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSplashScreenExit:(Landroid/window/SplashScreenView;)V:GetOnSplashScreenExit_Landroid_window_SplashScreenView_Handler:Android.Window.ISplashScreenOnExitAnimationListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("SC4Android.MainActivity+SplashScreenOnExitAnimationListener, Launch_android", MainActivity_SplashScreenOnExitAnimationListener.class, __md_methods);
	}

	public MainActivity_SplashScreenOnExitAnimationListener ()
	{
		super ();
		if (getClass () == MainActivity_SplashScreenOnExitAnimationListener.class) {
			mono.android.TypeManager.Activate ("SC4Android.MainActivity+SplashScreenOnExitAnimationListener, Launch_android", "", this, new java.lang.Object[] {  });
		}
	}

	public void onSplashScreenExit (android.window.SplashScreenView p0)
	{
		n_onSplashScreenExit (p0);
	}

	private native void n_onSplashScreenExit (android.window.SplashScreenView p0);

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
