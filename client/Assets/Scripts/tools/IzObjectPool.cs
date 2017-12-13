
//不再被继承
using System;
using System.Collections.Generic;

public sealed class IzObjectPool<T> where T:IzPoolElement,new()
{
    private Stack<T> _objectStack;
    private int _poolSize;
    private Object _objParam;

    public IzObjectPool(int poolSize,Object initParam = null)
    {
        _poolSize = poolSize;
        _objParam = initParam;
        _objectStack = new Stack<T>(poolSize);

    }

    private void ExtendObjectPool()
    {


    }

    public int poolSize{
        get{
            return _poolSize;
        }

    }

    public int CurrentElementCount
    {

        get{
            return _objectStack.Count;
        }
    }

    public T FetchObject(Object param = null)
    {

        if(param==null)
            param = _objParam;
        T result;
        if(_objectStack.Count>=1)
        {
            result = _objectStack.Pop();
            result.InitForUse(param);
        }
        else
        {
            result = new T();
            result.InitData(_objParam);
            result.InitForUse(param);

        }
        return result;
    }

    public void ReturnObject(T retObj)
    {
        if(retObj!=null)
        {
            retObj.Clear();
            _objectStack.Push(retObj);

        }
    }

    public void RefreshPool(Object initParam = null)
    {
        _objectStack.Clear();
        for(int i=0;i<_poolSize;i++)
        {
            T temp = new T();
            _objectStack.Push(temp);
            temp.InitData(initParam);
        }
    }

    public void Release()
    {
        int cnt = _objectStack.Count;
        T temp;
        for(int i=0;i<cnt;i++)
        {
            temp = _objectStack.Pop();
            temp.Destroy();
        }
        _objectStack.Clear();
    }
}
