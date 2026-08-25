'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { chromium } = require('@playwright/test');

const selector = '#ContentPlacenter_DropDownListmodulos';

async function loadLoginUrl(profilePath) {
  const raw = await fs.readFile(profilePath, 'utf8');
  const profile = JSON.parse(raw.replace(/^\uFEFF/, ''));
  const baseUrl = new URL(profile.baseUrl);
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) throw new Error('invalid-profile');
  return { loginUrl: new URL('gestor.aspx', baseUrl).toString(), module: profile.module };
}

async function main() {
  const [profilePath] = process.argv.slice(2);
  if (!profilePath || process.argv.length !== 3 || !path.isAbsolute(profilePath)) throw new Error('invalid-arguments');
  const { loginUrl, module } = await loadLoginUrl(profilePath);
  const browser = await chromium.launch({ headless: true });
  let stage = 'browser';
  try {
    const context = await browser.newContext();
    try {
      const page = await context.newPage();
      try {
        stage = 'navigate';
        const response = await page.goto(loginUrl, { waitUntil: 'commit', timeout: 30000 });
        const moduleControl = page.locator(selector);
        stage = 'module-control';
        await moduleControl.waitFor({ state: 'attached', timeout: 30000 });
        const count = await moduleControl.count();
        const onchange = await moduleControl.getAttribute('onchange');
        const enabled = await moduleControl.isEnabled();
        const visible = await moduleControl.isVisible();
        const values = await moduleControl.locator('option').evaluateAll((options) => options
          .map((option) => option.value)
          .filter((value) => /^[A-Za-z0-9_.-]{1,80}$/.test(value)));
        console.log(`LOGIN_MODULE_VALUES=${values.join(',')}`);
        console.log(`LOGIN_CONTROL_PRESENT_HTTP_${response?.status() || 0}_MODULE_COUNT_${count}_MODULE_POSTBACK_${/__doPostBack|postback/i.test(onchange || '') ? 'YES' : 'NO'}_ENABLED_${enabled ? 'YES' : 'NO'}_VISIBLE_${visible ? 'YES' : 'NO'}`);
        stage = 'module-option';
        await moduleControl.selectOption({ value: module });
        console.log('LOGIN_MODULE_SELECT_SUCCEEDED');
      } finally {
        await page.close();
      }
    } finally {
      await context.close();
    }
  } finally {
    await browser.close();
  }
}

main().catch(() => {
  console.log('LOGIN_CONTROL_UNAVAILABLE');
  process.exitCode = 1;
});
