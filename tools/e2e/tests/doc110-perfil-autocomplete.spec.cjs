'use strict';
const {test,expect}=require('@playwright/test');
const path=require('node:path');
const root=path.resolve(__dirname,'../../..');

// Navegador real con transporte simulado: no login, BD ni aceptación del ASMX.
test('COR-03: autocomplete existente, teclado, selección, limpieza y reapertura',async({page})=>{
  await page.setContent('<div id="doc110-perfiles" data-api="/webservice/WebServiceImportacionCarpetas.asmx" data-token="synthetic"></div>');
  await page.addScriptTag({path:path.join(root,'js/ui/jquery-3.4.1.min.js')});
  await page.addScriptTag({path:path.join(root,'js/ui/jquery-ui-1.8.22.custom.js')});
  await page.addStyleTag({path:path.join(root,'js/ui/jquery-ui.css')});
  await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css')});
  await page.evaluate(()=>{
    window.cor03Calls=[];
    window.PerfilImportacionApi={crear(){return {ejecutar(operation,payload){
      window.cor03Calls.push({operation,payload});
      const profiles=[{PerfilId:'1',Codigo:'OT-001',Nombre:'Orden de trabajo',Activo:true}];
      const items=payload.termino && !('OT-001 Orden de trabajo'.includes(payload.termino))?[]:profiles;
      return window.jQuery.Deferred().resolve({mensaje:'Listo',datos:{Elementos:items,Total:items.length}}).promise();
    }};}};
  });
  await page.addScriptTag({path:path.join(root,'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js')});
  await page.locator('[data-doc110=abrir]').click();
  let search=page.locator('[data-doc110=buscar-texto]');
  await search.fill('Orden');
  // El widget 1.8 escucha keydown para programar la consulta.
  await search.press('End');
  await expect(page.locator('.ui-autocomplete li')).toHaveText('OT-001 — Orden de trabajo');
  await search.press('ArrowDown');await search.press('Enter');
  await expect(page.locator('[data-doc110=buscar-texto]')).toHaveValue('OT-001');
  await expect.poll(()=>page.evaluate(()=>window.cor03Calls.at(-1).payload.codigo)).toBe('OT-001');
  await page.locator('[data-doc110=limpiar-busqueda]').click();
  await expect(search).toHaveValue('');
  await expect(search).toBeFocused();
  // Tecla global, sin locator.press que enfocaría el campo y ocultaría la regresión.
  await page.keyboard.press('Escape');
  await expect(page.locator('[role=dialog]')).toBeHidden();
  await page.locator('[data-doc110=abrir]').click();
  await search.fill('OT');await search.press('End');
  await expect(page.locator('.ui-autocomplete')).toBeVisible();
  await search.press('Escape');
  await expect(page.locator('.ui-autocomplete')).toBeHidden();
  await expect(page.locator('[role=dialog]')).toBeVisible();
  await search.press('Escape');
  await expect(page.locator('[role=dialog]')).toBeHidden();
  await page.locator('[data-doc110=abrir]').click();
  await expect(page.locator('.ui-autocomplete')).toHaveCount(1);
  await search.fill('inexistente');await search.press('End');
  await expect(page.locator('[role=status]')).toHaveText('No hay perfiles coincidentes.');
  // La página integrada mantiene estos administradores ocultos bajo el mismo host.
  await page.locator('#doc110-perfiles').evaluate(host=>{
    for(const name of ['proveedor','fuente']){
      const panel=document.createElement('section');panel.setAttribute('role','dialog');panel.hidden=true;
      const status=document.createElement('p');status.setAttribute('role','status');
      status.setAttribute('data-'+name,'resultado');panel.appendChild(status);host.appendChild(panel);
      const menu=document.createElement('ul');menu.className='ui-autocomplete';
      const item=document.createElement('li');item.textContent='OT-001 — Orden de trabajo';
      menu.appendChild(item);panel.appendChild(menu);
    }
    const external=document.createElement('ul');external.className='ui-autocomplete';
    external.style.display='none';document.body.appendChild(external);
  });
  await expect(page.locator('#doc110-perfiles section[role=dialog]')).toHaveCount(3);
  await expect(page.locator('.ui-autocomplete')).toHaveCount(4);
  await require('./support/doc110-profile-search.cjs').exerciseProfileSearch(page,{Codigo:'OT-001',Nombre:'Orden de trabajo'});
  await expect(page.locator('.ui-autocomplete')).toHaveCount(4);
});
