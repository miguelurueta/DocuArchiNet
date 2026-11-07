'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const fs=require('node:fs');
const root=path.resolve(__dirname,'../../..');
for(const allowed of ['0','1'])test('DOC-110: menú integrado y permisos adicionales '+allowed,async({page})=>{
 await page.setContent('<div id="doc110-perfiles" data-proveedores="'+allowed+'" data-fuentes="'+allowed+'"></div><nav id="menu_var"><ul class="navbar-nav"><li>Expedientes</li><li>Cargar</li></ul></nav>');
 await page.addScriptTag({path:path.join(root,'js/ui/jquery-3.4.1.min.js')});
 // Reproducir las inclusiones de Bootstrap de la página, incluso duplicados.
 await page.addScriptTag({path:path.join(root,'colorlib/vendor/bootstrap/js/popper.min.js')});
 const markup=fs.readFileSync(path.join(root,'Gestion/WebFormProducionDocumental.aspx'),'utf8');
 const scripts=[...markup.matchAll(/<script\s+src="\.\.\/(bootstrap\/js\/bootstrap(?:\.bundle)?\.min\.js)"/g)];
 for(const script of scripts)await page.addScriptTag({path:path.join(root,script[1])});
 await page.addStyleTag({path:path.join(root,'bootstrap/css/bootstrap.min.css')});
 await page.addStyleTag({path:path.join(root,'Styles/bootra-person.css')});
 await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
 await page.evaluate(()=>{window.PerfilImportacionApi={crear(){return {ejecutar(op){return jQuery.Deferred().resolve({mensaje:'Listo',datos:op==='ListarPerfiles'?{Elementos:[],Total:0}:[]}).promise();}};}};});
 for(const file of ['ProveedorAdminEditor.js','FuenteAdminEditor.js','PerfilImportacionEditor.js'])await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas',file)});
 const menu=page.locator('#doc110-navigation'),toggle=menu.locator('[data-toggle=dropdown]');
 await expect(menu).toHaveCount(1);await expect(page.locator('#doc110-perfiles > button')).toHaveCount(0);
 await expect(menu.locator('button')).toHaveCount(allowed==='1'?3:1);
 await page.evaluate(()=>window.PerfilImportacionEditor.inicializar());await expect(menu).toHaveCount(1);
 for(const id of allowed==='1'?['abrir','admin-proveedores','admin-fuentes']:['abrir']){
  await toggle.click();await menu.locator('[data-doc110='+id+']').click();
  await expect(toggle).toHaveAttribute('aria-expanded','false');
  await expect(page.locator('#doc110-perfiles section:not([hidden])')).toHaveCount(1);
  await page.keyboard.press('Escape');await expect(toggle).toBeFocused();
 }
 await toggle.press('Enter');await expect(toggle).toHaveAttribute('aria-expanded','true');await toggle.press('Escape');await expect(toggle).toHaveAttribute('aria-expanded','false');
 await toggle.click();await page.locator('#menu_var li').first().click();await expect(toggle).toHaveAttribute('aria-expanded','false');
});
