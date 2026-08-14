const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './tests',
  timeout: 90000,
  expect: { timeout: 15000 },
  fullyParallel: false,
  workers: 1,
  forbidOnly: Boolean(process.env.CI),
  retries: process.env.CI ? 1 : 0,
  reporter: [
    ['list'],
    ['json', { outputFile: 'artifacts/playwright-report.json' }]
  ],
  outputDir: 'artifacts/test-results',
  use: {
    browserName: 'chromium',
    channel: process.env.DOC10_E2E_BROWSER_CHANNEL || undefined,
    executablePath: process.env.DOC10_E2E_BROWSER_PATH || undefined,
    headless: true,
    ignoreHTTPSErrors: process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS === 'true',
    screenshot: 'only-on-failure',
    trace: 'retain-on-failure',
    video: 'off'
  }
});
