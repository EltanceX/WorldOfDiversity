package crc643823cf60aa4af36d;


public class MainActivity_ViewTreeObserverListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.ViewTreeObserver.OnPreDrawListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPreDraw:()Z:GetOnPreDrawHandler:Android.Views.ViewTreeObserver/IOnPreDrawListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("SC4Android.MainActivity+ViewTreeObserverListener, Launch_android", MainActivity_ViewTreeObserverListener.class, __md_methods);
	}

	public MainActivity_ViewTreeObserverListener ()
	{
		super ();
		if (getClass () == MainActivity_ViewTreeObserverListener.class) {
			mono.android.TypeManager.Activate ("SC4Android.MainActivity+ViewTreeObserverListener, Launch_android", "", this, new java.lang.Object[] {  });
		}
	}

	public boolean onPreDraw ()
	{
		return n_onPreDraw ();
	}

	private native boolean n_onPreDraw ();

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
