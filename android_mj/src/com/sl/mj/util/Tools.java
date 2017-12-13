package com.sl.mj.util;

import com.sl.mj.config.Config;
import com.unity3d.player.UnityPlayer;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;

public class Tools {
	
	public static void CallUnity(String method,String message)
	{
		UnityPlayer.UnitySendMessage(Config.UNITY_GAME, method, message);
	}
}
