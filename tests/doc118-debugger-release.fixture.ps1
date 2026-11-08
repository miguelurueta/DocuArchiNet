$ErrorActionPreference='Stop'
$doc118Interop='C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Interop.dll'
Add-Type -Path $doc118Interop
$doc118Source=[IO.File]::ReadAllText((Join-Path $PSScriptRoot '../tools/e2e/scripts/support/Doc118Debugger.cs'))
$doc118Checks=@'
public static class Doc118ReleaseChecks {
 public static string[] Run(){
  var results=new List<string>();
  for(int kind=0;kind<5;kind++){
   int mode=kind==1?1:0;bool frozen=true;var calls=new List<string>();string failure=null;
   try{Doc118Release.Run(()=>mode==1,()=>{calls.Add("break");if(kind!=2)mode=1;},()=>{if(mode!=1)throw new Exception("THAW_WHILE_RUNNING");calls.Add("thaw");if(kind!=3)frozen=false;},()=>frozen,()=>{calls.Add("go");if(kind!=4)mode=0;},()=>mode==0,20);}
   catch(Exception e){failure=e.Message;}
   var expected=new[]{"break,thaw,go","thaw,go","break","break,thaw","break,thaw,go"}[kind];
   var expectedError=new string[]{null,null,"RELEASE_BREAK_UNCONFIRMED","RELEASE_THAW_UNCONFIRMED","RELEASE_GO_UNCONFIRMED"}[kind];
   if(String.Join(",",calls)!=expected||failure!=expectedError)throw new Exception("RELEASE_CHECK_FAILED");results.Add("PASS");
  }
  return results.ToArray();
 }
}
'@
Add-Type -ReferencedAssemblies $doc118Interop -TypeDefinition ($doc118Source+[Environment]::NewLine+$doc118Checks)
[Doc118ReleaseChecks]::Run()|ConvertTo-Json -Compress
