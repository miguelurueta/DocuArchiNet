const fs = require('node:fs');
const path = require('node:path');
const root = path.resolve(__dirname, '../../..');
const { compileBehaviorHarness } = require(path.join(root, 'tests/doc110-perfil-importacion-behavior.test.cjs'));
const directory = path.join(root, '.tmp/doc111-persistence-harness');
fs.mkdirSync(directory, { recursive: true });
compileBehaviorHarness(directory);
console.log('DOC111_EXISTING_HARNESS_COMPILED');
