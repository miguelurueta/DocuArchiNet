using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Microsoft.Win32.SafeHandles;

// Local synthetic files only. This is a capability probe, not a product port.
public static class Doc118StagingProbe {
 [StructLayout(LayoutKind.Sequential)]struct FileInformation {
  public uint attributes;
  public System.Runtime.InteropServices.ComTypes.FILETIME created,accessed,written;
  public uint volume,sizeHigh,sizeLow,links,indexHigh,indexLow;
 }
 [DllImport("kernel32.dll",SetLastError=true)][return:MarshalAs(UnmanagedType.Bool)]static extern bool GetFileInformationByHandle(SafeFileHandle handle,out FileInformation information);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)][return:MarshalAs(UnmanagedType.Bool)]static extern bool GetVolumeInformationW(string root,StringBuilder name,uint nameLength,out uint serial,out uint componentLength,out uint flags,StringBuilder system,uint systemLength);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode)]static extern uint GetDriveTypeW(string root);
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)][return:MarshalAs(UnmanagedType.Bool)]static extern bool CreateHardLinkW(string link,string existing,IntPtr security);
 sealed class Proof {public string identity,hash;public long bytes;}
 sealed class Guard:IDisposable {
  FileStream stream;Proof expected;
  public Guard(FileStream value,Proof proof){stream=value;expected=proof;}
  public void Validate(){if(stream==null)throw new InvalidOperationException("GUARD_DISPOSED");if(Identity(stream)!=expected.identity||stream.Length!=expected.bytes||Hash(stream)!=expected.hash)throw new InvalidOperationException("RESOURCE_CHANGED");}
  public void Dispose(){var prior=stream;stream=null;if(prior!=null)prior.Dispose();}
 }
 sealed class Child {public Process process;public string directory,mode;}
 public sealed class CaseResult {public string name,code;public bool passed;public long elapsedMilliseconds;}
 public sealed class Result {
  public string issueKey="DOC-118",scope="LOCAL_SYNTHETIC_FILES_ONLY",status="UNCONFIRMED",recordedUtc=DateTime.UtcNow.ToString("o"),fileSystem,stage="START";
  public uint driveType;
  public bool cleanupConfirmed,localPrimitivesQualified,productionProviderQualified=false,providerPortImplemented=false,legacyRuntimeQualified=false,requiresDurableFence=true,fileIdIsPermanent=false,crossNodeIdentityProven=false;
  public int childrenStarted,childrenExited,appLoginAttempts=0,httpRequests=0,selectStatements=0,businessEffects=0;
  public List<CaseResult> cases=new List<CaseResult>();
 }
 static string root,workspace;static readonly List<Child> children=new List<Child>();
 static byte[] Bytes(){var value=new byte[4096];for(int i=0;i<value.Length;i++)value[i]=(byte)(i%251);return value;}
 static void Check(bool value,string code){if(!value)throw new InvalidOperationException(code);}
 static string Hash(FileStream stream){stream.Position=0;using(var sha=SHA256.Create()){var value=BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant();stream.Position=0;return value;}}
 static string HashFile(string file){using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read))return Hash(stream);}
 static string Identity(FileStream stream){FileInformation info;if(!GetFileInformationByHandle(stream.SafeFileHandle,out info)||info.links!=1||(info.indexHigh==0&&info.indexLow==0))throw new InvalidOperationException("IDENTITY_UNAVAILABLE");return info.volume.ToString("x8")+":"+info.indexHigh.ToString("x8")+info.indexLow.ToString("x8")+":"+info.created.dwHighDateTime.ToString("x8")+info.created.dwLowDateTime.ToString("x8");}
 static void LocalScope(string file,string directory){
  Check(Path.IsPathRooted(file)&&!file.StartsWith(@"\\",StringComparison.Ordinal),"UNSUPPORTED_PATH");var full=Path.GetFullPath(file);var expected=Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;Check(full.StartsWith(expected,StringComparison.OrdinalIgnoreCase),"OUTSIDE_FIXTURE");
  for(string current=full;!String.IsNullOrEmpty(current);current=Path.GetDirectoryName(current)){if(File.Exists(current)||Directory.Exists(current))Check((File.GetAttributes(current)&FileAttributes.ReparsePoint)==0,"LINK_NOT_ALLOWED");}
 }
 static Proof Observe(string file){using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read))return new Proof{identity=Identity(stream),hash=Hash(stream),bytes=stream.Length};}
 static Guard Exclusive(string file,string directory,Proof expected){
  LocalScope(file,directory);FileStream stream=null;
  try{stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.None);Check(!stream.CanWrite,"WRITABLE_GUARD");Check(Identity(stream)==expected.identity,"IDENTITY_MISMATCH");Check(stream.Length==expected.bytes&&Hash(stream)==expected.hash,"CONTENT_MISMATCH");var guard=new Guard(stream,expected);stream=null;return guard;}
  finally{if(stream!=null)stream.Dispose();}
 }
 static bool Sharing(IOException e){int code=e.HResult&65535;return code==32||code==33;}
 static void ExpectBlocked(Action action){try{action();}catch(IOException e){if(Sharing(e))return;throw;}throw new InvalidOperationException("SHARING_NOT_ENFORCED");}
 static void ExpectCode(Action action,string code){try{action();}catch(InvalidOperationException e){if(e.Message==code)return;throw;}throw new InvalidOperationException("REJECTION_NOT_ENFORCED");}
 static void Wait(Func<bool> predicate,int milliseconds){var timer=Stopwatch.StartNew();while(!predicate()&&timer.ElapsedMilliseconds<milliseconds)Thread.Sleep(10);Check(predicate(),"HANDSHAKE_TIMEOUT");}
 static string DirectoryFor(string name){var directory=Path.Combine(root,name);LocalScope(Path.Combine(directory,"original.bin"),root);Directory.CreateDirectory(directory);return directory;}
 static string Original(string directory){var file=Path.Combine(directory,"original.bin");File.WriteAllBytes(file,Bytes());return file;}
 static Child Start(string directory,string mode){
  var info=new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);info.Arguments="child \""+root+"\" \""+workspace+"\" \""+Path.GetFileName(directory)+"\" "+mode;info.UseShellExecute=false;info.CreateNoWindow=true;info.WindowStyle=ProcessWindowStyle.Hidden;info.WorkingDirectory=directory;
  info.EnvironmentVariables.Clear();foreach(var key in new[]{"PATH","SystemRoot","WINDIR","TEMP","TMP"}){var value=Environment.GetEnvironmentVariable(key);if(value!=null)info.EnvironmentVariables[key]=value;}
  var child=new Child{process=Process.Start(info),directory=directory,mode=mode};children.Add(child);Wait(()=>File.Exists(Path.Combine(directory,mode+".ready")),5000);return child;
 }
 static void Finish(Child child){Check(child.process.WaitForExit(5000)&&child.process.ExitCode==0,"CHILD_NOT_CLOSED");}
 static void Signal(Child child,string name){File.WriteAllText(Path.Combine(child.directory,name+".flag"),"SIGNAL");}
 static string Outcome(Child child){Finish(child);return File.ReadAllText(Path.Combine(child.directory,child.mode+".done"));}
 static int ChildMain(string caseName,string mode){
  string directory=Path.Combine(root,caseName),file=Path.Combine(directory,"original.bin");
  try{
   LocalScope(file,root);Check(mode=="hold"||mode=="exit"||mode=="reader"||mode=="exclusive","INVALID_MODE");var limit=Stopwatch.StartNew();
   if(mode=="hold"||mode=="exit"){
    using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read)){
     File.WriteAllText(Path.Combine(directory,mode+".ready"),"READY");
     while(limit.ElapsedMilliseconds<20000&&!File.Exists(Path.Combine(directory,"release.flag"))&&!File.Exists(Path.Combine(directory,"stop.flag"))){if(mode=="exit"&&File.Exists(Path.Combine(directory,"exit.flag")))Environment.Exit(0);Thread.Sleep(10);}
    }
    File.WriteAllText(Path.Combine(directory,mode+".done"),"RELEASED");return 0;
   }
   Check(mode=="reader"||mode=="exclusive","INVALID_MODE");File.WriteAllText(Path.Combine(directory,mode+".ready"),"READY");
   Wait(()=>File.Exists(Path.Combine(directory,"enter.flag"))||File.Exists(Path.Combine(directory,"stop.flag")),10000);
   string outcome="ACQUIRED";try{using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,mode=="reader"?FileShare.Read:FileShare.None)){} }catch(IOException e){if(!Sharing(e))throw;outcome="SHARING_BLOCKED";}
   File.WriteAllText(Path.Combine(directory,mode+".done"),outcome);return 0;
  }catch{try{File.WriteAllText(Path.Combine(directory,mode+".done"),"UNCONFIRMED");}catch{}return 2;}
 }
 static void RunCase(Result result,string name,Action action){var timer=Stopwatch.StartNew();var value=new CaseResult{name=name,code="UNCONFIRMED"};try{action();value.passed=true;value.code="PASS";}catch{value.passed=false;}value.elapsedMilliseconds=timer.ElapsedMilliseconds;result.cases.Add(value);}
 public static int Main(string[] args){
  if(args.Length<3)return 2;root=Path.GetFullPath(args[1]);workspace=Path.GetFullPath(args[2]);if(!root.StartsWith(Path.Combine(workspace,".tmp")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(root),"^doc118-staging-lock-probe-[a-f0-9]{32}$"))return 2;
  if(args[0]=="child")return args.Length==5?ChildMain(args[3],args[4]):2;
  var result=new Result();
  try{
   Check(args[0]=="run","INVALID_MODE");LocalScope(Path.Combine(root,"scope.bin"),workspace);result.stage="INSPECT_LOCAL_PROVIDER";
   uint serial,component,flags;var system=new StringBuilder(64);var volume=Path.GetPathRoot(root);Check(GetVolumeInformationW(volume,null,0,out serial,out component,out flags,system,64),"VOLUME_UNAVAILABLE");result.fileSystem=system.ToString();result.driveType=GetDriveTypeW(volume);
   Check(result.driveType==3&&result.fileSystem=="NTFS","PROVIDER_NOT_SUPPORTED");result.stage="RUN_SYNTHETIC_CASES";
   RunCase(result,"active_reader_blocks_recovery",()=>{string directory=DirectoryFor("active"),file=Original(directory);var proof=Observe(file);var child=Start(directory,"hold");var time=Stopwatch.StartNew();ExpectBlocked(()=>{using(var guard=Exclusive(file,directory,proof)){} });Check(time.ElapsedMilliseconds<1000,"ACQUISITION_WAITED");Signal(child,"release");Finish(child);Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"graceful_exit_allows_same_original",()=>{string directory=DirectoryFor("released"),file=Original(directory);var proof=Observe(file);var child=Start(directory,"hold");Signal(child,"release");Check(Outcome(child)=="RELEASED","CHILD_NOT_RELEASED");using(var guard=Exclusive(file,directory,proof))guard.Validate();Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"process_exit_releases_handle",()=>{string directory=DirectoryFor("exited"),file=Original(directory);var proof=Observe(file);var child=Start(directory,"exit");ExpectBlocked(()=>{using(var guard=Exclusive(file,directory,proof)){} });Signal(child,"exit");Finish(child);using(var guard=Exclusive(file,directory,proof))guard.Validate();Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"exclusive_guard_blocks_late_reader_and_contender",()=>{string directory=DirectoryFor("late"),file=Original(directory);var proof=Observe(file);using(var guard=Exclusive(file,directory,proof)){Child reader=Start(directory,"reader"),contender=Start(directory,"exclusive");Signal(reader,"enter");Check(Outcome(reader)=="SHARING_BLOCKED"&&Outcome(contender)=="SHARING_BLOCKED","LATE_OPEN_ALLOWED");ExpectBlocked(()=>{using(var stream=new FileStream(file,FileMode.Open,FileAccess.Write,FileShare.ReadWrite)){} });guard.Validate();}Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"identical_copy_has_different_identity",()=>{string directory=DirectoryFor("copy"),file=Original(directory),other=Path.Combine(directory,"copy.bin");var proof=Observe(file);File.WriteAllBytes(other,Bytes());Check(HashFile(other)==proof.hash,"FIXTURE_HASH_DIFFERENT");ExpectCode(()=>{using(var guard=Exclusive(other,directory,proof)){} },"IDENTITY_MISMATCH");using(var stream=new FileStream(other,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){} Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"hardlink_alias_rejected",()=>{string directory=DirectoryFor("alias"),file=Original(directory),alias=Path.Combine(directory,"alias.bin");var proof=Observe(file);Check(CreateHardLinkW(alias,file,IntPtr.Zero),"HARDLINK_FIXTURE_UNAVAILABLE");ExpectCode(()=>{using(var guard=Exclusive(alias,directory,proof)){} },"IDENTITY_UNAVAILABLE");Check(HashFile(alias)==proof.hash&&HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"same_path_replacement_rejected",()=>{string directory=DirectoryFor("replacement"),file=Original(directory);var proof=Observe(file);File.Move(file,Path.Combine(directory,"retained.bin"));File.WriteAllBytes(file,Bytes());ExpectCode(()=>{using(var guard=Exclusive(file,directory,proof)){} },"IDENTITY_MISMATCH");Check(HashFile(file)==proof.hash&&HashFile(Path.Combine(directory,"retained.bin"))==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"missing_original_not_created",()=>{string directory=DirectoryFor("missing"),file=Path.Combine(directory,"original.bin");bool refused=false;try{using(var guard=Exclusive(file,directory,new Proof())){} }catch(FileNotFoundException){refused=true;}Check(refused&&!File.Exists(file)&&Directory.GetFiles(directory).Length==0,"MISSING_WAS_CREATED");});
   RunCase(result,"changed_content_rejected_without_handle_leak",()=>{string directory=DirectoryFor("content"),file=Original(directory);var proof=Observe(file);var bytes=Bytes();bytes[0]=255;File.WriteAllBytes(file,bytes);var before=HashFile(file);ExpectCode(()=>{using(var guard=Exclusive(file,directory,proof)){} },"CONTENT_MISMATCH");using(var stream=new FileStream(file,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){} Check(HashFile(file)==before,"REJECTION_CHANGED_CONTENT");});
   RunCase(result,"disposed_guard_cannot_be_reused",()=>{string directory=DirectoryFor("disposed"),file=Original(directory);var guard=Exclusive(file,directory,Observe(file));guard.Dispose();ExpectCode(()=>guard.Validate(),"GUARD_DISPOSED");});
   RunCase(result,"reader_after_release_requires_durable_fence",()=>{string directory=DirectoryFor("fence"),file=Original(directory);var proof=Observe(file);using(var guard=Exclusive(file,directory,proof))guard.Validate();var child=Start(directory,"reader");Signal(child,"enter");Check(Outcome(child)=="ACQUIRED","POSITIVE_CONTROL_FAILED");Check(HashFile(file)==proof.hash,"CONTENT_CHANGED");});
   RunCase(result,"network_path_refused_without_open",()=>{ExpectCode(()=>LocalScope(@"\\unsupported\share\original.bin",root),"UNSUPPORTED_PATH");});
  }catch{result.stage="PROBE_UNCONFIRMED";}
  finally{
   result.childrenStarted=children.Count;foreach(var child in children){try{if(!child.process.HasExited)Signal(child,"stop");if(child.process.WaitForExit(5000))result.childrenExited++;}catch{}}
   result.cleanupConfirmed=result.childrenStarted==result.childrenExited;
  }
  result.localPrimitivesQualified=result.fileSystem=="NTFS"&&result.driveType==3&&result.cases.Count==12&&result.cases.TrueForAll(c=>c.passed)&&result.cleanupConfirmed;result.status=result.localPrimitivesQualified?"PASS":"UNCONFIRMED";
  Console.WriteLine(new JavaScriptSerializer().Serialize(result));return result.localPrimitivesQualified?0:2;
 }
}
