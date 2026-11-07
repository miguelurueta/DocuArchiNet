'use strict';
const {expect}=require('@playwright/test');

async function exerciseUpdatePanel(page){
  const visible=page.locator('#doc110-perfiles section[role=dialog]:not([hidden])');
  await page.keyboard.press('Escape');await expect(visible).toHaveCount(0);
  await expect(page.locator('#doc110-navigation-toggle')).toBeFocused();
  const ready=await page.evaluate(()=>{
    const manager=window.Sys?.WebForms?.PageRequestManager?.getInstance();
    if(!manager || !document.getElementById('Button_actualiza'))return false;
    window.doc110UpdatePanelProbe={completed:0,failed:false,host:document.getElementById('doc110-perfiles'),jquery:window.jQuery,dropdown:window.jQuery.fn.dropdown};
    manager.add_endRequest(function(sender,args){
      window.doc110UpdatePanelProbe.completed++;
      window.doc110UpdatePanelProbe.failed=Boolean(args.get_error());
    });
    return true;
  });
  expect(ready).toBe(true);
  for(let round=1;round<=2;round++){
    const postback=page.waitForResponse(r=>r.request().method()==='POST' &&
      new URL(r.url()).pathname.endsWith('/WebFormProducionDocumental.aspx') &&
      r.request().headers()['x-microsoftajax']==='Delta=true');
    // Control real oculto de WebForms: no invocar artificialmente Sys.Application.load.
    await page.evaluate(()=>document.getElementById('Button_actualiza').click());
    expect((await postback).status()).toBe(200);
    await expect.poll(()=>page.evaluate(()=>window.doc110UpdatePanelProbe.completed)).toBe(round);
    expect(await page.evaluate(()=>{
      const p=window.doc110UpdatePanelProbe;
      return !p.failed && p.host===document.getElementById('doc110-perfiles') &&
        p.jquery===window.jQuery && p.dropdown===window.jQuery.fn.dropdown;
    })).toBe(true);
    await expect(page.locator('#doc110-navigation')).toHaveCount(1);
    await expect(page.locator('[data-doc110=abrir]')).toHaveCount(1);
    let lists=0,postbacks=0;
    const observe=request=>{
      if(request.method()!=='POST')return;
      if(new URL(request.url()).pathname.endsWith('/ListarPerfiles'))lists++;
      if(new URL(request.url()).pathname.endsWith('/WebFormProducionDocumental.aspx'))postbacks++;
    };
    page.on('request',observe);
    try{
      await page.locator('#doc110-navigation-toggle').press('Enter');
      const listed=page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
      await page.locator('[data-doc110=abrir]').press('Enter');
      const response=await listed;expect(response.status()).toBe(200);
      expect((await response.json()).d?.ok).toBe(true);
      await expect(visible).toHaveCount(1);
      await expect(page.locator('[data-doc110=nuevo]')).toBeVisible();
      await page.keyboard.press('Escape');await expect(visible).toHaveCount(0);
      await expect(page.locator('#doc110-navigation-toggle')).toBeFocused();
      expect(lists).toBe(1);expect(postbacks).toBe(0);
    }finally{page.off('request',observe);}
  }
  console.log('DOC110_PASS: TWO_REAL_UPDATEPANEL_CYCLES');
  console.log('DOC110_PASS: UPDATEPANEL_SINGLE_MENU_AND_LIBRARY_INSTANCES');
  console.log('DOC110_PASS: UPDATEPANEL_KEYBOARD_REOPEN_ONE_LIST_REQUEST');
}
module.exports={exerciseUpdatePanel};
