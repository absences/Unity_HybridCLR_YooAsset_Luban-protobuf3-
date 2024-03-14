using GameFramework.Fsm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmComponent : BaseGameComponent
{
    private IFsmManager m_FsmManager = null;

    /// <summary>
    /// ��ȡ����״̬��������
    /// </summary>
    public int Count
    {
        get
        {
            return m_FsmManager.Count;
        }
    }
    public IFsmManager FsmManager
    {
        get { 
            if (m_FsmManager == null)
                m_FsmManager = new FsmManager();
            return m_FsmManager;
        }
    }

    private void Update()
    {
        m_FsmManager.Update(Time.deltaTime, Time.realtimeSinceStartup);
    }
    /// <summary>
    /// ����Ƿ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <returns>�Ƿ��������״̬����</returns>
    public bool HasFsm<T>() where T : class
    {
        return m_FsmManager.HasFsm<T>();
    }

    /// <summary>
    /// ����Ƿ��������״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <returns>�Ƿ��������״̬����</returns>
    public bool HasFsm(Type ownerType)
    {
        return m_FsmManager.HasFsm(ownerType);
    }

    /// <summary>
    /// ����Ƿ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="name">����״̬�����ơ�</param>
    /// <returns>�Ƿ��������״̬����</returns>
    public bool HasFsm<T>(string name) where T : class
    {
        return m_FsmManager.HasFsm<T>(name);
    }

    /// <summary>
    /// ����Ƿ��������״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <param name="name">����״̬�����ơ�</param>
    /// <returns>�Ƿ��������״̬����</returns>
    public bool HasFsm(Type ownerType, string name)
    {
        return m_FsmManager.HasFsm(ownerType, name);
    }

    /// <summary>
    /// ��ȡ����״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <returns>Ҫ��ȡ������״̬����</returns>
    public IFsm<T> GetFsm<T>() where T : class
    {
        return m_FsmManager.GetFsm<T>();
    }

    /// <summary>
    /// ��ȡ����״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <returns>Ҫ��ȡ������״̬����</returns>
    public FsmBase GetFsm(Type ownerType)
    {
        return m_FsmManager.GetFsm(ownerType);
    }

    /// <summary>
    /// ��ȡ����״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="name">����״̬�����ơ�</param>
    /// <returns>Ҫ��ȡ������״̬����</returns>
    public IFsm<T> GetFsm<T>(string name) where T : class
    {
        return m_FsmManager.GetFsm<T>(name);
    }

    /// <summary>
    /// ��ȡ����״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <param name="name">����״̬�����ơ�</param>
    /// <returns>Ҫ��ȡ������״̬����</returns>
    public FsmBase GetFsm(Type ownerType, string name)
    {
        return m_FsmManager.GetFsm(ownerType, name);
    }

    /// <summary>
    /// ��ȡ��������״̬����
    /// </summary>
    public FsmBase[] GetAllFsms()
    {
        return m_FsmManager.GetAllFsms();
    }

    /// <summary>
    /// ��ȡ��������״̬����
    /// </summary>
    /// <param name="results">��������״̬����</param>
    public void GetAllFsms(List<FsmBase> results)
    {
        m_FsmManager.GetAllFsms(results);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="owner">����״̬�������ߡ�</param>
    /// <param name="states">����״̬��״̬���ϡ�</param>
    /// <returns>Ҫ����������״̬����</returns>
    public IFsm<T> CreateFsm<T>(T owner, params FsmState<T>[] states) where T : class
    {
        return m_FsmManager.CreateFsm(owner, states);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="name">����״̬�����ơ�</param>
    /// <param name="owner">����״̬�������ߡ�</param>
    /// <param name="states">����״̬��״̬���ϡ�</param>
    /// <returns>Ҫ����������״̬����</returns>
    public IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class
    {
        return m_FsmManager.CreateFsm(name, owner, states);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="owner">����״̬�������ߡ�</param>
    /// <param name="states">����״̬��״̬���ϡ�</param>
    /// <returns>Ҫ����������״̬����</returns>
    public IFsm<T> CreateFsm<T>(T owner, List<FsmState<T>> states) where T : class
    {
        return m_FsmManager.CreateFsm(owner, states);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="name">����״̬�����ơ�</param>
    /// <param name="owner">����״̬�������ߡ�</param>
    /// <param name="states">����״̬��״̬���ϡ�</param>
    /// <returns>Ҫ����������״̬����</returns>
    public IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class
    {
        return m_FsmManager.CreateFsm(name, owner, states);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm<T>() where T : class
    {
        return m_FsmManager.DestroyFsm<T>();
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm(Type ownerType)
    {
        return m_FsmManager.DestroyFsm(ownerType);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="name">Ҫ���ٵ�����״̬�����ơ�</param>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm<T>(string name) where T : class
    {
        return m_FsmManager.DestroyFsm<T>(name);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <param name="ownerType">����״̬�����������͡�</param>
    /// <param name="name">Ҫ���ٵ�����״̬�����ơ�</param>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm(Type ownerType, string name)
    {
        return m_FsmManager.DestroyFsm(ownerType, name);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <typeparam name="T">����״̬�����������͡�</typeparam>
    /// <param name="fsm">Ҫ���ٵ�����״̬����</param>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm<T>(IFsm<T> fsm) where T : class
    {
        return m_FsmManager.DestroyFsm(fsm);
    }

    /// <summary>
    /// ��������״̬����
    /// </summary>
    /// <param name="fsm">Ҫ���ٵ�����״̬����</param>
    /// <returns>�Ƿ���������״̬���ɹ���</returns>
    public bool DestroyFsm(FsmBase fsm)
    {
        return m_FsmManager.DestroyFsm(fsm);
    }
}
