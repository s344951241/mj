using UnityEngine;
using System.Collections;
using NetWork;

public class LoginController : Singleton<LoginController> {


    public void LoginBack(Login.LoginRsp login)
    {
        if (login.usertype != 0)
        {
            if (login.usertype == 2)
            {
                GameConst.isDaili = false;
                GameConst.isGM = true;
            }
            else if (login.usertype == 1)
            {
                GameConst.isGM = false;
                GameConst.isDaili = true;
            }
        }
        MainRole.Instance.Id = login.id;
        //MainRole.Instance.Name = login.name
        DataMgr.Instance.money = login.money;
        Debug.Log("id" + login.id);
        //QuickTips.ShowGreenQuickTips("登录成功");
        ProtoReq.EnterRoom();
        HallPanel.Instance.load();
        if (login.extmoney > 0)
        {
            //QuickTips.ShowBlueQuickTips("登录奖励钻石：" + login.extmoney);
            AlertMgr.Instance.showAlert(ALERT_TYPE.type2, "登录奖励钻石：" + login.extmoney);
        }
        //HallPanel.Instance.load();
    }
}
