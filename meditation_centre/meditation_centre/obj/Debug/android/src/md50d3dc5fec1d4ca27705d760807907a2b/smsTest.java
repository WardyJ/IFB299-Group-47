package md50d3dc5fec1d4ca27705d760807907a2b;


public class smsTest
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("meditation_centre.smsTest, meditation_centre, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", smsTest.class, __md_methods);
	}


	public smsTest () throws java.lang.Throwable
	{
		super ();
		if (getClass () == smsTest.class)
			mono.android.TypeManager.Activate ("meditation_centre.smsTest, meditation_centre, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
