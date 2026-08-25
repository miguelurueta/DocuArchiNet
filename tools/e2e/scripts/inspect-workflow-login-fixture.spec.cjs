'use strict';

const fs = require('node:fs/promises');
const { test } = require('@playwright/test');

const selector = '#ContentPlacenter_DropDownListmodulos';
const attempts = 3;
const delayMilliseconds = Number(process.env.WORKFLOW_LOGIN_INSPECT_DELAY_MS || 0);
const launchOptions = {};
if (process.env.DOC32_E2E_BROWSER_PATH?.trim()) launchOptions.executablePath = process.env.DOC32_E2E_BROWSER_PATH.trim();
else if (process.env.DOC32_E2E_BROWSER_CHANNEL?.trim()) launchOptions.channel = process.env.DOC32_E2E_BROWSER_CHANNEL.trim();
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });

test('inspecciona repetidamente el selector público con el fixture de Playwright', async ({ browser }) => {
  try {
    if (!Number.isSafeInteger(delayMilliseconds) || delayMilliseconds < 0 || delayMilliseconds > 30000) throw new Error('delay');
    if (delayMilliseconds > 0) await new Promise((resolve) => setTimeout(resolve, delayMilliseconds));
    const profilePath = process.env.WORKFLOW_LOGIN_INSPECT_PROFILE;
    if (typeof profilePath !== 'string' || !profilePath) throw new Error('profile');
    const raw = await fs.readFile(profilePath, 'utf8');
    const profile = JSON.parse(raw.replace(/^\uFEFF/, ''));
    const baseUrl = new URL(profile.baseUrl);
    const loginUrl = new URL('gestor.aspx', baseUrl).toString();
    for (let attempt = 1; attempt <= attempts; attempt += 1) {
      const context = await browser.newContext({ ignoreHTTPSErrors: false });
      try {
        const page = await context.newPage();
        try {
          const response = await page.goto(loginUrl, { waitUntil: 'commit', timeout: 30000 });
          try {
            await page.locator(selector).waitFor({ state: 'attached', timeout: 15000 });
            await page.locator(selector).selectOption({ value: profile.module });
            console.log(`LOGIN_FIXTURE_ATTEMPT_${attempt}_CONTROL_SELECTED_HTTP_${response?.status() || 0}`);
          } catch {
            const pageKind = await page.evaluate(() => {
              if (document.querySelector('#ContentPlacenter_DropDownListmodulos')) return 'SELECTOR_PRESENT';
              if (document.querySelector('form')) return 'UNEXPECTED_FORM';
              if (!document.body || document.body.childElementCount === 0) return 'EMPTY_DOCUMENT';
              return 'UNEXPECTED_DOCUMENT';
            }).catch(() => 'DOCUMENT_UNAVAILABLE');
            console.log(`LOGIN_FIXTURE_ATTEMPT_${attempt}_CONTROL_ABSENT_HTTP_${response?.status() || 0}_${pageKind}`);
          }
        } catch {
          console.log(`LOGIN_FIXTURE_ATTEMPT_${attempt}_NAVIGATION_UNAVAILABLE`);
        } finally {
          await page.close();
        }
      } finally {
        await context.close();
      }
    }
  } catch {
    console.log('LOGIN_FIXTURE_NAVIGATION_UNAVAILABLE');
  }
});
