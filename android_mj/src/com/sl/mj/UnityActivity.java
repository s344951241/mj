package com.sl.mj;

import com.sl.mj.util.IatVoice;
import com.sl.mj.util.Tools;
import com.unity3d.player.UnityPlayerActivity;

import android.app.Activity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;

public class UnityActivity extends UnityPlayerActivity {

	private IatVoice mIatVoice;
	
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_unity);
        
    }

    public void initVoice()
    {
    	mIatVoice = new IatVoice();
    	mIatVoice.onCreate(this);
    }

    public void startRecord(String path)
    {
    	initVoice();
    	Tools.CallUnity("AddLog", "11111");
    	if(mIatVoice != null)
    	{
    		mIatVoice.startRecord();
    	}
    }

    public void stopRecord(String path)
    {
    	if(mIatVoice != null)
    	{	
    		mIatVoice.stopRecord();
    	}
    }

    public void playSound(String path)
    {
    	if(mIatVoice !=null)
    	{
    		mIatVoice.playMusic(path);
    	}
    }

    public void releaseSoundResource()
    {
    	if(mIatVoice != null)
    	{
    		mIatVoice.ReleaseSoundResource();
    	}
    }
    
    public void androidFun()
    {
    	Tools.CallUnity("BeCalledBack", "android");
    }
}
