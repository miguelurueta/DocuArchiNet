'use strict';
async function install(page){
 await page.evaluate(()=>{
  const parts=new Map(),validated=new Set();let header=null,prepRev=1,sealed=false;
  const status=()=>({existe:true,preparacionId:'77',revision:prepRev,estado:sealed?'SELLADA':'BORRADOR',planId:sealed?'45':null,totalRaices:header.totalRaices,totalArchivos:header.totalArchivos,partesRecibidas:parts.size,partesValidadas:validated.size,
   archivosRegistrados:Array.from(parts.values()).reduce((n,p)=>n+p.archivos.length,0),archivosRevisados:Array.from(parts.entries()).filter(([key])=>validated.has(key)).reduce((n,[,p])=>n+p.archivos.length,0)});
  function summary(type='RESUMEN',page=1){return{planId:'45',revision:1,planHash:'a'.repeat(64),nivelRef:'7',expiraUtc:'2099-01-01T00:00:00Z',puedeConfirmar:true,unidadesElegibles:manifest.raices.length,totalArchivos:manifest.archivos.length,totalIncidencias:0,tipo:type,pagina:page,puedeHaberMas:false,valores:[],incidencias:[],unidadesElegiblesPagina:[]};}
  window.doc120ExtraReply=(op,input)=>{
   if(['IniciarPreparacionPlan','RegistrarPartePreparacion','ValidarPartePreparacion','SellarPreparacionPlan','ConsultarPreparacionPlan','ConsultarRevisionPlan','ConsultarRecursoLote'].includes(op)&&window.doc120Deny)return Promise.reject({codigo:'ACCESO_DENEGADO',status:403});
   if(op==='IniciarPreparacionPlan'){header=structuredClone(input);return status();}
   if(op==='ConsultarPreparacionPlan')return header?status():{existe:false,estado:'NO_ENCONTRADA'};
   if(op==='RegistrarPartePreparacion'){const key=input.tipo+':'+input.numero;if(!parts.has(key)){parts.set(key,structuredClone(input));prepRev++;}return status();}
   if(op==='ValidarPartePreparacion'){const key=input.tipo+':'+input.numero;if(!parts.has(key))throw Error('MISSING_PART');if(!validated.has(key)){validated.add(key);prepRev++;}return status();}
   if(op==='SellarPreparacionPlan'){
    if(parts.size!==validated.size)throw Error('INCOMPLETE_PLAN');
    manifest={nivelRef:header.nivelRef,versionId:header.versionId,raices:Array.from(parts.values()).flatMap(p=>p.raices),archivos:Array.from(parts.values()).flatMap(p=>p.archivos)};
    roots=manifest.raices.map((r,i)=>({recursoId:String(52+i),idCliente:r.idCliente,nombre:r.nombre,estado:'PENDIENTE',revision:1,tieneIntentoActivo:false,recepcionListaParaCrear:false}));
    files=manifest.archivos.map((f,i)=>({recursoId:String(78+i),raizId:roots[manifest.raices.findIndex(r=>r.idCliente===f.raizIdCliente)].recursoId,idCliente:f.idCliente,nombre:f.nombre,rutaRelativa:f.rutaRelativa,bytesPrevistos:f.bytes,bytesRecibidos:'0',ultimaModificacionLocal:f.ultimaModificacionLocal,estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',revision:1,tieneIntentoActivo:false,valoresEfectivosFijados:false}));
    sealed=true;prepRev++;return summary();
   }
   if(op==='ConsultarRevisionPlan'){
    const output=summary(input.tipo,input.pagina);
    if(input.tipo==='VALORES'){
     const all=manifest.raices.map(r=>({Ambito:'RAIZ',RaizIdCliente:r.idCliente,ArchivoIdCliente:null,Valores:[{DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'CODIGO',Valor:'MISMO-CODIGO',Diferido:false},{DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'NOMBRE',Valor:r.nombre,Diferido:false}]})).concat(manifest.archivos.map(f=>({Ambito:'ARCHIVO',RaizIdCliente:f.raizIdCliente,ArchivoIdCliente:f.idCliente,Valores:[]})));
     output.valores=all.slice((input.pagina-1)*25,input.pagina*25);output.puedeHaberMas=input.pagina*25<all.length;output.unidadesElegiblesPagina=output.valores.filter(r=>r.Ambito==='RAIZ').map(r=>r.RaizIdCliente);
    }return output;
   }
   if(op==='ConsultarRecursoLote'){
    const snapshot=structuredClone({estado:doc120State(input.loteId),raices:roots,archivos:files});
    const row=(input.tipo==='RAICES'?snapshot.raices:snapshot.archivos).find(r=>r.recursoId===input.recursoId),parent=input.tipo==='ARCHIVOS'?snapshot.raices.find(r=>r.recursoId===row.raizId):null;
    const progress={recibidos:snapshot.archivos.filter(f=>f.estadoRecepcion==='RECIBIDO').length,expedientes:snapshot.raices.filter(r=>r.expedienteId).length,datos:snapshot.archivos.filter(f=>f.valoresEfectivosFijados).length,guardados:snapshot.archivos.filter(f=>f.documentoId).length,pendientesRecepcion:snapshot.archivos.filter(f=>f.estadoRecepcion==='PENDIENTE').length,incidencias:0,bytesRecibidos:String(snapshot.archivos.reduce((n,f)=>n+Number(f.bytesRecibidos),0))};
    return PreparacionSegmentadaImportacion.huellaRecursos(snapshot,crypto).then(hash=>({estado:snapshot.estado,tipo:input.tipo,recurso:row,padre:parent,huellaRecursos:hash,progreso:progress}));
   }
   return undefined;
  };
 });
}
async function selectFiles(page,count=1000){
 await page.getByLabel('Perfil publicado').selectOption('43');
 await page.evaluate(count=>{const transfer=new DataTransfer();for(let i=0;i<count;i++){const file=new File(['%PDF-x'],i+'.pdf',{type:'application/pdf',lastModified:0});Object.defineProperty(file,'webkitRelativePath',{value:'Egresos/'+i+'.pdf'});transfer.items.add(file);}const input=document.querySelector('input[webkitdirectory]');input.files=transfer.files;input.dispatchEvent(new Event('change',{bubbles:true}));},count);
}
module.exports={install,selectFiles};
