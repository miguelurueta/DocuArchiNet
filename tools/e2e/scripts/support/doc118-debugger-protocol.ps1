if(-not ('Doc118AtomicStatus' -as [type])){
 Add-Type -TypeDefinition @'
using System.IO;
using System.Text;
public static class Doc118AtomicStatus {
 public static void Write(string path,string value){
  var temporary=path+".new";File.WriteAllText(temporary,value,new UTF8Encoding(false));
  if(File.Exists(path))File.Replace(temporary,path,null);else File.Move(temporary,path);
 }
}
'@
}
function Confirm-Doc118ObserverCommand {
 param([Parameter(Mandatory=$true)]$Command,[Parameter(Mandatory=$true)][int]$ExpectedSequence)
 if($Command -isnot [pscustomobject] -or $Command.args -isnot [pscustomobject]){throw 'DOC118_OBSERVER_COMMAND_INVALID'}
 if((@($Command.PSObject.Properties.Name|Sort-Object)-join ',') -ne 'args,operation,sequence' -or $Command.sequence -isnot [int] -or $Command.sequence -ne $ExpectedSequence){throw 'DOC118_OBSERVER_COMMAND_INVALID'}
 if($Command.operation -notin @('ARM','INSPECT','RESUME','CLOSE')){throw 'DOC118_OBSERVER_COMMAND_INVALID'}
 $keys=@($Command.args.PSObject.Properties.Name|Sort-Object)
 if($Command.operation -ne 'ARM'){if($keys.Count -ne 0){throw 'DOC118_OBSERVER_COMMAND_INVALID'};return}
 if(($keys -join ',') -ne 'expA,expB,nameA,nameB,resourceSha256'){throw 'DOC118_OBSERVER_COMMAND_INVALID'}
 foreach($key in @('expA','expB')){if($Command.args.$key -isnot [int] -or $Command.args.$key -le 0){throw 'DOC118_OBSERVER_COMMAND_INVALID'}}
 if($Command.args.expA -eq $Command.args.expB -or $Command.args.nameA -cnotmatch '^DOC118-C0[23]-A-20261107\.pdf$' -or $Command.args.nameB -cne $Command.args.nameA.Replace('-A-','-B-') -or $Command.args.resourceSha256 -cnotmatch '^[a-f0-9]{64}$'){throw 'DOC118_OBSERVER_COMMAND_INVALID'}
}
