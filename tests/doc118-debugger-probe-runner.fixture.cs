// Compiled together with Doc118Debugger.cs to exercise its real release helper.
public sealed class Doc118ProbeResult {
 public string status="UNCONFIRMED",stage="START";
 public int processId,threadId,hresult;
 public string exceptionType;
 public bool privateIdeInitiallyEmpty,sourceBound,frozen,peerProgress,releaseConfirmed,targetCompletedOnce,cleanupConfirmed;
}
public static class Doc118ProbeRunner {
 static void Wait(Func<bool> predicate,int milliseconds){var until=DateTime.UtcNow.AddMilliseconds(milliseconds);do{try{if(predicate())return;}catch(COMException e){if(e.HResult!=unchecked((int)0x80010001)&&e.HResult!=unchecked((int)0x8001010A)&&e.HResult!=unchecked((int)0x80131302))throw;}Thread.Sleep(25);}while(DateTime.UtcNow<until);throw new InvalidOperationException();}
 static int Peer(string directory){try{int n;return Int32.TryParse(File.ReadAllText(Path.Combine(directory,"peer.txt")),out n)?n:-1;}catch(IOException){return -1;}}
 public static Doc118ProbeResult Run(int pid,string directory,string sourceFile){
  var result=new Doc118ProbeResult{processId=pid};EnvDTE._DTE ide=null;EnvDTE80.Debugger2 debugger=null;EnvDTE.Breakpoint breakpoint=null;EnvDTE.Thread held=null;
  try{
   result.stage="PIN_SYNTHETIC_PROCESS";var process=Process.GetProcessById(pid);if(process.ProcessName!="Doc118Probe")throw new InvalidOperationException();var started=process.StartTime.ToUniversalTime();
   result.stage="CREATE_PRIVATE_IDE";ide=(EnvDTE._DTE)Activator.CreateInstance(Type.GetTypeFromProgID("VisualStudio.DTE.18.0",true));ide.UserControl=false;ide.SuppressUI=true;ide.MainWindow.Visible=false;debugger=(EnvDTE80.Debugger2)ide.Debugger;
   if(!String.IsNullOrEmpty(ide.Solution.FullName)||debugger.DebuggedProcesses.Count!=0||debugger.Breakpoints.Count!=0)throw new InvalidOperationException();result.privateIdeInitiallyEmpty=true;
   result.stage="ATTACH_SYNTHETIC_ONLY";EnvDTE80.Process2 selected=null;foreach(EnvDTE.Process p in debugger.LocalProcesses)if(p.ProcessID==pid&&p.Name.EndsWith("Doc118Probe.exe",StringComparison.OrdinalIgnoreCase))selected=(EnvDTE80.Process2)p;if(selected==null)throw new InvalidOperationException();selected.Attach2(new string[]{"{449EC4CC-30D2-4032-9256-EE18EB41B62B}"});
   Wait(()=>debugger.DebuggedProcesses.Count==1&&debugger.DebuggedProcesses.Item(1).ProcessID==pid&&debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgRunMode,10000);
   result.stage="BIND_SYNTHETIC_SOURCE";sourceFile=Path.GetFullPath(sourceFile);var lines=File.ReadAllLines(sourceFile);int line=Array.FindIndex(lines,s=>s.Contains("// DOC118_PROBE_TARGET_LINE"))+1;if(line<=0)throw new InvalidOperationException();
   var created=debugger.Breakpoints.Add("",sourceFile,line,1,"",EnvDTE.dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue,"","",1,"",0,EnvDTE.dbgHitCountType.dbgHitCountTypeNone);if(created.Count!=1)throw new InvalidOperationException();breakpoint=created.Item(1);
   Wait(()=>{foreach(EnvDTE.Breakpoint b in breakpoint.Children)if(!String.IsNullOrEmpty(b.File)&&Path.GetFullPath(b.File).Equals(sourceFile,StringComparison.OrdinalIgnoreCase)&&b.FileLine>=line&&b.FileLine<=line+2)return true;return false;},10000);result.sourceBound=true;
   result.stage="ENTER_SYNTHETIC_BODY";File.WriteAllText(Path.Combine(directory,"enter.flag"),"ENTER");Wait(()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgBreakMode,10000);
   result.stage="READ_SYNTHETIC_FRAME";if(Process.GetProcessById(pid).StartTime.ToUniversalTime()!=started)throw new InvalidOperationException();
   Wait(()=>{held=debugger.CurrentThread;return debugger.CurrentProcess.ProcessID==pid&&held.StackFrames.Count>0&&held.StackFrames.Item(1).FunctionName.Contains("Doc118ReleaseProbe.ProbeTarget");},2000);
   result.stage="FREEZE_SYNTHETIC_THREAD";result.threadId=held.ID;held.Freeze();if(!held.IsFrozen)throw new InvalidOperationException();result.frozen=true;debugger.Go(false);Wait(()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgRunMode,1000);
   int before=Peer(directory);Wait(()=>Peer(directory)>=before+3,3000);if(!held.IsFrozen||held.ID!=result.threadId||debugger.CurrentMode!=EnvDTE.dbgDebugMode.dbgRunMode)throw new InvalidOperationException();result.peerProgress=true;
   result.stage="RELEASE_SYNC";Doc118Release.Run(()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgBreakMode,
    ()=>{result.stage="RELEASE_BREAK";debugger.Break(false);},
    ()=>{result.stage="RELEASE_THAW";if(debugger.CurrentProcess.ProcessID!=pid||held.ID!=result.threadId)throw new InvalidOperationException();held.Thaw();},
    ()=>held.IsFrozen,()=>{result.stage="RELEASE_GO";debugger.Go(false);},()=>debugger.CurrentMode==EnvDTE.dbgDebugMode.dbgRunMode,1000);
   result.releaseConfirmed=true;result.stage="WAIT_SYNTHETIC_COMPLETION";Wait(()=>File.Exists(Path.Combine(directory,"completed.flag")),3000);if(File.ReadAllText(Path.Combine(directory,"completed.flag"))!="1")throw new InvalidOperationException();result.targetCompletedOnce=true;result.status="RELEASE_PROBE_PASS";
  }catch(Exception e){result.status="RELEASE_PROBE_UNCONFIRMED";result.hresult=e.HResult;result.exceptionType=e.GetType().Name;}
  finally{
   try{
    // Detach the single fixture process even if its release check failed.
    try{if(breakpoint!=null)breakpoint.Delete();}finally{
     if(debugger!=null){var targets=new List<EnvDTE.Process>();foreach(EnvDTE.Process p in debugger.DebuggedProcesses){if(p.ProcessID!=pid)throw new InvalidOperationException();targets.Add(p);}foreach(var p in targets)p.Detach(true);result.cleanupConfirmed=debugger.DebuggedProcesses.Count==0;}
    }
   }catch{result.cleanupConfirmed=false;}
   if(ide!=null&&result.cleanupConfirmed){try{ide.Quit();}catch(COMException){}}
   try{File.WriteAllText(Path.Combine(directory,"stop.flag"),"STOP");}catch(IOException){}
  }
  return result;
 }
}
