'use strict';
const {test,expect}=require('@playwright/test');
const {openImportAdministrator}=require('./support/doc110-admin-navigation.cjs');
async function setup(page,respond=true){
  await page.route('https://doc110.invalid/**',route=>respond?route.fulfill({status:200,headers:{'access-control-allow-origin':'*'},body:'{}'}):route.abort());
  await page.setContent('<button id="doc110-navigation-toggle" aria-expanded="false">Importación</button>'+
    '<div id="menu" hidden><button data-doc110="admin-proveedores">Proveedores</button><button data-doc110="admin-fuentes">Fuentes</button></div>'+
    '<div id="doc110-perfiles"><section role="dialog" aria-label="Prueba" hidden><button>Cerrar</button></section></div>');
  await page.evaluate(()=>{
    const toggle=document.querySelector('#doc110-navigation-toggle'),menu=document.querySelector('#menu'),dialog=document.querySelector('section');
    window.navigationCalls=0;
    toggle.onclick=()=>{const open=toggle.getAttribute('aria-expanded')!=='true';toggle.setAttribute('aria-expanded',String(open));menu.hidden=!open;};
    dialog.querySelector('button').onclick=()=>{dialog.hidden=true;};
    document.querySelectorAll('[data-doc110]').forEach(button=>{button.onclick=()=>{
      window.navigationCalls++;menu.hidden=true;toggle.setAttribute('aria-expanded','false');dialog.hidden=false;
      fetch('https://doc110.invalid/'+(button.dataset.doc110==='admin-proveedores'?'ListarAdministracionProveedores':'ListarFuentes'),{method:'POST'}).catch(()=>{});
    };});
  });
}
test('driver abre menu cerrado o ya abierto y cambia de administrador sin cerrar el desplegable por error',async({page})=>{
  await setup(page);await page.locator('#doc110-navigation-toggle').click();
  const first=await openImportAdministrator(page,'admin-proveedores','ListarAdministracionProveedores');expect(first.status()).toBe(200);
  const next=await openImportAdministrator(page,'admin-fuentes','ListarFuentes');expect(next.status()).toBe(200);
  expect(await page.evaluate(()=>window.navigationCalls)).toBe(2);
});
test('driver limita espera de respuesta para permitir recuperacion y no repite click',async({page})=>{
  await setup(page,false);
  await expect(openImportAdministrator(page,'admin-proveedores','ListarAdministracionProveedores',500)).rejects.toThrow('DOC110_UI_RESPONSE_NOT_OBSERVED');
  expect(await page.evaluate(()=>window.navigationCalls)).toBe(1);
});
