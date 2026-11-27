"""Libros sintéticos DOC123. Solo stdlib; BIFF8/CFB y OpenXML, sin Office ni negocio.
Uso: python generar.py [directorio] [filas_para_frontera]. Salida determinista.
"""
import datetime, hashlib, json, math, pathlib, re, struct, sys, zipfile
from xml.sax.saxutils import escape

def rec(kind, payload=b''):
    return struct.pack('<HH', kind, len(payload)) + payload

def bof(kind):
    return rec(0x809, struct.pack('<HHHHII', 0x600, kind, 0xdbb, 1997, 0x41, 6))

def text(value):
    return struct.pack('<HB', len(value.encode('utf-16le'))//2, 1)+value.encode('utf-16le')

def cell(row, col, value, kind='s', style=0):
    return (row, col, kind, value, style)

def biff_cell(c):
    row, col, kind, value, style = c
    head=struct.pack('<HHH',row-1,col-1,style)
    if kind=='s': return rec(0x204,head+text(value))
    if kind=='n': return rec(0x203,head+struct.pack('<d',value))
    if kind=='e': return rec(0x205,head+bytes([7,1]))
    if kind=='b': return rec(0x205,head+bytes([int(value),0]))
    if kind=='blank': return rec(0x201,head)
    if kind.startswith('f'):
        cached=struct.pack('<d',42.0)
        if kind=='ferror': cached=bytes([2,0,7,0,0,0,255,255])
        if kind=='fmissing': cached=bytes([0,0,0,0,0,0,255,255])
        if kind=='fempty': cached=bytes([3,0,0,0,0,0,255,255])
        return rec(6,head+cached+struct.pack('<HIH',0,0,3)+bytes([0x1e,42,0]))
    raise ValueError(kind)

def compound_mini(workbook):
    """CFB v3 con mini-stream real, para contrastar otro contenedor común."""
    assert len(workbook)<4096
    count=math.ceil(len(workbook)/64); mini=workbook.ljust(count*64,b'\0'); n=math.ceil(len(mini)/512)
    hdr=bytearray(512); hdr[:8]=bytes.fromhex('D0CF11E0A1B11AE1')
    struct.pack_into('<HHHHH',hdr,24,0x3e,3,0xfffe,9,6)
    struct.pack_into('<IIIIIIIII',hdr,40,0,1,n+1,0,4096,n,1,0xfffffffe,0)
    struct.pack_into('<109I',hdr,76,*([n+2]+[0xffffffff]*108))
    def directory(name,kind,start,size,child=0xffffffff):
        b=bytearray(128); raw=(name+'\0').encode('utf-16le'); b[:len(raw)]=raw
        struct.pack_into('<HBBIII',b,64,len(raw),kind,1,0xffffffff,0xffffffff,child)
        struct.pack_into('<IQ',b,116,start,size); return b
    dirs=directory('Root Entry',5,0,len(mini),1)+directory('Workbook',2,0,len(workbook))+bytes(256)
    minifat=list(range(1,count))+[0xfffffffe]+[0xffffffff]*(128-count)
    fat=list(range(1,n))+[0xfffffffe,0xfffffffe,0xfffffffe,0xfffffffd]+[0xffffffff]*(128-n-3)
    return bytes(hdr)+mini.ljust(n*512,b'\0')+struct.pack('<128I',*minifat)+dirs+struct.pack('<128I',*fat)

def compound(workbook):
    # Stream >=4096: regular FAT, no mini-stream in these fixtures.
    workbook=workbook.ljust(max(4096,math.ceil(len(workbook)/512)*512),b'\0')
    n=len(workbook)//512
    fats=1
    while math.ceil((n+1+fats)/128)!=fats: fats=math.ceil((n+1+fats)/128)
    if fats>109: raise ValueError('fixture requires DIFAT')
    hdr=bytearray(512)
    hdr[:8]=bytes.fromhex('D0CF11E0A1B11AE1')
    struct.pack_into('<HHHHH',hdr,24,0x3e,3,0xfffe,9,6)
    struct.pack_into('<IIIIIIIII',hdr,40,0,fats,n,0,4096,0xfffffffe,0,0xfffffffe,0)
    struct.pack_into('<109I',hdr,76,*([n+1+i for i in range(fats)]+[0xffffffff]*(109-fats)))
    def directory(name,kind,start,size,child=0xffffffff):
        b=bytearray(128); raw=(name+'\0').encode('utf-16le'); b[:len(raw)]=raw
        struct.pack_into('<HBBIII',b,64,len(raw),kind,1,0xffffffff,0xffffffff,child)
        struct.pack_into('<IQ',b,116,start,size); return b
    dirs=directory('Root Entry',5,0xfffffffe,0,1)+directory('Workbook',2,0,len(workbook))+bytes(256)
    table=[i+1 for i in range(n-1)]+[0xfffffffe,0xfffffffe]+[0xfffffffd]*fats
    table += [0xffffffff]*(fats*128-len(table))
    return bytes(hdr)+workbook+dirs+struct.pack('<%dI'%len(table),*table)

def xls(sheets,mini=False):
    globals_=bof(5)+rec(0x42,struct.pack('<H',1200))+rec(0x22,struct.pack('<H',0))
    globals_+=rec(0x41e,struct.pack('<H',164)+text('00000'))+rec(0x41e,struct.pack('<H',165)+text('yyyy-mm-dd'))
    for fmt in [0,164,165]: globals_+=rec(0xe0,struct.pack('<HH',0,fmt)+bytes(16))
    def bound(offset,name):
        return rec(0x85,struct.pack('<IBBB',offset,0,0,len(name))+b'\x01'+name.encode('utf-16le'))
    sizes=sum(len(bound(0,n)) for n,_ in sheets)
    offset=len(globals_)+sizes+4
    bodies=[]; bounds=b''
    for name,cells in sheets:
        body=bof(0x10)+b''.join(biff_cell(c) for c in sorted(cells))+rec(0xa)
        bounds+=bound(offset,name); offset+=len(body); bodies.append(body)
    return (compound_mini if mini else compound)(globals_+bounds+rec(0xa)+b''.join(bodies))

def col(n):
    out=''
    while n: n,k=divmod(n-1,26); out=chr(65+k)+out
    return out

def xlsx(sheets):
    ns='http://schemas.openxmlformats.org/spreadsheetml/2006/main'
    parts={}
    parts['[Content_Types].xml']='<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>'+''.join('<Override PartName="/xl/worksheets/sheet%d.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>'%i for i in range(1,len(sheets)+1))+'</Types>'
    parts['_rels/.rels']='<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>'
    parts['xl/workbook.xml']='<workbook xmlns="'+ns+'" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets>'+''.join('<sheet name="'+escape(n,{'"':'&quot;'})+'" sheetId="%d" r:id="rId%d"/>'%(i,i) for i,(n,_) in enumerate(sheets,1))+'</sheets></workbook>'
    parts['xl/_rels/workbook.xml.rels']='<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">'+''.join('<Relationship Id="rId%d" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet%d.xml"/>'%(i,i) for i in range(1,len(sheets)+1))+'<Relationship Id="styles" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>'
    parts['xl/styles.xml']='<styleSheet xmlns="'+ns+'"><numFmts count="2"><numFmt numFmtId="164" formatCode="00000"/><numFmt numFmtId="165" formatCode="yyyy-mm-dd"/></numFmts><fonts count="1"><font/></fonts><fills count="1"><fill/></fills><borders count="1"><border/></borders><cellStyleXfs count="1"><xf/></cellStyleXfs><cellXfs count="3"><xf numFmtId="0"/><xf numFmtId="164" applyNumberFormat="1"/><xf numFmtId="165" applyNumberFormat="1"/></cellXfs></styleSheet>'
    for i,(_,cells) in enumerate(sheets,1):
        rows={}
        for row,c,k,v,style in sorted(cells):
            attr=' r="'+col(c)+str(row)+'" s="'+str(style)+'"'
            if k=='s': raw='<c'+attr+' t="inlineStr"><is><t xml:space="preserve">'+escape(v)+'</t></is></c>'
            elif k=='blank': raw='<c'+attr+'/>'
            elif k=='e': raw='<c'+attr+' t="e"><v>#DIV/0!</v></c>'
            elif k.startswith('f'):
                attr+= ' t="e"' if k=='ferror' else (' t="str"' if k=='fempty' else '')
                cache='' if k=='fmissing' else '<v>'+('#DIV/0!' if k=='ferror' else ('' if k=='fempty' else '42'))+'</v>'
                raw='<c'+attr+'><f>40+2</f>'+cache+'</c>'
            else: raw='<c'+attr+(' t="b"' if k=='b' else '')+'><v>'+str(int(v) if k=='b' else v)+'</v></c>'
            rows.setdefault(row,[]).append(raw)
        parts['xl/worksheets/sheet%d.xml'%i]='<worksheet xmlns="'+ns+'"><sheetData>'+''.join('<row r="%d">'%r+''.join(cells)+'</row>' for r,cells in rows.items())+'</sheetData></worksheet>'
    import io
    out=io.BytesIO()
    with zipfile.ZipFile(out,'w',zipfile.ZIP_DEFLATED) as z:
        for path,value in sorted(parts.items()):
            info=zipfile.ZipInfo(path,(2026,1,1,0,0,0)); info.compress_type=zipfile.ZIP_DEFLATED; z.writestr(info,value.encode('utf-8'))
    return out.getvalue()

def generate(out, count=0):
    out.mkdir(parents=True,exist_ok=True)
    hdr=['Código','','Nombre','Entero','Decimal','Fecha','TextoFecha','Transformar','Libre']
    data=[cell(1,i+1,v) for i,v in enumerate(hdr) if v]
    for r,code,name,integer,dec,date,tx,transform in [
        (2,123,"O'Brian n/\\ &;#% ñ",2147483647,1234.5,datetime.date(2025,12,31),'31/12/2025','1,2.3::último'),
        (4,'00007',' segunda ',-2147483648,-12.25,datetime.date(2024,2,29),'texto libre','4,5.6::fin'),
        (5,'Última','final',0,0,datetime.date(2025,1,1),'01/01/2025','7,8.9::último')]:
        data += [cell(r,1,code,'n' if isinstance(code,int) else 's',1 if isinstance(code,int) else 0),cell(r,3,name),cell(r,4,integer,'n'),cell(r,5,dec,'n'),cell(r,6,(date-datetime.date(1899,12,30)).days,'n',2),cell(r,7,tx),cell(r,8,transform)]
    cases=[cell(1,1,'Código'),cell(1,2,'Valor')]
    for r,k in [(2,'n'),(3,'e'),(4,'fnumber'),(5,'ferror'),(6,'fmissing'),(7,'fempty'),(8,'blank')]:
        cases += [cell(r,1,'K'+str(r)),cell(r,2,123 if k=='n' else None,k,1 if k=='n' else 0)]
    books={'valido':[('Datos',data),('Otra',[cell(1,1,'Código'),cell(2,1,'otra')])],'fidelidad':[('Casos',cases)],'cache-valido':[('Casos',[c for c in cases if c[0] in (1,2,4,7,8)])]}
    if count:
        for n in [count,count+1]: books['filas-'+str(n)]=[('Datos',[cell(1,1,'Código')]+[cell(i+2,1,str(i)) for i in range(n)])]
    hashes={}
    for name,sheets in books.items():
        for ext,writer in [('xls',xls),('xlsx',xlsx)]:
            content=writer(sheets); filename=name+'.'+ext; (out/filename).write_bytes(content); hashes[filename]=hashlib.sha256(content).hexdigest()
    for name in ('valido','fidelidad','cache-valido'):
        content=xls(books[name],mini=True); filename=name+'-mini.xls'; (out/filename).write_bytes(content); hashes[filename]=hashlib.sha256(content).hexdigest()
    (out/'sha256.json').write_text(json.dumps(hashes,indent=2)+'\n',encoding='utf-8')
    print('DOC123_SYNTHETIC_BOOKS',len(hashes))

def generate_capacity(out, run_tag):
    """Mismos escritores DOC123; archivos privados, sin acceso a base/HTTP."""
    if not re.fullmatch('[a-f0-9]{32}', run_tag):
        raise ValueError('DOC127_CAPACITY_TAG_REQUIRED')
    out.mkdir(parents=True, exist_ok=True)
    manifest=[]
    for ext,writer in [('xls',xls),('xlsx',xlsx)]:
        for mode in ('CREAR','AGREGAR','REEMPLAZAR'):
            for count in (1000,5000,10000):
                prefix='DOC127_'+run_tag+'_'+ext+'_'+mode+'_'+str(count)+'_'
                cells=[cell(1,1,'CLAVE'),cell(1,2,'VALOR')]
                for n in range(count):
                    cells.extend([cell(n+2,1,prefix+str(n).zfill(5)),cell(n+2,2,'V_'+prefix+str(n).zfill(5))])
                content=writer([('Datos',cells)])
                name=mode.lower()+'-'+str(count)+'.'+ext
                target=out/name
                if target.exists():
                    raise ValueError('DOC127_CAPACITY_FILE_ALREADY_EXISTS')
                target.write_bytes(content)
                manifest.append(dict(formato=ext.upper(),modo=mode,filasEsperadas=count,
                    archivoPath=str(target.resolve()),archivoHash=hashlib.sha256(content).hexdigest(),bytes=len(content),hoja='Datos'))
    (out/'capacity-manifest.json').write_text(json.dumps(dict(issue='DOC-127',runTag=run_tag,
        synthetic=True,realExecution=False,cases=manifest),indent=2)+'\n',encoding='utf-8')
    print('DOC127_SYNTHETIC_CAPACITY_BOOKS',len(manifest))

if __name__=='__main__':
    if len(sys.argv)>1 and sys.argv[1]=='--capacity':
        if len(sys.argv)!=4: raise ValueError('Usage: generar.py --capacity private_directory hex_tag32')
        generate_capacity(pathlib.Path(sys.argv[2]),sys.argv[3])
    else:
        generate(pathlib.Path(sys.argv[1]) if len(sys.argv)>1 else pathlib.Path(__file__).parent,int(sys.argv[2]) if len(sys.argv)>2 else 0)
