using A;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    // GameManager�� ����ϴ� �ΰ��� ������ ��� �ִ� 
    // 1. ���� ���ӿ��� �߻��ϴ� �̺�Ʈ�� ó�����ִ� ����[�������� ����]�� ����
    // 2. Ingame�� �ƴ� �ܺ� Ŭ������ Event�� Notify �����ִ� ���� 

    // => ���� �� �ִ°� �����ϱ�???
    // 1. UI ó���� �����ϰ� ������ �� �� �ִ�.
    // 2. �������� �̺�Ʈ, ó���� ����������.

    // partial�̶�? 
    // 1. �ϳ��� Ŭ������ �������� *.cs ���Ϸ� ����� ������ �� �ְ� �����ִ� Ű����, // ��ġ�� Ŀ���� ���� �� �ִ�!
    // !!! ���� ���� 1000�� ������ �ڵ� ���̾�Ʈ ����� ����~! �׶� ����ϴ°��� partial ->
    public partial class GameManager : MonoBehaviour // �߰�� ����
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            AwakeVariable(); // Awake ���� ����� ���ϱ⿡ �ذ��ϴ� ���
            //AwakeEvent();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        [Button()]
        // ���������Ϳ� ������ �׽�Ʈ �ϱ� ����
        public void UseStaticItem(int slotId, int useCount, bool forceUse = false)
        {
            // ����ó��
            if (forceUse)
            {
                
            }
            else
            {
                var targetItemData = UserDataModel.Singleton.UserItemData.Items.Find(x => x.slotID == slotId);
                if (targetItemData == null)
                    return;

                //if (GameDataModel.Singleton.GetItemData(targetItemData.itemID, out var itemData))
                //    UserDataModel.Singleton.UseInventoryItem(itemData, useCount);
            }
        }

        [Button()]
        public void ChangeMonsterState(AI_Controller monsterController)
        {
            
        }

        [Button()]
        public void UseGenerateItem(Vector3 position)
        {
            //GenerateItem(position);
        }


        [Button()]
        //// ���������Ϳ� ������ �׽�Ʈ �ϱ� ����
        //public void AddItem(string itemId, int useCount, bool forceUse = false)
        //{
        //    if (GameDataModel.Singleton.GetItemData(itemId, out var itemData))
        //        UserDataModel.Singleton.AddItemToInventory(itemData, useCount);
        //}

        //[Button()]
        // ���������Ϳ� ������ �׽�Ʈ �ϱ� ����
        public void MakePlayerDamaged(float damage)
        {
            //CharacterController.Instance.linkedCharacter.eventHandler.OnDamaged(damage, null);
        }

        public void OnPlayerDead()
        {
            // # Game Over UI�� ���� ���..
            // # Dungeon Reset
            // # ...
        }
    }
}
