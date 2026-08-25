'use strict';

const baseConfig = require('../playwright.config.cjs');

module.exports = {
  ...baseConfig,
  testDir: __dirname,
  testMatch: 'inspect-workflow-login-fixture.spec.cjs'
};
