using System;
using System.Collections.Generic;
using System.Linq;

public static class PerfilImportacionMigrationBehaviorTests {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        var rule=new ReglaPerfilImportacion {ReglaId=new string('a',32),DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="CODIGO",FuenteTipo="CONSTANTE",ValorConstante="Órdenes 00125"};
        var version=new VersionPerfilImportacion {Configuracion=new ConfiguracionPerfilImportacion {NombreVisible="Migración áé"},Reglas=new List<ReglaPerfilImportacion>{rule}};
        var json=PerfilImportacionJson.Escribir(rule);
        var before=PerfilImportacionJson.Huella(version);
        Check(before.SequenceEqual(Doc110MigrationJson.Hash(PerfilImportacionJson.Escribir(version.Configuracion),new[]{json})),"migration hash equals production canonicalization");
        var converted=Doc110MigrationJson.ConvertRule(json,rule.ReglaId,"4294967295");
        var next=PerfilImportacionJson.Leer<ReglaPerfilImportacion>(converted);
        Check(next.ReglaId=="4294967295" && next.ValorConstante==rule.ValorConstante,"changes only identity, preserves Unicode and zeros");
        version.Reglas[0]=next;
        var after=PerfilImportacionJson.Huella(version);
        Check(!before.SequenceEqual(after),"identity participates in hash");
        Check(after.SequenceEqual(Doc110MigrationJson.Hash(PerfilImportacionJson.Escribir(version.Configuracion),new[]{converted})),"converted hash verifiable by production");
        Check(rule.ReglaId==new string('a',32),"source snapshot unchanged");
        foreach(var bad in new[]{"0","01","-1","4294967296",new string('b',32)}){
            bool rejected=false;try{Doc110MigrationJson.ConvertRule(json,rule.ReglaId,bad);}catch(InvalidOperationException){rejected=true;}
            Check(rejected,"invalid incremental identity rejected");
        }
        bool mismatch=false;try{Doc110MigrationJson.ConvertRule(json,new string('c',32),"1");}catch(InvalidOperationException){mismatch=true;}
        Check(mismatch,"unexpected JSON identity blocks migration");
    }
}
