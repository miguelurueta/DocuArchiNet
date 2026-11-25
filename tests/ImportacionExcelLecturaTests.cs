using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using ExcelDataReader;

//Mismo ejecutable y dependencias del harness DOC122/DOC110; sin DB/HTTP.
public static class ImportacionExcelLecturaTests {
 static int checks;
 static void Check(bool yes,string message){checks++;if(!yes)throw new Exception("DOC123 "+message);}
 static ColumnaDestinoExcel Col(string n,string t,string full=null,int len=250,string charset="utf8",bool nullable=false){return new ColumnaDestinoExcel{Nombre=n,Tipo=t,TipoCompleto=full??t,Longitud=t=="varchar"?(long?)len:null,Charset=charset,Nulable=nullable};}
 static SolicitudLecturaExcel Request(string ext,string sheet="Datos"){
  var p=new PlantillaExcel{CAMPO_UNICO="CLAVE",Modo="AGREGAR",NOMBRE_PLANTILLA="imp_02_sintetico",Campos=new List<CampoPlantillaExcel>()};
  var s=new SolicitudLecturaExcel{FormatoEsperado=ext.ToUpperInvariant(),NombreArchivo="sintetico."+ext,Hoja=sheet,Plantilla=p,Destino=new DestinoPlantillaExcel{Existe=true,Motor="InnoDB"}};
  Map(s,"Código",Col("CLAVE","varchar"));return s;
 }
 static CampoPlantillaExcel Map(SolicitudLecturaExcel s,string source,ColumnaDestinoExcel col){var c=new CampoPlantillaExcel{CAMPO_FUENTE=source,CAMPO_DESTINO=col.Nombre,TIPO_CAMPO=col.Tipo};s.Plantilla.Campos.Add(c);s.Destino.Columnas.Add(col);return c;}
 static ResultadoLecturaExcel Read(string file,SolicitudLecturaExcel s){using(var f=File.OpenRead(Path.Combine("excel-fixtures",file))){f.Position=3;var r=new ServicioLecturaExcel().Leer(f,s);Check(f.CanRead&&f.Position==3,"stream ownership/position "+file);return r;}}
 static void Has(ResultadoLecturaExcel r,string code,int? row=null,int? column=null){Check(!r.Valido&&r.Filas.Count==0,"invalid output cannot expose rows "+code);Check(r.Incidencias.Any(i=>i.Codigo==code&&(!row.HasValue||i.Fila==row)&&(!column.HasValue||i.Columna==column)),"missing "+code+" at "+row+":"+column+" got "+string.Join(",",r.Incidencias.Select(i=>i.Codigo+"@"+i.Fila+":"+i.Columna)));}
 static string Value(ResultadoLecturaExcel r,int row,string field){var v=r.Filas.Single(f=>f.Fila==row).Valores.Single(c=>c.CampoDestino==field);return v.Texto??v.NumeroCanonico??(v.Fecha.HasValue?v.Fecha.Value.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture):null);}
 static void Fidelity(){
  foreach(var ext in new[]{"xls","xlsx"}){
   using(var f=File.OpenRead(Path.Combine("excel-fixtures","fidelidad."+ext)))using(var reader=ExcelReaderFactory.CreateReader(f)){
    var raw=new List<object>();while(reader.Read())raw.Add(reader.GetValue(1));
    Check(raw[2]==null&&raw[4]==null&&raw[5]==null&&raw[7]==null,"upstream erases distinct errors/cache/NULL "+ext);
    Check(Convert.ToDouble(raw[3])==42,"stored formula cache "+ext);
   }
   var s=Request(ext,"Casos");Map(s,"Valor",Col("VALOR","varchar",nullable:true));var r=Read("fidelidad."+ext,s);
   Has(r,"ERROR_CELDA_EXCEL",3,2);Has(r,"ERROR_CELDA_EXCEL",5,2);Has(r,"FORMULA_SIN_RESULTADO",6,2);
   Check(!r.LecturaCompleta&&r.FilasDatosObservadas==5&&r.TotalIncidencias==3,"missing cache stops without inventing totals "+ext);
  }
 }
 static void Valid(){
  foreach(var ext in new[]{"xls","xlsx"})foreach(var culture in new[]{"es-CO","en-US"}){
   var prior=CultureInfo.CurrentCulture;try{
    CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo(culture);
    var s=Request(ext);Map(s,"Nombre",Col("BENEFICIARIO","varchar",len:1000,charset:"latin1"));Map(s,"Entero",Col("ENTERO","int","int(11)"));Map(s,"Decimal",Col("DECIMAL","decimal","decimal(18,2)"));Map(s,"Fecha",Col("FECHA","date"));Map(s,"TextoFecha",Col("FECHA_TEXTO","varchar"));var t=Map(s,"Transformar",Col("SEGMENTO","varchar"));t.REMPLAZA_COMA=1;t.REMPLAZA_PUNTO=1;t.OPCION_SPLITER=1;t.CARACTER_SPLITER="::";t.NUMERO_MATRIZ_EXPLITER=1;
    var r=Read("valido."+ext,s);Check(r.Valido,"valid "+ext+" "+culture+" "+string.Join(",",r.Incidencias.Select(i=>i.Codigo)));
    Check(r.Filas.Select(f=>f.Fila).SequenceEqual(new[]{2,4,5}),"physical first/last/gaps");Check(r.Hojas.SequenceEqual(new[]{"Datos","Otra"}),"sheet catalog");Check(r.Encabezados[1].Nombre==""&&r.Encabezados[2].Columna==3,"physical column gap");
    Check(Value(r,2,"CLAVE")=="00123"&&Value(r,4,"CLAVE")=="00007"&&Value(r,5,"CLAVE")=="Última","zero codes and last row");Check(Value(r,2,"BENEFICIARIO")=="O'Brian n/\\ &;#% ñ"&&Value(r,4,"BENEFICIARIO")==" segunda ","content retained");Check(Value(r,2,"ENTERO")=="2147483647"&&Value(r,4,"ENTERO")=="-2147483648","int range");Check(Value(r,2,"DECIMAL")=="1234.5"&&Value(r,4,"DECIMAL")=="-12.25","decimal typed");Check(Value(r,2,"FECHA")=="2025-12-31"&&Value(r,4,"FECHA")=="2024-02-29","date typed");Check(Value(r,4,"FECHA_TEXTO")=="texto libre","FECHA varchar stays text");Check(Value(r,2,"SEGMENTO")=="último","literal split");
    t.NUMERO_MATRIZ_EXPLITER=0;r=Read("valido."+ext,s);Check(r.Valido&&Value(r,2,"SEGMENTO")=="123","remove comma then dot then split");
   }finally{CultureInfo.CurrentCulture=prior;}
  }
 }
 internal static byte[] Book(object[][] rows,string[] headers=null){
  headers=headers??new[]{"Código","Valor"};
  var xml=new StringBuilder("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");
  var all=new[]{headers.Cast<object>().ToArray()}.Concat(rows).ToArray();
  for(int r=0;r<all.Length;r++){
   xml.Append("<row r=\"").Append(r+1).Append("\">");
   for(int c=0;c<all[r].Length;c++){
    var v=all[r][c];if(v==null)continue;string address=((char)('A'+c)).ToString()+(r+1);
    if(v is DateTime)xml.Append("<c r=\"").Append(address).Append("\" s=\"2\"><v>").Append(((DateTime)v).ToOADate().ToString("R",CultureInfo.InvariantCulture)).Append("</v></c>");
    else if(v is string)xml.Append("<c r=\"").Append(address).Append("\" t=\"inlineStr\"><is><t xml:space=\"preserve\">").Append(System.Security.SecurityElement.Escape((string)v)).Append("</t></is></c>");
    else if(v is bool)xml.Append("<c r=\"").Append(address).Append("\" t=\"b\"><v>").Append((bool)v?1:0).Append("</v></c>");
    else xml.Append("<c r=\"").Append(address).Append("\"><v>").Append(Convert.ToString(v,CultureInfo.InvariantCulture)).Append("</v></c>");
   }
   xml.Append("</row>");
  }
  xml.Append("</sheetData></worksheet>");return ReplacePart(File.ReadAllBytes("excel-fixtures/valido.xlsx"),"xl/worksheets/sheet1.xml",xml.ToString());
 }
 static byte[] ReplacePart(byte[] original,string part,string xml){
  using(var source=new MemoryStream(original))using(var zip=new ZipArchive(source,ZipArchiveMode.Read))using(var result=new MemoryStream()){
   using(var dest=new ZipArchive(result,ZipArchiveMode.Create,true))foreach(var e in zip.Entries)using(var output=dest.CreateEntry(e.FullName).Open()){
    if(e.FullName==part){var b=Encoding.UTF8.GetBytes(xml);output.Write(b,0,b.Length);}else using(var input=e.Open())input.CopyTo(output);
   }
   return result.ToArray();
  }
 }
 static ResultadoLecturaExcel ReadBytes(byte[] book,SolicitudLecturaExcel s){using(var m=new MemoryStream(book)){m.Position=3;var r=new ServicioLecturaExcel().Leer(m,s);Check(m.CanRead&&m.Position==3,"memory stream ownership on success/failure");return r;}}
 // Documento ZIP completo: el relleno es un part XML, nunca bytes tras el EOCD.
 static byte[] BookAtSize(int size){
  var original=Book(new[]{new object[]{"K","ok"}});var padding=0;
  for(int attempt=0;attempt<6;attempt++){
   byte[] bytes;
   using(var input=new MemoryStream(original))using(var source=new ZipArchive(input,ZipArchiveMode.Read))using(var output=new MemoryStream()){
    using(var zip=new ZipArchive(output,ZipArchiveMode.Create,true)){
     foreach(var entry in source.Entries)using(var from=entry.Open())using(var to=zip.CreateEntry(entry.FullName).Open())from.CopyTo(to);
     using(var to=zip.CreateEntry("doc127-capacity.xml",CompressionLevel.NoCompression).Open()){
      var open=Encoding.UTF8.GetBytes("<capacity>");to.Write(open,0,open.Length);
      var spaces=Enumerable.Repeat((byte)' ',8192).ToArray();for(int left=padding;left>0;left-=Math.Min(left,spaces.Length))to.Write(spaces,0,Math.Min(left,spaces.Length));
      var close=Encoding.UTF8.GetBytes("</capacity>");to.Write(close,0,close.Length);
     }
    }
    bytes=output.ToArray();
   }
   if(bytes.Length==size)return bytes;
   padding+=size-bytes.Length;if(padding<0)throw new Exception("DOC127 invalid exact-size fixture");
  }
  throw new Exception("DOC127 exact-size fixture did not converge");
 }
 static ResultadoLecturaExcel Scalar(object value,ColumnaDestinoExcel col,Action<CampoPlantillaExcel> configure=null){var s=Request("xlsx");var c=Map(s,"Valor",col);if(configure!=null)configure(c);return ReadBytes(Book(new[]{new object[]{"K",value}}),s);}
 static void Boundaries(){
  foreach(var ext in new[]{"xls","xlsx"}){
   var s=Request(ext,"Casos");Map(s,"Valor",Col("VALOR","varchar",nullable:true));var r=Read("cache-valido."+ext,s);Check(r.Valido&&r.FilasDatosObservadas==4,"valid cached formula/NULL "+ext);Check(Value(r,4,"VALOR")=="42"&&Value(r,7,"VALOR")=="","cached numeric and empty textual formula "+ext);Check(r.Filas.Single(x=>x.Fila==8).Valores.Single(x=>x.CampoDestino=="VALOR").EsNulo,"actual NULL remains NULL "+ext);
   s=Request(ext);s.Limites.MaxFilasDatos=3;r=Read("valido."+ext,s);Check(r.Valido&&r.Filas.Count==3,"inclusive row limit "+ext);s.Limites.MaxFilasDatos=2;r=Read("valido."+ext,s);Has(r,"LIMITE_FILAS_EXCEDIDO",5);Check(!r.LecturaCompleta&&r.FilasDatosObservadas==3,"invalid row counts; blanks do not");
   s=Request(ext,"missing");Has(Read("valido."+ext,s),"HOJA_NO_ENCONTRADA");
   s=Request(ext);s.Plantilla.Campos[0].CAMPO_FUENTE="código";Has(Read("valido."+ext,s),"ENCABEZADO_NO_ENCONTRADO",1);
   s=Request(ext);s.Limites.MaxFilasDatos=0;Has(Read("valido."+ext,s),"CONFIGURACION_INVALIDA");s.Limites.MaxFilasDatos=1;s.Limites.MaxArchivoMB=int.MaxValue;Has(Read("valido."+ext,s),"CONFIGURACION_INVALIDA");
  }
  var data=Book(new[]{new object[]{"K","ok"}});Array.Resize(ref data,1000000);var req=Request("xlsx");req.Limites.MaxArchivoMB=1;Check(ReadBytes(data,req).Valido,"exact decimal MB allowed");Array.Resize(ref data,1000001);Has(ReadBytes(data,req),"LIMITE_ARCHIVO_EXCEDIDO");
  var exact=BookAtSize(10000000);var excess=BookAtSize(10000001);
  foreach(var book in new[]{exact,excess})using(var stream=new MemoryStream(book))using(var zip=new ZipArchive(stream,ZipArchiveMode.Read))using(var xml=zip.GetEntry("doc127-capacity.xml").Open()){
   var document=new System.Xml.XmlDocument();document.Load(xml);Check(document.DocumentElement.Name=="capacity","10MB fixture retains valid XML/ZIP content");
  }
  req=Request("xlsx");var accepted=ReadBytes(exact,req);Check(accepted.Valido&&accepted.LecturaCompleta&&accepted.Filas.Count==1,"exact decimal 10MB full recognized workbook");
  req.Limites.MaxArchivoMB=11;Check(ReadBytes(excess,req).Valido,"10MB+1 workbook is valid below a larger configured limit");
  req.Limites.MaxArchivoMB=10;Has(ReadBytes(excess,req),"LIMITE_ARCHIVO_EXCEDIDO");
  foreach(var invalid in new[]{Encoding.UTF8.GetBytes("Código,Valor\nK,not Excel"),new byte[0],new byte[100]}){var r=ReadBytes(invalid,Request("xlsx"));Check(!r.Valido&&!r.LecturaCompleta&&r.Filas.Count==0,"invalid files fail closed");}
  req=Request("xls");Has(ReadBytes(Book(new[]{new object[]{"K","ok"}}),req),"FORMATO_NO_COINCIDE");
  req=Request("xlsx");req.NombreArchivo="file.xls";Has(ReadBytes(Book(new[]{new object[]{"K","ok"}}),req),"FORMATO_NO_COINCIDE");
  req=Request("xlsx");req.Plantilla.Campos.Add(null);Has(ReadBytes(Book(new[]{new object[]{"K","ok"}}),req),"CONFIGURACION_INVALIDA");
  req=Request("xlsx");Has(ReadBytes(Book(new[]{new object[]{"K","ok"}},new[]{"Código","Código"}),req),"ENCABEZADO_AMBIGUO",1);
  req=Request("xlsx");Has(ReadBytes(Book(new object[0][]),req),"LIBRO_SIN_DATOS");
  var baseBook=File.ReadAllBytes("excel-fixtures/valido.xlsx");var macro=ReplacePart(baseBook,"[Content_Types].xml","<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.ms-excel.sheet.macroEnabled.main+xml\"/></Types>");Has(ReadBytes(macro,Request("xlsx")),"FORMATO_NO_SOPORTADO");
 }
 static void Types(){
  foreach(var tc in new[]{new[]{"tinyint","-128","127","128"},new[]{"tinyint unsigned","0","255","256"},new[]{"smallint","-32768","32767","32768"},new[]{"mediumint","-8388608","8388607","8388608"},new[]{"int","-2147483648","2147483647","2147483648"},new[]{"bigint","-9223372036854775808","9223372036854775807","9223372036854775808"},new[]{"bigint unsigned","0","18446744073709551615","18446744073709551616"}}){
   foreach(var n in new[]{tc[1],tc[2]})Check(Scalar(n,Col("V",tc[0].Split(' ')[0],tc[0])).Valido,"integer physical boundary "+tc[0]+" "+n);
   Has(Scalar(tc[3],Col("V",tc[0].Split(' ')[0],tc[0])),"RANGO_EXCEDIDO");
  }
  Has(Scalar("-1",Col("V","bigint","bigint unsigned")),"RANGO_EXCEDIDO");Has(Scalar("1.5",Col("V","int")),"ENTERO_INVALIDO");
  var huge=new string('9',35)+"."+new string('1',30);var r=Scalar(huge,Col("V","decimal","decimal(65,30)"));Check(r.Valido&&Value(r,2,"V")==huge,"DECIMAL65 beyond CLR preserved");Has(Scalar("1"+huge,Col("V","decimal","decimal(65,30)")),"PRECISION_EXCEDIDA");Has(Scalar("1.001",Col("V","decimal","decimal(5,2)")),"PRECISION_EXCEDIDA");Has(Scalar("-1",Col("V","decimal","decimal(5,2) unsigned")),"PRECISION_EXCEDIDA");
  Has(Scalar("1,23",Col("V","decimal","decimal(5,2)")),"NUMERO_INVALIDO");Check(Scalar(1e20,Col("V","decimal","decimal(30,2)")).Valido,"numeric exponent normalized");Check(Scalar(0.000001,Col("V","decimal","decimal(12,8)")).Valido,"negative exponent normalized");Has(Scalar(double.NaN,Col("V","decimal","decimal(5,2)")),"NUMERO_INVALIDO");
  foreach(var d in new[]{"2024-02-29","29/02/2024",new DateTime(2024,2,29).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture)})Check(Scalar(d,Col("V","date")).Valido,"exact DATE "+d);
  foreach(var d in new[]{"2025-02-29","02/29/2024","0000-00-00","0999-12-31","2024-01-01 12:00"})Has(Scalar(d,Col("V","date")),"FECHA_INVALIDA");
  Check(Scalar(new DateTime(2024,2,29,13,14,15),Col("V","date")).Valido,"typed DATE discards time");
  r=Scalar(null,Col("V","varchar",nullable:true));Check(r.Valido&&r.Filas[0].Valores[1].EsNulo,"nullable NULL allowed");Has(Scalar(null,Col("V","varchar")),"VALOR_REQUERIDO");Has(ReadBytes(Book(new[]{new object[]{"  ","x"}}),Request("xlsx")),"CLAVE_REQUERIDA");
  Has(Scalar("abcd",Col("V","varchar",len:3)),"LONGITUD_EXCEDIDA");Has(Scalar("ñ",Col("V","varchar",charset:"ascii")),"CHARSET_INCOMPATIBLE");Has(Scalar("汉",Col("V","varchar",charset:"latin1")),"CHARSET_INCOMPATIBLE");Check(Scalar("€ñ",Col("V","varchar",charset:"latin1")).Valido,"latin1 cp1252 exact");
  Has(Scalar("😀",Col("V","varchar",charset:"utf8")),"CHARSET_INCOMPATIBLE");Check(Scalar("😀",Col("V","varchar",charset:"utf8mb4",len:1)).Valido,"utf8mb4 scalar length not UTF16");
  r=Scalar("O'Brian n/\\",Col("V","varchar"));Check(r.Valido&&Value(r,2,"V")=="O'Brian n/\\","no legacy cleanup");
  Has(Scalar("a::b",Col("V","varchar"),c=>{c.OPCION_SPLITER=1;c.CARACTER_SPLITER="::";c.NUMERO_MATRIZ_EXPLITER=2;}),"TRANSFORMACION_INVALIDA");
  r=Scalar("a::::c",Col("V","varchar",nullable:true),c=>{c.OPCION_SPLITER=1;c.CARACTER_SPLITER="::";c.NUMERO_MATRIZ_EXPLITER=1;});Check(r.Valido&&Value(r,2,"V")=="","split preserves empty segments");
  r=Scalar("a",Col("V","varchar"),c=>{c.OPCION_SPLITER=1;c.CARACTER_SPLITER="::";c.NUMERO_MATRIZ_EXPLITER=0;});Check(r.Valido&&Value(r,2,"V")=="a","absent separator index zero");
 }
 static void Reports(){
  foreach(var name in new[]{"valido","cache-valido","fidelidad"}){
   var sMini=Request("xls",name=="valido"?"Datos":"Casos");if(name!="valido")Map(sMini,"Valor",Col("V","varchar",nullable:true));var mini=Read(name+"-mini.xls",sMini);Check(name=="fidelidad"?!mini.Valido:mini.Valido,"CFB mini-stream "+name);if(name=="fidelidad")Has(mini,"ERROR_CELDA_EXCEL",3,2);
  }
  var corrupted=File.ReadAllBytes("excel-fixtures/valido.xls");int fatSid=BitConverter.ToInt32(corrupted,76);Buffer.BlockCopy(BitConverter.GetBytes((uint)0),0,corrupted,(fatSid+1)*512,4);Has(ReadBytes(corrupted,Request("xls")),"LIBRO_INVALIDO");
  var cipher=File.ReadAllBytes("excel-fixtures/valido.xls");cipher[532]=0x2f;cipher[533]=0;Has(ReadBytes(cipher,Request("xls")),"LIBRO_INVALIDO"); //FILEPASS sintético, no contraseña ni descifrado.
  var coordinates=Request("xlsx");Map(coordinates,"Valor",Col("V","varchar",len:1));var physical=Enumerable.Range(0,7).Select(i=>new object[]{"K"+i,null,i==6?"long":"x"}).ToArray();Has(ReadBytes(Book(physical,new[]{"Código","","Valor"}),coordinates),"LONGITUD_EXCEDIDA",8,3);
  var s=Request("xlsx");Map(s,"Valor",Col("V","varchar",len:1));var rows=Enumerable.Range(0,1005).Select(i=>new object[]{"K"+i,"too long"}).ToArray();var r=ReadBytes(Book(rows),s);Has(r,"LONGITUD_EXCEDIDA",2,2);Check(r.LecturaCompleta&&r.TotalIncidencias==1005&&r.Incidencias.Count==1000&&r.IncidenciasTruncadas,"bounded diagnostics exact total");
  s=Request("xlsx");var onlyIgnored=new[]{new object[]{"K",null},new object[]{null,"only ignored"},new object[]{null,null},new object[]{" ",null}};r=ReadBytes(Book(onlyIgnored),s);Has(r,"CLAVE_REQUERIDA",3,1);Check(r.FilasDatosObservadas==3,"nonmapped content and spaces count, truly empty omitted");
  s=Request("xlsx");var large=Enumerable.Range(0,10000).Select(i=>new object[]{"K"+i}).ToArray();r=ReadBytes(Book(large),s);Check(r.Valido&&r.Filas.Count==10000&&r.Filas.Last().Fila==10001,"default10000 boundary includes last row");r=ReadBytes(Book(large.Concat(new[]{new object[]{"excess"}}).ToArray()),s);Has(r,"LIMITE_FILAS_EXCEDIDO",10002);Check(!r.LecturaCompleta,"default10001 never returns prefix");
  s=Request("xlsx");s.Limites.MaxFilasDatos=20000;large=Enumerable.Range(0,20000).Select(i=>new object[]{"K"+i}).ToArray();r=ReadBytes(Book(large),s);Check(r.Valido&&r.Filas.Count==20000&&r.Filas.Last().Fila==20001,"configured20000 boundary includes last row");r=ReadBytes(Book(large.Concat(new[]{new object[]{"excess"}}).ToArray()),s);Has(r,"LIMITE_FILAS_EXCEDIDO",20002);Check(!r.LecturaCompleta&&r.Filas.Count==0,"configured20001 never returns prefix");
 }
 static AnalisisPlantillaExcel Analyze(byte[] bytes,LimitesLecturaExcel limits=null,DestinoPlantillaExcel target=null){
  using(var stream=new MemoryStream(bytes)){stream.Position=3;try{return new ServicioAnalisisPlantillaExcel().Analizar(stream,"fixture.xlsx","Datos",limits??new LimitesLecturaExcel(),target);}finally{Check(stream.CanRead&&stream.Position==3,"analysis preserves caller stream on success/failure");}}
 }
 static void AnalysisReject(byte[] bytes,string code,LimitesLecturaExcel limits=null){try{Analyze(bytes,limits);}catch(ImportacionExcelException e){Check(e.Codigo==code,"analysis expected "+code+" got "+e.Codigo);return;}throw new Exception("analysis expected "+code);}
 static void TemplateAnalysis(){
  var bytes=Book(new[]{new object[]{"00125",1.25},new object[]{"00999",12345.1}},new[]{"Número Orden","Valor"});
  var r=Analyze(bytes);Check(r.Formato=="XLSX"&&r.Hoja=="Datos"&&r.Hojas.Count==2,"analysis sheets and real format");
  Check(r.ArchivoHash.Length==64&&r.FilasObservadas==2&&r.Muestra[0][0]=="00125","analysis hash, rows and zeros");
  Check(r.Campos[0].CAMPO_FUENTE=="Número Orden"&&r.Campos[0].CAMPO_DESTINO=="NUMERO_ORDEN"&&r.Campos[0].TIPO_CAMPO.StartsWith("VARCHAR("),"source exact and SQL name proposed");
  Check(r.Campos[1].TIPO_CAMPO=="DECIMAL(7,2)"&&r.Campos.All(c=>c.ID_CAMPOS_PLANTILLA==0&&c.REMPLAZA_COMA==0&&c.REMPLAZA_PUNTO==0&&c.OPCION_SPLITER==0&&c.NUMERO_MATRIZ_EXPLITER==0),"full column precision, explicit disabled transformations for new templates");
  var same=Analyze(bytes);Check(same.ArchivoHash==r.ArchivoHash,"analysis deterministic content hash");
  var target=new DestinoPlantillaExcel{Existe=true,Motor="InnoDB",Columnas=new List<ColumnaDestinoExcel>{Col("Numero_Orden","varchar","varchar(180)"),Col("Valor","decimal","decimal(18,2)")}};
  r=Analyze(bytes,null,target);Check(r.Campos[0].CAMPO_DESTINO=="Numero_Orden"&&r.Campos[0].TIPO_CAMPO=="varchar(180)"&&r.Campos[1].TIPO_CAMPO=="decimal(18,2)","existing schema supplies actual names/types");
  r=Analyze(Book(new[]{new object[]{"A","B"}},new[]{"Número Orden","Numero-Orden"}));Check(r.Campos[0].CAMPO_DESTINO=="NUMERO_ORDEN"&&r.Campos[1].CAMPO_DESTINO=="NUMERO_ORDEN_2","normalized name collision disambiguated");
  r=Analyze(Book(new[]{new object[]{null,1},new object[]{"0001","texto"}}));Check(r.Campos[1].TIPO_CAMPO.StartsWith("VARCHAR("),"mixed column remains text");
  r=Analyze(Book(new[]{new object[]{"A",new DateTime(2025,1,2)}}));Check(r.Campos[1].TIPO_CAMPO=="DATE","typed date suggestion");
  AnalysisReject(Book(new[]{new object[]{"A",new DateTime(2025,1,2,13,14,15)}}),"FECHA_CON_HORA_NO_ADMITIDA");
  r=Analyze(Book(new[]{new object[]{"A","01/02/2025"}}));Check(r.Campos[1].TIPO_CAMPO.StartsWith("VARCHAR("),"ambiguous textual date is not guessed");
  var formatted=Book(new[]{new object[]{125,"V"}});string sheetXml;using(var zip=new ZipArchive(new MemoryStream(formatted),ZipArchiveMode.Read))using(var text=new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml").Open()))sheetXml=text.ReadToEnd();
  r=Analyze(ReplacePart(formatted,"xl/worksheets/sheet1.xml",sheetXml.Replace("<c r=\"A2\">","<c r=\"A2\" s=\"1\">")));Check(r.Muestra[0][0]=="00125"&&r.Campos[0].TIPO_CAMPO.StartsWith("VARCHAR("),"numeric cell with zero format remains text");
  AnalysisReject(ReplacePart(formatted,"xl/worksheets/sheet1.xml","<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData><row r=\"1\"><c r=\"A1\" t=\"inlineStr\"><is><t>Código</t></is></c><c r=\"IW1\" t=\"inlineStr\"><is><t>Ultima</t></is></c></row></sheetData></worksheet>"),"ENCABEZADOS_INVALIDOS");
  AnalysisReject(ReplacePart(formatted,"xl/worksheets/sheet1.xml",sheetXml.Replace("<v>125</v>","<f>1+2</f>")),"FORMULA_SIN_RESULTADO");
  r=Analyze(Book(new[]{new object[]{"A",new string('x',1000)}}));Check(r.Campos[1].TIPO_CAMPO=="VARCHAR(1250)"&&r.Muestra[0][1].Length==120,"full text length with bounded preview");
  var many=Enumerable.Range(0,20000).Select(i=>new object[]{"K"+i,"V"}).ToArray();
  r=Analyze(Book(many),new LimitesLecturaExcel{MaxFilasDatos=20000});Check(r.FilasObservadas==20000&&r.Muestra.Count==25,"analysis full20000 bounded sample25");
  AnalysisReject(Book(many.Concat(new[]{new object[]{"excess","V"}}).ToArray()),"LIMITE_FILAS_EXCEDIDO",new LimitesLecturaExcel{MaxFilasDatos=20000});
  AnalysisReject(Book(new[]{new object[]{"A","B"}},new[]{"Código","Código"}),"ENCABEZADO_AMBIGUO");
  AnalysisReject(Book(new[]{new object[]{"A","B"}},new[]{"Código",""}),"ENCABEZADOS_INVALIDOS");
  AnalysisReject(Book(new[]{new object[]{"A","B"}},new[]{"","Valor"}),"ENCABEZADOS_INVALIDOS");
  AnalysisReject(Book(new[]{new object[]{"A","B"}},new[]{new string('x',121),"Valor"}),"ENCABEZADOS_INVALIDOS");
  AnalysisReject(Book(new object[0][]),"LIBRO_SIN_DATOS");AnalysisReject(Book(new[]{new object[]{"A",new string('x',4097)}}),"LONGITUD_NO_ADMITIDA");
  using(var empty=new MemoryStream(bytes)){try{new ServicioAnalisisPlantillaExcel().Analizar(empty,"fixture.xlsx","Inexistente",new LimitesLecturaExcel());throw new Exception("missing sheet must reject");}catch(ImportacionExcelException e){Check(e.Codigo=="HOJA_NO_ENCONTRADA","analysis missing sheet");}}
  foreach(var ext in new[]{"xls","xlsx"}){
   using(var stream=File.OpenRead("excel-fixtures/cache-valido."+ext)){var a=new ServicioAnalisisPlantillaExcel().Analizar(stream,"fixture."+ext,"Casos",new LimitesLecturaExcel());Check(a.FilasObservadas==4&&a.Formato==ext.ToUpperInvariant(),"analysis reuses cached formula fixture "+ext);}
   try{using(var stream=File.OpenRead("excel-fixtures/fidelidad."+ext))new ServicioAnalisisPlantillaExcel().Analizar(stream,"fixture."+ext,"Casos",new LimitesLecturaExcel());throw new Exception("analysis fidelity must reject");}catch(ImportacionExcelException e){Check(e.Codigo=="ERROR_CELDA_EXCEL","analysis preserves formula error detection "+ext);}
  }
  var tooLarge=new byte[1000001];AnalysisReject(tooLarge,"LIMITE_ARCHIVO_EXCEDIDO",new LimitesLecturaExcel{MaxArchivoMB=1});
 }
 public static void Run(){Fidelity();Valid();Boundaries();Types();Reports();TemplateAnalysis();Console.WriteLine("DOC123_NATIVE_PASS checks="+checks+"; synthetic XLS/CFB and XLSX, no MySQL/HTTP");}
}
