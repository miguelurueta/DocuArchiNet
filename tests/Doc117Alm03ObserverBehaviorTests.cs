using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
public static class Doc117Alm03ObserverBehaviorTests {
    static Assembly Lab; static Type Core, Gate, Storage; static MethodInfo Prepare, Writer;
    static object Call(object target, MethodInfo method, params object[] args) { try { return method.Invoke(target,args); } catch(TargetInvocationException ex) { throw ex.InnerException; } }
    static void Check(bool value, string code) { if(!value) throw new Exception(code); }
    static object NewCore(string dir) { return Activator.CreateInstance(Core,BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{dir,Guid.NewGuid().ToString("N"),new[]{new string('a',64),new string('b',64),new string('c',64)}},null); }
    static object Ticket(object core,string key,string session) { return Call(core,Prepare,key,session); }
    static string Execute(object ticket,int exp,string mode="YES") {
        Gate.GetField("Current",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,ticket);
        var data=Activator.CreateInstance(Lab.GetType("stru_datos_image_lista"));data.GetType().GetField("id_registro").SetValue(data,12);
        var args=new object[]{"GAB","synthetic.pdf",mode,exp,"TYPE",79,0,null,"","","PDF",11,data};
        var result=(string)Call(Activator.CreateInstance(Storage),Writer,args);
        if(mode=="YES") { Check((int)args[11]==20000+exp,"BYREF_DOCUMENT_CHANGED");Check((int)args[12].GetType().GetField("id_registro").GetValue(args[12])==30000+exp,"BYREF_INVENTORY_CHANGED"); }
        if(mode=="EARLY")Check((int)args[11]==11,"EARLY_BYREF_CHANGED");return result;
    }
    static void Callback(Action<int> callback) { Storage.GetField("WriterAction").SetValue(null,callback); }
    static int Count { get { return (int)Storage.GetField("Calls").GetValue(null); } }
    static Exception RunThread(Action action, out Thread thread) { Exception failure=null;thread=new Thread(()=>{try{action();}catch(Exception ex){failure=ex;}});thread.Start();thread.Join();return failure; }
    static void Expect(Action action,string code) { try{action();}catch(Exception ex){Check(ex.Message.Contains(code),"WRONG_FAILURE_"+code);return;}throw new Exception("MISSING_FAILURE_"+code); }
    public static int Main(string[] args) {
      try {
        Lab=Assembly.LoadFrom(args[0]);Core=Lab.GetType("Doc117Alm03ObservationCore");Gate=Lab.GetType("Doc117Alm03TestGate");Storage=Lab.GetType("ClassAlmacenamiento");Prepare=Core.GetMethod("Preparar",BindingFlags.Instance|BindingFlags.NonPublic);Writer=Storage.GetMethod("AlmacenamientoDocumentoProduccionDocumental");
        var results=new List<object>();
        foreach(var mode in new[]{"overlap","serialized","early","caught","uncaught","exit-io","entry-io","duplicate","reservation"}) {
          Storage.GetField("Calls").SetValue(null,0);Callback(x=>{});
          string dir=Path.Combine(args[1],mode);var core=NewCore(dir);
          object a=Ticket(core,"IMPORT_A","synthetic-session-one"),b=Ticket(core,"IMPORT_B","synthetic-session-one"),individual=Ticket(core,"INDIVIDUAL","synthetic-session-two");
          if(mode=="overlap") {
            // Explicit barriers belong only to the fake writer in this test.
            using(var entered=new CountdownEvent(2)) {
              Callback(exp=>{if(exp==901)return;entered.Signal();if(!entered.Wait(5000))throw new Exception("TEST_WRITERS_SERIALIZED");});
              Exception first=null,second=null;var t1=new Thread(()=>{try{Execute(a,900);}catch(Exception ex){first=ex;}});var t2=new Thread(()=>{try{Execute(individual,902);}catch(Exception ex){second=ex;}});
              t1.Start();t2.Start();t1.Join();t2.Join();Check(first==null&&second==null,"SYNTHETIC_OVERLAP_FAILED");
            }
          } else if(mode=="entry-io") {
            Directory.CreateDirectory(Path.Combine(dir,"entry-IMPORT_A.json"));Expect(()=>Execute(a,900),"ENTRY_NOT_DURABLE");Check(Count==0,"ENTRY_FAILURE_CALLED_WRITER");results.Add(new{mode,dir});continue;
          } else if(mode=="reservation") {
            File.WriteAllText(Path.Combine(dir,"reserved-IMPORT_A.json"),"{}");Expect(()=>Execute(a,900),"RESERVATION_CHANGED");Check(Count==0,"CHANGED_RESERVATION_CALLED_WRITER");results.Add(new{mode,dir});continue;
          } else {
            if(mode=="exit-io")Directory.CreateDirectory(Path.Combine(dir,"exit-IMPORT_A.json"));
            if(mode=="uncaught")Expect(()=>Execute(a,900,"ESCAPE"),"SYNTHETIC_UNCAUGHT");
            else if(mode=="early")Check(Execute(a,900,"EARLY")=="EARLY","EARLY_RETURN_CHANGED");
            else if(mode=="caught")Check(Execute(a,900,"CATCH")=="LEGACY_REJECTED","CAUGHT_RETURN_CHANGED");
            else Check(Execute(a,900)=="YES","SUCCESS_RETURN_CHANGED");
            if(mode=="duplicate")Expect(()=>Execute(a,900),"WRITER_ALREADY_ENTERED");
            Check(Execute(individual,902)=="YES","INDIVIDUAL_RETURN_CHANGED");
          }
          Check(Execute(b,901)=="YES","SECOND_IMPORT_CHANGED");Check(Count==3,"WRITER_COUNT_CHANGED");
          foreach(var file in Directory.GetFiles(dir,"*.json"))Check(!File.ReadAllText(file).Contains("synthetic-session"),"RAW_SESSION_PERSISTED");
          results.Add(new{mode,dir});
        }
        var sessionCore=NewCore(Path.Combine(args[1],"sessions"));Ticket(sessionCore,"IMPORT_A","same");Expect(()=>Ticket(sessionCore,"INDIVIDUAL","same"),"SESSIONS_NOT_DISTINCT");Expect(()=>Ticket(sessionCore,"IMPORT_B","different"),"SESSION_CHANGED");Expect(()=>Ticket(sessionCore,"IMPORT_A","same"),"RESOURCE_ALREADY_PREPARED");
        File.WriteAllText(Path.Combine(args[1],"native-results.json"),JsonConvert.SerializeObject(results));Console.WriteLine("DOC117_ALM03_NATIVE_OBSERVER: PASS");return 0;
      } catch { Console.Error.WriteLine("DOC117_ALM03_NATIVE_OBSERVER: FAILED");return 2; }
    }
}
