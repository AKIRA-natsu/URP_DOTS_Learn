using System;

namespace AKIRA
{
    /// <summary>
    /// <para>component base</para>
    /// </summary>
    public interface IComponentData : IDisposable { }
}

/// 来源：https://gwb.tencent.com/community/detail/127381
// public class BaseResource : IDisposable
// {
//     //前面我们说了析构函数实际上是重写了 System.Object 中的虚方法 Finalize, 默认情况下,一个类是没有析构函数的,也就是说,对象被垃圾回收时不会被调用Finalize方法   
//     ~BaseResource()
//     {
//         // 为了保持代码的可读性性和可维护性,千万不要在这里写释放非托管资源的代码   
//         // 必须以Dispose(false)方式调用,以false告诉Dispose(bool disposing)函数是从垃圾回收器在调用Finalize时调用的   
//         Dispose(false);
//     }
//     // 无法被客户直接调用   
//     // 如果 disposing 是 true, 那么这个方法是被客户直接调用的,那么托管的,和非托管的资源都可以释放   
//     // 如果 disposing 是 false, 那么函数是从垃圾回收器在调用Finalize时调用的,此时不应当引用其他托管对象所以,只能释放非托管资源   
//     protected virtual void Dispose(bool disposing)
//     {
//         // 那么这个方法是被客户直接调用的,那么托管的,和非托管的资源都可以释放   
//         if (disposing)
//         {
//             // 释放 托管资源   
//             OtherManagedObject.Dispose();
//         }
//         //释放非托管资源   
//         DoUnManagedObjectDispose();
//         // 那么这个方法是被客户直接调用的,告诉垃圾回收器从Finalization队列中清除自己,从而阻止垃圾回收器调用Finalize方法.   
//         if (disposing)
//             GC.SuppressFinalize(this);
//     }
//     //可以被客户直接调用   
//     public void Dispose()
//     {
//         //必须以Dispose(true)方式调用,以true告诉Dispose(bool disposing)函数是被客户直接调用的   
//         Dispose(true);
//     }
// }