using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

[ComImport,Guid("00000016-0000-0000-C000-000000000046"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDoc118MessageFilter {
 [PreserveSig]int HandleInComingCall(int kind,IntPtr caller,int ticks,IntPtr info);
 [PreserveSig]int RetryRejectedCall(IntPtr callee,int ticks,int kind);
 [PreserveSig]int MessagePending(IntPtr callee,int ticks,int kind);
}
public sealed class Doc118MessageFilter:IDoc118MessageFilter {
 [DllImport("ole32.dll")]static extern int CoRegisterMessageFilter(IDoc118MessageFilter next,out IDoc118MessageFilter previous);
 public static void Install(){IDoc118MessageFilter prior;Marshal.ThrowExceptionForHR(CoRegisterMessageFilter(new Doc118MessageFilter(),out prior));}
 public static void Remove(){IDoc118MessageFilter prior;CoRegisterMessageFilter(null,out prior);}
 public int HandleInComingCall(int kind,IntPtr caller,int ticks,IntPtr info){return 0;}
 public int RetryRejectedCall(IntPtr callee,int ticks,int kind){return kind==2&&ticks<1000?50:-1;}
 public int MessagePending(IntPtr callee,int ticks,int kind){return 2;}
}

// Owns a fresh, hidden IDE. Never adopts the user's debugger/breakpoints.
// No Stop/Terminate, expression injection, writes to debuggee or function calls.
public static class Doc118Release {
 public static void Run(Func<bool> inBreak,Action requestBreak,Action thaw,Func<bool> isFrozen,Action go,Func<bool> inRun,int milliseconds){
  if(milliseconds<=0||milliseconds>1000)throw new ArgumentOutOfRangeException();
  if(!inBreak())requestBreak();
  var until=DateTime.UtcNow.AddMilliseconds(milliseconds);
  while(!inBreak()&&DateTime.UtcNow<until)Thread.Sleep(10);
  if(!inBreak())throw new InvalidOperationException("RELEASE_BREAK_UNCONFIRMED");
  thaw();if(isFrozen())throw new InvalidOperationException("RELEASE_THAW_UNCONFIRMED");
  go();until=DateTime.UtcNow.AddMilliseconds(milliseconds);
  while(!inRun()&&DateTime.UtcNow<until)Thread.Sleep(10);
  if(!inRun())throw new InvalidOperationException("RELEASE_GO_UNCONFIRMED");
 }
}
public sealed class Doc118Debugger : IDisposable {
 EnvDTE._DTE ide;
 EnvDTE80.Debugger2 debugger;
 int processId;
 string source;
 int line;
 EnvDTE.Breakpoint breakpoint;
 EnvDTE.Thread held;
 Timer watchdog;
 long freezeStarted;
 int threadId,entriesA,entriesB,armCount;
 int expA,expB;
 string nameA,nameB,resource;
 bool released=true,closed,qualifiedSource;
 volatile bool releaseDue;
 string fault;
 DateTime processStartedUtc;
 public bool CleanupConfirmed {get;private set;}
 public string Stage {get;private set;}
 public void Initialize(int target,string sourceFile,int sourceLine) {
  Stage="CREATE_PRIVATE_IDE";
  processId=target;processStartedUtc=Process.GetProcessById(target).StartTime.ToUniversalTime();source=Path.GetFullPath(sourceFile);line=sourceLine;
  ide=(EnvDTE._DTE)Activator.CreateInstance(Type.GetTypeFromProgID("VisualStudio.DTE.18.0",true));
  Stage="CHECK_PRIVATE_IDE";
  ide.UserControl=false;ide.SuppressUI=true;ide.MainWindow.Visible=false;
  debugger=(EnvDTE80.Debugger2)ide.Debugger;
  if(!String.IsNullOrEmpty(ide.Solution.FullName)||debugger.DebuggedProcesses.Count!=0||debugger.Breakpoints.Count!=0)throw new InvalidOperationException();
  EnvDTE80.Process2 selected=null;
  Stage="FIND_TARGET_PROCESS";
  foreach(EnvDTE.Process candidate in debugger.LocalProcesses)if(candidate.ProcessID==target&&candidate.Name.EndsWith("w3wp.exe",StringComparison.OrdinalIgnoreCase))selected=(EnvDTE80.Process2)candidate;
  if(selected==null)throw new InvalidOperationException();
  Stage="ATTACH_TARGET";
  selected.Attach2(new string[]{"{449EC4CC-30D2-4032-9256-EE18EB41B62B}"});
  var until=DateTime.UtcNow.AddSeconds(10);
  while(debugger.DebuggedProcesses.Count==0&&DateTime.UtcNow<until)Thread.Sleep(100);
  if(debugger.DebuggedProcesses.Count!=1||debugger.DebuggedProcesses.Item(1).ProcessID!=processId||debugger.CurrentMode!=EnvDTE.dbgDebugMode.dbgRunMode)throw new InvalidOperationException();
  Stage="BIND_SOURCE";
  AddBreakpoint("False");
  until=DateTime.UtcNow.AddSeconds(10);
  while(!SourceBound()&&DateTime.UtcNow<until)Thread.Sleep(100);
  qualifiedSource=SourceBound();
  if(!qualifiedSource)throw new InvalidOperationException();
  Stage="INITIALIZED";
  // COM calls remain on the controller's STA thread. The timer only signals it.
  watchdog=new Timer(_=>{if(held!=null&&!released&&(Stopwatch.GetTimestamp()-freezeStarted)*1000.0/Stopwatch.Frequency>=20000)releaseDue=true;},null,100,100);
 }
 void AddBreakpoint(string condition){
  if(breakpoint!=null){breakpoint.Delete();breakpoint=null;}
  var created=debugger.Breakpoints.Add("",source,line,1,condition,EnvDTE.dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue,"Basic","",1,"",0,EnvDTE.dbgHitCountType.dbgHitCountTypeNone);
  if(created.Count!=1)throw new InvalidOperationException();breakpoint=created.Item(1);
 }
 bool SourceBound(){
  if(breakpoint==null)return false;
  try{foreach(EnvDTE.Breakpoint b in breakpoint.Children){
   if(!String.IsNullOrEmpty(b.File)&&Path.GetFullPath(b.File).Equals(source,StringComparison.OrdinalIgnoreCase)&&b.FileLine>=line&&b.FileLine<=line+2&&!String.IsNullOrEmpty(b.FunctionName)&&b.FunctionName.Contains("AlmacenamientoDocumentoProduccionDocumental"))return true;
  }}catch(COMException){}return false;
 }
 public void Arm(int a,string an,int b,string bn,string digest){
  Stage="ARM_VALIDATE";
  if(closed||!qualifiedSource||!released||armCount>=2||a<=0||b<=0||a==b||!System.Text.RegularExpressions.Regex.IsMatch(an,"^DOC118-C0[23]-A-20261107\\.pdf$")||!System.Text.RegularExpressions.Regex.IsMatch(bn,"^DOC118-C0[23]-B-20261107\\.pdf$")||!System.Text.RegularExpressions.Regex.IsMatch(digest,"^[a-f0-9]{64}$"))throw new InvalidOperationException();
  expA=a;expB=b;nameA=an;nameB=bn;resource=digest;entriesA=entriesB=0;threadId=0;held=null;fault=null;releaseDue=false;armCount++;
  Stage="ARM_ADD_BREAKPOINT";qualifiedSource=false;
  AddBreakpoint("(IdExpediente = "+a+" AndAlso NombreDocumento = \""+an+"\") OrElse (IdExpediente = "+b+" AndAlso NombreDocumento = \""+bn+"\")");
  Stage="ARM_BIND_SOURCE";
  var until=DateTime.UtcNow.AddMilliseconds(1500);
  while(!SourceBound()&&DateTime.UtcNow<until)Thread.Sleep(50);
  qualifiedSource=SourceBound();
  if(!qualifiedSource)throw new InvalidOperationException();released=false;Stage="ARMED";
 }
 string Value(string local){var value=debugger.GetExpression(local,false,500);if(!value.IsValidValue)throw new InvalidOperationException();return value.Value;}
 public void Poll(){
  if(releaseDue&&!released){fault="WATCHDOG_RELEASE";Resume();}
  if(closed||debugger.CurrentMode!=EnvDTE.dbgDebugMode.dbgBreakMode)return;
  if(resource==null||debugger.CurrentProcess.ProcessID!=processId){fault="UNEXPECTED_BREAK";Detach();return;}
  var thread=debugger.CurrentThread;
  if(!thread.StackFrames.Item(1).FunctionName.Contains("ClassAlmacenamiento.AlmacenamientoDocumentoProduccionDocumental")){fault="UNEXPECTED_BREAK";Detach();return;}
  int exp;if(!Int32.TryParse(Value("IdExpediente"),out exp)){fault="IDENTITY_UNCONFIRMED";Detach();return;}
  var name=Value("NombreDocumento");bool isA=exp==expA&&name=="\""+nameA+"\"",isB=exp==expB&&name=="\""+nameB+"\"";
  if(!isA&&!isB){fault="IDENTITY_UNCONFIRMED";Detach();return;}
  if(isA)entriesA++;else entriesB++;
  if(isA&&entriesA==1&&!released){
   held=thread;threadId=thread.ID;freezeStarted=Stopwatch.GetTimestamp();thread.Freeze();if(!thread.IsFrozen)throw new InvalidOperationException();
  }else if(!released){fault="ADDITIONAL_WRITER_INSIDE_INTERVAL";}
  debugger.Go(false);
 }
 public object Inspect(){
  bool alive=false;try{var p=Process.GetProcessById(processId);alive=p.ProcessName.Equals("w3wp",StringComparison.OrdinalIgnoreCase)&&p.StartTime.ToUniversalTime()==processStartedUtc;}catch{}
  if(!alive&&!closed)fault="TARGET_EXITED";
  bool frozen=false;try{frozen=held!=null&&held.IsFrozen;}catch{if(held!=null&&!released)fault="FROZEN_READ_UNCONFIRMED";}
  return new{sourceBound=qualifiedSource,processId=processId,workerAlive=alive,threadId=threadId,frozen=frozen,otherThreadsRunning=!closed&&alive&&debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgRunMode,released=released,resourceSha256=resource,entriesA=entriesA,entriesB=entriesB,fault=fault,closed=closed};
 }
 public void Resume(){
  if(released)return;
  if(held!=null){
   Doc118Release.Run(()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgBreakMode,
    ()=>{Stage="RELEASE_BREAK";debugger.Break(false);},
    ()=>{Stage="RELEASE_THAW";if(debugger.CurrentProcess.ProcessID!=processId||held.ID!=threadId)throw new InvalidOperationException();held.Thaw();},
    ()=>held.IsFrozen,()=>{Stage="RELEASE_GO";debugger.Go(false);},
    ()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgRunMode,1000);
  }
  released=true;releaseDue=false;Stage="RELEASED";
 }
 void Detach(){
  if(debugger==null){closed=true;CleanupConfirmed=released;return;}
  try{Resume();}finally{
   try{if(breakpoint!=null){breakpoint.Delete();breakpoint=null;}}
   finally{
    var targets=new List<EnvDTE.Process>();foreach(EnvDTE.Process process in debugger.DebuggedProcesses)if(process.ProcessID==processId)targets.Add(process);
    foreach(var process in targets)process.Detach(true);
    closed=debugger.DebuggedProcesses.Count==0;
    CleanupConfirmed=closed&&released;
   }
  }
 }
 public void Dispose(){
  try{Detach();}finally{
   // Keep the release watchdog alive until detach/release is confirmed.
   if(CleanupConfirmed&&watchdog!=null)watchdog.Dispose();
   if(ide!=null&&(debugger==null||debugger.DebuggedProcesses.Count==0)){try{ide.Quit();}catch(COMException){} }
  }
 }
}
