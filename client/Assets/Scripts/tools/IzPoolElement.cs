
using System;

public interface IzPoolElement
{
    void InitData(Object param = null);
    void InitForUse(Object param = null);
    void Clear();
    void Destroy();
    void Recycle();

}