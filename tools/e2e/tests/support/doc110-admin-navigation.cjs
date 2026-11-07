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
