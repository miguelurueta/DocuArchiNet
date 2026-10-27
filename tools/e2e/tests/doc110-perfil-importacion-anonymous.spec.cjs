'use strict';
const {test,expect}=require('@playwright/test');
const fs=require('node:fs');
const path=require('node:path');

// Reutiliza el ambiente de Producción ya configurado. Sin login ni datos reales.
const file=process.env.DOC110_E2E_PROFILE_PATH ||
  path.resolve(__dirname,'../profiles/doc88-production-document-upload.local.runtime.json');
const profile=JSON.parse(fs.readFileSync(file,'utf8').replace(/^\uFEFF/,''));
test.use({screenshot:'off',trace:'off',video:'off'});
test.describe.configure({retries:0});

test('DOC-110: formulario de login real disponible',async({page})=>{
  await page.goto(new URL('gestor.aspx',profile.baseUrl).href);
  await expect(page.locator('#ContentPlacenter_DropDownListmodulos')).toBeAttached();
  await expect(page.locator('#ContentPlacenter_TextBoxuser')).toBeAttached();
});

for(const method of ['ListarFuentes','ObtenerFuente','HistorialFuente','ConsultarMetadatosFuente','CrearFuente','GuardarFuente','ActivarFuente','RevocarFuente','ListarPerfiles','ObtenerPerfil','ConsultarHistorial','ConsultarCatalogo','ConsultarProveedores','ConsultarMetadatosProveedor',
  'CrearPerfil','GuardarBorrador','EnsayarReglas','PublicarVersion','CrearVersion',
  'ActualizarCabecera','CambiarDisponibilidad','ValidarSeleccionPerfil']) {
  test('DOC-110: '+method+' rechaza sesión anónima',async({request})=>{
    const response=await request.post(new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,profile.baseUrl).href,{
      data:{solicitud:{}},maxRedirects:0
    });
    // No adjuntar cuerpos, URLs ni cabeceras a los reportes.
    expect(response.status()).toBe(401);
    let result;try{result=(await response.json()).d;}catch{throw new Error('Respuesta de rechazo no JSON.');}
    expect(Boolean(result && result.ok===false && result.codigo==='SESION_REQUERIDA')).toBe(true);
    expect(result.datos==null).toBe(true);
    expect(response.headers()['cache-control'] || '').toContain('no-store');
  });
}
