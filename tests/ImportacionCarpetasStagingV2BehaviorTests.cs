using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)]
    [return:MarshalAs(UnmanagedType.Bool)] static extern bool CreateHardLinkW(string link,string original,IntPtr security);
    static object StagingV2(object target,string name,params object[] args){
        try{return target.GetType().GetMethod(name,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Invoke(target,args);}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    static object OriginalV2(object value){
        try{return value.GetType().GetProperty("Original").GetValue(value,null);}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    static void SharingV2(Action action,bool directoryRename=false){
        bool blocked=false;try{action();}catch(IOException e){var code=e.HResult&65535;if(code!=32&&code!=33&&!(directoryRename&&code==5))throw;blocked=true;}
        catch(UnauthorizedAccessException){if(!directoryRename)throw;blocked=true;}
        Check(blocked,"V2 sharing not enforced");
    }
    sealed class StagingV2Fixture:IDisposable {
        public readonly DocumentAdapterFixture Source=new DocumentAdapterFixture();
        public readonly AlmacenStagingImportacion Store;public readonly object Document;public readonly string Path;
        public StagingV2Fixture(){
            Source.Root.Revision=Math.Max(1U,Source.Root.Revision);
            Store=NewStore();Path=Source.Stage.PathFor("30");
            using(var retained=(IDisposable)StagingV2(Store,"ConservarOriginalV2",Source.Lot.Contexto))Document=V2("Preparar",Source.Prepare(),OriginalV2(retained),Source.Lot.Revision);
        }
        public AlmacenStagingImportacion NewStore(){return new AlmacenStagingImportacion(Source.Stage.Receipt.Root,Source.Stage.Receipt.Limits,Source.Lot,Source.FileRecord,10,200);}
        public object Guard(object document=null){return StagingV2(Store,"AbrirGuardiaV2",Source.Lot.Contexto,document??Document);}
        public void Dispose(){Source.Dispose();}
    }

    // Modo acotado del mismo ejecutable de pruebas; jamás ejecuta el resto del
    // harness en el hijo. Solo abre un .bin de la fixture temporal autorizada.
    public static int RunStagingV2Child(string[] args){
        try{
            if(args.Length!=4||args[0]!="--doc118-staging-v2-reader"||(args[3]!="hold"&&args[3]!="try"))return 2;
            var root=System.IO.Path.GetFullPath(args[1]);var file=System.IO.Path.GetFullPath(args[2]);
            if(System.IO.Path.GetDirectoryName(root)!=System.IO.Path.GetFullPath(System.IO.Path.GetTempPath()).TrimEnd(System.IO.Path.DirectorySeparatorChar)||!System.IO.Path.GetFileName(root).StartsWith("doc111-reception-",StringComparison.Ordinal)||!file.StartsWith(root+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||System.IO.Path.GetExtension(file)!=".bin")return 2;
            var ready=System.IO.Path.Combine(root,"v2-reader.ready");var release=System.IO.Path.Combine(root,"v2-reader.release");
            var epoch=(string)typeof(AlmacenStagingImportacion).GetField("EpocaStagingV2",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
            File.WriteAllText(System.IO.Path.Combine(root,"v2-child.epoch"),epoch);
            if(args[3]=="try"){
                try{using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read)){}return 3;}
                catch(IOException e){return (e.HResult&65535)==32||(e.HResult&65535)==33?0:2;}
            }
            using(var stream=new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read)){
                File.WriteAllText(ready,"READY");var timer=Stopwatch.StartNew();
                while(!File.Exists(release)&&timer.ElapsedMilliseconds<5000)Thread.Sleep(10);
                return File.Exists(release)?0:2;
            }
        }catch{return 2;}
    }
    static Process StartStagingV2Child(StagingV2Fixture f,string mode){
        var info=new ProcessStartInfo(Assembly.GetExecutingAssembly().Location){UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden,WorkingDirectory=f.Source.Stage.Receipt.Root};
        info.Arguments="--doc118-staging-v2-reader \""+f.Source.Stage.Receipt.Root+"\" \""+f.Path+"\" "+mode;
        info.EnvironmentVariables.Clear();foreach(var key in new[]{"PATH","PATHEXT","SystemRoot","WINDIR","TEMP","TMP"}){var value=Environment.GetEnvironmentVariable(key);if(value!=null)info.EnvironmentVariables[key]=value;}
        return Process.Start(info);
    }

    static void StagingV2Cases(){
        int cases=0,children=0,closed=0;
        Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 staging V2 case: "+name+": passed");};
        run("real_provider_retention_and_matching_guard",()=>{using(var f=new StagingV2Fixture()){
            using(var keep=(IDisposable)StagingV2(f.Store,"ConservarOriginalV2",f.Source.Lot.Contexto)){
                Rejected("OPERACION_EN_CURSO",()=>f.Guard());StagingV2(keep,"Validar");
                Rejected("OPERACION_EN_CURSO",()=>StagingV2(keep,"ConservarDurante",new Action(()=>{})));
                SharingV2(()=>{using(var w=new FileStream(f.Path,FileMode.Open,FileAccess.Write,FileShare.Read)){} });
                var copy=OriginalV2(keep);copy.GetType().GetProperty("NodoRef").SetValue(copy,new string('f',32),null);
                Check((string)OriginalV2(keep).GetType().GetProperty("NodoRef").GetValue(OriginalV2(keep),null)!=new string('f',32),"V2 aliases evidence");
            }
            using(var guard=(IDisposable)StagingV2(f.NewStore(),"AbrirGuardiaV2",f.Source.Lot.Contexto,f.Document))StagingV2(guard,"Validar");
        }});
        run("guard_blocks_reader_writer_delete_and_ancestor_rename",()=>{using(var f=new StagingV2Fixture()){
            var before=File.ReadAllBytes(f.Path);var parent=System.IO.Path.GetDirectoryName(f.Path);
            Directory.Move(parent,parent+".control");Directory.Move(parent+".control",parent);
            using(var g=(IDisposable)f.Guard()){
                SharingV2(()=>{using(var r=new FileStream(f.Path,FileMode.Open,FileAccess.Read,FileShare.Read)){} });
                SharingV2(()=>{using(var w=new FileStream(f.Path,FileMode.Open,FileAccess.Write,FileShare.ReadWrite)){} });
                SharingV2(()=>File.Delete(f.Path));SharingV2(()=>File.Move(f.Path,f.Path+".moved"));
                SharingV2(()=>Directory.Move(parent,parent+".moved"),true);StagingV2(g,"Validar");
            }Directory.Move(parent,parent+".control");Directory.Move(parent+".control",parent);
            Check(File.ReadAllBytes(f.Path).SequenceEqual(before),"V2 guard changes content");
        }});
        run("disposed_guard_and_retention_refuse_reuse",()=>{using(var f=new StagingV2Fixture()){
            foreach(var exclusive in new[]{true,false}){
                var held=exclusive?f.Guard():StagingV2(f.Store,"ConservarOriginalV2",f.Source.Lot.Contexto);
                ((IDisposable)held).Dispose();Rejected("OPERACION_EN_CURSO",()=>StagingV2(held,"Validar"));Rejected("OPERACION_EN_CURSO",()=>OriginalV2(held));((IDisposable)held).Dispose();
            }
        }});
        run("missing_original_not_created",()=>{using(var f=new StagingV2Fixture()){
            File.Move(f.Path,f.Path+".retained");var parent=System.IO.Path.GetDirectoryName(f.Path);var count=Directory.GetFiles(parent).Length;
            Rejected("RESULTADO_INCIERTO",()=>f.Guard());Check(!File.Exists(f.Path)&&Directory.GetFiles(parent).Length==count,"V2 created missing original");
        }});
        run("same_bytes_replacement_rejected_without_handle_leak",()=>{using(var f=new StagingV2Fixture()){
            File.Move(f.Path,f.Path+".retained");File.Copy(f.Path+".retained",f.Path);
            Rejected("RESULTADO_INCIERTO",()=>f.Guard());using(var exclusive=new FileStream(f.Path,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){}
            File.Move(f.Path,f.Path+".replacement");Check(File.ReadAllBytes(f.Path+".replacement").SequenceEqual(File.ReadAllBytes(f.Path+".retained")),"Rejection changes bytes");
        }});
        run("changed_content_rejected_without_handle_leak",()=>{using(var f=new StagingV2Fixture()){
            var bytes=File.ReadAllBytes(f.Path);bytes[bytes.Length/2]^=1;File.WriteAllBytes(f.Path,bytes);
            Rejected("RESULTADO_INCIERTO",()=>f.Guard());using(var exclusive=new FileStream(f.Path,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){}
            Check(File.ReadAllBytes(f.Path).SequenceEqual(bytes),"V2 rejection changes content");
        }});
        run("hardlinks_rejected_and_rejection_releases_handles",()=>{using(var f=new StagingV2Fixture()){
            var alias=f.Path+".alias";Check(CreateHardLinkW(alias,f.Path,IntPtr.Zero),"V2 fixture hardlink unavailable");
            Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Guard());File.Delete(alias);using(var g=(IDisposable)f.Guard())StagingV2(g,"Validar");
        }});
        run("junction_ancestor_rejected_without_touching_target",()=>{using(var f=new StagingV2Fixture()){
            var link=System.IO.Path.GetDirectoryName(f.Path);var target=link+".retained";Directory.Move(link,target);
            var info=new ProcessStartInfo("cmd.exe","/d /c mklink /J \""+link+"\" \""+target+"\""){UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden,RedirectStandardOutput=true,RedirectStandardError=true};
            try{
                using(var helper=Process.Start(info)){children++;helper.StandardOutput.ReadToEnd();helper.StandardError.ReadToEnd();Check(helper.WaitForExit(5000)&&helper.ExitCode==0,"V2 junction fixture unavailable");closed++;}
                Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Guard());Check(File.Exists(System.IO.Path.Combine(target,System.IO.Path.GetFileName(f.Path))),"V2 touched junction target");
            }finally{
                Check(System.IO.Path.GetFullPath(link).StartsWith(System.IO.Path.GetFullPath(f.Source.Stage.Receipt.Root)+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),"V2 junction cleanup outside fixture");
                if(Directory.Exists(link)){Check((File.GetAttributes(link)&FileAttributes.ReparsePoint)!=0,"V2 expected fixture link");Directory.Delete(link,false);}
                Directory.Move(target,link);
            }
        }});
        run("network_device_relative_and_stream_paths_refused_before_io",()=>{
            var inspect=typeof(AlmacenStagingImportacion).GetMethod("ExigirProveedorLocalV2",BindingFlags.NonPublic|BindingFlags.Static);
            foreach(var name in new[]{@"\\unsupported\share\original.bin",@"\\?\C:\original.bin",@"C:original.bin",@"C:\original.bin:stream","original.bin"})
                Rejected("CAPACIDAD_NO_DISPONIBLE",()=>{try{inspect.Invoke(null,new object[]{name});}catch(TargetInvocationException e){throw e.InnerException;}});
        });
        run("v1_and_missing_document_are_ineligible",()=>{using(var f=new StagingV2Fixture()){
            var old=V2("Leer",V2Json(f.Document));old.GetType().GetProperty("Version").SetValue(old,(ushort)1,null);
            V2Rejected(()=>f.Guard(old));V2Rejected(()=>StagingV2(f.Store,"AbrirGuardiaV2",f.Source.Lot.Contexto,null));
        }});
        run("other_epoch_is_not_certified_by_content",()=>{using(var f=new StagingV2Fixture()){
            var j=JObject.Parse(V2Json(f.Document));j["Inicio"]["Original"]["NodoRef"]=new string('f',32);
            Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Guard(V2("Leer",j.ToString())));
        }});
        run("provider_and_physical_identity_must_match",()=>{using(var f=new StagingV2Fixture()){
            foreach(var field in new[]{"ProveedorRef","IdentidadFisicaHash","ContenidoHash","Bytes"}){
                var j=JObject.Parse(V2Json(f.Document));j["Inicio"]["Original"][field]=field=="Bytes"?"1":new string('f',field=="ProveedorRef"?32:64);
                Rejected("RESULTADO_INCIERTO",()=>f.Guard(V2("Leer",j.ToString())));
            }using(var exclusive=new FileStream(f.Path,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){}
        }});
        run("scope_and_fingerprints_must_match",()=>{using(var f=new StagingV2Fixture()){
            foreach(var field in new[]{"IntentoId","LoteId","RaizId","ArchivoId","UsuarioRef","AmbitoRef","NivelRef","ContextoHash","SnapshotHash","RecepcionHash","ValoresHash"}){
                var j=JObject.Parse(V2Json(f.Document));j["Inicio"][field]=field.EndsWith("Hash",StringComparison.Ordinal)?new string('f',64):"999";
                Rejected("RESULTADO_INCIERTO",()=>f.Guard(V2("Leer",j.ToString())));
            }
        }});
        run("authority_and_active_metadata_required",()=>{using(var f=new StagingV2Fixture()){
            var context=ReceiptClone(f.Source.Lot.Contexto);context.PuedeImportar=false;Rejected("ACCESO_DENEGADO",()=>StagingV2(f.Store,"AbrirGuardiaV2",context,f.Document));
            f.Source.FileRecord.EstadoEfecto="NO_INICIADO";Rejected("SOLICITUD_INVALIDA",()=>StagingV2(f.NewStore(),"AbrirGuardiaV2",f.Source.Lot.Contexto,f.Document));
        }});
        run("missing_lock_not_created_and_busy_lock_blocks",()=>{using(var f=new StagingV2Fixture()){
            var path=System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(f.Path)),"recepcion.lock");
            using(var busy=new FileStream(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None))Rejected("OPERACION_EN_CURSO",()=>f.Guard());
            File.Move(path,path+".retained");Rejected("RESULTADO_INCIERTO",()=>f.Guard());Check(!File.Exists(path),"V2 created lock");
        }});
        run("guard_section_preserves_exclusion_on_success_and_exception",()=>{using(var f=new StagingV2Fixture())using(var g=(IDisposable)f.Guard()){
            int calls=0;StagingV2(g,"ConservarDurante",new Action(()=>{calls++;Rejected("OPERACION_EN_CURSO",()=>g.Dispose());Rejected("OPERACION_EN_CURSO",()=>StagingV2(g,"ConservarDurante",new Action(()=>{})));}));
            bool thrown=false;try{StagingV2(g,"ConservarDurante",new Action(()=>{throw new InvalidOperationException("V2_SYNTHETIC_CALLBACK");}));}catch(InvalidOperationException){thrown=true;}
            Check(calls==1&&thrown,"V2 section skipped or swallowed callback");StagingV2(g,"Validar");SharingV2(()=>{using(var r=new FileStream(f.Path,FileMode.Open,FileAccess.Read,FileShare.Read)){} });
        }});
        run("concurrent_dispose_waits_for_guard_section",()=>{using(var f=new StagingV2Fixture()){
            var g=(IDisposable)f.Guard();Task disposal=null;using(var started=new ManualResetEventSlim(false)){
                StagingV2(g,"ConservarDurante",new Action(()=>{
                    disposal=Task.Run(()=>{started.Set();g.Dispose();});Check(started.Wait(2000),"Dispose contender not scheduled");
                    Check(!disposal.Wait(50),"Dispose released an active section");SharingV2(()=>{using(var r=new FileStream(f.Path,FileMode.Open,FileAccess.Read,FileShare.Read)){} });
                }));Check(disposal.Wait(2000),"Dispose contender not completed");
            }Rejected("OPERACION_EN_CURSO",()=>StagingV2(g,"Validar"));
        }});
        run("v1_reader_also_blocks_new_guard",()=>{using(var f=new StagingV2Fixture())using(var old=f.Store.ConservarVerificado(f.Source.Lot.Contexto))Rejected("OPERACION_EN_CURSO",()=>f.Guard());});
        run("other_process_reader_blocks_then_guard_succeeds",()=>{using(var f=new StagingV2Fixture()){
            using(var child=StartStagingV2Child(f,"hold")){
                children++;var root=f.Source.Stage.Receipt.Root;var timer=Stopwatch.StartNew();
                try{
                    while(!File.Exists(System.IO.Path.Combine(root,"v2-reader.ready"))&&!child.HasExited&&timer.ElapsedMilliseconds<2500)Thread.Sleep(10);
                    Check(File.Exists(System.IO.Path.Combine(root,"v2-reader.ready"))&&!child.HasExited,"V2 child not holding");Rejected("OPERACION_EN_CURSO",()=>f.Guard());
                }finally{File.WriteAllText(System.IO.Path.Combine(root,"v2-reader.release"),"RELEASE");if(child.WaitForExit(6000)&&child.ExitCode==0)closed++;}
                Check(closed==children,"V2 child not closed");
            }
            var j=JObject.Parse(V2Json(f.Document));var childEpoch=File.ReadAllText(System.IO.Path.Combine(f.Source.Stage.Receipt.Root,"v2-child.epoch"));
            Check(childEpoch!=(string)j["Inicio"]["Original"]["NodoRef"],"V2 process epoch reused");j["Inicio"]["Original"]["NodoRef"]=childEpoch;
            Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Guard(V2("Leer",j.ToString())));
            using(var g=(IDisposable)f.Guard())StagingV2(g,"Validar");
        }});
        run("guard_blocks_late_reader_in_other_process",()=>{using(var f=new StagingV2Fixture())using(var g=(IDisposable)f.Guard())using(var child=StartStagingV2Child(f,"try")){
            children++;Check(child.WaitForExit(6000)&&child.ExitCode==0,"V2 late child reader entered");closed++;
        }});
        run("no_operational_changes_or_private_json_projection",()=>{using(var f=new StagingV2Fixture()){
            var before=PerfilImportacionJson.Escribir(new{f.Source.Lot,f.Source.Root,f.Source.FileRecord,f.Source.Attempt});using(var g=(IDisposable)f.Guard()){
                var json=PerfilImportacionJson.Escribir(OriginalV2(g));Check(!json.Contains(f.Path)&&!json.Contains(Environment.MachineName)&&!json.Contains("Token")&&!json.Contains("App_Data"),"V2 private metadata exposed");
            }Check(before==PerfilImportacionJson.Escribir(new{f.Source.Lot,f.Source.Root,f.Source.FileRecord,f.Source.Attempt})&&f.Source.Starts==0&&f.Source.Writers==0&&f.Source.Writes==0&&f.Source.Events==0,"V2 changes operational state");
        }});
        Check(cases==21&&children==3&&closed==3,"V2 staging case/child accounting changed");
        Console.WriteLine("DOC-118 V2 real staging behavior cases: 21 passed; synthetic child processes: 3 closed; no recovery activated");
    }
}
