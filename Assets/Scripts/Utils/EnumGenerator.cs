
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

public class EnumGenerator
{

    //Create new enum from arrays
    public static System.Enum CreateEnum(List<string> list)
    {

        System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
        AssemblyName aName = new AssemblyName("Enum");
        AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);
        EnumBuilder enumerator = mb.DefineEnum("Enum", TypeAttributes.Public, typeof(int));

        int i = 0;
        enumerator.DefineLiteral("None", i); //Here = enum{ None }

        foreach (string names in list)
        {
            i++;
            enumerator.DefineLiteral(names, i);
        }

        //Here = enum { None, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

        System.Type finished = enumerator.CreateType();

        return (System.Enum)System.Enum.ToObject(finished, 0);
    }
    
    /*public static IEnumerable<T> GetValues<T>() {
        return System.Enum.GetValues(typeof(T)).Cast<T>();
    } */

}
