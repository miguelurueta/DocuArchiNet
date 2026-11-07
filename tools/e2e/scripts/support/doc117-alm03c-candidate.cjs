'use strict';
const fs=require('node:fs'),path=require('node:path');
const scope=require('../../tests/support/doc117-alm03c-scope.cjs'),canonical=require('./doc117-alm03-canonical-description.cjs'),legacy=require('./doc117-alm03-scoped-candidate.cjs'),{ROOT,sha}=scope;
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_CANDIDATE_'+c);};
const scopeDigest=s=>sha(JSON.stringify(scope.validateScope(s))),resourceHashes=s=>{scope.validateScope(s);return[...s.resources.map(r=>sha(JSON.stringify(r))),sha(JSON.stringify({individual:s.individual,descriptionSha256:s.descriptionBinding.descriptionSha256}))];};
function renderGuard(input){
 const s=scope.validateScope(input),hashes=resourceHashes(s),values={Site:s.runtimeRoot,Run:s.runId,Lot:s.lotId,Snapshot:s.snapshotSha256,Cabinet:s.individual.cabinetName,IndividualName:s.individual.fileName,IndividualContent:s.individual.contentSha256,IndividualDigest:hashes[2],DigestA:hashes[0],DigestB:hashes[1]};
 for(let i=0;i<2;i++)for(const [key,field]of Object.entries({Root:'rootId',File:'fileId',Exp:'expedienteId',Rel:'relacionNivelId',Name:'fileName',Content:'contentSha256',Request:'requestSha256'}))values[key+(i?'B':'A')]=s.resources[i][field];
 let source=fs.readFileSync(path.join(ROOT,legacy.GUARD),'utf8');for(const [key,value]of Object.entries(values)){const pattern=new RegExp('(Private Const '+key+' As String = )"[^"]+"','g');check([...source.matchAll(pattern)].length===1,'TEMPLATE_CHANGED');source=source.replace(pattern,(_,p)=>p+'"'+value.replaceAll('"','""')+'"');}source=canonical.transformGuard(source,s.descriptionBinding);
 const header=`    Private Const CanonicalScopeSha256 As String = "${scopeDigest(s)}"
    Friend Shared Sub MarcarPreparacion()
        Try
            Dim h = HttpContext.Current
            If h Is Nothing OrElse h.Session Is Nothing OrElse Not String.Equals(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar), Path.GetFullPath(Site).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Return
            If Convert.ToString(h.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture) <> "141" OrElse Convert.ToString(h.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture) <> "2" Then Return
            If h.Request.Url.Scheme <> "https" OrElse h.Request.Url.Host <> "localhost" OrElse h.Request.Url.Port <> 443 OrElse Not String.Equals(h.Request.Url.AbsolutePath, "/GestionDocumental-Docuarchi.net/webservice/WebServiceProducion.asmx/ServiceSolicitaCargarDocumentoExpediente", StringComparison.OrdinalIgnoreCase) Then Return
            h.Response.AppendHeader("X-Doc117-Alm03c-Scope", CanonicalScopeSha256)
        Catch
            ' The client refuses dispatch without the matching readiness header.
        End Try
    End Sub
`;
 const rejection=`    Private Shared Sub RecordGuardRejection(code As String)
        Try
            Select Case code
                Case "SITE_CHANGED", "CONTEXT_CHANGED", "CALLER_CHANGED", "FILE_CHANGED", "COMMAND_CHANGED", "DESTINATION_CHANGED", "RESOURCE_CHANGED", "INDIVIDUAL_SELECTION_CHANGED", "INDIVIDUAL_ARGUMENTS_CHANGED", "CALLER_SCOPE_CHANGED", "ATTEMPT_ALREADY_PINNED", "UNBOUND_WRITER", "WRITER_ARGUMENTS_CHANGED"
                Case Else : Return
            End Select
            Dim h = HttpContext.Current
            If h Is Nothing OrElse h.Session Is Nothing OrElse Not String.Equals(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar), Path.GetFullPath(Site).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Return
            If Convert.ToString(h.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture) <> "141" OrElse Convert.ToString(h.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture) <> "2" Then Return
            If h.Request.Url.Scheme <> "https" OrElse h.Request.Url.Host <> "localhost" OrElse h.Request.Url.Port <> 443 Then Return
            If Not String.Equals(h.Request.Url.AbsolutePath, "/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx", StringComparison.OrdinalIgnoreCase) AndAlso Not String.Equals(h.Request.Url.AbsolutePath, "/GestionDocumental-Docuarchi.net/webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote", StringComparison.OrdinalIgnoreCase) Then Return
            Dim directory = Path.Combine(Site, "App_Data", "DOC117-ALM03C-REJECT-" & Run), target = Path.Combine(directory, "guard-rejection.json"), cursor = target
            Do
                If (System.IO.Directory.Exists(cursor) OrElse File.Exists(cursor)) AndAlso (File.GetAttributes(cursor) And FileAttributes.ReparsePoint) <> 0 Then Return
                If String.Equals(cursor.TrimEnd(Path.DirectorySeparatorChar), Site.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Exit Do
                cursor = Path.GetDirectoryName(cursor)
                If cursor Is Nothing Then Return
            Loop
            System.IO.Directory.CreateDirectory(directory)
            If (File.GetAttributes(directory) And FileAttributes.ReparsePoint) <> 0 Then Return
            Dim bytes = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(New With {.version = 1, .protocol = "ALM03C_GUARD_REJECTION_V1", .runId = Run, .scopeSha256 = CanonicalScopeSha256, .code = "DOC117_ALM03_GUARD_" & code}))
            Using output As New FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.Read)
                output.Write(bytes, 0, bytes.Length) : output.Flush(True)
            End Using
        Catch
            ' Never replace the original rejection or overwrite previous evidence.
        End Try
    End Sub
`;
 check(source.split('    Friend Shared Function PrepararIndividual(').length===2,'TEMPLATE_CHANGED');source=source.replace('    Friend Shared Function PrepararIndividual(',header+rejection+'    Friend Shared Function PrepararIndividual(');const failure='        Return New InvalidOperationException("DOC117_ALM03_GUARD_" & code)';check(source.split(failure).length===2,'FAILURE_CHANGED');source=source.replace(failure,'        RecordGuardRejection(code)\r\n'+failure);return'\uFEFF'+source.replace(/^\uFEFF/,'');
}
function generate(directory,s){
 const g=legacy.generate(directory,s,{validateScope:scope.validateScope,renderGuard,scopeDigest,resourceHashes}),service=path.join(g.dir,'WebServiceProducion.Laboratory.vb'),original=fs.readFileSync(path.join(ROOT,'webservice/WebServiceProducion.asmx.vb'),'utf8'),signature='    Public Function ServiceSolicitaCargarDocumentoExpediente(ByVal parameter As Object)';check(original.split(signature).length===2,'SERVICE_CHANGED');fs.writeFileSync(service,original.replace(signature,signature+'\r\n        Doc117Alm03ClosedGuard.MarcarPreparacion()'),{flag:'wx'});let targets=fs.readFileSync(g.targetsPath,'utf8');check(targets.split('</ItemGroup>').length===2,'TARGETS_CHANGED');fs.writeFileSync(g.targetsPath,targets.replace('</ItemGroup>','<Compile Remove="webservice\\WebServiceProducion.asmx.vb" /><Compile Include="'+service.replaceAll('&','&amp;').replaceAll('"','&quot;')+'" /></ItemGroup>'));return{...g,caseId:scope.CASE,preparationServiceSha256:sha(fs.readFileSync(service)),canonicalDescriptionSha256:s.descriptionBinding.descriptionSha256};
}
module.exports={ROOT,sha,scopeDigest,resourceHashes,renderGuard,generate};
