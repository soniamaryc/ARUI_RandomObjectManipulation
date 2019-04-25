using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crossHairScript : MonoBehaviour {




	public Texture2D crosshairTexture;
	public float crosshairScale = 1;
	void OnGUI()
	{
		//if not paused
		if(Time.timeScale != 0)
		{
			if(crosshairTexture!=null)
				GUI.DrawTexture(new Rect((Screen.width-crosshairTexture.width*crosshairScale)/2 ,(Screen.height-crosshairTexture.height*crosshairScale)/2, crosshairTexture.width*crosshairScale, crosshairTexture.height*crosshairScale),crosshairTexture);
			else
				Debug.Log("No crosshair texture set in the Inspector");
		}
	}

//
//	public Texture2D m_CrosshairTex;
//	Vector2 m_WindowSize;    //More like "last known window size".
//	Rect m_CrosshairRect;
//

//	void Start () {
//		m_CrosshairTex = new Texture2D(2,2);
//		m_WindowSize = new Vector2(Screen.width, Screen.height);
//		CalculateRect();
//
//	}
//
//	void Update () {
//		m_WindowSize = new Vector2(Screen.width, Screen.height);
//		if(m_WindowSize.x != Screen.width || m_WindowSize.y != Screen.height)
//		{
//			CalculateRect();
//		}
//	
//	}
//
//	void CalculateRect()
//	{
//		m_CrosshairRect = new Rect( (m_WindowSize.x - m_CrosshairTex.width)/2.0f, (m_WindowSize.y - m_CrosshairTex.height)/2.0f,m_CrosshairTex.width, m_CrosshairTex.height);
//	}
//
//	void OnGUI() {
//
//
//	
//		GUI.DrawTexture(m_CrosshairRect, m_CrosshairTex);
//
//
//	}
//
}
