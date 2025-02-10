using Common.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;

string dllPath = @"E:\code\c#\Hamkaran Bootcamp\Project\Producer\Dlls\SamplePlugin.dll";

Assembly assembly = Assembly.LoadFrom(dllPath);

Type[] types = assembly.GetTypes();

foreach (Type type in types)
    Console.WriteLine(type.FullName);


salam salam = new salam();
salam.Produce += Salam_Produce;

void Salam_Produce(object? sender, string e)
{
    Console.WriteLine(e);
}

class salam
{
    public event EventHandler<string> Produce;
    public salam()
    {
        if(Produce is not  null)
            Produce(this, "hello");
    }
}