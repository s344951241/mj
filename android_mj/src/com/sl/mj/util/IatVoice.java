package com.sl.mj.util;

import java.io.File;
import java.io.IOException;
import com.unity3d.player.UnityPlayerActivity;
import android.media.MediaPlayer;
import android.media.MediaRecorder;
import android.media.MediaPlayer.OnCompletionListener;

public class IatVoice implements OnCompletionListener {
	
	private MediaPlayer mMediaPlayer;
	private MediaRecorder mMediaRecorder;
	public String voiceDir = "";
	public String recordVoicePath = "";//杈撳叆闊抽鏂囦欢
	UnityPlayerActivity mActivity;

	public void onCreate(UnityPlayerActivity activity)
	{
		this.mActivity = activity;
		voiceDir = activity.getExternalFilesDir(null).getAbsolutePath();// + File.separator + "UnityVoice";
		recordVoicePath = voiceDir + File.separator + "wavaudio.amr";
		initMediaplayer();
	}

	private void initMediaplayer()
	{
		if(mMediaPlayer != null)
		{
			mMediaPlayer.reset();
			mMediaPlayer.release();
			mMediaPlayer = null;
		}

		mMediaPlayer = new MediaPlayer();
	}

	public void startRecord()
	{
		stopRecord();
		mMediaRecorder = new MediaRecorder();
		mMediaRecorder.setAudioSource(MediaRecorder.AudioSource.MIC);
		mMediaRecorder.setOutputFormat(MediaRecorder.OutputFormat.AMR_NB);
		//System.out.print("recordVoicePath"+recordVoicePath);
		Tools.CallUnity("AddLog", "recordVoicePath"+recordVoicePath);
		mMediaRecorder.setOutputFile(recordVoicePath);
		mMediaRecorder.setAudioEncoder(MediaRecorder.AudioEncoder.AMR_NB);

		mMediaRecorder.setMaxDuration(20000);
		mMediaRecorder.setAudioEncodingBitRate(4000);
		mMediaRecorder.setAudioSamplingRate(8000);
		
		try {
			mMediaRecorder.prepare();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			Tools.CallUnity("AddLog", "22222");
			e.printStackTrace();
		}
		catch (IllegalStateException e)
		{
			Tools.CallUnity("AddLog", e.getMessage());
			
		}
		mMediaRecorder.setOnInfoListener(new MediaRecorder.OnInfoListener()
		{
			@Override
			public void onInfo(MediaRecorder arg0, int arg1, int arg2)
			{
				if(arg1 == MediaRecorder.MEDIA_RECORDER_INFO_MAX_DURATION_REACHED)
				{
					stopRecord();
				}
			}	
		});
		mMediaRecorder.setOnErrorListener(new MediaRecorder.OnErrorListener() {
			
			@Override
			public void onError(MediaRecorder mr, int what, int extra) {
				
				
			}
		});
		mMediaRecorder.start();

		
	}

	public void stopRecord()
	{
		if(mMediaRecorder != null)
		{
			mMediaRecorder.stop();
			mMediaRecorder.release();
			mMediaRecorder = null;
			Tools.CallUnity("AND_RecordEnd", recordVoicePath);
		}
	}



	public void playMusic(String path)
	{
		//path = voiceDir + path;
		Tools.CallUnity("AddLog", path);
		try
		{
			mMediaPlayer.reset();
			mMediaPlayer.setDataSource(path);
			mMediaPlayer.setOnCompletionListener(this);
			mMediaPlayer.prepare();
			mMediaPlayer.start();
			mMediaPlayer.setLooping(false);
		} catch(IOException e)
		{
			Tools.CallUnity("AND_PlaySound", "1");//0浠ｈ〃鎴愬姛锛�1浠ｈ〃澶辫触锛屽悓IOS_PlaySound
			Tools.CallUnity("AddLog", "ex:"+e.getMessage());
		}

	}

	public void ReleaseSoundResource()
	{
		if(mMediaPlayer != null)
		{
			mMediaPlayer.stop();
		}
	}


	@Override
	public void onCompletion(MediaPlayer media)
	{
		if(mMediaPlayer != null)
		{
			Tools.CallUnity("AND_PlaySound", "0");//0
		}
	}
}
