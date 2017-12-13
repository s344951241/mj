using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoiceMsgController : Singleton<VoiceMsgController> {

    public void voiceChat(Table.VoiceChat chat)
    {
        PluginTool.Instance.WriteVoiceToFile(chat.data, chat.index);
        PluginTool.Instance.queue.Enqueue(new ChatDate(chat.index, chat.id));

        if (!PluginTool.Instance.isPlaying)
        {
            if (PluginTool.Instance.queue.Count != 0)
            {
                PluginTool.Instance.isPlaying = true;
                ChatDate ch = PluginTool.Instance.queue.Dequeue();
                PluginTool.Instance.PlaySound(URL.localCachePath + ch.index + ".amr");
            }
        }
    }

    public void quickMsg(Table.QuickMsg msg)
    {

    }


}
