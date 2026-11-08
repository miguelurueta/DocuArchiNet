using System;
using System.IO;
using System.Threading;

// Synthetic process only. No HTTP, application assemblies, DSN or documents.
public static class Doc118ReleaseProbe {
 static string directory;
 static int entries;
 public static void ProbeTarget(bool execute){
  if(!execute)return;
  Interlocked.Increment(ref entries); // DOC118_PROBE_TARGET_LINE
  File.WriteAllText(Path.Combine(directory,"completed.flag"),entries.ToString());
 }
 public static int Main(string[] args){
  if(args.Length!=1||!Path.IsPathRooted(args[0]))return 2;
  directory=args[0];ProbeTarget(false);
  var until=DateTime.UtcNow.AddMinutes(3);
  var target=new Thread(()=>{while(!File.Exists(Path.Combine(directory,"enter.flag"))&&DateTime.UtcNow<until&&!File.Exists(Path.Combine(directory,"stop.flag")))Thread.Sleep(25);if(File.Exists(Path.Combine(directory,"enter.flag")))ProbeTarget(true);});target.IsBackground=true;target.Start();
  var peer=new Thread(()=>{int count=0;while(DateTime.UtcNow<until&&!File.Exists(Path.Combine(directory,"stop.flag"))){var file=Path.Combine(directory,"peer.txt");File.WriteAllText(file+".new",(++count).ToString());if(File.Exists(file))File.Replace(file+".new",file,null);else File.Move(file+".new",file);Thread.Sleep(100);}});peer.IsBackground=true;peer.Start();
  File.WriteAllText(Path.Combine(directory,"ready.flag"),"READY");
  while(DateTime.UtcNow<until&&!File.Exists(Path.Combine(directory,"stop.flag")))Thread.Sleep(50);
  return 0;
 }
}
