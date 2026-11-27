'use strict';
const {expect}=require('@playwright/test');
async function openImportAdministrator(page,access,method,timeout=15000){
  const visible=page.locator('#doc110-perfiles section[role=dialog]:not([hidden])');
  if(await visible.count()){
    await expect(visible).toHaveCount(1);await expect(visible).not.toHaveAttribute('aria-busy','true');
    await visible.getByRole('button',{name:'Cerrar',exact:true}).click({timeout});
  }
  await expect(visible).toHaveCount(0);
  const toggle=page.locator('#doc110-navigation-toggle');
  // Abrir, sin alternar a ciegas un menú que ya pudiera estar abierto.
  if(await toggle.getAttribute('aria-expanded')!=='true')await toggle.click({timeout});
  const target=page.locator('[data-doc110='+access+']');
  await expect(target).toBeVisible({timeout});
  const pending=page.waitForResponse(r=>r.url().endsWith('/'+method)&&r.request().method()==='POST',{timeout})
    .catch(()=>{throw new Error('DOC110_UI_RESPONSE_NOT_OBSERVED: '+method+'; sin reintento automatico.');});
  const [response]=await Promise.all([pending,target.click({timeout})]);
  return response;
}
module.exports={openImportAdministrator};

async function openExcelImporter(page,timeout=15000){
 const dialog=page.locator('#doc126-excel [role=dialog]');
 if(await dialog.isVisible())return dialog;
 const toggle=page.locator('#doc110-navigation-toggle');
 if(await toggle.getAttribute('aria-expanded')!=='true')await toggle.click({timeout});
 const pending=page.waitForResponse(r=>r.url().endsWith('/AbrirCargaExcel')&&r.request().method()==='POST',{timeout});
 await page.locator('[data-doc126=abrir]').click({timeout});const response=await pending;
 expect(response.status()).toBe(200);await expect(dialog).toBeVisible({timeout});
 await expect(dialog).toHaveAttribute('aria-busy','false',{timeout});return dialog;
}
module.exports.openExcelImporter=openExcelImporter;

async function verifyExcelFormIsolation(page){
 const result=await page.evaluate(()=>{
  const main=document.getElementById('form1'),host=document.getElementById('doc126-excel');
  const controls=host?Array.from(host.querySelectorAll('input,select,button,textarea')):[];
  return {mainFound:!!main,controls:controls.length,ownedByMain:controls.filter(e=>e.form===main).length,
   isolated:controls.every(e=>e.form&&e.form.isConnected&&e.form!==main&&!main.contains(e.form)),
   mainValidationEnabled:!!main&&!main.noValidate};
 });
 if(!result.mainFound||!result.controls||result.ownedByMain||!result.isolated||!result.mainValidationEnabled)throw Error('DOC127_WEBFORMS_ISOLATION_REQUIRED');
 return result;
}
module.exports.verifyExcelFormIsolation=verifyExcelFormIsolation;
