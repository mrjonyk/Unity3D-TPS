﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//包裹
public class Container : Photon.PunBehaviour
{
    //物品列表
    public List<ContainerItem> items;

    void Awake()
    {
        items = new List<ContainerItem>();

        //for test
        items.Add(new ContainerItem(101, "Ammo_Rifle", 180,0));

        items.Add(new ContainerItem(102, "Ammo_Handgun",120,0));

    }

    /*
     * 向items中添加物品
     */
    public void Add(ContainerItem item)
    {
        //单机情况下
        if(PhotonNetwork.connected == false)
        {
            //背包已有物品，则堆叠
            var containerItem = GetContainerItem(item.Id);
            if (containerItem != null)
                Put(item.Id, item.currentNum);
            else
                //否则新建
                items.Add(new ContainerItem(item.Id, item.Name, item.Maximum, item.currentNum));
            return;
        }
        else
        {
            //Debug.Log("call AdditemRPC ");
            PhotonView photonView = PhotonView.Get(this);
            if (photonView && photonView.isMine)
                photonView.RPC("AdditemRPC", PhotonTargets.All, item.Id, item.Name, item.Maximum, item.currentNum);
        }
    }
    [PunRPC]
    public void AdditemRPC(int Id, string Name, int Maximum, int currentNum)
    {
        //Debug.Log("AdditemRPC called");
        //背包已有物品，则堆叠
        var containerItem = GetContainerItem(Id);
        if (containerItem != null)
            Put(Id,currentNum);
        else
            //否则新建
            items.Add(new ContainerItem(Id, Name, Maximum,currentNum));
        return;
    }

    public void Put(int itemID, int amount)
    {
        var containerItem = items.Where(x => x.Id == itemID).FirstOrDefault();
        if (containerItem == null)
            return;
        containerItem.Set(amount);
    }

    //从容器中拿出拿出value数量物品id
    public int TakeFromContainer(int itemId, int amount)
    {

        var containerItem = GetContainerItem(itemId);
        if (containerItem == null)
            return -1;
        return containerItem.Get(amount);

    }

    public int GetAmountRemaining(int itemId)
    {
        var containerItem = GetContainerItem(itemId);
        if (containerItem == null)
            return -1;
        return containerItem.currentNum;
    }
    //从items中寻找第一个为id的ContainerItem对象
    private ContainerItem GetContainerItem(int  itemId)
    {
        //=>是 .net3.5语法，lambda表达式
        //等于什么呢 
        //foreach(ContainerItem x in items)
        //where （x.Id == id）
        //return x；
        var containerItem = items.Where(x => x.Id == itemId).FirstOrDefault();
        if (containerItem == null)
            return null;
        return containerItem;
    }
}
