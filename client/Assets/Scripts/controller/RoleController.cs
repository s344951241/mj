using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleController :Singleton<RoleController> {

    public Dictionary<int, Player> _playerDic;

    public RoleController()
    {
        _playerDic = new Dictionary<int, Player>();

    }

    public void addPlayer(Table.Role role)
    {
        role.pos = role.pos - 1;
        if (role.id == MainRole.Instance.Id)
        {
            MainRole.Instance.Pos = role.pos;
            Debug.Log("MyPos" + role.pos);
        }
        Player player = new Player(role.id, role.name, role.pos,role.sex);
        player.Url = role.url;
        //player.Sex = role.sex;
        player.IsReady = role.ready;
        if (_playerDic.ContainsKey(role.id))
        {
            _playerDic.Remove(role.id);

        }
        Debug.Log("role:" + role.id + "|" + role.name + "|" + role.pos + "|" + role.url);
        _playerDic.Add(role.id, player);
    }

    public Player getPlayerById(int id)
    {
        return _playerDic[id];
    }

    public int getPlayerPos(int id)
    {
        return getPlayerById(id).Pos;
    }

    public Player getPlayerByPos(int pos)
    {
        foreach (var item in _playerDic)
        {
            if (item.Value.Pos == pos)
                return item.Value;
        }
        return null;
    }
    public void clear()
    {
        _playerDic.Clear();
    }
}
