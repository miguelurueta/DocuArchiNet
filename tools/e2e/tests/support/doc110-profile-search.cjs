'use strict';
const {expect}=require('@playwright/test');
const crypto=require('node:crypto');

// Solo búsqueda/listado. No abre documentos ni ejecuta mutaciones.
async function exerciseProfileSearch(page,sample){
  const dialog=page.locator('#doc110-perfiles section[role=dialog][aria-labelledby=doc110-title]');
  const search=dialog.locator('[data-doc110=buscar-texto]');
  const menu=dialog.locator('.ui-autocomplete');
  await search.fill(sample.Codigo);await search.press('End');
  const items=menu.locator('li');
  const label=sample.Codigo+' — '+sample.Nombre;
  await expect(items.filter({hasText:label}).first()).toBeVisible();
  const labels=await items.allTextContents();
  const index=labels.indexOf(label);
  expect(index>=0).toBe(true);
  const before=await search.elementHandle();
  for(let i=0;i<=index;i++)await search.press('ArrowDown');
  await search.press('Enter');
  await expect.poll(()=>before.evaluate(el=>el.isConnected)).toBe(false);
  await before.dispose();
  await expect(search).toHaveValue(sample.Codigo);
  const selected=await search.elementHandle();
  await dialog.locator('[data-doc110=limpiar-busqueda]').click();
  await expect.poll(()=>selected.evaluate(el=>el.isConnected)).toBe(false);
  await selected.dispose();
  await expect(search).toHaveValue('');await expect(search).toBeFocused();
  await search.fill('DOC110_ABSENT_'+crypto.randomBytes(16).toString('hex'));await search.press('End');
  await expect(dialog.locator('[data-doc110=estado]')).toHaveText('No hay perfiles coincidentes.');
  await expect(menu).toBeHidden();
  await page.keyboard.press('Escape');await expect(dialog).toBeHidden();
  const toggle=page.locator('#doc110-navigation-toggle');
  if(await toggle.count() && await toggle.getAttribute('aria-expanded')!=='true')await toggle.click();
  await page.locator('[data-doc110=abrir]').click();
  await expect(dialog).toBeVisible();await expect(search).toHaveValue('');
  await expect(menu).toHaveCount(1);
  console.log('DOC110_PASS: AUTOCOMPLETE_KEYBOARD_EMPTY_RESULT_REOPEN');
}
module.exports={exerciseProfileSearch};
